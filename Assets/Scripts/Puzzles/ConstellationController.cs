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
    //list of bools associated with if pedestals have mirrors
    private List<bool> _mirroredPedestals = new List<bool>();
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
            _mirroredPedestals.Add(false);
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
            _mirroredPedestals[senderIndex] = true;
            PedestalChecker(senderIndex);
        }
        else
        {
            Debug.LogError("PedestalHasMirror sender is not in list!! fix this now");
        }
    }

    // Go through each node and get the pedestal that are paired with the sender pedestal
    // if the paired pedestal also has a miror, activate the beam effect
    // then check if constellation is complete
    private void PedestalChecker(int senderIndex)
    {
        // Go through each node 
        for (int i = 0; i < _pedestalNodeList.Count; i++)
        {
            PedestalNode node = _pedestalNodeList[i];
            PedestalConstellation pedestalA = node.PedestalA;
            PedestalConstellation pedestalB = node.PedestalB;

            // get the pedestal that are paired with the sender pedestal
            if (pedestalA == _pedestalList[senderIndex])
            {
                int otherIndex = _pedestalList.IndexOf(pedestalB);
                // if the paired pedestal also has a miror, activate the beam effect
                if (_mirroredPedestals[otherIndex] == true)
                {
                    pedestalA.ActivateEffect(pedestalB);
                    node.NodeMirrored = true;
                    ConstellationChecker();
                }
            }
            else if(pedestalB == _pedestalList[senderIndex])
            {
                int otherIndex = _pedestalList.IndexOf(pedestalA);
                // if the paired pedestal also has a miror, activate the beam effect
                if (_mirroredPedestals[otherIndex] == true)
                {
                    pedestalB.ActivateEffect(pedestalA);
                    node.NodeMirrored = true;
                    ConstellationChecker();
                }
            }
        }
    }

    // checks if constellation is complete, if so sends an event
    private void ConstellationChecker()
    {
        bool done = true;

        //checks if any nodes are don't have a mirror
        // if they are not, returns false
        foreach (var node in _pedestalNodeList)
        {
            if(node.NodeMirrored != true)
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
    public bool NodeRifted = false; //use for when mirror and rift system is implemented
}
