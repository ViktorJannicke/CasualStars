﻿using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 51f; //vergessen im gamemanager einzustellen//
    public bool move;
    public bool isSatelite;
    public Transform target;
    public Vector3 targetPos;
    public int requiredDistance = 1;
    public bool changeScoreOnTargetReached = false;
    public int ScoreBonus = 1;
    public bool destoryOnTargetReached;

    private void FixedUpdate()
    {
        if (move)
        {
            if (target != null)
            {
                    targetPos = target.position;
            }

            var heading = targetPos - transform.position;
            var distance = heading.magnitude;
            Vector3 direction = heading / distance;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (distance < requiredDistance)
            {
                move = false;

                if (changeScoreOnTargetReached)
                {
                    NGameManager.manager.Score += ScoreBonus;
                }
                if (destoryOnTargetReached)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void stop()
    {
        targetPos = transform.position;
    }

    public bool isStopped { get { return !move; } }
}
