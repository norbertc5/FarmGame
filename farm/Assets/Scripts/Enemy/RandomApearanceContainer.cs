using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomApearanceContainer : MonoBehaviour
{
    // this class loads and contains all necessary materials for creating random NPCs appearance

    protected SkinnedMeshRenderer skinnedMeshRenderer;

    // all materials are necessary to set up random ones correctly
    static protected Material trousers;
    static protected Material boots;
    static protected Material shirt;

    // random materials
    static protected List<Material> hairMats = new List<Material>();
    static protected List<Material> faceMats = new List<Material>();
    static protected List<Material> skinMats = new List<Material>();
    protected GameObject[] hairObjects;
    static protected int[] skinMatsIndexes;

    private void Awake()
    {
        // load materials from Resources
        trousers = Resources.Load("HumanAppearance/Trousers") as Material;
        boots = Resources.Load("HumanAppearance/Boots") as Material;
        shirt = Resources.Load("HumanAppearance/Shirt") as Material;
        LoadMaterialsToArray("Hair", ref hairMats);
        LoadMaterialsToArray("Faces", ref faceMats);
        LoadMaterialsToArray("Skins", ref skinMats);

        // skinIndexes is a txt file which contains an id to SkinMats
        // based on skinIndexes content, skins are attached to faces
        // it's to make skin color simmilar to face color
        skinMatsIndexes = new int[faceMats.Count];
        TextAsset txtFile = Resources.Load("HumanAppearance/SkinIndexes") as TextAsset;
        string[] strArr = new String[10];
        strArr = txtFile.text.Split('\n');

        for (int i = 0; i < strArr.Length; i++)
        {
            skinMatsIndexes[i] = int.Parse(strArr[i]);
        }
    }

    /// <summary> Load materaials from Resources/{folderName} and assign it to a list. </summary>
    void LoadMaterialsToArray(string folderName, ref List<Material> list)
    {
        foreach(Material mat in Resources.LoadAll($"HumanAppearance/{folderName}"))
        {
            list.Add(mat);
        }
    }
}
