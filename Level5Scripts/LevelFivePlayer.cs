using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFivePlayer : MonoBehaviour
{
    private string currentlyProtectedFrom;
    private bool hasPlayerDied = false;

    private void Update()
    {
        CheckBlockerType();
    }

    // Use this to check every frame for blocker walked on top of.
    private void CheckBlockerType()
    {
        // Get layer mask.
        var layer_mask = LayerMask.GetMask("Blocker");
        // Fire raycast.
        var hit_info = Physics2D.Raycast(this.gameObject.transform.GetChild(1).gameObject.transform.position, -Vector2.up, 0.01f, layer_mask);
        // If raycast hit.
        if (hit_info)
        {
            if (currentlyProtectedFrom != hit_info.transform.name.ToLower())
            {
                currentlyProtectedFrom = hit_info.transform.name.ToLower();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If collided with wind.
        if (collision.gameObject.tag == "Wind")
        {
            Debug.Log(currentlyProtectedFrom + " : " + collision.gameObject.GetComponent<WindInfo>().direction);
            if (collision.gameObject.GetComponent<WindInfo>().direction != currentlyProtectedFrom && !hasPlayerDied) { 
                hasPlayerDied = true;
                GameObject.FindWithTag("LevelFiveManager").GetComponent<LevelFiveManager>().PlayerDeath();
            }
        }
        // If collided with "start spawning wind" collider.
        if (collision.gameObject.name == "StartSpawnCollider")
        {
            GameObject.FindWithTag("LevelFiveManager").GetComponent<LevelFiveManager>().canSpawnWind = true;
            Destroy(collision.gameObject);
        }
    }
}
