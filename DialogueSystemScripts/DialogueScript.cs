using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// TO USE THIS SCRIPT, CALL THE FUNCTION StartDialogue() AND PASS IN AN ARRAY OF THE LINES YOU NEED!

public class DialogueScript : MonoBehaviour
{
    #region Variables
    [Header("Lines")]
    [SerializeField] public string[] lines;
    private int index;
    
    [Space(8)]

    private GameManager gameManager;

    [Header("Text Settings")]
    // Text speeds
    public float text_speed;
    private float normal_text_speed = 0.04f;
    public float skipped_text_speed = 0.01f;

    private bool hasSkipped = false;

    [SerializeField] private TextMeshProUGUI text_component;

    [Space(8)]

    // All animation variables.
    [Header("Animation Settings")]
    public Sprite[] anim_Sprites; // Array holding all current sprite assets.
    public float[] anim_Speeds = { 0.1f, 0.1f, 0.05f }; // Animation speed // 0 = variable speed / 1 = normal anim speed / 2 = skipped anim speed. //
    private bool anim_Complete = false; // Variable for anim complete checks.
    private int anim_SpriteIndex; // Index of sprite array.
    public Image anim_Image;
    Coroutine anim_Coroutine;
    private bool anim_CoroutineRunning;

    [Space(8)]

    [HideInInspector] public bool dialogueComplete;

    private string currentLineTrimmed;

    // Nameplate
    private TextMeshProUGUI text_nameplate;

    [Header("Audio")]
    private AudioSource audioSource;
    [SerializeField] private bool makePredictable;
    [SerializeField] private DialogueAudioPersonalitySO[] dialogue_personalities;
    private DialogueAudioPersonalitySO dialogue_currentPersonality;
    private Dictionary<string, DialogueAudioPersonalitySO> dialoguePersonalitiesDictionary;
    #endregion

    private void Awake()
    {
        // Get reference to game manager.
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // Get nameplate text component.
        text_nameplate = this.transform.GetChild(0).gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        // add audio source component and set default audio personality
        audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.8f;
        dialogue_currentPersonality = dialogue_personalities[0];

        // Initialise dictionary of dialogue character "personalities".
        InitialiseDialogueInfosDictionary();
    }

    private void Update()
    {
        // If any button then skip to next level of... skip? (can be changed)
        if (Input.anyKeyDown)
        {
            // If text is equal to what is displayed on screen.
            if (text_component.text == currentLineTrimmed)
            {
                text_speed = normal_text_speed;
                anim_Speeds[0] = anim_Speeds[1];
                hasSkipped = false;
                NextLine();
            }
            // If text is not equal to what is displayed on screen and HAS NOT skipped current dialogue.
            else if (text_component.text != currentLineTrimmed && !hasSkipped)
            {
                text_speed = skipped_text_speed;
                anim_Speeds[0] = anim_Speeds[2];
                hasSkipped = true;
            }
            // If text is not equal to what is displayed on screen and HAS skipped current dialogue.
            else if (text_component.text != currentLineTrimmed && hasSkipped)
            {
                StopAllCoroutines();
                text_component.text = currentLineTrimmed;
            }
        }
        // If dialogue has finished, reset sprite and speed.
        if (anim_Sprites.Length != 0)
        {
            // Checks for animation
            if (text_component.text == currentLineTrimmed)
            {
                StopAllCoroutines();
                anim_Image.sprite = anim_Sprites[0];
                anim_Speeds[0] = anim_Speeds[1];
            }
        }
    }

    // Call this in any script to start dialogue sequence.
    public void StartDialogue(int character, string[] newLines)
    {
        dialogueComplete = false;

        // set character based on int input.
        SetCharacter(character);

        // Clear current text and reset speed.
        text_component.text = string.Empty;
        text_speed = normal_text_speed;

        // Set lines array length to length of given newLines array length.
        // Set lines array to newLines array.
        lines = new string[newLines.Length];
        lines = newLines;

        // Line list index initialised to value 0.
        index = 0;

        // Trim string.
        TrimString(lines[index]);

        // Start dialogue output coroutine.
        StartCoroutine(OutputDialogue());
        // Start icon animation UI if there are sprites to play.
        if (anim_Sprites.Length != 0)
            StartCoroutine(cr_PlayAnimUI());
    }

    // This coroutine output's the dialogue letter by letter.
    private IEnumerator OutputDialogue()
    {
        bool shouldOutput = true;
        int charIndex = 0;
        int currentlyDisplayedCharacters = 0;
        int amountToSkip = 0;
        char[] charArray = lines[index].ToCharArray();
        foreach (char c in lines[index].ToCharArray())
        {
            string temp = c.ToString();
            // If "_" to skip colour codes.
            if (temp == "_")
            {
                shouldOutput = false;
                amountToSkip = 15;
            }
            // If "-" to skip end of colour codes.
            if (temp == "-")
            {
                shouldOutput = false;
                amountToSkip = 8;
            }
            // If should output.
            if (shouldOutput)
            {
                // Call play sound function and pass in vars.
                PlayDialogueSound(currentlyDisplayedCharacters, charArray[charIndex]);
                // Display character.
                text_component.text += c;
                // Increment int.
                currentlyDisplayedCharacters++;
                if (amountToSkip == 0) { yield return new WaitForSeconds(text_speed); }
                else { amountToSkip--; }
            }
            shouldOutput = true;
            charIndex++;
        }
    }

