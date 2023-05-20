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
    PlayerMovement playerMovemnet;

    private void Awake()
    {
        defaulyYPos = transform.localPosition.y;
        playerMovemnet = FindObjectOfType<PlayerMovement>();
    }

    void Start()
    {
    }

    void Update()
    {
        if(PlayerMovement.isWalking || PlayerMovement.isRunning)
        {
            // set bobbingSpeed and bobbingAmplitude according to player speed
            if (PlayerMovement.isWalking)
            {
                bobbingSpeed = defaultSpeed;
                bobbingAmplitude = defaultAmplitude;
            }
            else if (PlayerMovement.isRunning)
            {
                bobbingSpeed = defaultSpeed * 2;
                bobbingAmplitude = defaultAmplitude * 2;
            }

            // bobbing
            if(!PlayerMovement.isCrouching)
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
        // camera returns to started positon when player isn't moving
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
