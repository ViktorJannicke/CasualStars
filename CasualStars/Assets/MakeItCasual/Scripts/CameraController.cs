using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 cameraPos1;
    public Vector3 cameraPos2;
    /*public Vector3 cameraRot1;
    public Vector3 cameraRot2;*/

    private void Start()
    {
        InputManager im = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InputManager>();
        im.camController = this;
        im.Main = gameObject.GetComponent<Camera>();
    }

    public void translateAway()
    {
        transform.localPosition = cameraPos2;
    }

    public void translateTowards()
    {
        transform.localPosition = cameraPos1;
    }
}