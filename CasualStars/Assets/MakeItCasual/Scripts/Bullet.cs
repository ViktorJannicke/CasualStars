using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bullet : MonoBehaviour
{
    float timer = 0;
    public int bulletDamage = 25;
    public float maxTime;


    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= maxTime)
            Destroy(gameObject);
    }
}
