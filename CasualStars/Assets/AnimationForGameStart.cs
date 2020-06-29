using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationForGameStart : MonoBehaviour
{
    public GameObject Camera;
    public Vector3 cameraPos1;
    public Vector3 cameraPos2;
    public Vector3 cameraRot1;
    public Vector3 cameraRot2;
    public int timerMax = 40;

    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer < timerMax)
        {
            Vector3 pos = Camera.transform.position;
            pos.x = cameraPos1.x + translate(cameraPos1.x, cameraPos2.x, timer);
            pos.y = cameraPos1.y + translate(cameraPos1.y, cameraPos2.y, timer);
            pos.z = cameraPos1.z + translate(cameraPos1.z, cameraPos2.z, timer);
            Camera.transform.position = pos;

            Vector3 rot = Camera.transform.rotation.eulerAngles;
            rot.x = cameraRot1.x + translate(cameraRot1.x, cameraRot2.x, timer);
            rot.y = cameraRot1.y + translate(cameraRot1.y, cameraRot2.y, timer);
            rot.z = cameraRot1.z + translate(cameraRot1.z, cameraRot2.z, timer);
            Camera.transform.rotation = Quaternion.Euler(rot);
        }
    }

    public float translate(float val1, float val2, float time)
    {
        float next = 0;

        next = val2 - val1;

        if(time > 40)
        {
            return next / timerMax * 40;
        } else {
            return next / timerMax * time;
        }
    }
}
