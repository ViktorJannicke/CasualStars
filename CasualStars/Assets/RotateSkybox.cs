using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Skybox))]
public class RotateSkybox : MonoBehaviour
{
    public Vector3 sb_Rot;
    Material mt;

    float time = 0;
    void Start()
    {
        // Construct a rotation matrix and set it for the shader

        mt = GetComponent<Skybox>().material;
    }

    private void Update()
    {
        sb_Rot.x += 0.0003f * time;
        time += Time.deltaTime;
        Quaternion rot = Quaternion.Euler(sb_Rot);
        Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rot, new Vector3(1, 1, 1));
        mt.SetMatrix("_Rotation", m);
    }
}
