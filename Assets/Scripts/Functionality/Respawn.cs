using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    #region External Data
    [SerializeField] private Transform _forwardPuzzleRespawnPoint;
    [SerializeField] private Transform _backwardPuzzleRespawnPoint1;
    [SerializeField] private Transform _backwardPuzzleRespawnPoint2;
    #endregion

    #region Internal Data
    private Transform _currentSpawnPoint;
    private int _p1SpawnPointGroup = 0;     // What player 1's back spawn group will be
    private int _p2SpawnPointGroup = 0;     // What player 2's back spawn group will be
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        // For forward puzzle
        if (_currentSpawnPoint == _forwardPuzzleRespawnPoint)
        {
            if (other.GetComponent<Player1>() || other.GetComponent<Player2>())
            {
                other.gameObject.transform.position = _currentSpawnPoint.position;
            }
        }
        else
        {
            // Assign Player 1 to a spawn point grouping
            if (other.GetComponent<Player1>())
            {
                if (_p1SpawnPointGroup == 1)
                {
                    other.gameObject.transform.position = _backwardPuzzleRespawnPoint1.position;
                }
                else if (_p1SpawnPointGroup == 2)
                {
                    other.gameObject.transform.position = _backwardPuzzleRespawnPoint2.position;
                }
            }
            // Assign Player 2 to a spawn point grouping
            else if (other.GetComponent<Player2>())
            {
                if (_p2SpawnPointGroup == 1)
                {
                    other.gameObject.transform.position = _backwardPuzzleRespawnPoint1.position;
                }
                else if (_p2SpawnPointGroup == 2)
                {
                    other.gameObject.transform.position = _backwardPuzzleRespawnPoint2.position;
                }
            }
        }
    }

    // Assuming players should not be separated (as per the locked in designs)
    public void ChangeToForwardRespawn()
    {
        _currentSpawnPoint = _forwardPuzzleRespawnPoint;
    }

    public void ChangeToBackRespawn(List<SoftPuzzleFixedPlatform> fixedPlatforms)
    {
        _currentSpawnPoint = null;

        if (fixedPlatforms.Count == 2)
        {
            for (int i = 0; i < fixedPlatforms.Count; i++)
            {
                if (fixedPlatforms[i].HasPlayer1)
                {
                    _p1SpawnPointGroup = i + 1;
                }

                if (fixedPlatforms[i].HasPlayer2)
                {
                    _p2SpawnPointGroup = i + 1;
                }
            }
        }
        else if (fixedPlatforms.Count == 1)
        {
            _p1SpawnPointGroup = 1;
            _p2SpawnPointGroup = 1;
        }
        else
        {
            Debug.Log("Should there always be fixed platforms on the back puzzle?");
        }
    }
}
