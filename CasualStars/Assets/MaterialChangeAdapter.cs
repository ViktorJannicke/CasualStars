using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChangeAdapter : MonoBehaviour
{

    public Renderer[] targetRenderers;
    [Range(0f, 1f)]
    public float emissiveIntensity = 0f;
    public Color baseEmissiveColor;

    // You may want to alter this value depending on how intense you want your emissive strength to be
    private const float MaxIntensity = 1000f;

    private MaterialPropertyBlock[] _propBlocks;

    private void Start()
    {
        _propBlocks = new MaterialPropertyBlock[targetRenderers.Length];
        for (int i = 0; i < _propBlocks.Length; i++)
        {
            _propBlocks[i] = new MaterialPropertyBlock();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < _propBlocks.Length; i++)
        {
            // Get Material Property Block from Material at first index on specified Glove Renderer
            targetRenderers[i].GetPropertyBlock(_propBlocks[i], 0);

        _propBlocks[i].SetColor("_EmissiveColor", baseEmissiveColor * Mathf.Lerp(0f, MaxIntensity, emissiveIntensity));

        // Update Material on specified Glove Renderer at first index of materials
        targetRenderers[i].SetPropertyBlock(_propBlocks[i], 0);
        }
    }
}
