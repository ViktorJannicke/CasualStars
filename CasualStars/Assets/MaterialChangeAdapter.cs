using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChangeAdapter : MonoBehaviour
{
    public Light light;
    public Color color;
    public float intensity;

    private static Color _color;
    public static float _intensity;

    public bool load;
    public bool set;

    void Start()
    {
        if(set)
        {
            setLightColor();
        }
        if(load)
        {
            loadLightColor();
        }
    }

    public void setLightColor()
    {
        _color = color;
        _intensity = intensity;
    }

    public void loadLightColor()
    {
        light.color = _color;
        light.intensity = _intensity;
    }
}
