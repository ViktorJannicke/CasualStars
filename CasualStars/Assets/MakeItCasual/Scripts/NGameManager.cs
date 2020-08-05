using UnityEngine;
using TMPro;
using System;
using UnityEngine.Playables;
using Boo.Lang;
using UnityEngine.UI;

public class NGameManager : MonoBehaviour
{
   // public static NGameManager manager;

    public int Score = 25;
    public TextMeshProUGUI score;
    public TextMeshProUGUI time;

    public float timer = 120;
    public float[] timervals = {60,30};

    [Header("Asteroid Control Values")]
    public GameObject[] AsteroidprefabIn;
    public Transform InGroup;
    public GameObject[] AsteroidprefabOut;
    public Transform OutGroup;
    public GameObject player;

    [Header("Asteroid Spawn Values")]
    public LayerMask mask;
    public Vector3 center;
    public Vector3 sizeIn;
    public Vector3 sizeInOffset;
    public Vector3 sizeOut;
    public int spawnRadius;
    public int[] spawnCountIn;
    public int[] spawnCountOut;

    [Header("Asteroid Debug")]
    public bool drawIn;
    public bool drawOut;

    [Header("GameStart")]
    public GameObject TopBar;
    public bool gameStarted;
    public float gameStartTime = 6;
    public TextMeshProUGUI gameStartText;
    public float[] playerSpeed;
    public Transform portal;

    [Header("GameEnd")]
    public bool gameEnd = false;
    public SceneManagement sm;

    bool once = true;

