using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int bulletDamage = 0;
    public float timer;
    public float timeTillDeath;
    public float spawnDelay = 5;

    bool disabled = true;
    private void Update()
    {
        if (disabled)
        {
            if (timer >= spawnDelay)
            {
                timer = 0;
                disabled = false;
            }
            else if (timer < spawnDelay)
            {
                timer += Time.deltaTime;
            }
        }
        else
        {

            if (timer >= timeTillDeath)
            {
                Destroy(gameObject);
            }
            else if (timer < timeTillDeath)
            {
                timer += Time.deltaTime;
            }
        }
    }
}
