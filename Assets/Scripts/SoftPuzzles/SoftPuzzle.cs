using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftPuzzle : MonoBehaviour
{
    #region External Data
    [SerializeField] private List<GameObject> _rewardObjs;
    #endregion

    #region Internal Data
    private List<ISoftPuzzleReward> _rewards = new List<ISoftPuzzleReward>();
    #endregion

    private void Awake()
    {
        // Make sure only 1 or 2 reward objects are assigned
        if (_rewardObjs.Count < 0 || _rewardObjs.Count > 2)
        {
            Debug.LogError("Please assign either 1 or 2 reward objects to SoftPuzzle!");
        }

        // Check if objects added subscribe to IReward
        foreach (GameObject obj in _rewardObjs)
        {
            var pickupObj = obj.GetComponent<ISoftPuzzleReward>();

            if (pickupObj == null)
            {
                Debug.LogError($"{obj} cannot be a reward in the SoftPuzzle!");
            }

            pickupObj.SetSoftPuzzle(this);
            _rewards.Add(pickupObj);
        }
    }

    public void CheckAllRewardsHeld()
    {
        foreach (ISoftPuzzleReward reward in _rewards)
        {
            // If all rewards are not held, exit out of function
            if (!reward.HeldInSoftPuzzle)
            {
                return;
            }
        }

        CompleteSoftPuzzle();
    }

    // Remove all references to the soft puzzle so players don't get teleported every time they touch reward obj
    private void CompleteSoftPuzzle()
    {
        foreach (ISoftPuzzleReward reward in _rewards)
        {
            reward.HeldInSoftPuzzle = false;
            reward.SetSoftPuzzle(null);
        }

        EventManager.EventTrigger(EventType.SOFTPUZZLE_COMPLETE, null);
    }

    public void SetRewardGrouper(Transform transform)
    {
        Debug.Log(_rewards.Count);
        foreach (ISoftPuzzleReward reward in _rewards)
        {
            reward.SetRewardGrouper(transform);
        }
    }
}
