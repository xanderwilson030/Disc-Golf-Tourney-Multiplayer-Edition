using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 *  This class handles game events for events going out during the course levels
 */

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    // Declaring our events
    public UnityEvent<int> e_TriggerHoleRatingText;
    public UnityEvent e_PauseMenuTriggered;


    private void Awake()
    {
        instance = this;

        // Initialize our events

        if (e_TriggerHoleRatingText == null)
        {
            e_TriggerHoleRatingText = new UnityEvent<int>();
        }

        if (e_PauseMenuTriggered == null)
        {
            e_PauseMenuTriggered = new UnityEvent();
        }
    }
}
