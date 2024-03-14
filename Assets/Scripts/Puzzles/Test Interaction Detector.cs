using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractionDetector : MonoBehaviour
{
    public GameObject _floatingTextPrefab; //Reference Floating Text Prefab
    private bool _player1Present = false;
    private bool _player2Present = false;

    // On enabling of the attached GameObject
    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_1_INTERACT, Player1InteractHandler);
        EventManager.EventSubscribe(EventType.PLAYER_2_INTERACT, Player2InteractHandler);
    }

    // On disabling of the attached GameObject
    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_1_INTERACT, Player1InteractHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_INTERACT, Player2InteractHandler);
    }

    void ShowFloatingText() //Define show floating text method
    {
        Instantiate(_floatingTextPrefab, transform.position, Quaternion.identity, transform);
    }
    
    // Set state variable when entering a region
    private void OnTriggerEnter(Collider collision)
    {        
        // If player 1 or player 2 enter the collider
        // set their respective bool to true
        if(collision.tag == "Player1")
        {
            _player1Present = true;
            
            if (_floatingTextPrefab) //Only show floating text if the prefab is assigned
            {
                ShowFloatingText(); //Show flaoting text method
            }
        }

        else if (collision.tag == "Player2")
        {
            _player2Present = true;

            if (_floatingTextPrefab) //Only show floating text if the prefab is assigned
            {
                ShowFloatingText(); //Show flaoting text method
            }
        }
    }

    // Reset state variable if no longer on a region
    private void OnTriggerExit(Collider collision)
    {  
        // If player 1 or player 2 leave the collider
        // set their respective bool to false
        if(collision.tag == "Player1")
        {
            _player1Present = false;
        }
        else if (collision.tag == "Player2")
        {
            _player2Present = false;
        }
    }

    private void Player1InteractHandler(object data)
    {
        if (data != null)
        {
            Debug.LogError("Player1InteractHandler is NOT null");
        }

        // Check if player 1 is actually near object
        if (_player1Present)
        {
            // Dummy print, to know it is working
            Debug.Log("Player 1 just interacted with a nearby object!");
        }
    }

    private void Player2InteractHandler(object data)
    {
        if (data != null)
        {
            Debug.LogError("Player2InteractHandler is NOT null");
        }

        // Check if player 2 is actually near object
        if (_player2Present)
        {
            // Dummy print, to know it is working
            Debug.Log("Player 2 just interacted with a nearby object!");
        }
    }
}
