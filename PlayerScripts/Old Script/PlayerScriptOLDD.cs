using System.Collections;
using UnityEngine;

public class PlayerScriptOLDD : MonoBehaviour
{
    #region Variables
    #region Movement Variables
    private float moveSpeed;

    // ** 0 = non-running speed , 1 = running speed ** \\
    [Header("Movement")]
    [Space(1)]
    [SerializeField] private float[] moveSpeeds = { 2.8f, 3.5f };

    private Vector3 movement;
    private Vector2 playerInput;
    #endregion

    #region Managers
    [Space(2)]
    [Header("Managers")]
    [Space(1)]
    private GameManager gameManager;
    #endregion

    #region Sound Variables
    [Space(10)]
    [Header("Sound Effect Settings !!1!!111!!")]
    [SerializeField] private AudioClip[] audio_footstepSounds;
    [SerializeField] private float audio_soundTiming;
    private AudioSource audio_source;
    private Coroutine audio_couroutine;
    private bool areSoundsPlaying = false;

    [SerializeField] private float audio_movementSoundVolume;
    [Space(10)]
    #endregion

    #region Vertex Info
    [Header("Vertex Settings")]
    [Space(1)]
    // ** 0 = Top , 1 = Left , 2 = Right , 3 = Bottom ** \\
    [SerializeField] private GameObject[] vertexColliders = new GameObject[4];
    [SerializeField] private bool[] vertexCollidersBool = new bool[4];
    #endregion

    #region Movement Sprites
    [Space(2)]
    [Header("Movement Sprites")]
    [Space(1)]
    [SerializeField] private Sprite[] sprite_moveUp;
    [SerializeField] private Sprite[] sprite_moveDown;
    [SerializeField] private Sprite[] sprite_moveLeft;
    [SerializeField] private Sprite[] sprite_moveRight;
    #endregion

    #region Animation Variables
    private float[] anim_Speeds = { 0.14f, 0.3f };
    private SpriteRenderer anim_PlayerSpriteRenderer;
    private Coroutine anim_Coroutine;
    private int anim_SpriteIndex = 1;
    private Sprite[] anim_CurrentSprites;
    private string currentPlayerFacingRotation;

    private bool isAnimRunning = false;
    #endregion
    #endregion

    #region On Awake
    private void Awake()
    {
        // Get game manager
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // Get player sprite renderer
        anim_PlayerSpriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();

        anim_CurrentSprites = sprite_moveDown;

        currentPlayerFacingRotation = "down";

        if (GameObject.FindWithTag("AudioSource") != null)
        {
            audio_source = GameObject.FindWithTag("AudioSource").GetComponent<AudioSource>();
        }
    }
    #endregion

    #region Every Frame
    private void Update()
    {
        // Basic running.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = moveSpeeds[1];
        }
        else { moveSpeed = moveSpeeds[0]; }

        if (movement != Vector3.zero && !areSoundsPlaying && gameManager.playSoundEffects)
        {
            StartCoroutine(audio_PlaySounds());
        }

        if (playerInput == Vector2.zero && gameManager.acceptInput)
        {
            StopMovementAnimation();
            StopMovementSounds();
        }

        // Debug.Log(isAnimRunning);

        // Clear player input vector.
        playerInput = Vector2.zero;

        if (gameManager.GetComponent<GameManager>().acceptInput)
        {
            // Check for top and bottom collision bool and whether button is being pressed.
            if (!vertexCollidersBool[0] && Input.GetAxisRaw("Vertical") > 0)
            {
                playerInput.y += 1f;
                anim_CurrentSprites = sprite_moveUp;
                currentPlayerFacingRotation = "up";

                if (!isAnimRunning)
                {
                    StartCoroutine(cr_PlayAnimUI(anim_Speeds[0]));
                    isAnimRunning = true;
                }
            }
            if (!vertexCollidersBool[3] && Input.GetAxisRaw("Vertical") < 0)
            {
                playerInput.y += -1f;
                anim_CurrentSprites = sprite_moveDown;
                currentPlayerFacingRotation = "down";

                if (!isAnimRunning)
                {
                    StartCoroutine(cr_PlayAnimUI(anim_Speeds[0]));
                    isAnimRunning = true;
                }
            }

            // Check for left and right collision bool and whether button is being pressed.
            if (!vertexCollidersBool[1] && Input.GetAxisRaw("Horizontal") < 0)
            {
                playerInput.x += -1f;
                anim_CurrentSprites = sprite_moveLeft;
                currentPlayerFacingRotation = "left";

                if (!isAnimRunning)
                {
                    StartCoroutine(cr_PlayAnimUI(anim_Speeds[0]));
                    isAnimRunning = true;
                }
            }
            if (!vertexCollidersBool[2] && Input.GetAxisRaw("Horizontal") > 0)
            {
                playerInput.x += 1f;
                anim_CurrentSprites = sprite_moveRight;
                currentPlayerFacingRotation = "right";

                if (!isAnimRunning)
                {
                    StartCoroutine(cr_PlayAnimUI(anim_Speeds[0]));
                    isAnimRunning = true;
                }
            }
        }

