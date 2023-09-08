using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Transform hingesTrans;
    [SerializeField] float speed = 1;
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip closeSound;
    AudioSource source;
    float elapsedTime;
    Vector3 startRotation;
    bool isUsing;
    int useAngle;
    public bool isOpen;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(isUsing) 
        {
            // turning the door
            hingesTrans.localEulerAngles = Vector3.Lerp(startRotation, 
                new Vector3(startRotation.x, startRotation.y, startRotation.z + useAngle), elapsedTime);

            elapsedTime += Time.deltaTime * speed;
            if (elapsedTime > 1.1f)  // it's 1.1f to avoid issue
            {
                isUsing = false;
                elapsedTime = 0;
            }
        }
    }

    /// <summary> Turn door by angle. If door is open angle will be reversed to make closing. </summary>
    /// <param name="angle"></param>
    public void Use(int angle)
    {
        if (isUsing)
            return;

        isUsing = true;
        useAngle = angle;
        startRotation = hingesTrans.localEulerAngles;

        if (isOpen)
        {
            useAngle = -useAngle; //reverse angle to close 
            source.PlayOneShot(closeSound);
        }
        else
            source.PlayOneShot(openSound);

        isOpen = !isOpen;
    }
}
