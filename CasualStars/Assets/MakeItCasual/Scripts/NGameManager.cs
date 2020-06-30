using UnityEngine;
using TMPro;
using System;
using UnityEngine.Playables;

public class NGameManager : MonoBehaviour
{
   // public static NGameManager manager;

    public int Score = 25;
    public TextMeshProUGUI score;
    public TextMeshProUGUI time;

    public float timer = 120;

    [Header("Asteroid Control Values")]
    public GameObject AsteroidprefabIn;
    public Transform InGroup;
    public GameObject AsteroidprefabOut;
    public Transform OutGroup;
    public GameObject AsteroidprefabMove;
    public Transform MoveGroup;
    public GameObject AsteroidprefabMoveSpawner;
    public Transform SpawnGroup;
    public GameObject player;

    [Header("Asteroid Spawn Values")]
    public LayerMask mask;
    public Vector3 center;
    public Vector3 sizeIn;
    public Vector3 sizeOut;
    public int spawnRadius;
    public int spawnCount;
    public float movingSpawnOffsetX;
    public float movingSpawnOffsetY;

    [Header("Asteroid Debug")]
    public bool drawIn;
    public bool drawOut;

    [Header("GameStart")]
    public bool gameStarted;
    public float gameStartTime = 6;
    public TextMeshProUGUI gameStartText;
    public float[] playerSpeed;

    [Header("GameEnd")]
    public bool gameEnd = false;
    public float gameEndTime = 6.5f;
    public SceneManagement sm;
    public PlayableAsset playable;
    public PlayableDirector director;

