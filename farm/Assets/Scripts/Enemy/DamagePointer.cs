using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DamagePointer : MonoBehaviour
{
    // this script points how much damage should enemy take after hittiong by raycast specyfic body part

    public enum BodyParts { Limb, Body, Head};
    public BodyParts bodyPart;
}
