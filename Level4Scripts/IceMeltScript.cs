using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMeltScript : MonoBehaviour
{
    #region Variables
    private int amountToMelt = 12;
    [SerializeField] private float timeBetweenMelt;

    private int currentMeltLevel = 1;

    [SerializeField] private Sprite[] iceSprites;

    [SerializeField] private GameObject keyPrefab;

    private SpriteRenderer iceSpriteRenderer;

    Coroutine iceMeltCoroutine;

    bool isIceWithKey = false;

    public bool isIceMelting = false;

    private LevelFourPlayer levelFourPlayer;
    #endregion

    private void Awake()
    {
        iceSpriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        levelFourPlayer = GameObject.FindWithTag("Player").GetComponent<LevelFourPlayer>();
    }

    private void Start()
    {
        if (this.gameObject.name == "IceCubeWithKey") { isIceWithKey = true; } 
        else { isIceWithKey = false; }
    }

    // Use when melting ice.
    public IEnumerator MeltIce()
    {
        // If level is below max level to melt.
        while (currentMeltLevel < amountToMelt)
        {
            yield return new WaitForSeconds(timeBetweenMelt);
            ChangeIceSprite(currentMeltLevel); // Chance ice sprite.
            currentMeltLevel++; // Increment melt level.
            Debug.Log("Ice should be metling rn :3");
        }
        // If ice has key.
        if (isIceWithKey)
        {
            if(!(GameObject.FindWithTag("Key")))
            {
                Debug.Log("Spawning Key");
                GameObject go = Instantiate(keyPrefab); // Spawn key.
                go.transform.position = new Vector2(this.transform.position.x - 0.035f, this.transform.position.y - 0.1085f); // Set key to current ice position.
            }
        }
        // Destroy ice gameobject.
        Destroy(this.gameObject);
    }

    // Change ice sprite.
    private void ChangeIceSprite(int value) { iceSpriteRenderer.sprite = iceSprites[value];  }
}
