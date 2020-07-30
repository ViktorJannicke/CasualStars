using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyBehavior : MonoBehaviour
{
    public enum Behavior {none, floating, floating_look_at_Player};
    public Behavior behavior;

    public float val = 0;
    public float maxLeft;
    public float maxRight;
    public bool turnLeft;
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
                if (turnLeft)
                {
                    Vector3 rot = visual.transform.rotation.eulerAngles;
                    float angle = rot.z;
                    angle = (angle > 180) ? angle - 360 : angle;
                    if (angle > maxLeft)
                    {
                        turnLeft = false;
                    }
                    else
                    {
                        rot.x += applyOnX ? val : 0;
                        rot.y += applyOnY ? val : 0;
                        rot.z += applyOnZ ? val : 0;
                        visual.transform.rotation = Quaternion.Euler(rot);
                        Vector3 pos = transform.position;
                        pos.x += applyOnX ? val : 0;
                        pos.y += applyOnY ? val : 0;
                        pos.z += applyOnZ ? val : 0;
                        transform.position = pos;
                    }
                }
                else
                {
                    Vector3 rot = visual.transform.rotation.eulerAngles;
                    float angle = rot.z;
                    angle = (angle > 180) ? angle - 360 : angle;
                    if (angle < maxRight)
                    {
                        turnLeft = true;
                    }
                    else
                    {
                        rot.x -= applyOnX ? val : 0;
                        rot.y -= applyOnY ? val : 0;
                        rot.z -= applyOnZ ? val : 0;
                        visual.transform.rotation = Quaternion.Euler(rot);
                        Vector3 pos = transform.position;
                        pos.x -= applyOnX ? val : 0;
                        pos.y -= applyOnY ? val : 0;
                        pos.z -= applyOnZ ? val : 0;
                        transform.position = pos;
                    }
                }
            }
            else if(behavior == Behavior.floating_look_at_Player)
            {
                if (turnLeft)
                {;
                    count += val;
                    if (count > maxLeft)
                    {
                        turnLeft = false;
                    }
                    else
                    {
                        visual.transform.LookAt(player);
                        Vector3 pos = transform.position;
                        pos.x += applyOnX ? val : 0;
                        pos.y += applyOnY ? val : 0;
                        pos.z += applyOnZ ? val : 0;
                        transform.position = pos;
                    }
                }
                else
                {
                    count -= val;
                    if (count < maxRight)
                    {
                        turnLeft = true;
                    }
                    else
                    {
                        visual.transform.LookAt(player);
                        Vector3 pos = transform.position;
                        pos.x -= applyOnX ? val : 0;
                        pos.y -= applyOnY ? val : 0;
                        pos.z -= applyOnZ ? val : 0;
                        transform.position = pos;
                    }
                }
            }
        }
    }
}
