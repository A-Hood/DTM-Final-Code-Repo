using System.Collections;
using UnityEngine;

/// <summary>
/// Attatch this to the camera holder game object whenever you want to
/// move the camera to the player's position.
/// </summary>

public class CameraFollowAllAxis : MonoBehaviour
{
    private GameObject player;
    private GameManager gameManager;

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

    // Move the camera holder's position to the players x and y values every frame.
    private void Update()
    {
        if (!gameManager.isPlayerDead && player != null) { this.gameObject.transform.position = player.transform.position; }
    }
}
