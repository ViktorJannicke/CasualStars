using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ripple : MonoBehaviour
{

    public float val = 0;
    public float maxLeft;
    public float maxRight;
    public bool turnLeft;

    public RotateSkybox rtsb;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(turnLeft)
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
                rtsb.sb_Rot.z += val;
                transform.rotation = Quaternion.Euler(rot);
                Vector3 pos = transform.position;
                pos.x += val/4;
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
                rtsb.sb_Rot.z -= val;
                transform.rotation = Quaternion.Euler(rot);
                Vector3 pos = transform.position;
                pos.x -= val/4;
                transform.position = pos;
            }
        }

    }
}
