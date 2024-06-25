using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelNinePlayer : MonoBehaviour
{
    private bool isCollidingWithBee;
    private GameObject bee_collidedWithGO;
    private int bee_collectedCount = 0;

    private bool isCollidingWithJadeBone;
    private GameObject jb_collidedWithGO;
    private int jb_collectedCount = 0;

    public bool canCollectBees = false;

    private GameManager gameManager;
    public LevelNineManager manager_levelNine;

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (canCollectBees)
        {
            if (Input.GetKeyDown(gameManager.key_Interact) && isCollidingWithBee)
            {
                Destroy(bee_collidedWithGO);
                bee_collectedCount++;
                if (bee_collectedCount == 6) { manager_levelNine.func_StartMinigame(); }
            }
            if (Input.GetKeyDown(gameManager.key_Interact) && isCollidingWithJadeBone)
            {
                Destroy(jb_collidedWithGO);
                jb_collectedCount++;
                if (jb_collectedCount == 5) { StartCoroutine(manager_levelNine.cr_FinishLevel()); /* finish level here */ }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If collided with cutscene trigger.
        if (collision.gameObject == manager_levelNine.cutsceneTrigger)
        {
            Destroy(manager_levelNine.cutsceneTrigger);
            StartCoroutine(manager_levelNine.cr_FirstTalkWithMictlan());
        }
        // If enter collision with bee.
        if (collision.gameObject.tag == "Bee") { 
            isCollidingWithBee = true;
            bee_collidedWithGO = collision.gameObject;
            if (canCollectBees)
                manager_levelNine.ShowInteractKeybindText("Press ", " to pickup the <color=#FFBF00>b</color><color=#000000>e</color><color=#FFBF00>e</color>");
        }
        // If enter collision with jade bone.
        if (collision.gameObject.tag == "JadeBone")
        {
            isCollidingWithJadeBone = true;
            jb_collidedWithGO = collision.gameObject;
            manager_levelNine.ShowInteractKeybindText("Press ", " to pickup the <color=#006424>Jade Bones</color>");
        }
        // Change sorting layer.
        if (collision.gameObject.name == "ChangeSortingLayerTo2") { this.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2; }
        if (collision.gameObject.name == "ChangeSortingLayerTo7") { this.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 7; }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // If exiting bee collision.
        if (collision.gameObject.tag == "Bee")
        {
            isCollidingWithBee = false;
            bee_collidedWithGO = null;
            manager_levelNine.HideInteractKeybindText();
        }
        // If exiting jade bone collision.
        if (collision.gameObject.tag == "JadeBone")
        {
            isCollidingWithJadeBone = false;
            jb_collidedWithGO = null;
            manager_levelNine.HideInteractKeybindText();
        }
        // Change sorting layer to normal.
        if (collision.gameObject.name == "ChangeSortingLayerTo2" || collision.gameObject.name == "ChangeSortingLayerTo6")
            this.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
    }
}
