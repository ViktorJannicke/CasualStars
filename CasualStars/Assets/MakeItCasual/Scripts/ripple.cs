using Boo.Lang;
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

    public bool fokusCameraOnNearestObject = false;

    public float speed2 = 1;
    Transform fokusObject;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!fokusCameraOnNearestObject && activated)
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
                    rot.z += (mainmenu ? valmainMenu : val);
                    rtsb.sb_Rot.z += (mainmenu ? valmainMenu : val) / 2;
                    transform.rotation = Quaternion.Euler(rot);
                    Vector3 pos = transform.position;
                    pos.x += (mainmenu ? valmainMenu : val) / (mainmenu ? 32 : 16);
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
                    rot.z -= (mainmenu ? valmainMenu : val);
                    rtsb.sb_Rot.z -= (mainmenu ? valmainMenu : val) / 2;
                    transform.rotation = Quaternion.Euler(rot);
                    Vector3 pos = transform.position;
                    pos.x -= (mainmenu ? valmainMenu : val) / (mainmenu ? 32 : 16);
                    transform.position = pos;
                }
            }
        } else if(fokusCameraOnNearestObject)
        {
            if(fokusObject == null)
            {
                float refDistance = 100000000;
                GameObject[] objects = GameObject.FindGameObjectsWithTag("AsteroidIn");
                foreach(GameObject obj in objects)
                {
                    if (obj == null)
                        continue;

                    float distance = Vector3.Distance(obj.transform.position, transform.position);
                    if (distance < refDistance)
                    {
                        refDistance = distance;
                        fokusObject = obj.transform;
                    }
                }
            }
            else
            {
                Quaternion OriginalRot = transform.rotation;
                transform.LookAt(fokusObject);
                Quaternion NewRot = transform.rotation;
                transform.rotation = OriginalRot;
                transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, speed2 * Time.deltaTime);
            }
        }
    }
}

public static class ExtensionMethods
{

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static Vector3 Remap(this Vector3 value, float from1, float to1, float from2, float to2)
    {
        Vector3 val;
        val.x =  (value.x - from1) / (to1 - from1) * (to2 - from2) + from2;
        val.y = (value.y - from1) / (to1 - from1) * (to2 - from2) + from2;
        val.z = (value.z - from1) / (to1 - from1) * (to2 - from2) + from2;
        return val;
    }

}