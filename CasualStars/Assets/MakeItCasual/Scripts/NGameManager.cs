using System.Collections.Generic;
using UnityEngine;

public class NGameManager : MonoBehaviour
{
    public static NGameManager manager;

    [Header("Player Base Values")]
    public int bHealth;
    public int bShield;
    public int bScore;
    public int bCredits;

    [Header("Other Values")]
    public GameObject Asteroidprefab;
    public Vector3 center;
    public Vector3 size;
    public bool stopSpawning = false;
    public int spawnTime;
    public int spawnDelay;
    public int spawnRadius;

    public float timer = 0;
    public float maxTime = 300;

    void Start()
    {
        /*PlayerData[] db = new PlayerData[playerDataPair.Count];
        playerDataPair.Values.CopyTo(db, 0);*/

        if(manager != null)
        {
            Destroy(gameObject);
            return;
        }

        manager = this;

    }


    GameObject spawnPlayer()
    {
        Vector3 pos = new Vector3(25,0,25);//center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 3, Random.Range(-size.z / 2, size.z / 2));

        /*while (Physics.CheckSphere(pos, spawnRadius))
        {
            pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 3, Random.Range(-size.z / 2, size.z / 2));
        }*/
        GameObject m = Instantiate(Asteroidprefab, pos, Quaternion.identity);
        return m;
    }

    public void Asteroidsspawn()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));

        while (Physics.CheckSphere(pos, spawnRadius))
        {
            pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
        }
        Instantiate(Asteroidprefab, pos, Quaternion.identity);
    }

    public Vector3 getNextPosition()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));

        while (Physics.CheckSphere(pos, spawnRadius))
        {
            pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
        }

        return pos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(transform.localPosition + center, size);
    }
}