using UnityEngine;

public class LevelThreePlayer : MonoBehaviour
{
    private LevelThreeManager levelThreeManager;
    private bool hasCollidedWithTrigger = false;


    public void SetLevelThreeManager(LevelThreeManager temp) { levelThreeManager = temp; }
    
    // Checking player collision events.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If collides with spike wall.
        if (collision.gameObject.tag == "SpikeWall")
        {
            if (!hasCollidedWithTrigger)
            {
                hasCollidedWithTrigger = true;
                levelThreeManager.OnPlayerCollisionWithSpikedWall();
            }
        }
        // If collides with falling spike trigger.
        if(collision.gameObject.tag == "FallingSpike")
        {
            if (!hasCollidedWithTrigger)
            {
                hasCollidedWithTrigger = true;
                levelThreeManager.StartFallingSpikeCR(collision.gameObject.transform.position);
            }
        }
    }
}
