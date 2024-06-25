using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayAnimationScript : MonoBehaviour
{
    #region Animation Settings
    [Header("Animation Settings")]
    public Sprite[] anim_Sprites;
    public float anim_Speeds;
    private bool anim_Complete = false;
    private int anim_SpriteIndex;
    private SpriteRenderer anim_Image;
    Coroutine anim_Coroutine;
    private bool anim_CoroutineRunning;

    [Space(10)]
    [Header("Should the animation be running?")]
    public bool shouldAnimationBeRunning;
    #endregion

    [Header("Default Sprite")]
    public Sprite sprite_default;

    private void Awake()
    {
        // Get reference to sprite renderer.
        anim_Image = this.GetComponent<SpriteRenderer>();
        // Start the play animation coroutine.
        StartCoroutine(cr_PlayAnimUI());
    }

    public IEnumerator cr_PlayAnimUI()
    {
        yield return new WaitForSeconds(anim_Speeds); // Wait length of time between frames.
        if (shouldAnimationBeRunning)
        {
            if (anim_SpriteIndex >= anim_Sprites.Length) //  Check if the index is more than the sprite array's length.
            {
                anim_SpriteIndex = 0; // Reset sprite index.
            }

            anim_Image.sprite = anim_Sprites[anim_SpriteIndex]; // Set image to current sprite index.
            anim_SpriteIndex++; // Increment the sprite index.
        }

        if (!anim_Complete) // Start again if the anim is not completed.
        {
            anim_Coroutine = StartCoroutine(cr_PlayAnimUI());
        }
    }

    public void ResetSprite() { this.anim_Image.sprite = sprite_default; }
}
