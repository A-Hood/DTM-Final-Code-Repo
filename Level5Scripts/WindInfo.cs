using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindInfo : MonoBehaviour
{
    public float moveSpeed = 9f;

    public Vector2 destination;

    public string direction;

    public bool startMoving = false;

    private void Update()
    {
        // If at destination.
        if (new Vector2(this.transform.position.x, this.transform.position.y) == destination) { Destroy(gameObject); }
        // Move wind.
        if (startMoving) { transform.position = Vector2.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime); }
    }
}
