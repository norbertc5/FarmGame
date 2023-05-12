using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Moveement Settings")]
    [SerializeField] float speed = 10;
    [SerializeField] float jumpHeight = 10;
    [SerializeField] float mass = 5;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    float gravity = -10;
    bool isGrounded;
    float xInput;
    float yInput;
    CharacterController characterController;
    Vector3 velocity;
    const float DEFAULT_SPEED = 10;
    const float SPRINT_SPEED = 20;

    [Header("Audio")]
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;
    int actualFootstepSoundIndex;
    AudioSource playerSource;
    bool hasPlayedLandSoundPlayed;
    Coroutine landSoundCoroutine;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(PlayFootstepSounds());
    }

    void Update()
    {
        #region Movement

        xInput = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        yInput = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        Vector3 move = transform.right * xInput + transform.forward * yInput;

        // player do not speed up when key on horizontal and vertical axis are pressed at the same time
        if(xInput != 0 && yInput != 0)
          move = move.normalized * speed * Time.deltaTime;

        characterController.Move(move);

        #endregion

        #region Jump

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            playerSource.PlayOneShot(jumpSound);
            hasPlayedLandSoundPlayed = false;
        }

        // land sound plays when player jumps or falls
        if(!isGrounded)
        {
            hasPlayedLandSoundPlayed = false;
            landSoundCoroutine = StartCoroutine(PlayLandSound());
        }

        #endregion

        #region Gravity

        // gravity from rigidbody does not work with character controller
        velocity.y += gravity * mass * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.6f, groundLayer);

        // when player is on the ground, velocity restarts
        if (isGrounded && velocity.y < 0)
            velocity.y = -2;

        #endregion

        #region Sprint

        if (Input.GetKeyDown(KeyCode.LeftShift))
            speed = SPRINT_SPEED;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            speed = DEFAULT_SPEED;

        #endregion
    }

    IEnumerator PlayFootstepSounds()
    {
        while(true)
        {
            if((xInput != 0 || yInput != 0) && isGrounded)
            {
                playerSource.PlayOneShot(footstepSounds[actualFootstepSoundIndex]);
                actualFootstepSoundIndex++;

                if (actualFootstepSoundIndex > footstepSounds.Length - 1)
                    actualFootstepSoundIndex = 0;

                yield return new WaitForSeconds((1 / speed) * 5);
            }
            yield return null;
        }
    }

    IEnumerator PlayLandSound()
    {
        // delay to play land sound no right after jump
        yield return new WaitForSeconds(0.1f);

        while(!hasPlayedLandSoundPlayed)
        {
            if (isGrounded && !hasPlayedLandSoundPlayed)
            {
                playerSource.PlayOneShot(landSound);
                hasPlayedLandSoundPlayed = true;
                StopCoroutine(landSoundCoroutine);
            }
            yield return null;
        }
    }
}
