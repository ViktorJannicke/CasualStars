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
    public GameObject[] AsteroidprefabIn;
    public Transform InGroup;
    public GameObject[] AsteroidprefabOut;
    public Transform OutGroup;
    public GameObject[] AsteroidprefabMove;
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
    public int[] spawnCountIn;
    public int[] spawnCountOut;
    public int[] spawnCountMove;
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
        
        player.GetComponent<Movement>().speed = playerSpeed[MasterManager.mm.difficulty];

        int counter = 0;
        for (int i = 0; i <= spawnCountIn[MasterManager.mm.difficulty]; i++)
        {
            counter += AsteroidsspawnIn();
        }
        Debug.Log(counter);

        counter = 0;
        for (int i = 0; i <= spawnCountOut[MasterManager.mm.difficulty]; i++)
        {
            counter += AsteroidsspawnOut();
        }
        Debug.Log(counter);

        counter = 0;
        for (int i = 0; i <= spawnCountMove[MasterManager.mm.difficulty]; i++)
        {
            counter += AsteroidsspawnMovableSpawner();
        }
        Debug.Log(counter);
    }

    public int AsteroidsspawnIn()
    {
        int count = 0;
        int selector = UnityEngine.Random.Range(0, AsteroidprefabIn.Length);

        Vector3 pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-sizeIn.x / 2, sizeIn.x / 2), UnityEngine.Random.Range(-sizeIn.y / 2, sizeIn.y / 2), UnityEngine.Random.Range(-sizeIn.z / 2, sizeIn.z / 2));
        float avarageScale = (AsteroidprefabIn[selector].transform.localScale.z + AsteroidprefabIn[selector].transform.localScale.y + AsteroidprefabIn[selector].transform.localScale.z) / 4;
        while (Physics.CheckSphere(pos, spawnRadius * avarageScale, mask))
        {
            pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-sizeIn.x / 2, sizeIn.x / 2), UnityEngine.Random.Range(-sizeIn.y / 2, sizeIn.y / 2), UnityEngine.Random.Range(-sizeIn.z / 2, sizeIn.z / 2));
            count++;
            if(count > 500)
            {
                return 0;
            }
        }
        GameObject prefab = AsteroidprefabIn[selector];
        GameObject asteroid = Instantiate(prefab, pos, prefab.transform.rotation);
        asteroid.transform.parent = InGroup;
        Obstacle o = asteroid.GetComponent<Obstacle>();
        o.manager = this;
        return 1;
    }
    public int AsteroidsspawnOut()
    {
        int count = 0;
        int selector = UnityEngine.Random.Range(0, AsteroidprefabOut.Length);

        Vector3 pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-sizeOut.x / 2, sizeOut.x / 2), UnityEngine.Random.Range(-sizeOut.y / 2, sizeOut.y / 2), UnityEngine.Random.Range(-sizeOut.z / 2, sizeOut.z / 2));
        float avarageScale = (AsteroidprefabOut[selector].transform.localScale.z + AsteroidprefabOut[selector].transform.localScale.y + AsteroidprefabOut[selector].transform.localScale.z) / 4;
        while ((pos.x < (sizeIn.x / 2) || pos.y < (sizeIn.y / 2) || pos.x > -(sizeIn.x / 2) || pos.y > -(sizeIn.y / 2)) && Physics.CheckSphere(pos, spawnRadius * avarageScale, mask))
        {
            pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-sizeOut.x / 2, sizeOut.x / 2), UnityEngine.Random.Range(-sizeOut.y / 2, sizeOut.y / 2), UnityEngine.Random.Range(-sizeOut.z / 2, sizeOut.z / 2));
            count++;
            if (count > 500)
            {
                return 0;
            }
        }
        GameObject prefab = AsteroidprefabOut[selector];
        GameObject asteroid = Instantiate(prefab, pos, prefab.transform.rotation);
        asteroid.transform.parent = OutGroup;
        Obstacle o = asteroid.GetComponent<Obstacle>();
        o.manager = this;
        return 1;
    }
    public int AsteroidsspawnMovableSpawner()
    {
        int count = 0;

        Vector3 pos = transform.localPosition + center + new Vector3(0, 4, UnityEngine.Random.Range(-sizeOut.z / 2, sizeOut.z*0.25f));
        GameObject prefab = AsteroidprefabMoveSpawner;
        GameObject asteroid = Instantiate(prefab, pos, prefab.transform.rotation);
        asteroid.transform.parent = SpawnGroup;
        asteroid.GetComponent<SpawnAsteroid>().manager = this;
        return 1;
    }

    public void spawnMovingAsteroid(Vector3 posIn)
    {
        int count = 0;
        int selector = UnityEngine.Random.Range(0, AsteroidprefabMove.Length);

        float x = UnityEngine.Random.Range(0, 2) == 0 ? sizeOut.x / 2 + movingSpawnOffsetX : -sizeOut.x / 2 - movingSpawnOffsetX;
        float y = UnityEngine.Random.Range(-movingSpawnOffsetY, movingSpawnOffsetY);

        Vector3 pos = new Vector3(x, y, posIn.z);
        float avarageScale = (AsteroidprefabMove[selector].transform.localScale.z + AsteroidprefabMove[selector].transform.localScale.y + AsteroidprefabMove[selector].transform.localScale.z) / 4;
        while ((pos.x < (sizeIn.x / 2) || pos.y < (sizeIn.y / 2) || pos.x > -(sizeIn.x / 2) || pos.y > -(sizeIn.y / 2)) && Physics.CheckSphere(pos, spawnRadius * avarageScale, mask))
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
        GameObject prefab = AsteroidprefabMove[selector];
        GameObject asteroid = Instantiate(prefab, pos, prefab.transform.rotation);
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