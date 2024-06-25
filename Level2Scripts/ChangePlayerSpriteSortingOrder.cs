using UnityEngine;

// this is so player sprite isn't infront of the boulder's sprites that it shouldn't be.

public class ChangePlayerSpriteSortingOrder : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject player_bottom;
    private SpriteRenderer player_SpriteRenderer;

    public int sortingOrderToSwitchTo;
    #endregion

    private void Start()
    {
        player_SpriteRenderer = GameObject.FindWithTag("Player").transform.GetChild(0).GetComponent<SpriteRenderer>();
        player_bottom = GameObject.FindWithTag("Player").transform.GetChild(2).gameObject;
    }

    // Change player sorting order whenever colliding with a new lane.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player_bottom)
        {
            Debug.Log(collision.gameObject.name);
            player_SpriteRenderer.sortingOrder = sortingOrderToSwitchTo;
        }

    }
}