    // This is used to go to the next line.
    private void NextLine()
    {
        // If not at the end of the line array.
        if (index < lines.Length - 1)
        {
            index++;
            text_component.text = string.Empty;
            TrimString(lines[index]);
            StartCoroutine(OutputDialogue());
            if (anim_Sprites.Length != 0)
                StartCoroutine(cr_PlayAnimUI());
        }
        // If at the end of the line array.
        else
        {
            // Set this gameobject not active and set dialogue complete to true.
            this.transform.parent.gameObject.SetActive(false);
            dialogueComplete = true;
        }
    }

    // This is used to play the speech animation.
    IEnumerator cr_PlayAnimUI()
    {
        yield return new WaitForSeconds(anim_Speeds[0]); // Wait length of time between frames.
        if (anim_SpriteIndex >= anim_Sprites.Length) //  Check if the index is more than the sprite array's length.
        {
            anim_SpriteIndex = 0; // Reset sprite index.
        }

        anim_Image.sprite = anim_Sprites[anim_SpriteIndex]; // Set image to current sprite index.
        anim_SpriteIndex++; // Increment the sprite index.

        if (!anim_Complete) // Start again if the anim is not completed.
        {
            anim_Coroutine = StartCoroutine(cr_PlayAnimUI());
        }
    }

    // SET CHARACTER BASED ON NUM SYSTEM (NUM REF CHARACTER)
    public void SetCharacter(int character)
    {
        anim_Image.sprite = null;

        switch(character)
        {
            // Narrator (Xolotl)
            case 1:
                anim_Image.sprite = gameManager.sprites_xolotl[0];

                anim_Sprites = gameManager.sprites_xolotl;
                text_nameplate.fontSize = 45;
                text_nameplate.text = "Xolotl";
                SetCurrentDialoguePersonality("xolotl");

                break;

            // Mictlantecuhtli
            case 2:
                anim_Image.sprite = gameManager.sprites_mictlantecuhtli[0];

                anim_Sprites = gameManager.sprites_mictlantecuhtli;
                text_nameplate.fontSize = 30;
                text_nameplate.text = "Mictlantecuhtli";
                SetCurrentDialoguePersonality("mictlantecuhtli");

                break;

            // Vermillion Dog
            case 3:
                anim_Image.sprite = gameManager.sprites_vermillion[0];

                anim_Sprites = gameManager.sprites_vermillion;
                text_nameplate.fontSize = 30;
                text_nameplate.text = "Vermillion Dog";
                SetCurrentDialoguePersonality("vermillion");

                break;

            // Bee
            case 4:
                anim_Image.sprite = gameManager.sprites_bees[0];

                anim_Sprites = gameManager.sprites_bees;
                text_nameplate.fontSize = 45;
                text_nameplate.text = "Bee";
                SetCurrentDialoguePersonality("bee");

                break;
        }
    }

    // Used to trim the string of colour code stuff.
    private void TrimString(string line)
    {
        currentLineTrimmed = String.Join("", line.Split('-', '_'));
    }

    #region Audio
    private void InitialiseDialogueInfosDictionary()
    {
        dialoguePersonalitiesDictionary = new Dictionary<string, DialogueAudioPersonalitySO>();
        foreach (DialogueAudioPersonalitySO audioInfo in dialogue_personalities)
        {
            dialoguePersonalitiesDictionary.Add(audioInfo.id, audioInfo);
        }
    }

    private void SetCurrentDialoguePersonality(string id)
    {
        DialogueAudioPersonalitySO audioInfo = null;
        dialoguePersonalitiesDictionary.TryGetValue(id, out audioInfo);
        if (audioInfo != null)
        {
            this.dialogue_currentPersonality = audioInfo;
        }
        else
        {
            Debug.LogWarning("No dialogue personality with id: " + id);
        }
    }

    private void PlayDialogueSound(int currentCharacterCount, char currentChar)
    {
        // Set vars to current personality.
        AudioClip[] audioClips = dialogue_currentPersonality.dialogue_clips;
        int frequency = dialogue_currentPersonality.dialogue_frequencyLevel;
        float minPitch = dialogue_currentPersonality.dialogue_minPitch;
        float maxPitch = dialogue_currentPersonality.dialogue_maxPitch;
        bool stopSound = dialogue_currentPersonality.stopSound;
        float volume = dialogue_currentPersonality.dialogue_volume;

        // play sound based on the config 
        if (currentCharacterCount % frequency == 0)
        {
            if (stopSound)
            {
                audioSource.Stop();
            }
            AudioClip soundClip = null;
            audioSource.volume = volume;
            // predictable audio using hash
            if (makePredictable)
            {
                // get hash from current character
                int hashCode = gameManager.gameObject.GetComponent<LetterKeyInfo>().ReturnHashkey((currentChar.ToString()).ToLower());
                // set sound clip
                int predictableIndex = hashCode % audioClips.Length;
                soundClip = audioClips[predictableIndex];
                // set pitch
                int minPitchInt = (int)(minPitch * 100);
                int maxPitchInt = (int)(maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;
                // cannot divide by 0
                if (pitchRangeInt != 0)
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt / 100f;
                    audioSource.pitch = predictablePitch;
                }

            }
            audioSource.PlayOneShot(soundClip);
        }
    }
    #endregion
}
