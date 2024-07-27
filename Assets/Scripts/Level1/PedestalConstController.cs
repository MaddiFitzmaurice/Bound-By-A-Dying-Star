using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PedestalConstController : MonoBehaviour
{
    #region EXTERNAL DATA
    //List of pedestals in constellation
    [Header("Pedestal Data")]
    [SerializeField] private List<ConstPedestal> _pedestalList;
    [SerializeField] private List<PedestalData> _pedestaData = new List<PedestalData>();
    [SerializeField] private BeamEmitter _affordanceBeam;
    [Header("Cinematics")]
    [SerializeField] PlayableAsset _cutsceneDoor;
    [SerializeField] PlayableAsset _cutsceneBeam;
    #endregion

    #region INTERNAL DATA
    private int _pedestalNum;
    private bool _affordanceBeamActivated = false;
    #endregion

    void Awake()
    {
        _pedestalNum = _pedestalList.Count;
        EventManager.EventInitialise(EventType.LVL1_STARBEAM_ACTIVATE);
        EventManager.EventInitialise(EventType.LVL1_STAR_ACTIVATE);
    }

    void Start()
    {
        //set all _mirroredPedestals bools to false
        for (int i = 0; i < _pedestalNum; i++)
        {
            _pedestaData.Add(new PedestalData());
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

    // Activate Affordance beam
    public void AffordanceBeamActivate(PedestalLinkData linkData)
    {
        ConstPedestal pedestalSender = linkData.sender;
        int senderIndex = _pedestalList.IndexOf(pedestalSender);
        
        // Set preset peddestal values so it is shooting beam
        pedestalSender.ActivateEffect();
        foreach (ConstPedestal pedestal in _pedestalList)
        {
            pedestal.ActivateOrb();
        }

        _pedestaData[senderIndex].ShootingBeam = true;
        _pedestaData[senderIndex].RecieveBeam = true;
        PedestalChecker(senderIndex);
    }

    // Sets value of preset pedestals
    public void PedestalPreset(PedestalLinkData linkData)
    {
        ConstPedestal pedestalSender = linkData.sender;
        int senderIndex = _pedestalList.IndexOf(pedestalSender);
        
        // Set preset peddestal values so it is shooting beam
        pedestalSender.ActivateEffect();
        _pedestaData[senderIndex].ShootingBeam = true;
        _pedestaData[senderIndex].RecieveBeam = true;
        PedestalChecker(senderIndex);
    }

    // Set pedestal to have mirror, then run check function
    public void PedestalHasMirror(ConstPedestal sender)
    {
        int senderIndex = _pedestalList.IndexOf(sender);

        // Send star twinkle event
        EventManager.EventTrigger(EventType.LVL1_STAR_ACTIVATE, senderIndex);

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
    public void BeamRightDirection(ConstPedestal sender)
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
    public void PedestalHasBeam(List<ConstPedestal> pedestalDestinations)
    {
        // Go through pedestalDestinations list and set the destinations to be recieving beam
        foreach (ConstPedestal pedestal in pedestalDestinations)
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
    // if the pedestal has a mirror and is recieving a beam but not shooting a beam
    // tell the pedestal to activate the beam effect
    // then check if constellation is complete
    private void PedestalChecker(int senderIndex)
    {
        ConstPedestal pedestalSender = _pedestalList[senderIndex];
        List<ConstPedestal> pedestalDestinations = pedestalSender.ReturnDestinations();

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
        bool mirrorsDone = true;

        for (int i = 0; i < _pedestalNum; i++)
        {
            // Check if each pedestal has a mirror
            if(!_pedestaData[i].HasMirror)
            {
                mirrorsDone = false;
            }

            // Check if each pedestal has a mirror, recieving a beam and it's beam is facing the right direction
            if(!_pedestaData[i].RightBeamDirection || !_pedestaData[i].RecieveBeam || !_pedestaData[i].HasMirror)
            {
                done = false;
            }

            // Check if each pedestal has a mirror, recieving a beam, shooting a beam and it's beam is facing the right direction
            if(_pedestaData[i].ShootingBeam && _pedestaData[i].RightBeamDirection && _pedestaData[i].RecieveBeam &&_pedestaData[i].HasMirror)
            {
                _pedestalList[i].ActivateSkyBeam();
            }
        }

        // Activate affordance beam from the arch to the 1st pedestal
        // Also activate all the orbs
        if (mirrorsDone & !_affordanceBeamActivated)
        {
            //_affordanceBeamActivated = true;
            //_affordanceBeam.SetBeamStatus(true);
            //PedestalLinkData linkData = _pedestalList[0].GetPedestalLinkData();
            //AffordanceBeamActivate(linkData);
            StartCoroutine(BeamActivationSequence());
        }

        // Main puzzle is complete!
        if (done)
        {
            Debug.Log("Constellation Complete!");
            EventManager.EventTrigger(EventType.CUTSCENE_PLAY, _cutsceneDoor);
        }
    }

    private IEnumerator BeamActivationSequence()
    {
        EventManager.EventTrigger(EventType.CUTSCENE_PLAY, _cutsceneBeam);
        yield return new WaitForSeconds(2f);
        _affordanceBeamActivated = true;
        _affordanceBeam.SetBeamStatus(true);
        PedestalLinkData linkData = _pedestalList[0].GetPedestalLinkData();
        AffordanceBeamActivate(linkData);
    }
}

// Data class for individual pedestals
[Serializable]
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
    public ConstPedestal sender;
    // Pedestal's beam destination references
    public List<ConstPedestal> pedestalDestinations;
    public PedestalLinkData(List<ConstPedestal> pedestalDestinations, ConstPedestal sender)
    {
        this.pedestalDestinations = pedestalDestinations;
        this.sender = sender;
    }
}