        PlayerMovement();
    }
    #endregion

    #region Movement Function
    private void PlayerMovement()
    {
        movement = playerInput.normalized;

        movement *= Time.deltaTime;
        movement *= moveSpeed;

        transform.Translate(movement);
    }
    #endregion

    #region Vertext Functions
    public void ChangeVertexBool(bool changed, string name)
    {
        if (name == "Top")
        {
            vertexCollidersBool[0] = changed;
        }
        else if (name == "Left")
        {
            vertexCollidersBool[1] = changed;
        }
        else if (name == "Right")
        {
            vertexCollidersBool[2] = changed;
        }
        else if (name == "Bottom")
        {
            vertexCollidersBool[3] = changed;
        }
    }
    #endregion

    #region Level Start
    // To be started in the "StartLevel" enumerator of every level manager if walk out is needed.
    public IEnumerator StartLevelWalk(string direction, Vector2 destination)
    {
        switch (direction)
        {
            case ("up"):
                anim_CurrentSprites = sprite_moveUp;
                break;
            case ("down"):
                anim_CurrentSprites = sprite_moveDown;
                break;
            case ("left"):
                anim_CurrentSprites = sprite_moveLeft;
                break;
            case ("right"):
                anim_CurrentSprites = sprite_moveRight;
                break;
        }

        StartCoroutine(cr_PlayAnimUI(anim_Speeds[1]));

        float step = 0.9f * Time.deltaTime;

        while (new Vector2(this.transform.position.x, this.transform.position.y) != destination)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, destination, step);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
    #endregion

    #region Animation Functions
    IEnumerator cr_PlayAnimUI(float anim_Speed)
    {
        if (anim_SpriteIndex >= anim_CurrentSprites.Length) //  Check if the index is more than the sprite array's length.
        {
            anim_SpriteIndex = 0; // Reset sprite index.
        }

        anim_PlayerSpriteRenderer.sprite = anim_CurrentSprites[anim_SpriteIndex]; // Set image to current sprite index.
        anim_SpriteIndex++; // Increment the sprite index.

        yield return new WaitForSeconds(anim_Speed); // Wait length of time between frames.

        anim_Coroutine = StartCoroutine(cr_PlayAnimUI(anim_Speed));
    }

    public void StopMovementAnimation()
    {
        // this may need changing but works for now i guess lol
        StopAllCoroutines();
        isAnimRunning = false;
        anim_SpriteIndex = 1;

        // set to standing position of last pressed directional key
        if (currentPlayerFacingRotation == "down")
        {
            anim_PlayerSpriteRenderer.sprite = sprite_moveDown[0];
        }
        else if (currentPlayerFacingRotation == "up")
        {
            anim_PlayerSpriteRenderer.sprite = sprite_moveUp[0];
        }
        else if (currentPlayerFacingRotation == "left")
        {
            anim_PlayerSpriteRenderer.sprite = sprite_moveLeft[0];
        }
        else if (currentPlayerFacingRotation == "right")
        {
            anim_PlayerSpriteRenderer.sprite = sprite_moveRight[0];
        }
    }
    #endregion


    #region Sound Coroutine
    private IEnumerator audio_PlaySounds()
    {
        areSoundsPlaying = true;

        audio_source.volume = audio_movementSoundVolume;

        int randNum = Random.Range(0, audio_footstepSounds.Length);

        audio_source.clip = audio_footstepSounds[randNum];

        audio_source.Play();

        yield return new WaitForSeconds(audio_soundTiming);

        audio_couroutine = StartCoroutine(audio_PlaySounds());
    }

    public void StopMovementSounds()
    {
        StopCoroutine(audio_PlaySounds());
        areSoundsPlaying = false;
    }
    #endregion
}
