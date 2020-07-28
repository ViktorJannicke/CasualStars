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

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            if (behavior == Behavior.floating)
            {
                if (turnLeft)
                {
                    Vector3 rot = transform.rotation.eulerAngles;
                    float angle = rot.z;
                    angle = (angle > 180) ? angle - 360 : angle;
                    if (angle > maxLeft)
                    {
                        turnLeft = false;
                    }
                    else
                    {
                        rot.z += val;
                        transform.rotation = Quaternion.Euler(rot);
                        Vector3 pos = transform.position;
                        pos.x +=  val;
                        pos.y +=  val;
                        pos.z += val;
                        transform.position = pos;
                    }
                }
                else
                {
                    Vector3 rot = transform.rotation.eulerAngles;
                    float angle = rot.z;
                    angle = (angle > 180) ? angle - 360 : angle;
                    if (angle < maxRight)
                    {
                        turnLeft = true;
                    }
                    else
                    {
                        rot.z -= val;
                        transform.rotation = Quaternion.Euler(rot);
                        Vector3 pos = transform.position;
                        pos.x -= val;
                        pos.y -= val;
                        pos.z -= val;
                        transform.position = pos;
                    }
                }
            }
            else if(behavior == Behavior.floating_look_at_Player)
            {
                if (turnLeft)
                {
                    Vector3 rot = transform.rotation.eulerAngles;
                    float angle = rot.z;
                    angle = (angle > 180) ? angle - 360 : angle;
                    if (angle > maxLeft)
                    {
                        turnLeft = false;
                    }
                    else
                    {
                        visual.transform.LookAt(player);
                        Vector3 pos = transform.position;
                        pos.x += val;
                        pos.y += val;
                        pos.z += val;
                        transform.position = pos;
                    }
                }
                else
                {
                    Vector3 rot = transform.rotation.eulerAngles;
                    float angle = rot.z;
                    angle = (angle > 180) ? angle - 360 : angle;
                    if (angle < maxRight)
                    {
                        turnLeft = true;
                    }
                    else
                    {
                        visual.transform.LookAt(player);
                        Vector3 pos = transform.position;
                        pos.x -= val;
                        pos.y -= val;
                        pos.z -= val;
                        transform.position = pos;
                    }
                }
            }
        }
    }
}
