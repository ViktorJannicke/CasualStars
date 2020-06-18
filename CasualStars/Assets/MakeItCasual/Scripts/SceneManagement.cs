using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public bool loadScoreSubmition;

    private void Update()
    {
        if(loadScoreSubmition)
        {
            loadScoreSubmition = false;
            LoadScoreSubmition();
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
    public void LoadScoreboard()
    {
        SceneManager.LoadSceneAsync("Scoreboard");
    }
    public void LoadScoreSubmition()
    {
        SceneManager.LoadSceneAsync("ScoreSubmition");
    }
    public void LoadSettings()
    {
        SceneManager.LoadSceneAsync("Settings");
    }

    public void LoadGame()
    {
        SceneManager.LoadSceneAsync("Game");
    }

    public void LoadAdFromGameStart()
    {
        MasterManager.mm.nextScene = "Game";
        SceneManager.LoadSceneAsync("Werbung");
    }

    public void LoadAdFromGameEnd()
    {
        MasterManager.mm.nextScene = "ScoreSubmition";
        MasterManager.mm.lastScore *= 2;
        SceneManager.LoadSceneAsync("Werbung");
    }
}
