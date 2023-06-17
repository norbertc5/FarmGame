using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsPool : ScriptableObject
{
    public GameObject objectPrefab;
    public int instantionsAmount = 10;
}
