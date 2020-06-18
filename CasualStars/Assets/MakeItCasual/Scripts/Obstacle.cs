using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public GameObject splitObject;
    public int splitCount;
    public int ScoreBonus;
    public NGameManager manager;

    public void Kill()
    {
        for (int i = 0; i < splitCount; i++)
        {
            GameObject splitO = Instantiate(splitObject, transform);
            splitO.GetComponent<Obstacle>().manager = manager;
            splitO.transform.parent = null;
        }

        manager.Score += ScoreBonus;
        Destroy(gameObject);
    }
}

