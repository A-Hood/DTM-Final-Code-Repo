using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowYAxis : MonoBehaviour
{
    private GameObject player;
    private GameManager gameManager;

    public float yStoppingPoint1, yStoppingPoint2;

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        StartCoroutine(GetPlayerReference());
    }

    private IEnumerator GetPlayerReference()
    {
        yield return new WaitUntil(() => GameObject.FindWithTag("Player"));
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (!gameManager.isPlayerDead && player != null)
        {
            // Set position of camera to players x-axis only. Stop camera movement when player hits or goes over set value.
            if (player.transform.position.y >= yStoppingPoint1 && player.transform.position.y <= yStoppingPoint2)
            {
                this.transform.position = new Vector2(this.transform.position.x, player.transform.position.y);
            }
        }
    }
}
