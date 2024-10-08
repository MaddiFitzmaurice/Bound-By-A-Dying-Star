using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

public class PedestalConstController : MonoBehaviour
{
    #region EXTERNAL DATA
    //List of pedestals in constellation
    [Header("Pedestal Data")]
    [SerializeField] private List<ConstPedestal> _pedestalList;
    [SerializeField] private BeamEmitter _affordanceBeam;
    [Header("Cinematics")]
    [SerializeField] PlayableAsset _cutsceneDoor;
    [SerializeField] PlayableAsset _cutsceneBeam;
    [SerializeField] VideoClip _finalCutscene;
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

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.INGAME_CUTSCENE_FINISHED, FinalInGameCutsceneEnd);
        EventManager.EventSubscribe(EventType.PRERENDERED_CUTSCENE_FINISHED, FinalPreRenderedCutsceneEnd);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.INGAME_CUTSCENE_FINISHED, FinalInGameCutsceneEnd);
        EventManager.EventUnsubscribe(EventType.PRERENDERED_CUTSCENE_FINISHED, FinalPreRenderedCutsceneEnd);
    }

    void Start()
    {
        for (int i = 0; i < _pedestalList.Count; i++)
        {
            if (_pedestalList[i] == null)
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

        // Set preset pedestal values so it is shooting beam
        pedestalSender.ActivateEffect(); // Set up first mirror beam

        // Set all mirrors to be able to rotate
        foreach (ConstPedestal pedestal in _pedestalList)
        {
            pedestal.CanRotate();
        }

        pedestalSender.SetReflectingBeam(true);
        pedestalSender.SetReceivingBeam(true);
        PedestalChecker(senderIndex);
    }

    // Sets value of preset pedestals
    public void PedestalPreset(PedestalLinkData linkData)
    {
        ConstPedestal pedestalSender = linkData.sender;
        int senderIndex = _pedestalList.IndexOf(pedestalSender);

        // Set preset pedestal values so it is shooting beam
        pedestalSender.ActivateEffect();
        pedestalSender.SetReflectingBeam(true);
        pedestalSender.SetReceivingBeam(true);
        PedestalChecker(senderIndex);
    }

    // Set pedestal to have mirror, then run check function
    public void PedestalHasMirror(ConstPedestal sender)
    {
        int senderIndex = _pedestalList.IndexOf(sender);

        // Send star twinkle event
        EventManager.EventTrigger(EventType.LVL1_STAR_ACTIVATE, senderIndex);

        // check to make sender is in list and that list is not empty
        if (senderIndex != -1 || _pedestalList.Count != 0)
        {
            sender.SetHasMirror(true);
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
        if (senderIndex != -1 || _pedestalList.Count != 0)
        {
            sender.SetAligned(true);
        }
        else
        {
            Debug.LogError("BeamRightDirection sender is not in list!! fix this now");
        }
    }

    // Set all pedestal destinations to be receiving beam
    public void PedestalHasBeam(List<ConstPedestal> pedestalDestinations)
    {
        // Go through pedestalDestinations list and set the destinations to be receiving beam
        foreach (ConstPedestal pedestal in pedestalDestinations)
        {
            int senderIndex = _pedestalList.IndexOf(pedestal);
            // check to make sender is in list and that list is not empty
            if (senderIndex != -1 || _pedestalList.Count != 0)
            {
                pedestal.SetReceivingBeam(true);
                PedestalChecker(senderIndex);
            }
            else
            {
                Debug.LogError("PedestalHasBeam sender is not in list!! fix this now");
            }
        }
    }

    // Go through each pedestal
    // if the pedestal has a mirror and is receiving a beam but not shooting a beam
    // tell the pedestal to activate the beam effect
    // then check if constellation is complete
    private void PedestalChecker(int senderIndex)
    {
        ConstPedestal pedestalSender = _pedestalList[senderIndex];
        List<ConstPedestal> pedestalDestinations = pedestalSender.ReturnDestinations();

        foreach (var pedestalOther in pedestalDestinations)
        {
            if (pedestalSender.GetHasMirror() && pedestalSender.GetIsReceivingBeam() && !pedestalSender.GetIsReflectingBeam())
            {
                pedestalSender.ActivateEffect();
                pedestalSender.SetReflectingBeam(true);
            }
        }

        ConstellationChecker();
    }

    // checks if constellation is complete, if so sends an event
    private void ConstellationChecker()
    {
        bool done = true;
        bool mirrorsDone = true;

        for (int i = 0; i < _pedestalNum; i++)
        {
            ConstPedestal pedestal = _pedestalList[i];

            // Check if each pedestal has a mirror
            if (!pedestal.GetHasMirror())
            {
                mirrorsDone = false;
            }

            // Check if each pedestal has a mirror, receiving a beam and it's beam is facing the right direction
            if (!pedestal.GetIsAligned() || !pedestal.GetIsReceivingBeam() || !pedestal.GetHasMirror())
            {
                done = false;
            }

            // Check if each pedestal has a mirror, receiving a beam, shooting a beam and it's beam is facing the right direction
            if (pedestal.GetIsReflectingBeam() && pedestal.GetIsAligned() && pedestal.GetIsReceivingBeam() && pedestal.GetHasMirror())
            {
                pedestal.ActivateSkyBeam();
            }
        }

        // Activate affordance beam from the arch to the 1st pedestal
        // Also activate all the orbs
        if (mirrorsDone && !_affordanceBeamActivated)
        {
            StartCoroutine(BeamActivationSequence());
        }

        // Main puzzle is complete!
        if (done)
        {
            Debug.Log("Constellation Complete!");
            EventManager.EventTrigger(EventType.CONSTELLATION_COMPLETE, null);
            EventManager.EventTrigger(EventType.INGAME_CUTSCENE_PLAY, _cutsceneDoor);
        }
    }

    private IEnumerator BeamActivationSequence()
    {
        EventManager.EventTrigger(EventType.INGAME_CUTSCENE_PLAY, _cutsceneBeam);
        yield return new WaitForSeconds(2f);
        _affordanceBeamActivated = true;
        _affordanceBeam.SetBeamStatus(true);
        PedestalLinkData linkData = _pedestalList[0].GetPedestalLinkData();
        AffordanceBeamActivate(linkData);
    }

    #region EVENT HANDLERS
    public void FinalInGameCutsceneEnd(object data)
    {
        if (data is PlayableAsset cutscene)
        {
            if (cutscene == _cutsceneDoor)
            {
                EventManager.EventTrigger(EventType.PRERENDERED_CUTSCENE_PLAY, _finalCutscene);
            }
        }
    }

    public void FinalPreRenderedCutsceneEnd(object data)
    {
        if (data is VideoClip clip)
        {
            if (clip == _finalCutscene)
            {
                EventManager.EventTrigger(EventType.DISABLE_GAMEPLAY_INPUTS, null);
                EventManager.EventTrigger(EventType.ENABLE_MAINMENU_INPUTS, null);
                EventManager.EventTrigger(EventType.MAIN_MENU, null);
            }
        }
    }
    #endregion
}

// Data class for sending a pedestal and its beam destinations to the controller 
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