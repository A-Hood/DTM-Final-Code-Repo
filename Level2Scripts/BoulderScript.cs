using System.Collections;
using UnityEngine;

// ONLY USE THIS SCRIPT ON BOULDER, ONLY TO MOVE BOULDER FROM SPAWN LOCATION TO DEATH LOCATION !!

public class BoulderScript : MonoBehaviour
{
    #region Variables
    public Transform boulder_DeathLocation;

    private float boulder_moveSpeed = 10f;

    [Header("Animation Settings")]
    [SerializeField] private Sprite[] boulder_Sprites;
    private SpriteRenderer boulder_SpriteRenderer;
    private int boulder_SpriteArrayIndex;
    private float boulder_AnimSpeed = 0.1f;
    private Coroutine boulder_AnimCoroutine;
    #endregion

    private void Awake()
    {
       // Get reference to boulder sr
       boulder_SpriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();

       // Start boulder animation.
       StartCoroutine(boulder_Anim());
    }

    private void FixedUpdate()
    {
        // Move the boulder every frame towards ending position.
        if (this.transform.position.x != boulder_DeathLocation.position.x)
        {
            // Move enemy towards checkpoint currently on
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(boulder_DeathLocation.position.x, this.transform.position.y, 90f), boulder_moveSpeed * Time.deltaTime); // Move to checkpoint
        }
        // If boulder has made it then destroy it.
        else
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator boulder_Anim()
    {
        if (boulder_SpriteArrayIndex >= boulder_Sprites.Length) //  Check if the index is more than the sprite array's length.
        {
            boulder_SpriteArrayIndex = 0; // Reset sprite index.
        }

        boulder_SpriteRenderer.sprite = boulder_Sprites[boulder_SpriteArrayIndex]; // Set image to current sprite index.
        boulder_SpriteArrayIndex++; // Increment the sprite index.

        yield return new WaitForSeconds(boulder_AnimSpeed); // Wait length of time between frames.

        boulder_AnimCoroutine = StartCoroutine(boulder_Anim());
    }
}
