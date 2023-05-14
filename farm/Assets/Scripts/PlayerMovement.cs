using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 10;
    float xInput;
    float yInput;
    CharacterController characterController;
    const float DEFAULT_SPEED = 10;
    const float SPRINT_SPEED = 20;
    const float CROUCH_SPEED = 5;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 10;
    [SerializeField] float mass = 5;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckRadius = 0.6f;
    float gravity = -10;
    bool isGrounded;
    Vector3 velocity;

    [Header("Crouch")]
    [SerializeField] Transform ceilingCheck;
    [SerializeField] LayerMask ceilingLayer;
    bool isUnderCeiling;
    bool isCrouching;
    bool isResetingtSize;
    float elapsedTime;
    Vector3 startScale;
    Vector3 endScale;


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

        #region Crouch

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isResetingtSize = true;
            startScale = transform.localScale;
            endScale = new Vector3(1, 0.5f, 1);
           // transform.localScale = new Vector3(1, 0.5f, 1);  // make player smaller
            speed = CROUCH_SPEED;
            isCrouching = true;
            elapsedTime = 0;
        }
        // point, wehen player released crouch button or when stop being under sth where he was crouching
        if(!Input.GetKey(KeyCode.LeftControl) && !isUnderCeiling && isCrouching)
        {
            startScale = transform.localScale;
            endScale = Vector3.one;
            isResetingtSize = true;
            velocity.y = 6;
            speed = DEFAULT_SPEED;
            isCrouching = false;
        }
        // time after stop crouching, when player has time to resize
        if(isResetingtSize)
        {
            elapsedTime += 2 * Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime);

            if (transform.localScale == endScale)
                isResetingtSize = false;
        }

        #endregion
    }

    private void FixedUpdate()
    {
        isUnderCeiling = Physics.CheckSphere(ceilingCheck.position, 1, ceilingLayer);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    IEnumerator PlayFootstepSounds()
    {
        while(true)
        {
            if((xInput != 0 || yInput != 0) && isGrounded && !isCrouching)
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
