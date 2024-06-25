using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel4Script : MonoBehaviour
{
    private GameManager gameManager;

    private LevelFourManager level_manager;

    private PlayerScript player_script;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        level_manager = GameObject.FindWithTag("LevelManager").GetComponent<LevelFourManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        { 
            player_script = collision.gameObject.GetComponent<PlayerScript>();

            FinishLevel();
        }
    }

    private void FinishLevel()
    {
        gameManager.acceptInput = false;
        player_script.StopMovementAnimation();

        StartCoroutine(level_manager.cr_FinishLevel());
    }
}
