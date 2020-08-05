using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public bool loadSpaceship;
    public bool loadGameEnd;
    private void Start()
    {
        if(loadSpaceship)
        {

            LoadMainMenuFromIngame();
            loadSpaceship = false;
        }
    }

    private void Update()
    {
        if(loadGameEnd)
        {
            loadGameEnd = false;
            LoadGameEnd();
        }
    }
    public void LoadLostGame()
    {
        SceneManager.LoadSceneAsync("LostGame");
    }
    public void LoadSpaceship()
    {
        SceneManager.LoadSceneAsync("Spaceship", LoadSceneMode.Additive);
    }
    public void LoadMainMenuFromIngame()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        LoadSpaceship();
    }
    public void LoadMainMenuFromScoreboard()
    {
        SceneManager.UnloadSceneAsync("Scoreboard");
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
    }
    public void LoadMainMenuFromSettings()
    {
        SceneManager.UnloadSceneAsync("Settings");
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
    }
    public void LoadMainMenuFromScoreSubmition()
    {
        SceneManager.UnloadSceneAsync("ScoreSubmition");
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
    }
    public void LoadSettingsFromCredits()
    {
        SceneManager.UnloadSceneAsync("Credits");
        SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Additive);
    }
    public void LoadCreditsFromSettings()
    {
        SceneManager.UnloadSceneAsync("Settings");
        SceneManager.LoadSceneAsync("Credits", LoadSceneMode.Additive);
    } 

    public void LoadScoreboardFromMainMenu()
    {
        SceneManager.UnloadSceneAsync("MainMenu");
        SceneManager.LoadSceneAsync("Scoreboard", LoadSceneMode.Additive);
    }

    public void LoadScoreboardFromScoreSubmition()
    {
        SceneManager.UnloadSceneAsync("ScoreSubmition");
        SceneManager.LoadSceneAsync("Scoreboard", LoadSceneMode.Additive);
    }

    public void LoadScoreSubmitionFromGameEnd()
    {
        SceneManager.UnloadSceneAsync("GameEnd");
        SceneManager.LoadSceneAsync("ScoreSubmition", LoadSceneMode.Additive);
    }

    public void LoadSettings()
    {
        SceneManager.UnloadSceneAsync("MainMenu");
        SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Additive);
    }

    public void LoadNewTransition()
    {
        MasterManager.mm.lastScore = 0;
        MasterManager.mm.difficulty = 0;
        SceneManager.LoadSceneAsync("GameplayTransition");
    }

    public void LoadTransition()
    {
        MasterManager.mm.difficulty++;
        SceneManager.LoadSceneAsync("GameplayTransition");
    }

    public void LoadGame()
    {
        SceneManager.LoadSceneAsync("Game");
    }

    public void LoadGameEnd()
    {
        SceneManager.LoadSceneAsync("GameEnd");
        LoadSpaceship();
    }

    public void LoadAdFromGame()
    {
        SceneManager.LoadSceneAsync("Werbung");
    }
}
