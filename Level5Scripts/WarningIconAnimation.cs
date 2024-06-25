using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WarningIconAnimation : MonoBehaviour
{
    // All animation variables.
    [Header("Animation Settings")]
    public Sprite[] anim_Sprites; // Array holding all sprite assets.
    public float anim_Speed;
    private bool anim_Complete = false; // Variable for anim complete checks.
    private int anim_SpriteIndex; // Index of sprite array.
    private Image anim_Image;
    Coroutine anim_Coroutine;

    private void Awake()
    {
        anim_Image = this.GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(PlayAnimUI());
    }

    private void OnDisable()
    {
        anim_SpriteIndex = 0;
        StopAllCoroutines();
    }

    IEnumerator PlayAnimUI()
    {
        yield return new WaitForSeconds(anim_Speed); // Wait length of time between frames.
        if (anim_SpriteIndex >= anim_Sprites.Length) //  Check if the index is more than the sprite array's length.
        {
            anim_SpriteIndex = 0; // Reset sprite index.
        }

        anim_Image.sprite = anim_Sprites[anim_SpriteIndex]; // Set image to current sprite index.
        anim_SpriteIndex++; // Increment the sprite index.

        if (!anim_Complete) // Start again if the anim is not completed.
        {
            anim_Coroutine = StartCoroutine(PlayAnimUI());
        }
    }
}
