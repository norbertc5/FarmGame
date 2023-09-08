using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public const float WALK_SPREAD = 100;
    public const float RUN_SPREAD = 150;
    public const float JUMP_SPREAD = 60;
    public const float CROUCH_SPREAD = 90;

    [SerializeField] float minSpread = 70;
    [SerializeField] float maxSpread = 200;
    [SerializeField] float returningSpeed = 1;
    RectTransform crosshairRectTransform;
    public static float spread;
    public static float baseSpread = 100;

    void Awake()
    {
        crosshairRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        spread = Mathf.Clamp(spread, minSpread, maxSpread);
        crosshairRectTransform.sizeDelta = new Vector2(baseSpread + spread, baseSpread + spread);
        spread -= returningSpeed * Time.deltaTime;
    }
}
