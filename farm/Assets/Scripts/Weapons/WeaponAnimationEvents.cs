using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationEvents : MonoBehaviour
{
    [SerializeField] AudioSource source;
    Animator animator;
    Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
    }

    void PlaySound(AudioClip sound)
    {
        source.PlayOneShot(sound);
    }

    void PlayAnimation(string animName)
    {
        // using to e.g. play shotgun pump animation after shot
        animator.CrossFade(animName, 0);
    }

    void TurnOffWeapons()
    {
        // using to turn off weapons when death
        if(player.playerHealth <= 0)
            this.gameObject.SetActive(false);
    }
}
