using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEightPlayer : MonoBehaviour
{
    private GameManager gameManager;

    public LevelEightManager level_manager;

    private bool isCollidingWithBoat = false;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (isCollidingWithBoat && Input.GetKeyDown(gameManager.key_Interact) && !level_manager.hasEnteredBoat) { level_manager.EnterBoat(); }        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Boat") { isCollidingWithBoat = true; }

        if (collision.gameObject.tag == "FinishCollider") { level_manager.FinishLevel(); }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Boat") { isCollidingWithBoat = false; }
    }
}
