using UnityEngine;
using UnityEngine.SceneManagement;

public class _SceneManager : MonoBehaviour
{
    private static _SceneManager Instance;

    // Save level to transition to for after level transition animation has played.
    private int levelToTransitionTo;
    // Save whether player has finished game for credits.
    [HideInInspector] public bool _hasFinishedGame;

    void Start()
    {
        if (Instance)
            Destroy(gameObject);
        // Don't delete this gameobject between scene transitions.
        DontDestroyOnLoad(gameObject);
    }

    // Use this to change scene.
    public void func_SceneSwitch(string sceneName)   { SceneManager.LoadScene(sceneName); }
    // Use this to return the current scene's name.
    public string ReturnCurrentSceneName() { return SceneManager.GetActiveScene().name; }

    // Run when transition into a new level, keeps the int value of what level to transition to after clip has been played.
    public void RunLevelTransition(int level)
    {
        levelToTransitionTo = level;

        func_SceneSwitch("LevelTransition");
    }

    // use this after the clip is player to transition into the level
    public void TransitionToLevel()
    {
        switch (levelToTransitionTo)
        {
            case 1:
                func_SceneSwitch("Level1");
                break;
            case 2:
                func_SceneSwitch("Level2");
                break;
            case 3:
                func_SceneSwitch("Level3");
                break;
            case 4:
                func_SceneSwitch("Level4");
                break;
            case 5:
                func_SceneSwitch("Level5");
                break;
            case 6:
                func_SceneSwitch("Level6");
                break;
            case 7:
                func_SceneSwitch("Level7");
                break;
            case 8:
                func_SceneSwitch("Level8");
                break;
            case 9:
                func_SceneSwitch("Level9");
                break;
        }
        levelToTransitionTo = 0;
    }

    public int GetLevelToTransitionTo() { return levelToTransitionTo; }

    // Go to credits and save whether the player has finished the game.
    public void GoToCredits(bool hasFinishedGame) 
    { 
        _hasFinishedGame = hasFinishedGame;
        func_SceneSwitch("Credits"); 
    }

    // Use this to restart the current scene.
    public void RestartScene()
    {
        Debug.Log("Restarting current scene: " + ReturnCurrentSceneName());
        func_SceneSwitch(ReturnCurrentSceneName());
    }
}
