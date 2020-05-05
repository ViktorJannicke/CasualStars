using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 cameraPos1;
    public Vector3 cameraPos2;
    /*public Vector3 cameraRot1;
    public Vector3 cameraRot2;*/

    bool direction;

    public float seconds = 10;
    public float timer = 12;
    public Vector3 Difference;
    Vector3 start;
    public float percent;

    void getDifference()
    {
        if (direction)
        {
            Difference = cameraPos2 - cameraPos1;
            start = cameraPos1;
        }
        else
        {
            Difference = cameraPos1 - cameraPos2;
            start = cameraPos2;
        }
    }

    public void translateAway()
    {
        timer = 0;
        direction = true;
        getDifference();
    }

    public void translateTowards()
    {
        timer = 0;
        direction = false;
        getDifference();
    }

    private void Update()
    {
        if (timer <= seconds)
        {
            timer += Time.deltaTime;
            percent = timer / seconds;
            transform.localPosition = start + Difference * percent;
        }
    }
}