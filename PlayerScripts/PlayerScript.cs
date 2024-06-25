using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    #region Variables
    #region Movement Variables
    private float moveSpeed = 2.8f;
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
    [Header("Sound Effect Settings")]
    [SerializeField] private AudioClip[] audio_footstepSounds;
    [SerializeField] private float[] audio_soundTiming = { 0.3f, 0.6f };
    private AudioSource audio_source;
    private Coroutine audio_coroutine;
    private bool areSoundsPlaying = false;

    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private bool shouldRandomisePitch;

    [SerializeField] private float audio_movementSoundVolume;

    private string ground_currentType;
    [Space(10)]
    #endregion

    #region Movement Sprites
    [Space(2)]
    [Header("Movement Sprites")]
    [Space(1)]
    public Sprite[] sprite_moveUp;
    public Sprite[] sprite_moveDown;
    public Sprite[] sprite_moveLeft;
    public Sprite[] sprite_moveRight;
    #endregion

    #region Animation Variables
    private float[] anim_Speeds = { 0.14f, 0.3f };
    private SpriteRenderer anim_PlayerSpriteRenderer;
    private Coroutine anim_Coroutine;
    private int anim_SpriteIndex = 1;
    [HideInInspector] public Sprite[] anim_CurrentSprites;
    private string currentPlayerFacingRotation;

    private bool isAnimRunning = false;
    #endregion

    #region Vertex
    [SerializeField] private bool[] vertexCollidersBool = new bool[4];
    [SerializeField] private Transform raycastOrigin;
    private float raycastDistance = 0.25f;
    #endregion
    #endregion

    #region On Awake
    private void Awake()
    {
        // Get reference to game manager script.
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // Get reference to player sprite renderer.
        anim_PlayerSpriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        // Set current sprite to down.
        anim_CurrentSprites = sprite_moveDown;
        // Initialise direction variable.
        currentPlayerFacingRotation = "down";
        // Get reference to raycast origin transform.
        raycastOrigin = this.transform.GetChild(1).GetComponent<Transform>();
        // Get reference to audio source component if it exists.
        if (GameObject.FindWithTag("AudioSource") != null)
        {
            audio_source = GameObject.FindWithTag("AudioSource").GetComponent<AudioSource>();
        }
    }
    #endregion

    #region Every Frame
    private void Update()
    {
        // Shoot collision raycasts and check ground beneath player.
        ShootRaycasts();
        CheckGroundType();

        // If movement and sounds aren't playing (when they should).
        if (movement != Vector3.zero && !areSoundsPlaying && gameManager.playSoundEffects)
        {
            StartCoroutine(audio_PlaySounds(audio_soundTiming[0]));
        }
        // If not movement and movement inputs should be accepted.
        if (playerInput == Vector2.zero && gameManager.acceptInput)
        {
            StopMovementAnimation();
            StopMovementSounds();
        }

        // Clear player input vector.
        playerInput = Vector2.zero;
        // If should accept player movement input.
        // then gather player movement input and store in playerInput var.
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
            if (!vertexCollidersBool[1] && Input.GetAxisRaw("Vertical") < 0)
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
            if (!vertexCollidersBool[2] && Input.GetAxisRaw("Horizontal") < 0)
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
            if (!vertexCollidersBool[3] && Input.GetAxisRaw("Horizontal") > 0)
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
        // Perform movement based on movement input.
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

    #region Walk
    // Move player gameobject to any position.
    // direction: string of what direction the player sprite should be facing.
    // destination: vector 2 of where the player should travel to.
    // speed: speed the player should travel at.
    public IEnumerator Walk(string direction, Vector2 destination, float speed)
    {
        // Initialise the player sprite array.
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
        // Start movement animation and sounds. (Using different timings)
        StartCoroutine(cr_PlayAnimUI(anim_Speeds[1]));
        StartCoroutine(audio_PlaySounds(audio_soundTiming[1]));
        // Move the player every frame.
        while (new Vector2(this.transform.position.x, this.transform.position.y) != destination)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(this.transform.position, destination, step);
            yield return new WaitForEndOfFrame();
        }
        // Once complete stop movement animation and sounds.
        StopMovementAnimation();
        StopMovementSounds();
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

    // Use this to stop movement animation from playing and to reset player sprite.
    public void StopMovementAnimation()
    {
        StopAllCoroutines();
        isAnimRunning = false;
        anim_SpriteIndex = 1;

        SetPlayerAnimationDirection(currentPlayerFacingRotation);
    }

    public void SetAnimationToCurrentindex()
    {
        Debug.Log(anim_SpriteIndex);
        if (anim_SpriteIndex >= anim_CurrentSprites.Length)
        {
            anim_SpriteIndex = anim_CurrentSprites.Length - 1;
        }
        anim_PlayerSpriteRenderer.sprite = anim_CurrentSprites[anim_SpriteIndex];
    }

    public void SetPlayerAnimationDirection(string direction)
    {
        currentPlayerFacingRotation = direction;

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

    #region Collision
    // Raycast variables.
    RaycastHit2D hitInfo;
    int layerMask;

    private void ShootRaycasts()
    {
        // Get layer mask.
        layerMask = LayerMask.GetMask("Wall");
        // For each collider boolean, fire a raycast.
        for (int i = 0; i < vertexCollidersBool.Length; i++)
        {
            // Fire raycast and initialise hitinfo var.
            // Up
            if (i == 0) { hitInfo = Physics2D.Raycast(raycastOrigin.position, Vector2.up, raycastDistance, layerMask); }
            // Down
            else if (i == 1) { hitInfo = Physics2D.Raycast(raycastOrigin.position, Vector2.down, raycastDistance, layerMask); }
            // Left
            else if (i == 2) { hitInfo = Physics2D.Raycast(raycastOrigin.position, Vector2.left, raycastDistance, layerMask); }
            // Right
            else if (i == 3) { hitInfo = Physics2D.Raycast(raycastOrigin.position, Vector2.right, raycastDistance, layerMask); }

            // Debug lines in editor.
            Debug.DrawLine(raycastOrigin.position, new Vector3(raycastOrigin.position.x - raycastDistance, raycastOrigin.position.y, 0), Color.magenta);
            Debug.DrawLine(raycastOrigin.position, new Vector3(raycastOrigin.position.x + raycastDistance, raycastOrigin.position.y, 0), Color.magenta);
            Debug.DrawLine(raycastOrigin.position, new Vector3(raycastOrigin.position.x, raycastOrigin.position.y - raycastDistance, 0), Color.magenta);
            Debug.DrawLine(raycastOrigin.position, new Vector3(raycastOrigin.position.x, raycastOrigin.position.y + raycastDistance, 0), Color.magenta);

            // Set boolean to be true if it detects a hit, false if not.
            if (hitInfo) { vertexCollidersBool[i] = true; }
            else { vertexCollidersBool[i] = false; }
        }
    }
    #endregion

    #region Sound Functions
    // Use this function to fire a raycast to check the type of ground under the player.
    // Change the sounds that should be playing if they are different from the current.
    private void CheckGroundType()
    {
        // Get layer mask.
        var layer_mask = LayerMask.GetMask("Ground");
        // Fire raycast and initialise hit_info var.
        var hit_info = Physics2D.Raycast(raycastOrigin.position, -Vector2.up, 0.01f, layer_mask);

        // If did hit, check if the hit gameobject's ground type is different from the current.
        if (hit_info)
        {
            if (ground_currentType != hit_info.transform.gameObject.GetComponent<GroundTypeScript>().ground_data.ground_type && 
                hit_info.transform.gameObject.GetComponent<GroundTypeScript>().ground_data.ground_type != null)
            {
                ground_currentType = hit_info.transform.gameObject.GetComponent<GroundTypeScript>().ground_data.ground_type; // Set string.

                audio_footstepSounds = hit_info.transform.gameObject.GetComponent<GroundTypeScript>().ground_data.ground_sounds; // Set sounds.
            }
        }
    }

    private IEnumerator audio_PlaySounds(float timing)
    {
        // Sounds playing true.
        areSoundsPlaying = true;
        // Wait for amount of time between steps before playing sound (placed here to stop the sound instantly playing).
        yield return new WaitForSeconds(timing);
        // If should randomise pitch then do.
        if (shouldRandomisePitch)
        {
            audio_source.pitch = Random.Range(minPitch, maxPitch);
        }
        audio_source.volume = audio_movementSoundVolume; // Set volume.
        int randNum = Random.Range(0, audio_footstepSounds.Length); // Get random number from sound effect array.
        var clip = audio_footstepSounds[randNum]; // Set sound to play.
        audio_source.PlayOneShot(clip); // Play sound.
        audio_coroutine = StartCoroutine(audio_PlaySounds(timing)); // Start coroutine again passing in timing.
    }

    // Use this to stop movement sounds.
    public void StopMovementSounds()
    {
        StopCoroutine(audio_PlaySounds(0));
        areSoundsPlaying = false;
    }
    #endregion
}
