using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public Transform raycast_origin;
    private LayerMask player_layerMask;

    private void Awake()
    {
        player_layerMask = LayerMask.GetMask("Player");

        raycast_origin = this.transform.GetChild(1).gameObject.GetComponent<Transform>();
    }

    private void Update()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycast_origin.position, Vector2.up, 0.05f, player_layerMask);

        if (hit) { GameObject.FindWithTag("Player").GetComponent<LevelSixPlayer>().CheckPlayerProtection(this.gameObject.GetComponent<WindInfo>().direction, this.gameObject); }
    }
}
