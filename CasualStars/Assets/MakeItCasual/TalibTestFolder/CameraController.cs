using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 cameraPos1;
    public Vector3 cameraPos2;
    /*public Vector3 cameraRot1;
    public Vector3 cameraRot2;*/

    public void translateAway()
    {
        transform.localPosition = cameraPos2;
    }

    public void translateTowards()
    {
        transform.localPosition = cameraPos1;
    }
}