    public static NGameManager manager;
    public Transform canvas;
    public Transform scoreText;
    public Slider progressBar;
    private void Update()
    {
        if (gameEnd)
        {
                if (once)
                {
                    once = false;
                    MasterManager.mm.lastScore += Score;
                    if (MasterManager.mm.difficulty < MasterManager.mm.maxDifficulty)
                    {
                        sm.LoadTransition();
                    }
                    else
                    {
                        sm.LoadAdFromGame();
                    }
                }
        }
        else if (gameStarted)
        {
            score.text = "Score: " + Score;
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer) - (minutes * 60);

            DateTime d = new DateTime(2001, 01, 01, 01, minutes, seconds);


            time.text = "Time: " + d.ToString("mm:ss");

            float t = Time.deltaTime;
            if (timer - t <= 0.1f)
            {
                if (once)
                {
                    gameStarted = false;
                    gameEnd = true;
                    Camera.main.gameObject.GetComponent<ripple>().activated = false;
                    Camera.main.gameObject.GetComponent<RotateSkybox>().activated = false;
                }
            }
            else if (timer - t <= 7.01 && timer - t >= 6.01f)
            {
                once = true;
                timer -= t;
            }
            else
            {
                if(timer == timervals[MasterManager.mm.difficulty])
                {
                    TopBar.SetActive(true);
                }
                progressBar.value = player.transform.position.z;

                timer -= t;
            }
        }
        else
        {
            float t = Time.deltaTime;
            gameStartTime -= t;
            if (gameStartTime <= 0.01)
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
                if (gameStartTime <= 1.01f)
                {
                    gameStartText.text = "Los";
                }
                else if (gameStartTime <= 2.01f)
                {
                    gameStartText.text = "1";
                }
                else if (gameStartTime <= 3.01f)
                {
                    gameStartText.text = "2";
                }
                else if (gameStartTime <= 4.01f)
                {
                    gameStartText.text = "3";
                }
            }
        }
    }

    private void Awake()
    {
        manager = this;
        timer = timervals[MasterManager.mm.difficulty];
    }

    private void Start()
    {
        progressBar.minValue = player.transform.position.z;
        progressBar.maxValue = portal.transform.position.z;

        int scoreValue = 0;
        foreach (int val in spawnCountIn) { scoreValue += val; }
        

        TopBar.SetActive(false);
        Vector3 pos = portal.position;
        pos.z = sizeOut.z - 250;
        portal.position = pos;

        player.GetComponent<Movement>().speed = playerSpeed[MasterManager.mm.difficulty];

        int counter = 0;
        int grandCounter = 0;
        for (int i = 0; i < spawnCountIn[MasterManager.mm.difficulty]; i++)
        {
            counter += AsteroidsspawnIn();
        }
        Debug.Log(counter);
        grandCounter += (counter*5);

        counter = 0;
        for (int i = 0; i < spawnCountOut[MasterManager.mm.difficulty]; i++)
        {
            counter += AsteroidsspawnOut();
        }
        Debug.Log(counter);
        grandCounter += (counter*1);
        MasterManager.mm.maxScore += grandCounter;
        checkAlongPath();
    }

    public void checkAlongPath()
    {
        for (int i = 0; i < sizeOut.z; i++)
        {
            Collider[] colliders = Physics.OverlapSphere(new Vector3(0,4,i), 50, mask);
            foreach(Collider col in colliders)
            {
                if(col.CompareTag("AsteroidOut"))
                {
                    Destroy(col.gameObject);
                }
            }
        }
    }

    public int AsteroidsspawnIn()
    {
        int count = 0;
        int selector = UnityEngine.Random.Range(0, AsteroidprefabIn.Length+1);
        if(selector == 0 || selector == 1)
        {
            selector = 0;
        } else
        {
            selector = 1;
        }

        Vector3 pos = transform.localPosition + center + sizeInOffset + new Vector3(UnityEngine.Random.Range(-sizeIn.x / 2, sizeIn.x / 2), UnityEngine.Random.Range(-sizeIn.y / 2, sizeIn.y / 2), UnityEngine.Random.Range(-sizeIn.z / 2, sizeIn.z / 2));
        float avarageScale = (AsteroidprefabIn[selector].transform.localScale.z + AsteroidprefabIn[selector].transform.localScale.y + AsteroidprefabIn[selector].transform.localScale.z) / 4;
        while (Physics.CheckSphere(pos, spawnRadius * 4f * avarageScale, mask))
        {
            pos = transform.localPosition + center + sizeInOffset + new Vector3(UnityEngine.Random.Range(-sizeIn.x / 2, sizeIn.x / 2), UnityEngine.Random.Range(-sizeIn.y / 2, sizeIn.y / 2), UnityEngine.Random.Range(-sizeIn.z / 2, sizeIn.z / 2));
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
        ApplyBehavior bh = asteroid.GetComponent<ApplyBehavior>();
        bh.turnLeft = (UnityEngine.Random.Range(0, 2) == 1 ? true : false);
        bh.player = player.transform;
        return 1;
    }
    public int AsteroidsspawnOut()
    {
        int count = 0;
        int selector = UnityEngine.Random.Range(0, AsteroidprefabOut.Length);

        Vector3 pos = transform.localPosition + center + new Vector3(UnityEngine.Random.Range(-sizeOut.x / 2, sizeOut.x / 2), UnityEngine.Random.Range(-sizeOut.y / 2, sizeOut.y / 2), UnityEngine.Random.Range(-sizeOut.z / 2, sizeOut.z / 2));
        float avarageScale = (AsteroidprefabOut[selector].transform.localScale.z + AsteroidprefabOut[selector].transform.localScale.y + AsteroidprefabOut[selector].transform.localScale.z) / 4;

        while (Physics.CheckSphere(pos, spawnRadius * 0.75f * avarageScale, mask))
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
        ApplyBehavior bh = asteroid.GetComponent<ApplyBehavior>();
        bh.turnLeft = (UnityEngine.Random.Range(0, 2) == 1 ? true : false);
        bh.player = player.transform;

        return 1;
    }

    private void OnDrawGizmosSelected()
    {
        if(drawIn) {
            drawOut = false;

            Gizmos.color = new Color(1, 0, 0, 0.5f);

            Gizmos.DrawCube(transform.localPosition + center + sizeInOffset, sizeIn);
        }
        if (drawOut)
        {
            drawIn = false;

            Gizmos.color = new Color(1, 0, 0, 0.5f);

            Gizmos.DrawCube(transform.localPosition + center, sizeOut);
        }

    }
}