using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelOnePlayer : MonoBehaviour
{
    private GameObject a_collidedWithWood;

    private GameObject gameManager;
    private UIManager uiManager;
    private LevelOneManager levelOneManager;

    [HideInInspector] public bool colWithWood;
    [HideInInspector] public bool colWithBridge;
    [HideInInspector] public bool colWithDog;

    [HideInInspector] public bool canPickUpWood;

    private void Awake()
    {
        // Get game manager game object.
        gameManager = GameObject.FindWithTag("GameManager");
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        levelOneManager = GameObject.FindWithTag("LevelOneManager").GetComponent<LevelOneManager>();
    }

    private void Update()
    {
        if (!canPickUpWood)
        {
            CheckForDogInteract();
        }
        // Check both of these funcions every frame.
        else
        {
            PickUpWood();
            DepositWood();
        }
    }

    // check for interaction with dog
    private void CheckForDogInteract()
    {
        if (Input.GetKeyDown(gameManager.GetComponent<GameManager>().key_Interact) && colWithDog)
        {
            canPickUpWood = true;
            levelOneManager.HideKeybindText();
            StartCoroutine(levelOneManager.cr_TalkToVermillionDog());
        }
    }

    // ** Allows the player to pick up wood piece if collision is detected and the pick up key is pressed ** \\
    private void PickUpWood()
    {
        if (Input.GetKeyDown(gameManager.GetComponent<GameManager>().key_Interact) && colWithWood)
        {
            // Destroy wood plank game object
            Destroy(a_collidedWithWood);
            // Play pickup audio clip
            levelOneManager.PlaySound(levelOneManager.soundclip_pickup);
            // Increment heldWood
            gameManager.GetComponent<GameManager>().heldWood++;
            // Set text
            levelOneManager.SetHeldWoodText();
        }
    }

    // ** Allows the player to deposit wood in the bridge if collision with bridge is detected and deposit key is pressed ** \\
    private void DepositWood()
    {
        if (Input.GetKeyDown(gameManager.GetComponent<GameManager>().key_Interact) && colWithBridge)
        {
            if (gameManager.GetComponent<GameManager>().heldWood >= 1)
            {
                if (gameManager.GetComponent<GameManager>().bridgeStoredWood < 10)
                {
                    gameManager.GetComponent<GameManager>().heldWood--;
                    gameManager.GetComponent<GameManager>().bridgeStoredWood++;
                    // Play deposit audio clip
                    levelOneManager.PlaySound(levelOneManager.soundclip_deposit);
                    // Change bridge sprite to current amount of wood deposited.
                    levelOneManager.GetComponent<LevelOneManager>().SetBridgeSprite();
                    // Set text
                    levelOneManager.SetDepositedWoodText();
                    levelOneManager.SetHeldWoodText();
                    // Check is current stored wood is equal to 10.
                    if (gameManager.GetComponent<GameManager>().bridgeStoredWood == 10)
                    {
                        levelOneManager.audio_generalSource.Stop();
                        // Play bridge complete audio clip
                        levelOneManager.PlaySound(levelOneManager.soundclip_complete);
                        // Destroy collider blocking player from bridge
                        levelOneManager.GetComponent<LevelOneManager>().DestroyBridgeBlockCollider();
                        // hide ui keybind prompt
                        levelOneManager.HideKeybindText();
                        // start bridge completed dialogue with vermillion
                        StartCoroutine(levelOneManager.cr_BridgeCompletedDialogue());
                    }
                }
            }
        }
    }

    // If player has collided with something.
    private void OnCollisionEnter2D(Collision2D col)
    {
        // If can't pick up wood yet.
        if (!canPickUpWood)
        {
            // If colliding with dog.
            if (col.gameObject.tag == "Dog")
            {
                colWithDog = true;
                levelOneManager.SetKeybindText("Press ", " to talk to the Vermillion Dog!", gameManager.GetComponent<GameManager>().key_Interact);
            }
        }
        // If can pick up wood.
        else
        {
            // If player has collided with wood plank.
            if (col.gameObject.tag == "Wood")
            {
                colWithWood = true;
                a_collidedWithWood = col.gameObject;
                levelOneManager.SetKeybindText("Press ", " to pick up wood!", gameManager.GetComponent<GameManager>().key_Interact);
            }
            // If player has collided with bridge.
            if (col.gameObject.tag == "Bridge")
            {
                colWithBridge = true;
                if (gameManager.GetComponent<GameManager>().bridgeStoredWood < 10)
                {
                    levelOneManager.SetKeybindText("Press ", " to deposit in bridge!", gameManager.GetComponent<GameManager>().key_Interact);
                }
            }
        }
        // If collided with "portal".
        if (col.gameObject.tag == "Portal")
        {
            // Start coroutine for teleporting player.
            StartCoroutine(levelOneManager.GetComponent<TeleportManager>().TeleportPlayer(col.gameObject.GetComponent<TeleportInfo>().teleportTarget));
        }
    }

    // If player has stopped colliding with something. (Hide keybind text)
    private void OnCollisionExit2D(Collision2D col)
    {
        a_collidedWithWood = null;

        // If left wood collision.
        if (col.gameObject.tag == "Wood")
        {
            colWithWood = false;
            levelOneManager.HideKeybindText();
        }
        // If left bridge collision.
        if (col.gameObject.tag == "Bridge")
        {
            colWithBridge = false;
            levelOneManager.HideKeybindText();
        }
        // If left dog collision.
        if (col.gameObject.tag == "Dog")
        {
            colWithDog = false;
            levelOneManager.HideKeybindText();
        }
    }
}
