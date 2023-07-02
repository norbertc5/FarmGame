using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadingSoundsPlayer : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip magInSound;
    [SerializeField] AudioClip magOutSound;

    void PlayMagInSound()
    {
        source.PlayOneShot(magInSound);
    }

    void PlayMagOutSound()
    {
        source.PlayOneShot(magOutSound);
    }
}
