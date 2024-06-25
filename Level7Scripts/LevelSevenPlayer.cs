using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSevenPlayer : MonoBehaviour
{
    private bool isCollidingWithDoor = false;
    private PlayerScript player_script;
    private GameManager gameManager;
    public LevelSevenManager levelSevenManager;

    private GameObject door_collidingWith;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        player_script = this.GetComponent<PlayerScript>();
    }

    private void Update()
    {
        if (isCollidingWithDoor && Input.GetKeyDown(gameManager.key_Interact)) { StartCoroutine(levelSevenManager.InteractWithDoor(door_collidingWith)); }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If colliding with door.
        if (collision.gameObject.tag == "Door") { 
            isCollidingWithDoor = true;
            door_collidingWith = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // If exit colliding door.
        if (collision.gameObject.tag == "Door") { 
            isCollidingWithDoor = false;
            door_collidingWith = null;
        }
    }
}