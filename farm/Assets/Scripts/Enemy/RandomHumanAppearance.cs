using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.VersionControl;

public class RandomHumanAppearance : RandomApearanceContainer
{
    private void Awake()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        hairObjects = new GameObject[3];

        for (int i = 0; i < hairObjects.Length; i++)
        {
            hairObjects[i] = transform.GetChild(i+1).gameObject;
        }
    }

    void Start()
    {
        // draw materials and assign it to mesh
        int faceIndex = UnityEngine.Random.Range(0, faceMats.Count);
        int hairIndex = UnityEngine.Random.Range(0, hairMats.Count);
        Material[] m = new Material[] { trousers, boots, shirt,
            skinMats[skinMatsIndexes[faceIndex]],
            faceMats[faceIndex],
            hairMats[hairIndex] };
        skinnedMeshRenderer.materials = m;
        try { hairObjects[UnityEngine.Random.Range(0, hairObjects.Length)].SetActive(true); } catch { }

        for (int i = 0; i < hairObjects.Length; i++)
        {
            try { hairObjects[i].GetComponent<SkinnedMeshRenderer>().materials = new Material[] { hairMats[hairIndex] }; } catch { }
        }
    }
}