    bool once = true;
    private void Update()
    {
        if (gameEnd)
        {
            float t = Time.deltaTime;
            gameEndTime -= t;
            if (gameEndTime <= 0)
            {
                if (once)
                {
                    once = false;
                    MasterManager.mm.lastScore = Score;
                    sm.LoadGameEnd();
                }
            }
        }
        else if (gameStarted)
        {
            score.text = "Score: \n" + Score;
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer) - (minutes * 60);

            DateTime d = new DateTime(2001, 01, 01, 01, minutes, seconds);


            time.text = "Time: \n" + d.ToString("mm:ss");

            float t = Time.deltaTime;
            if (timer - t <= 0)
            {
                if (once)
                {
                    director.playableAsset = playable;
                    director.Play();
                    gameStarted = false;
                    gameEnd = true;
                    player.GetComponent<Movement>().move = false;
                    Camera.main.gameObject.GetComponent<ripple>().activated = false;
                    Camera.main.gameObject.GetComponent<RotateSkybox>().activated = false;
                }
            }
            else if (timer - t <= 7 && timer - t >= 6)
            {
                once = true;
                timer -= t;
            }
            else
            {
                timer -= t;
            }
        }
        else
        {
            float t = Time.deltaTime;
            gameStartTime -= t;
            if (gameStartTime <= 0)
            {
                if (once)
                {
                    once = false;
                    gameStartText.enabled = false;
                    gameStarted = true;
                    player.GetComponent<Movement>().move = true;
                    Camera.main.gameObject.GetComponent<ripple>().activated = true;
                    Camera.main.gameObject.GetComponent<RotateSkybox>().activated = true;
                }
            }
            else
            {
                if (gameStartTime <= 1)
                {
                    gameStartText.text = "Los";
                }
                else if (gameStartTime <= 2)
                {
                    gameStartText.text = "1";
                }
                else if (gameStartTime <= 3)
                {
                    gameStartText.text = "2";
                }
                else if (gameStartTime <= 4)
                {
                    gameStartText.text = "3";
                }
            }
        }
    }
    private void Start()
    {

        int counter = 0;
        for (int i = 0; i <= (spawnCount / 4*(MasterManager.mm.difficulty+1)) / 4; i++)
        {
            counter += AsteroidsspawnIn();
        }

        counter = 0;
        for (int i = 0; i <= (spawnCount / 4 * (MasterManager.mm.difficulty + 1)) * 2; i++)
        {
            counter += AsteroidsspawnOut();
        }

        player.GetComponent<Movement>().speed = playerSpeed[MasterManager.mm.difficulty];

        counter = 0;
        for (int i = 0; i <= (spawnCount / 4 * (MasterManager.mm.difficulty + 1)) / 8; i++)
        {
            counter += AsteroidsspawnMovableSpawner();
        }
    }

    public int AsteroidsspawnIn()
    {
        int count = 0;

        Vector3 pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-sizeIn.x / 2, sizeIn.x / 2), UnityEngine.Random.Range(-sizeIn.y / 2, sizeIn.y / 2), UnityEngine.Random.Range(-sizeIn.z / 2, sizeIn.z / 2));

        while (Physics.CheckSphere(pos, spawnRadius, mask))
        {
            pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-sizeIn.x / 2, sizeIn.x / 2), UnityEngine.Random.Range(-sizeIn.y / 2, sizeIn.y / 2), UnityEngine.Random.Range(-sizeIn.z / 2, sizeIn.z / 2));
            count++;
            if(count > 500)
            {
                return 0;
            }
        }
        GameObject asteroid = Instantiate(AsteroidprefabIn, pos, AsteroidprefabIn.transform.rotation);
        asteroid.transform.parent = InGroup;
        Obstacle o = asteroid.GetComponent<Obstacle>();
        o.manager = this;
        return 1;
    }
    public int AsteroidsspawnOut()
    {
        int count = 0;


        Vector3 pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-sizeOut.x / 2, sizeOut.x / 2), UnityEngine.Random.Range(-sizeOut.y / 2, sizeOut.y / 2), UnityEngine.Random.Range(-sizeOut.z / 2, sizeOut.z / 2));

        while (Physics.CheckSphere(pos, spawnRadius * ((pos.x+pos.y+pos.z*1.25) > (sizeIn.x + sizeIn.y + sizeIn.z) ? 1 : 2), mask))
        {
            pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-sizeOut.x / 2, sizeOut.x / 2), UnityEngine.Random.Range(-sizeOut.y / 2, sizeOut.y / 2), UnityEngine.Random.Range(-sizeOut.z / 2, sizeOut.z / 2));
            count++;
            if (count > 500)
            {
                return 0;
            }
        }

        GameObject asteroid = Instantiate(AsteroidprefabOut, pos, AsteroidprefabOut.transform.rotation);
        asteroid.transform.parent = OutGroup;
        Obstacle o = asteroid.GetComponent<Obstacle>();
        o.manager = this;
        return 1;
    }
    public int AsteroidsspawnMovableSpawner()
    {
        int count = 0;


        Vector3 pos = transform.localPosition + center + new Vector3(0, 4, UnityEngine.Random.Range(-sizeOut.z / 2, sizeOut.z / 4));

        GameObject asteroid = Instantiate(AsteroidprefabMoveSpawner, pos, AsteroidprefabMoveSpawner.transform.rotation);
        asteroid.transform.parent = SpawnGroup;
        asteroid.GetComponent<SpawnAsteroid>().manager = this;
        return 1;
    }

    public void spawnMovingAsteroid(Vector3 posIn)
    {
        int count = 0;

        float x = UnityEngine.Random.Range(0, 2) == 0 ? sizeOut.x / 2 + movingSpawnOffsetX : -sizeOut.x / 2 - movingSpawnOffsetX;
        float y = UnityEngine.Random.Range(-movingSpawnOffsetY, movingSpawnOffsetY);

        Vector3 pos = new Vector3(x, y, posIn.z);

        while (Physics.CheckSphere(pos, spawnRadius * ((pos.x + pos.y + pos.z * 1.25) > (sizeIn.x + sizeIn.y + sizeIn.z) ? 1 : 2), mask))
        {

            x = UnityEngine.Random.Range(0, 2) == 0 ? sizeOut.x / 2 + movingSpawnOffsetX : -sizeOut.x / 2 - movingSpawnOffsetX;
            y = UnityEngine.Random.Range(-movingSpawnOffsetY, movingSpawnOffsetY);

            pos = new Vector3(x, y, posIn.z);
            count++;
            if (count > 25)
            {
                return;
            }
        }

        GameObject asteroid = Instantiate(AsteroidprefabMove, pos, AsteroidprefabMove.transform.rotation);
        asteroid.transform.parent = MoveGroup;
        Obstacle o = asteroid.GetComponent<Obstacle>();
        o.manager = this;

        Movement mv = asteroid.GetComponent<Movement>();
        mv.targetPos = posIn + new Vector3(-x, -y, posIn.z);
        mv.move = true;
    }

    private void OnDrawGizmosSelected()
    {
        if(drawIn) {
            drawOut = false;

            Gizmos.color = new Color(1, 0, 0, 0.5f);

            Gizmos.DrawCube(transform.localPosition + center, sizeIn);
        }
        if (drawOut)
        {
            drawIn = false;

            Gizmos.color = new Color(1, 0, 0, 0.5f);

            Gizmos.DrawCube(transform.localPosition + center, sizeOut);
        }

    }
}