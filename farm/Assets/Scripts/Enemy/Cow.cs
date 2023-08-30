using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : Enemy
{
    private void Awake()
    {
        AssignComponents();
        SetRagdollActive(false);
    }
}
