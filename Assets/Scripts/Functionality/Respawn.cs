using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    #region External Data
    [SerializeField] public Transform ForwardPuzzleRespawnPoint;
    [SerializeField] public Transform BackwardPuzzleRespawnPoint;
    [SerializeField] public Transform CurrentSpawnPoint;
    #endregion

    #region Internal Data
    
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player1>() || other.GetComponent<Player2>())
        {
            other.gameObject.transform.position = CurrentSpawnPoint.position;
        }
    }
}
