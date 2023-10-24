using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using static Unity.Burst.Intrinsics.Arm;
using TMPro;
using Cinemachine;

public class CarController : Car
{
    [Header("Other objects")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] GameObject humanObject;
    public Transform playerGetOffTrans;
    [SerializeField] TextMeshProUGUI tractorTutorialText;
    [SerializeField] GameObject getOnTrigger;
    //[SerializeField] Transform minimapCam;
    [SerializeField] MinimapIcon minimapIcon;
    [Header("Sounds")]
    [SerializeField] AudioClip engineStartSound;
    [SerializeField] AudioClip engineStopSound;
    AudioSource source;
    bool canChangePitch;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        SetCenterOfMass();
        tractorTutorialText.text = GameManager.MarkText(tractorTutorialText.text, "38383880");
    }

    private void Update()
    {
        RotateWheelsMeshes();
        float verticalInput = Input.GetAxis("Vertical");

        // slowing down when no vertical input
        if (verticalInput == 0)
        {
            axleInfos[1].leftWheel.brakeTorque = 2000;
            axleInfos[1].rightWheel.brakeTorque = 2000;
        }
        else
        {
            axleInfos[1].leftWheel.brakeTorque = 0;
            axleInfos[1].rightWheel.brakeTorque = 0;
        }

        // braking
        if(Input.GetKey(KeyCode.Space))
        {
            axleInfos[1].leftWheel.brakeTorque = 20000;
            axleInfos[1].rightWheel.brakeTorque = 20000;
        }

        minimapIcon.transform.localEulerAngles = new Vector3(0, 0, -transform.eulerAngles.y + 180);

        #region Vehicle sound

        // change pitch to simulate engine work
        if(canChangePitch)
        {
            if (verticalInput > 0 || verticalInput < 0)
                source.pitch += Time.deltaTime / 2;
            else
                source.pitch -= Time.deltaTime / 2;
            source.pitch = Mathf.Clamp(source.pitch, 0.8f, 1.4f);
        }

        #endregion
    }

    public void FixedUpdate()
    {
        Drive(maxMotorTorque * Input.GetAxis("Vertical"), maxSteeringAngle * Input.GetAxis("Horizontal"));
    }

    /// <summary> Set all things needed to drive. </summary>
    /// <param name="getOn"></param>
    void PlayerInteraction(bool getOn)
    {
        // invoke when player get off/on vehicle
        cameraTransform.gameObject.SetActive(getOn);
        humanObject.SetActive(getOn);
        GetComponent<Rigidbody>().isKinematic = !getOn;
        tractorTutorialText.gameObject.SetActive(getOn);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        getOnTrigger.SetActive(!getOn);

        if (getOn)
            StartCoroutine(PlayEngineStartSound());
        else
            StartCoroutine(PlayEngineStopSound());
    }

    IEnumerator PlayEngineStartSound()
    {
        source.enabled = true;
        canChangePitch = false;
        source.pitch = 1;
        source.PlayOneShot(engineStartSound, 0.5f);
        yield return new WaitForSeconds(engineStartSound.length);
        canChangePitch = true;
        source.pitch = 0.8f;
        source.Play();
    }

    IEnumerator PlayEngineStopSound()
    {
        source.Stop();
        source.pitch = 0.8f;
        source.PlayOneShot(engineStopSound, 0.5f);
        yield return new WaitForSeconds(engineStopSound.length);
        source.enabled = false;
    }
}