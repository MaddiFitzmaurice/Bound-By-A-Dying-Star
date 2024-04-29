using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConstellationController : MonoBehaviour
{
    //List of pedestals in constellation
    [SerializeField] private List<PedestalConstellation> _pedestalList;

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

    // Sets value of preset pedestals
    public void PedestalPreset(PedestalLinkData linkData)
    {
        PedestalConstellation pedestalSender = linkData.sender;
        int senderIndex = _pedestalList.IndexOf(pedestalSender);
        
        // Set preset peddestal values so it is shooting beam
        pedestalSender.ActivateEffect();
        _pedestaData[senderIndex].ShootingBeam = true;
        _pedestaData[senderIndex].RecieveBeam = true;
        PedestalChecker(senderIndex);
    }

    // Set pedestal to have mirror, then run check function
    public void PedestalHasMirror(PedestalConstellation sender)
    {
        int senderIndex = _pedestalList.IndexOf(sender);
        // check to make sender is in list and that list is not empty
        if(senderIndex != -1 || _pedestalList.Count != 0)
        {
            _pedestaData[senderIndex].HasMirror = true;
            PedestalChecker(senderIndex);
        }
        else
        {
            Debug.LogError("PedestalHasMirror sender is not in list!! fix this now");
        }
    }

    // Set pedestal be facing right direction
    public void BeamRightDirection(PedestalConstellation sender)
    {
        int senderIndex = _pedestalList.IndexOf(sender);
        // check to make sender is in list and that list is not empty
        if(senderIndex != -1 || _pedestalList.Count != 0)
        {
            _pedestaData[senderIndex].RightBeamDirection = true;
        }
        else
        {
            Debug.LogError("BeamRightDirection sender is not in list!! fix this now");
        }
    }

    // Set all pedestal destinations to be recieving beam
    public void PedestalHasBeam(List<PedestalConstellation> pedestalDestinations)
    {
        // Go through pedestalDestinations list and set the destinations to be recieving beam
        foreach (PedestalConstellation pedestal in pedestalDestinations)
        {
            int senderIndex = _pedestalList.IndexOf(pedestal);
            // check to make sender is in list and that list is not empty
            if(senderIndex != -1 || _pedestalList.Count != 0)
            {
                _pedestaData[senderIndex].RecieveBeam = true;
                PedestalChecker(senderIndex);
            }
            else
            {
                Debug.LogError("PedestalHasBeam sender is not in list!! fix this now");
            }
        }
    }

    // Go through each pedestal
    // if the pedestal has a mirror, is recieving a beam but not shooting a beam
    // tell the pedestal to activate the beam effect
    // then check if constellation is complete
    private void PedestalChecker(int senderIndex)
    {
        PedestalConstellation pedestalSender = _pedestalList[senderIndex];
        List<PedestalConstellation> pedestalDestinations = pedestalSender.ReturnDestinations();

        foreach (var PedestalOther in pedestalDestinations)
        {
            PedestalData senderData = _pedestaData[senderIndex];

            if (senderData.HasMirror && senderData.RecieveBeam && !senderData.ShootingBeam)
            {
                pedestalSender.ActivateEffect();
                senderData.ShootingBeam = true;
            }
            ConstellationChecker();
        }
    }

    // checks if constellation is complete, if so sends an event
    private void ConstellationChecker()
    {
        bool done = true;

        foreach (var PedestalData in _pedestaData)
        {
            if(!PedestalData.RightBeamDirection || !PedestalData.RecieveBeam || !PedestalData.HasMirror)
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

// Data class for indivigual pedestals
//[Serializable]
public class PedestalData
{
    //bools associated with if pedestals have mirrors
    public bool HasMirror = false; 
    //bools associated with if pedestals are shooting a beam
    public bool ShootingBeam = false; 
    //bools associated with if pedestals are receiving a beam
    public bool RecieveBeam = false; 
    //bools associated with if beams are facing their correct direction
    public bool RightBeamDirection = false; 
}

// Data class for sending a peddestal and it's beam destinations to the controller 
//[Serializable]
public class PedestalLinkData
{
    // Pedestal reference
    public PedestalConstellation sender;
    // Pedestal's beam destination references
    public List<PedestalConstellation> pedestalDestinations;
    public PedestalLinkData(List<PedestalConstellation> pedestalDestinations, PedestalConstellation sender)
    {
        this.pedestalDestinations = pedestalDestinations;
        this.sender = sender;
    }
}
