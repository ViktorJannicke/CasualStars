using UnityEngine;
using TMPro;
using System.Runtime.Serialization;
using System;
using System.Globalization;
using UnityEngine.SceneManagement;

public class NGameManager : MonoBehaviour
{
   // public static NGameManager manager;

    public int Score = 25;
    public TextMeshProUGUI score;
    public TextMeshProUGUI time;

    public float timer = 120;

    [Header("Asteroid Control Values")]
    public GameObject planetPrefab;
    public Transform planetSpawnPoint;
    public GameObject Asteroidprefab;
    public bool checkX;
    public bool checkY;
    public bool checkZ;
    public GameObject player;

    [Header("Asteroid Spawn Values")]
    public LayerMask mask;
    public Vector3 center;
    public Vector3 size;
    public bool stopSpawning = false;

    public int spawnTime;
    public int spawnDelay;
    public int spawnRadius;
    public int spawnCount;

    [Header("GameStart")]
    public bool gameStarted;
    public float gameStartTime = 4;
    public TextMeshProUGUI gameStartText;

    [Header("GameEnd")]
    public SceneManagement sm;
    
    private void Update()
    {
        if (gameStarted)
        {
            score.text = "Score: \n" + Score;
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer) - (minutes * 60);

            DateTime d = new DateTime(2001, 01, 01, 01, minutes, seconds);


            time.text = "Time: \n" + d.ToString("mm:ss");

            float t = Time.deltaTime;
            if (timer - t <= 0)
            {
                timer = -1000;

                MasterManager.mm.lastScore = Score;
                sm.LoadGameEnd();
            }
            else
            {
                timer -= t;
            }
        } else
        {
            float t = Time.deltaTime;
            if (gameStartTime - t <= 0)
            {
                gameStartText.enabled = false;
                gameStarted = true;
            }
            else
            {
                if (gameStartTime - t <= 1)
                {
                    gameStartText.text = "Los";
                }
                else if (gameStartTime - t <= 2)
                {
                    gameStartText.text = "1";
                }
                else if (gameStartTime - t <= 3)
                {
                    gameStartText.text = "2";
                }
                else if (gameStartTime - t <= 4)
                {
                    gameStartText.text = "3";

                }

                gameStartTime -= t;
            }


        }
    }
    private void Start()
    {
        int counter = 0;
        for (int i = 0; i <= (spawnCount/3*(MasterManager.mm.difficulty+1)); i++)
        {
            counter += Asteroidsspawn();
        }

        Debug.Log(counter);
    }

    public int Asteroidsspawn()
    {
        int count = 0;

        Vector3 pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-size.x / 2, size.x / 2), UnityEngine.Random.Range(-size.y / 2, size.y / 2), UnityEngine.Random.Range(-size.z / 2, size.z / 2));

        while (Physics.CheckSphere(pos, spawnRadius, mask))
        {
            pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-size.x / 2, size.x / 2), UnityEngine.Random.Range(-size.y / 2, size.y / 2), UnityEngine.Random.Range(-size.z / 2, size.z / 2));
            count++;
            if(count > 500)
            {
                return 0;
            }
        }
        GameObject asteroid = Instantiate(Asteroidprefab, pos, Asteroidprefab.transform.rotation);
        asteroid.transform.parent = transform;
        Obstacle o = asteroid.GetComponent<Obstacle>();
        o.manager = this;
        return 1;
    }

    public void Planetspawn()
    {
        GameObject planet = Instantiate(planetPrefab, planetSpawnPoint);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(transform.localPosition + center, size);
    }
}