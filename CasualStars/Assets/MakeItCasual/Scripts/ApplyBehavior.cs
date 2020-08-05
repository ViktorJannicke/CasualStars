using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyBehavior : MonoBehaviour
{
    public enum Behavior { none, floating, floating_look_at_Player };
    public Behavior behavior;

    public float val = 0;
    public float maxLeft;
    public float maxRight;
    public float maxUp;
    public float maxDown;
    public float maxForward;
    public float maxBackward;
    public bool turnLeft;
    public bool turnUp;
    public bool turnForward;
    public bool activated = true;
    public Transform player;
    public GameObject visual;

    bool applyOnX;
    bool applyOnY;
    bool applyOnZ;

    GameObject lastHit;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AsteroidOut") && collision.gameObject.CompareTag("AsteroidIn") && lastHit != collision.gameObject)
        {
            lastHit = collision.gameObject;
            ApplyBehavior bh = collision.gameObject.GetComponent<ApplyBehavior>();
            bh.turnLeft = !bh.turnLeft;
        }
    }

    private void Start()
    {
        visual.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        applyOnX = (Random.Range(0, 2) == 0 ? true : false);
        applyOnY = (Random.Range(0, 2) == 0 ? true : false);
        applyOnZ = (Random.Range(0, 2) == 0 ? true : false);
    }

    float count = 0;
    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            if (behavior == Behavior.floating)
            {
                Vector3 rot = visual.transform.rotation.eulerAngles;
                Vector3 pos = transform.position;
                if (applyOnX && turnLeft)
                {

                    float angle = pos.x;

                    if (angle > maxLeft)
                    {
                        turnLeft = false;
                    }
                    else
                    {
                        rot.x += val;
                        pos.x += val;
                    }
                }
                else if (applyOnX && !turnLeft)
                {
                    float angle = pos.x;

                    if (angle < maxRight)
                    {
                        turnLeft = true;
                    }
                    else
                    {
                        rot.x -= val;
                        pos.x -= val;
                    }
                }

                if (applyOnY && turnUp)
                {

                    float angle = pos.y;

                    if (angle > maxUp)
                    {
                        turnUp = false;
                    }
                    else
                    {
                        rot.y += val;
                        pos.y += val;
                    }
                }
                else if (applyOnY && !turnUp)
                {
                    float angle = pos.y;

                    if (angle < maxDown)
                    {
                        turnUp = true;
                    }
                    else
                    {
                        rot.y -= val;
                        pos.y -= val;
                    }
                }

                if (applyOnZ && turnForward)
                {

                    float angle = pos.z;

                    if (angle > maxForward)
                    {
                        turnForward = false;
                    }
                    else
                    {
                        rot.z += val;
                        pos.z += val;
                    }
                }
                else if (applyOnY && !turnUp)
                {
                    float angle = pos.z;

                    if (angle < maxBackward)
                    {
                        turnForward = true;
                    }
                    else
                    {
                        rot.z -= val;
                        pos.z -= val;
                    }
                }
                visual.transform.rotation = Quaternion.Euler(rot);
                transform.position = pos;
            }
            else if (behavior == Behavior.floating_look_at_Player)
            {
                visual.transform.LookAt(player);

                Vector3 pos = transform.position;
                if (applyOnX && turnLeft)
                {

                    float angle = pos.x;
                    if (angle > maxLeft)
                    {
                        turnLeft = false;
                    }
                    else
                    {
                        pos.x += val;
                    }
                }
                else if (applyOnX && !turnLeft)
                {
                    float angle = pos.x;
                    if (angle < maxRight)
                    {
                        turnLeft = true;
                    }
                    else
                    {
                        pos.x -= val;
                    }
                }

                if (applyOnY && turnUp)
                {

                    float angle = pos.y;

                    if (angle > maxUp)
                    {
                        turnUp = false;
                    }
                    else
                    {
                        pos.y += val;
                    }
                }
                else if (applyOnY && !turnUp)
                {
                    float angle = pos.y;

                    if (angle < maxDown)
                    {
                        turnUp = true;
                    }
                    else
                    {
                        pos.y -= val;
                    }
                }

                if (applyOnZ && turnForward)
                {

                    float angle = pos.z;

                    if (angle > maxForward)
                    {
                        turnForward = false;
                    }
                    else
                    {
                        pos.z += val;
                    }
                }
                else if (applyOnY && !turnUp)
                {
                    float angle = pos.z;

                    if (angle < maxBackward)
                    {
                        turnForward = true;
                    }
                    else
                    {
                        pos.z -= val;
                    }
                }
                transform.position = pos;
            }
        }
    }
}
