using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSixPlayer : MonoBehaviour
{
    private string currentlyProtectedFrom;
    public LevelSixManager levelSixManager;

    private PlayerScript player_script;

    private PlayAnimationScript fire_animation_script;

    private Sprite[] sprites_walk_right;

    private void Awake()
    {
        this.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 1.2f);
        this.gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.15f);
    }

    private void Start()
    {
        player_script = this.gameObject.GetComponent<PlayerScript>();
        fire_animation_script = levelSixManager.fire_go.GetComponent<PlayAnimationScript>();

        levelSixManager.fire_go.GetComponentInChildren<SpriteRenderer>().enabled = false;

        sprites_walk_right = player_script.sprite_moveRight;
    }

    private void Update()
    {
        if (levelSixManager.startWalk)
            GetDirectionalInput();
    }

    private void GetDirectionalInput()
    {
        if (Input.GetKey(KeyCode.UpArrow)) { 
            if (currentlyProtectedFrom != "down")
            {
                fire_animation_script.sprite_default = levelSixManager.sprites_fire_up[0];

                player_script.anim_CurrentSprites = levelSixManager.sprites_playerWithSnakeFlamethrower_up;
                player_script.SetAnimationToCurrentindex();

                fire_animation_script.anim_Sprites = null;
                fire_animation_script.anim_Sprites = levelSixManager.sprites_fire_up;
                levelSixManager.fire_go.GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
                levelSixManager.fire_go.GetComponentInChildren<SpriteRenderer>().enabled = true;
                fire_animation_script.ResetSprite();
            }
            currentlyProtectedFrom = "down";
        }
        else if (Input.GetKey(KeyCode.DownArrow)) { 
            if (currentlyProtectedFrom != "up")
            {
                fire_animation_script.sprite_default = levelSixManager.sprites_fire_down[0];

                player_script.anim_CurrentSprites = levelSixManager.sprites_playerWithSnakeFlamethrower_down;
                player_script.SetAnimationToCurrentindex();

                fire_animation_script.anim_Sprites = null;
                fire_animation_script.anim_Sprites = levelSixManager.sprites_fire_down;
                levelSixManager.fire_go.GetComponentInChildren<SpriteRenderer>().sortingOrder = 3;
                levelSixManager.fire_go.GetComponentInChildren<SpriteRenderer>().enabled = true;
                fire_animation_script.ResetSprite();
            }
            currentlyProtectedFrom = "up";
        }
        else if (Input.GetKey(KeyCode.LeftArrow)) { 
            if (currentlyProtectedFrom != "right")
            {
                fire_animation_script.sprite_default = levelSixManager.sprites_fire_left[0];
                
                player_script.anim_CurrentSprites = levelSixManager.sprites_playerWithSnakeFlamethrower_left;
                player_script.SetAnimationToCurrentindex();

                fire_animation_script.anim_Sprites = null;
                fire_animation_script.anim_Sprites = levelSixManager.sprites_fire_left;
                levelSixManager.fire_go.GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
                levelSixManager.fire_go.GetComponentInChildren<SpriteRenderer>().enabled = true;
                fire_animation_script.ResetSprite();
            }
            currentlyProtectedFrom = "right";
        }
        else if (Input.GetKey(KeyCode.RightArrow)) { 
            if (currentlyProtectedFrom != "left")
            {
                fire_animation_script.sprite_default = levelSixManager.sprites_fire_right[0];
                
                player_script.anim_CurrentSprites = levelSixManager.sprites_playerWithSnakeFlamethrower_right;
                player_script.SetAnimationToCurrentindex();

                fire_animation_script.anim_Sprites = null;
                fire_animation_script.anim_Sprites = levelSixManager.sprites_fire_right;
                levelSixManager.fire_go.GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
                levelSixManager.fire_go.GetComponentInChildren<SpriteRenderer>().enabled = true;
                fire_animation_script.ResetSprite();
            }
            currentlyProtectedFrom = "left";
        }

        else { 
            if(currentlyProtectedFrom != null)
            {
                player_script.anim_CurrentSprites = sprites_walk_right;
            }
            currentlyProtectedFrom = null;


            levelSixManager.fire_go.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        //if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)) { currentlyProtectedFrom = null; }
    }

    // Use this to check what player is protected from.
    public void CheckPlayerProtection(string direction, GameObject go)
    {
        if (currentlyProtectedFrom == direction)
        {
            levelSixManager.PlaySound(levelSixManager.sound_block, true);
            Destroy(go);
        }
        else 
        {
            levelSixManager.PlaySound(levelSixManager.sound_arrow, false);
            levelSixManager.PlayerDeath(); 
        }
    }
}
