using UnityEngine;
using TMPro;
using System.Runtime.Serialization;
using System;
using System.Globalization;
using UnityEngine.SceneManagement;

public class NGameManager : MonoBehaviour
{
    public static NGameManager manager;

    public int Score = 25;
    public TextMeshProUGUI score;
    public TextMeshProUGUI time;

    public float timer = 120;

    [Header("Asteroid Control Values")]
    public GameObject Asteroidprefab;
    public bool checkX;
    public bool checkY;
    public bool checkZ;
    public GameObject player;

    [Header("Asteroid Spawn Values")]

    public Vector3 center;
    public Vector3 size;
    public bool stopSpawning = false;

    public int spawnTime;
    public int spawnDelay;
    public int spawnRadius;
    public int spawnCount;
    
    private void Update()
    {
        score.text = "Score: \n" + Score;
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer) - (minutes * 60);

        DateTime d = new DateTime(2001,01,01,01,minutes,seconds);


        time.text = "Time: \n" + d.ToString("mm:ss");

        float t = Time.deltaTime;
        if (timer - t <= 0)
        {
            timer = 0;
            SceneManager.LoadSceneAsync("MainMenu");
        }
        else
        {
            timer -= t;
        }
    }

    void Start()
    {

        if(manager != null)
        {
            Destroy(gameObject);
            return;
        }

        manager = this;

        for(int i = 0; i < spawnCount; i++)
        {
            Asteroidsspawn();
        }
    }

    public void Asteroidsspawn()
    {
        int count = 0;

        Vector3 pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-size.x / 2, size.x / 2), UnityEngine.Random.Range(-size.y / 2, size.y / 2), UnityEngine.Random.Range(-size.z / 2, size.z / 2));

        while (Physics.CheckSphere(pos, spawnRadius))
        {
            pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-size.x / 2, size.x / 2), UnityEngine.Random.Range(-size.y / 2, size.y / 2), UnityEngine.Random.Range(-size.z / 2, size.z / 2));
            count++;
            if(count > 500)
            {
                return;
            }
        }
        GameObject asteroid = Instantiate(Asteroidprefab, pos, Quaternion.identity);
        asteroid.GetComponent<Obstacle>().target = player;
        Movement m = asteroid.GetComponent<Movement>();
        m.checkX = checkX;
        m.checkY = checkY;
        m.checkZ = checkZ;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(transform.localPosition + center, size);
    }
}