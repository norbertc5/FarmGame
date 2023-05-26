using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    [HideInInspector] public bool isBobing;
    float bobbingSpeed = 3;
    float bobbingAmplitude = 0.2f;
    [SerializeField] float defaultSpeed = 6;
    [SerializeField] float defaultAmplitude = 0.2f;
    float elapsedTime;
    float defaulyYPos;
    bool hasStartedReturning;

    private void Awake()
    {
        defaulyYPos = transform.localPosition.y;
    }

    void Update()
    {
        if(PlayerMovement.IsWalking || PlayerMovement.IsRunning)
        {
            // set bobbingSpeed and swayAmplitude according to player swaySpeed
            if (PlayerMovement.IsWalking)
            {
                bobbingSpeed = defaultSpeed;
                bobbingAmplitude = defaultAmplitude;
            }
            else if (PlayerMovement.IsRunning)
            {
                bobbingSpeed = defaultSpeed * 2;
                bobbingAmplitude = defaultAmplitude * 2;
            }

            // bobbing
            if(!PlayerMovement.IsCrouching)
            {
                elapsedTime += bobbingSpeed * Time.deltaTime;
                transform.localPosition = new Vector3(transform.localPosition.x, defaulyYPos + Mathf.Sin(elapsedTime) * bobbingAmplitude);
                hasStartedReturning = false;
            }
        }
        else if(!hasStartedReturning)
        {
            StartCoroutine(ReturnCamera());
            hasStartedReturning = true;
        }
    }

    IEnumerator ReturnCamera()
    {
        // camera returns to started positon when player isn'timeToStartSway moving
        Vector3 startPos = transform.localPosition;
        float t = 0;

        while(transform.localPosition.y != defaulyYPos)
        {
            t += bobbingSpeed * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPos, new Vector3(transform.localPosition.x, defaulyYPos), t);

            yield return null;
        }
    }
}
