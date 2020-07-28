﻿using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public GameObject splitObject;
    public bool split;
    public int splitCount;
    public int ScoreBonus;
    public NGameManager manager;
    public MeshRenderer[] targetRender;
    public Material baseMaterial;
    public Material replaceMaterial;

    public void Kill()
    {
        if (split)
        {
            int count = 1;
            for (int i = 0; i < splitCount; i++)
            {
                GameObject splitO = Instantiate(splitObject, transform.position + new Vector3(0, 0, count*10), transform.rotation);
                splitO.GetComponent<Obstacle>().manager = manager;
                splitO.transform.parent = null;
                count++;
            }
        }

        manager.Score += ScoreBonus;
        Destroy(gameObject);
    }

    public void showOutline()
    {
        foreach (MeshRenderer renderer in targetRender)
        {
            renderer.material = replaceMaterial;
        }
    }
    public void hideOutline()
    {
        foreach (MeshRenderer renderer in targetRender)
        {
            renderer.material = baseMaterial;
        }
    }
}

