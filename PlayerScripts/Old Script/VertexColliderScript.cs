using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexColliderScript : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            //player.GetComponent<PlayerScriptOLD>().ChangeVertexBool(true, this.gameObject.name);
            Debug.Log("Collision entered on " + this.gameObject.name);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            //player.GetComponent<PlayerScriptOLD>().ChangeVertexBool(false, this.gameObject.name);
            Debug.Log("Collision exited on " + this.gameObject.name);
        }
    }
}
