using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBoulderCollision : MonoBehaviour
{
    private LevelTwoManager levelTwoManager;

    private void Awake()
    {
        // Get reference to level manager.
        levelTwoManager = GameObject.FindWithTag("LevelTwoManager").GetComponent<LevelTwoManager>();
    }

    // Check whether boulder collides with the player.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);

        if (collision.gameObject.tag == "Player")
        {
            StopAllCoroutines();
            levelTwoManager.PlayerDeath(GameObject.FindWithTag("Player"));
        }
    }
}
