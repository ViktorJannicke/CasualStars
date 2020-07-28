using UnityEngine;

[RequireComponent(typeof(Skybox))]
public class RotateSkybox : MonoBehaviour
{
    public Material skybox1;
    public Material skybox2;
    public Material skybox3;

    public Vector3 sb_Rot;
    Material mt;

    public bool activated = true;

    float time = 0;
    void Start()
    {
        // Construct a rotation matrix and set it for the shader

        Skybox sb = GetComponent<Skybox>();
        
        switch(Random.Range(0,4))
        {
            case 0:
                sb.material = skybox2;
                break;
            case 1:
                sb.material = skybox1;
                break;
            case 2:
                sb.material = skybox2;
                break;
            case 3:
                sb.material = skybox3;
                break;
            default:
                sb.material = skybox1;
                break;
        }

        mt = sb.material;
        sb.material.SetFloat("Exposure", Random.Range(0.5f, 1.5f));
    }

    private void Update()
    {
        if (activated)
        {
            sb_Rot.x += 0.0001f * time;
            time += Time.deltaTime;
            Quaternion rot = Quaternion.Euler(sb_Rot);
            Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rot, new Vector3(1, 1, 1));
            mt.SetMatrix("_Rotation", m);
        }
    }
}
