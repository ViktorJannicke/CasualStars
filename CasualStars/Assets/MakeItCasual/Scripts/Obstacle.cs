using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Obstacle : MonoBehaviour
{
    public Movement movement;
    public GameObject target;
    public GameObject splitObject;
    public int splitCount;
    public int ScoreBonus;
    public NGameManager manager;

    float timer = 0f;

    private void Start()
    {
        movement.moveTo(target.transform.position);
    }

    private void Update()
    {
        if (timer > 1f && movement.isStopped)
        {
            if (manager.Score - 25 <= 0)
            {
                manager.Score = 0;
            }
            else
            {
                manager.Score -= 25;
            }
            Destroy(gameObject);

        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    public void Kill()
    {
        for (int i = 0; i < splitCount; i++)
        {
            GameObject splitO = Instantiate(splitObject, transform);
            splitO.GetComponent<Obstacle>().target = target;
            Movement mT = splitO.GetComponent<Movement>();
            mT.checkX = movement.checkX;
            mT.checkY = movement.checkY;
            mT.checkZ = movement.checkZ;
        }

        manager.Score += ScoreBonus;
        Destroy(gameObject);
    }
}

