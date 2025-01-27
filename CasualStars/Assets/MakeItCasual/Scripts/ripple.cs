﻿using Boo.Lang;
using UnityEngine;

public class ripple : MonoBehaviour
{

    public float val = 0;
    public float maxLeft;
    public float maxRight;
    public bool turnLeft;
    [Header("Main Menu")]
    public bool mainmenu;
    public float valmainMenu;

    public bool activated = true;

    public RotateSkybox rtsb;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            if (turnLeft)
            {
                Vector3 rot = transform.localRotation.eulerAngles;
                float angle = rot.z;
                angle = (angle > 180) ? angle - 360 : angle;
                if (angle > maxLeft)
                {
                    turnLeft = false;
                }
                else
                {
                    rot.z += (mainmenu ? valmainMenu : val);
                    rtsb.sb_Rot.z += (mainmenu ? valmainMenu : val) / 2;
                    transform.localRotation = Quaternion.Euler(rot);
                    Vector3 pos = transform.localPosition;
                    pos.x += (mainmenu ? valmainMenu : val) / (mainmenu ? 32 : 16);
                    transform.localPosition = pos;
                }
            }
            else
            {
                Vector3 rot = transform.localRotation.eulerAngles;
                float angle = rot.z;
                angle = (angle > 180) ? angle - 360 : angle;
                if (angle < maxRight)
                {
                    turnLeft = true;
                }
                else
                {
                    rot.z -= (mainmenu ? valmainMenu : val);
                    rtsb.sb_Rot.z -= (mainmenu ? valmainMenu : val) / 2;
                    transform.localRotation = Quaternion.Euler(rot);
                    Vector3 pos = transform.localPosition;
                    pos.x -= (mainmenu ? valmainMenu : val) / (mainmenu ? 32 : 16);
                    transform.localPosition = pos;
                }
            }
        }
    }
}