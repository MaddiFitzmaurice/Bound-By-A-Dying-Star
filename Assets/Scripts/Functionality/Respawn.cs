using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Transform _respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player1>() || other.GetComponent<Player2>())
        {
            other.gameObject.transform.position = _respawnPoint.position;
        }
    }
}
