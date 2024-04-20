using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConstellationController : MonoBehaviour
{
    //List of pedestals in constellation
    [SerializeField] private List<PedestalConstellation> _pedestalList;
    //List of each node data class (connection between two pedestals)
    [SerializeField] private List<PedestalNode> _pedestalNodeList;

    private List<PedestalData> _pedestaData = new List<PedestalData>();
    private int _pedestalNum;
    // Awake is called before the first frame update
    void Awake()
    {
        _pedestalNum = _pedestalList.Count;
    }

    void Start()
    {
        //set all _mirroredPedestals bools to false
        for (int i = 0; i < _pedestalNum; i++)
        {
            _pedestaData.Add(new PedestalData());
        }

        foreach (var item in _pedestalList)
        {
            if(item == null)
            {
                Debug.LogError("_pedestalList has an empty value!!! fix this now");
            }
        }
    }

    // Set pedestal to have mirror, then run check function
    public void PedestalHasMirror(PedestalConstellation sender)
    {
        int senderIndex = _pedestalList.IndexOf(sender);
        // check to make sender is in list and that list is not empty
        if(senderIndex != -1 || _pedestalList.Count != 0)
        {
            _pedestaData[senderIndex].HasMirror = true;
            PedestalChecker(senderIndex, true);

            //BeamChecker(senderIndex, true);
        }
        else
        {
            Debug.LogError("PedestalHasMirror sender is not in list!! fix this now");
        }
    }

    // Set pedestal to be recieving a beam, then run check function
    public void PedestalHasBeam(List<PedestalConstellation> pedestalDestinations)
    {
        foreach (PedestalConstellation pedestal in pedestalDestinations)
        {
            int senderIndex = _pedestalList.IndexOf(pedestal);
            // check to make sender is in list and that list is not empty
            if(senderIndex != -1 || _pedestalList.Count != 0)
            {
                _pedestaData[senderIndex].RecieveBeam = true;
                PedestalChecker(senderIndex, true);

                //BeamChecker(senderIndex, false);
            }
            else
            {
                Debug.LogError("PedestalHasBeam sender is not in list!! fix this now");
            }
        }
    }

    public void PedestalPreset(PedestalConstellation sender)
    {
        int senderIndex = _pedestalList.IndexOf(sender);
        // check to make sender is in list and that list is not empty
        if(senderIndex != -1 || _pedestalList.Count != 0)
        {
            //_beamedPedestals[senderIndex] = true;
            PedestalChecker(senderIndex, false);

            // Go through each node 
            for (int i = 0; i < _pedestalNodeList.Count; i++)
            {
                PedestalNode node = _pedestalNodeList[i];
                PedestalConstellation pedestalA = node.PedestalA;
                PedestalConstellation pedestalB = node.PedestalB;
                int otherIndex = -1;

                // get the pedestal that are paired with the sender pedestal
                if (pedestalA == _pedestalList[senderIndex])
                { 
                    otherIndex = _pedestalList.IndexOf(pedestalB);
                }
                else if(pedestalB == _pedestalList[senderIndex])
                {
                    pedestalA = node.PedestalB;
                    pedestalB = node.PedestalA;
                    otherIndex = _pedestalList.IndexOf(pedestalB);
                }

                if (otherIndex != -1)
                {
                    pedestalA.ActivateEffect(pedestalB);
                    _pedestaData[senderIndex].ShootingBeam = true;
                    _pedestaData[senderIndex].RecieveBeam = true;
                }
            }
        }
        else
        {
            Debug.LogError("PedestalHasBeam sender is not in list!! fix this now");
        }
    }

    private void BeamChecker(int senderIndex, bool mirrorMode)
    {
        // Go through each node 
        for (int i = 0; i < _pedestalNodeList.Count; i++)
        {
            PedestalNode node = _pedestalNodeList[i];
            PedestalConstellation pedestalA = node.PedestalA;
            PedestalConstellation pedestalB = node.PedestalB;
            int otherIndex = -1;

            // get the pedestal that are paired with the sender pedestal
            if (pedestalA == _pedestalList[senderIndex])
            {
                otherIndex = _pedestalList.IndexOf(pedestalB);
            }
            else if(pedestalB == _pedestalList[senderIndex])
            {
                pedestalA = node.PedestalB;
                pedestalB = node.PedestalA;
                otherIndex = _pedestalList.IndexOf(pedestalB);
            }

            if (otherIndex != -1)
            {
                if (mirrorMode)
                {
                    if (_pedestaData[senderIndex].HasMirror && _pedestaData[otherIndex].RecieveBeam)
                    {
                        pedestalA.ActivateEffect(pedestalB);
                    }
                }
                else
                {
                    if (_pedestaData[otherIndex].HasMirror && _pedestaData[senderIndex].RecieveBeam)
                    {
                        pedestalA.ActivateEffect(pedestalB);
                    }
                }
            }
        }
    }

    // Go through each node and get the pedestal that are paired with the sender pedestal
    // if the paired pedestal also has a miror, activate the beam effect
    // then check if constellation is complete
    private void PedestalChecker(int senderIndex, bool mirrorMode)
    {
        // Go through each node 
        for (int i = 0; i < _pedestalNodeList.Count; i++)
        {
            PedestalNode node = _pedestalNodeList[i];
            PedestalConstellation pedestalA = node.PedestalA;
            PedestalConstellation pedestalB = node.PedestalB;
            int otherIndex = -1;

            // get the pedestal that are paired with the sender pedestal
            if (pedestalA == _pedestalList[senderIndex])
            {
                otherIndex = _pedestalList.IndexOf(pedestalB);
            }
            else if(pedestalB == _pedestalList[senderIndex])
            {
                pedestalA = node.PedestalB;
                pedestalB = node.PedestalA;
                otherIndex = _pedestalList.IndexOf(pedestalB);
            }

            if (otherIndex != -1)
            {
                PedestalData senderData = _pedestaData[senderIndex];
                PedestalData otherData = _pedestaData[otherIndex];
                if (otherData.HasMirror && senderData.HasMirror && !node.NodeMirrored)
                {
                    node.NodeMirrored = true;
                    ConstellationChecker();
                }
                
                if (otherData.RecieveBeam && !node.NodeBeamed)
                {
                    node.NodeBeamed = true;
                    ConstellationChecker();
                }

                if (mirrorMode)
                {
                    if (senderData.HasMirror && senderData.RecieveBeam && !senderData.ShootingBeam)
                    {
                        pedestalA.ActivateEffect(pedestalB);
                        senderData.ShootingBeam = true;
                    }
                }
                else
                {
                    if (otherData.HasMirror && senderData.RecieveBeam && !otherData.ShootingBeam)
                    {
                        pedestalB.ActivateEffect(pedestalA);
                        otherData.ShootingBeam = true;
                    }
                }
            }
        }
    }

    // checks if constellation is complete, if so sends an event
    private void ConstellationChecker()
    {
        bool done = true;

        //checks if any nodes are don't have a beam on them
        // if they are not, returns false
        foreach (var node in _pedestalNodeList)
        {
            if(!node.NodeBeamed || !node.NodeMirrored)
            {
                done = false;
                return;
            }
        }

        if (done)
        {
            Debug.Log("Constellation Complete!");
            EventManager.EventTrigger(EventType.PUZZLE_DONE, 1);
        }
    }
}

// Data class for pedestal nodes
[Serializable]
public class PedestalNode
{
    public PedestalConstellation PedestalA;
    public PedestalConstellation PedestalB;
    public bool NodeMirrored = false; //use when a pedestal node has mirror at both ends
    public bool NodeBeamed = false; //use for when mirror and rift system is implemented
}

// Data class for indivigual pedestals
public class PedestalData
{
    //bools associated with if pedestals have mirrors
    public bool HasMirror = false; 
    //bools associated with if pedestals are shooting a beam
    public bool ShootingBeam = false; 
    //bools associated with if pedestals are receiving a beam
    public bool RecieveBeam = false; 
}
