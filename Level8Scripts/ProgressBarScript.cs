using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarScript : MonoBehaviour
{
    [SerializeField] private Transform bar_start, bar_end, player_head_transform;

    public void MoveProgressBar(float min, float max, float current)
    {
        float percent_player = (current / (max - min)); // work out percentage of way through the level (player)
        float bar_difference = bar_end.transform.position.x - bar_start.transform.position.x; // work out difference between start and end of bar (pb)
        float bar_toAdd = bar_difference * percent_player; // work out how much to add to the initial bar start for the player head

        // Move player head position to percentage across.
        if (percent_player <= 1)
            player_head_transform.position = new Vector2(bar_start.transform.position.x + bar_toAdd, player_head_transform.position.y); // set player head's position
    }
}