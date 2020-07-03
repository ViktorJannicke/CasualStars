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

    public void LoadSpaceship()
    {
        SceneManager.LoadSceneAsync("Spaceship", LoadSceneMode.Additive);
    }
    public void LoadMainMenuFromIngame()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        LoadSpaceship();
    }
    public void LoadMainMenuFromHowToPlay()
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
    public void LoadMainMenuFromGameStart()
    {
        SceneManager.UnloadSceneAsync("GameStart");
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
    }
    public void LoadMainMenuFromScoreSubmition()
    {
        SceneManager.UnloadSceneAsync("ScoreSubmition");
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
    }
    public void LoadMainMenuFromCredits()
    {
        SceneManager.UnloadSceneAsync("Credits");
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
    }
    public void LoadCreditsFromMainMenu()
    {
        SceneManager.UnloadSceneAsync("MainMenu");
        SceneManager.LoadSceneAsync("Credits", LoadSceneMode.Additive);
    }
    public void LoadGameStart()
    {
        if (MasterManager.mm.firstStart)
        {
            MasterManager.mm.firstStart = false;
            SceneManager.LoadSceneAsync("HowToPlay");
        }
        else
        {
            SceneManager.UnloadSceneAsync("MainMenu");
            SceneManager.LoadSceneAsync("GameStart", LoadSceneMode.Additive);
        }
    }
    public void LoadGameStartFromHowToPlay()
    {
            SceneManager.LoadSceneAsync("GameStart");
            LoadSpaceship();
    }
    public void LoadGameStartFromScoreSubmition()
    {
        SceneManager.UnloadSceneAsync("ScoreSubmition");
        SceneManager.LoadSceneAsync("GameStart", LoadSceneMode.Additive);
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

    public void LoadGame()
    {
        SceneManager.LoadSceneAsync("Game");
    }
    public void LoadGameEnd()
    {
        SceneManager.LoadSceneAsync("GameEnd");
        LoadSpaceship();
    }

    public void LoadAdFromGameStart()
    {
        MasterManager.mm.nextScene = "Game";
        SceneManager.UnloadSceneAsync("GameStart");
        SceneManager.LoadSceneAsync("Werbung", LoadSceneMode.Additive);
    }

    public void LoadAdFromGameEnd()
    {
        MasterManager.mm.nextScene = "ScoreSubmition";
        MasterManager.mm.lastScore *= 2;
        SceneManager.UnloadSceneAsync("GameEnd");
        SceneManager.LoadSceneAsync("Werbung", LoadSceneMode.Additive);
    }
}
