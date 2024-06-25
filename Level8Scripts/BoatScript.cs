using System.Collections;
using UnityEngine;

public class BoatScript : MonoBehaviour
{
    private GameObject player;
    private PlayerScript player_script;

    private Vector2 _destination;

    [HideInInspector] public LevelEightManager level_manager;

    private int lane_current = 2;

    private GameObject boat_notMovingGO;
    private GameObject boat_movingGO;

    private Coroutine audio_cr;

    private void Awake()
    {
        StartCoroutine(GetReferenceToPlayer());

        level_manager = GameObject.FindWithTag("LevelEightManager").GetComponent<LevelEightManager>();

        // get different versions of the boat
        boat_notMovingGO = this.transform.GetChild(0).gameObject;
        boat_movingGO = this.transform.GetChild(1).gameObject;
    }

    private void Update()
    {
        if (level_manager.boatCanBeControlled)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && lane_current < 3)
            {
                lane_current++;
                level_manager.PlaySound(level_manager.sound_laneSwitch);
                _destination.y = level_manager.GetDestinationFromLane(lane_current);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && lane_current > 1)
            {
                lane_current--;
                level_manager.PlaySound(level_manager.sound_laneSwitch);
                _destination.y = level_manager.GetDestinationFromLane(lane_current);
            }
        }
    }

    // set moving boat to be active
    public void SetMovingBoatActive()
    {
        // set player sprite alpha to = 0
        // set boat not moving go to not active
        // set boat moving go to active
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        boat_notMovingGO.SetActive(false);
        boat_movingGO.SetActive(true);
    }
    // set moving boat to be active
    public void SetNotMovingBoatActive()
    {
        // set player sprite alpha to = 1
        // set boat not moving go to active
        // set boat moving go to not active
        boat_movingGO.SetActive(false);
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        boat_notMovingGO.SetActive(true);
    }

    private IEnumerator GetReferenceToPlayer()
    {
        yield return new WaitUntil(() => GameObject.FindWithTag("Player"));

        player = GameObject.FindWithTag("Player");
        player_script = player.GetComponent<PlayerScript>();
    }

    // only use x-axis
    public IEnumerator cr_MoveBoatForward(float x, float speed)
    {
        _destination.x = x;
        // Start playing rowing sounds.
        StartCoroutine(cr_PlayBoatRowSounds(1.2f, level_manager.sound_rowing));
        // Move boat to destination on x-axis.
        while (this.transform.position.x != _destination.x)
        {
            float step = speed * Time.deltaTime;

            this.transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(_destination.x, this.transform.position.y), step);
            yield return new WaitForEndOfFrame();
        }
        // Stop boat rowing sounds.
        StopBoatSounds();
    }

    public IEnumerator cr_MoveBoatVertical(float y, float speed)
    {
        _destination.y = y;
        // Move boat on y-axis.
        while (this.transform.position.x != _destination.x)
        {
            float step = speed * Time.deltaTime;

            this.transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(this.transform.position.x, _destination.y), step);
            yield return new WaitForEndOfFrame();
            if (this.transform.position.y != _destination.y) { level_manager.boatCanBeControlled = false; }
            else if (this.transform.position.y == _destination.y && !level_manager.boatCanBeControlled) { level_manager.boatCanBeControlled = true; }
        }
    }

    public void UpdateBoatDestination(float y) { _destination.y = y; }
    public void _StopAllCoroutines() { StopAllCoroutines(); }

    public IEnumerator cr_PlayBoatRowSounds(float timing, AudioClip[] clips)
    {
        // Choose clip from amount of clips.
        AudioClip clip = clips[(int)Random.Range(0, clips.Length)];
        // Play sound.
        level_manager.PlaySound(clip);
        // Wait for amount of time.
        yield return new WaitForSeconds(timing);
        // Start coroutine again.
        audio_cr = StartCoroutine(cr_PlayBoatRowSounds(timing, clips));
    }
    public void StopBoatSounds()
    {
        StopCoroutine(audio_cr);
    }

    #region Collision Detection
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Blocker") { level_manager.PlayerDeath(); }
    }
    #endregion
}