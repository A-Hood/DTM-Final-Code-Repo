using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeScript : MonoBehaviour
{
    public int lane;

    [SerializeField] private float move_speed;

    public Vector2 destination;

    private void FixedUpdate()
    {
        float step = Time.deltaTime * move_speed;

        // Bee travel to destination.
        transform.position = Vector2.MoveTowards(this.transform.position, destination, step);

        // Upon getting to destination, play sound and delete gameobject.
        if (this.transform.position.y == destination.y) {
            GameObject.FindWithTag("LevelManager").GetComponent<LevelNineManager>().PlaySound(1, GameObject.FindWithTag("LevelManager").GetComponent<LevelNineManager>().sound_conch[lane - 1]);
            Destroy(gameObject); 
        }
    }
}
