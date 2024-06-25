using System.Collections;
using UnityEngine;

public class CameraFollowXAxis : MonoBehaviour
{
    private GameObject player;
    private GameManager gameManager;

    public float xStoppingPoint1, xStoppingPoint2;

    private void Start()
    {
        // Get reference to game manager.
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        // Get reference to player.
        StartCoroutine(GetPlayerReference());
    }

    // This component is normally spawned before the player,
    // use this coroutine so it only adds the player reference when the player is spawned.
    private IEnumerator GetPlayerReference()
    {
        yield return new WaitUntil(() => GameObject.FindWithTag("Player"));
        player = GameObject.FindWithTag("Player");
    }

    // Move camera holder's position to the player's x value only,
    // making sure that the camera is only staying between the  2 stopping points.
    private void Update()
    {
        if (!gameManager.isPlayerDead && player != null)
        {
            // Set position of camera to players x-axis only. Stop camera movement when player hits or goes over set value.
            if (player.transform.position.x >= xStoppingPoint1 && player.transform.position.x <= xStoppingPoint2)
            {
                this.transform.position = new Vector2(player.transform.position.x, this.transform.position.y);
            }
        }
    }
}
