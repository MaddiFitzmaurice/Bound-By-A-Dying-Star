using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class PedestalConstControllerOLD : MonoBehaviour
{
    #region EXTERNAL DATA
    //List of pedestals in constellation
    [Header("Pedestal Data")]
    [SerializeField] private List<ConstPedestalOLD> _pedestalList;
    [SerializeField] private List<PedestalDataOLD> _pedestaData = new List<PedestalDataOLD>();
    [Header("Cinematics")]
    [SerializeField] PlayableAsset _cutsceneDoor;
    #endregion

    #region INTERNAL DATA
    private int _pedestalNum;
    #endregion

    void Awake()
    {
        _pedestalNum = _pedestalList.Count;
        EventManager.EventInitialise(EventType.LVL1_STARBEAM_ACTIVATE);
    }

    void Start()
    {
        //set all _mirroredPedestals bools to false
        for (int i = 0; i < _pedestalNum; i++)
        {
            _pedestaData.Add(new PedestalDataOLD());
        }

        for (int i = 0; i < _pedestalList.Count; i++)
        {
            if(_pedestalList[i] == null)
            {
                Debug.LogError("_pedestalList has an empty value!!! fix this now");
            }
            else
            {
                // Set pedestal id to be index in list
                _pedestalList[i].SetID(i);
            }
        }
    }

    // Sets value of preset pedestals
    public void PedestalPreset(PedestalLinkDataOLD linkData)
    {
        ConstPedestalOLD pedestalSender = linkData.sender;
        int senderIndex = _pedestalList.IndexOf(pedestalSender);
        
        // Set preset peddestal values so it is shooting beam
        pedestalSender.ActivateEffect();
        _pedestaData[senderIndex].ShootingBeam = true;
        _pedestaData[senderIndex].RecieveBeam = true;
        PedestalChecker(senderIndex);
    }

    // Set pedestal to have mirror, then run check function
    public void PedestalHasMirror(ConstPedestalOLD sender)
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
    public void BeamRightDirection(ConstPedestalOLD sender)
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
    public void PedestalHasBeam(List<ConstPedestalOLD> pedestalDestinations)
    {
        // Go through pedestalDestinations list and set the destinations to be recieving beam
        foreach (ConstPedestalOLD pedestal in pedestalDestinations)
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
        ConstPedestalOLD pedestalSender = _pedestalList[senderIndex];
        List<ConstPedestalOLD> pedestalDestinations = pedestalSender.ReturnDestinations();

        foreach (var PedestalOther in pedestalDestinations)
        {
            PedestalDataOLD senderData = _pedestaData[senderIndex];

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

        for (int i = 0; i < _pedestalNum; i++)
        {
            if(!_pedestaData[i].RightBeamDirection || !_pedestaData[i].RecieveBeam || !_pedestaData[i].HasMirror)
            {
                done = false;
            }

            if(_pedestaData[i].ShootingBeam && _pedestaData[i].RightBeamDirection && _pedestaData[i].RecieveBeam &&_pedestaData[i].HasMirror)
            {
                _pedestalList[i].ActivateSkyBeam();
            }
        }

        if (done)
        {
            Debug.Log("Constellation Complete!");
            EventManager.EventTrigger(EventType.PLAY_CINEMATIC, _cutsceneDoor);
        }
    }
}

// Data class for individual pedestals
[Serializable]
public class PedestalDataOLD
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
public class PedestalLinkDataOLD
{
    // Pedestal reference
    public ConstPedestalOLD sender;
    // Pedestal's beam destination references
    public List<ConstPedestalOLD> pedestalDestinations;
    public PedestalLinkDataOLD(List<ConstPedestalOLD> pedestalDestinations, ConstPedestalOLD sender)
    {
        this.pedestalDestinations = pedestalDestinations;
        this.sender = sender;
    }
}
