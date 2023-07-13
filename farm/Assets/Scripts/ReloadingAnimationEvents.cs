using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadingAnimationEvents : MonoBehaviour
{
    [SerializeField] AudioSource source;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void PlaySound(AudioClip sound)
    {
        source.PlayOneShot(sound);
    }

    void PlayAnimation(string animName)
    {
        animator.CrossFade(animName, 0);
    }
}
