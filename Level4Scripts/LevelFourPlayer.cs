using UnityEngine;

public class LevelFourPlayer : MonoBehaviour
{
    private GameManager gameManager;
    public IceMeltScript iceMeltScript;
    public LevelFourManager manager_level;

    bool isCollidingWithIceBlock = false;
    bool isCollidingWithKey = false;
    bool isCollidingWithDoor = false;

    GameObject keyGameObject;
    GameObject door_colliding;

    bool isIceMeltRunning = false;


    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        bool isButton = Input.GetKey(gameManager.key_Interact);
        if (!gameManager.hasKey)
        {
            if (isCollidingWithIceBlock)
            {
                if (isButton && !isIceMeltRunning)
                {
                    Debug.Log("Has Detected Button Pressed");
                    if (!iceMeltScript.isIceMelting) { StartCoroutine(iceMeltScript.MeltIce()); }
                    isIceMeltRunning = true;
                }
            }
            if (isCollidingWithKey)
            {
                if (Input.GetKeyDown("f")) { CollectKey(keyGameObject); }
            }
        }
        else
        {
            if (isCollidingWithDoor && Input.GetKeyDown(gameManager.key_Interact))
            {
                OpenDoor();
            }
        }
        if (!isButton && isIceMeltRunning)
        {
            StopAllCoroutines();
            isIceMeltRunning = false;
        }
    }
           

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "IceCube")
        {
            iceMeltScript = collision.gameObject.GetComponent<IceMeltScript>();
            isCollidingWithIceBlock = true;
            manager_level.ShowKeybindText("Hold ", " to melt the ice cube!", gameManager.key_Interact);
        }
        if(collision.gameObject.tag == "Key")
        {
            isCollidingWithKey = true;
            keyGameObject = collision.gameObject;
            manager_level.ShowKeybindText("Press ", " pick up the key!", gameManager.key_Interact);
        }
        if(collision.gameObject.tag == "Door")
        {
            isCollidingWithDoor = true;
            door_colliding = collision.gameObject.transform.parent.gameObject.transform.parent.gameObject;

            if (!gameManager.hasKey) { manager_level.DisplayDoorTextKeybind(false); }
            else { manager_level.DisplayDoorTextKeybind(true); }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "IceCube")
        {
            if(isIceMeltRunning)
            {
                if (iceMeltScript != null)
                    StopAllCoroutines();
                iceMeltScript = null;
                isCollidingWithIceBlock = false;
            }
            manager_level.HideKeybindText();
        }
        if(collision.gameObject.tag == "Key")
        {
            isCollidingWithKey = false;
            keyGameObject = null;
            manager_level.HideKeybindText();
        }
        if (collision.gameObject.tag == "Door")
        {
            isCollidingWithDoor = false;
            door_colliding = null;
            manager_level.HideKeybindText();
        }
    }

    private void CollectKey(GameObject key)
    {
        Destroy(key.gameObject);
        manager_level.PlaySound(manager_level.sound_pickupKey);
        gameManager.hasKey = true;
    }

    private void OpenDoor()
    {
        Destroy(door_colliding);
        gameManager.hasKey = false;
        manager_level.PlaySound(manager_level.sound_doorOpen);
        manager_level.HideKeybindText();
    }
}