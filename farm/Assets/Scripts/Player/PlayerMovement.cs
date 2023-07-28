using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : PlayerManager
{
    [Header("Movement")]
    [SerializeField] float speed = 10;
    float xInput;
    float yInput;
    const float DEFAULT_SPEED = 10;
    const float RUN_SPEED = 20;
    const float CROUCH_SPEED = 5;
    public static bool IsWalking { get; private set; }
    public static bool IsRunning { get; private set; }
    public static bool canMove;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 10;
    [SerializeField] float mass = 5;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckRadius = 0.6f;
    float gravity = -10;
    public bool IsGrounded { get; private set; }
    Vector3 velocity;
    const float DEFAULT_Y_VELOCITY = -2;

    [Header("Crouch")]
    [SerializeField] Transform ceilingCheck;
    [SerializeField] LayerMask ceilingLayer;
    bool isUnderCeiling;
    public static bool IsCrouching { get; private set; }
    bool isResizing;
    float elapsedTime;
    Vector3 startScale;
    Vector3 endScale;
    Vector3 weaponStartPosition;
    Vector3 weaponEndPosition;
    Vector3 weaponDefaultPos;

    [Header("Audio")]
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;
    int actualFootstepSoundIndex;
    bool hasPlayedLandSoundPlayed;
    Coroutine landSoundCoroutine;

    private void Start()
    {
        StartCoroutine(PlayFootstepSounds());
        weaponDefaultPos = shooting.transform.localPosition;
        ChangeMovementPossibility(true);
    }

    void Update()
    {
        if(canMove)
        {
            #region Movement

            if (IsGrounded)
            {
                xInput = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
                yInput = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            }
            Vector3 move = transform.right * xInput + transform.forward * yInput;

            // Player do not speed up when key on horizontal and vertical axis are pressed at the same time
            if (xInput != 0 && yInput != 0)
                move = move.normalized * speed * Time.deltaTime;

            characterController.Move(move);

            #endregion

            #region Jump

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded && !IsCrouching && !isUnderCeiling)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * DEFAULT_Y_VELOCITY * gravity);
                playerSource.PlayOneShot(jumpSound);
                Crosshair.spread += Crosshair.JUMP_SPREAD;
            }

            // land sound plays when Player jumps or falls
            if (!IsGrounded)
            {
                hasPlayedLandSoundPlayed = false;
                landSoundCoroutine = StartCoroutine(PlayLandSound());
            }

            #endregion
        }

        #region Gravity

        // gravity from rigidbody does not work with character controller
        velocity.y += gravity * mass * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // when Player is on the ground, velocity restarts
        if (IsGrounded && velocity.y < 0)
            velocity.y = DEFAULT_Y_VELOCITY;

        #endregion

        #region Run

        if (Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded && !IsCrouching && (xInput != 0 || yInput != 0))
        {
            speed = RUN_SPEED;
            IsWalking = false;
            IsRunning = true;
            Crosshair.baseSpread = Crosshair.RUN_SPREAD;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = DEFAULT_SPEED;
            IsRunning = false;
            Crosshair.baseSpread = Crosshair.WALK_SPREAD;
        }

        #endregion

        #region Crouch

        if (Input.GetKeyDown(KeyCode.LeftControl) && IsGrounded && !isUnderCeiling)
        {
            ResizePlayer(transform.localScale, new Vector3(1, 0.5f, 1),weaponDefaultPos, new Vector3(weaponDefaultPos.x, -0.7f));
            speed = CROUCH_SPEED;
            IsCrouching = true;
            IsWalking = false;
            Crosshair.baseSpread = Crosshair.CROUCH_SPREAD;
        }
        // point, when Player released crouch button or when stop being under sth where he was crouching
        if(!Input.GetKey(KeyCode.LeftControl) && !isUnderCeiling && IsCrouching)
        {
            ResizePlayer(transform.localScale, Vector3.one, shooting.transform.localPosition, weaponDefaultPos);
            speed = DEFAULT_SPEED;
            IsCrouching = false;
            Crosshair.baseSpread = Crosshair.WALK_SPREAD;
        }
        // time after stop crouching, when Player has time to resize
        if(isResizing)
        {
            elapsedTime += 2 * Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime);
            shooting.transform.localPosition = Vector3.Lerp(weaponStartPosition, weaponEndPosition, elapsedTime);

            // Player smoothly stand up from crouching
            if (!IsCrouching)
                velocity.y = 1;
            else // Player can'timeToStartSway fly when spam crouch button
                velocity.y = DEFAULT_Y_VELOCITY;

            if (transform.localScale == endScale)
                isResizing = false;
        }

        #endregion

        // if Player is moving
        if(xInput != 0 || yInput != 0)
        {
            // IsRunning can't be true together with IsWalking
            // Player can walk OR sprint
            if (!IsRunning && !IsCrouching)
                IsWalking = true;
        }
        else
            IsWalking = false;
    }

    private void FixedUpdate()
    {
        isUnderCeiling = Physics.CheckSphere(ceilingCheck.position, 1, ceilingLayer);
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    IEnumerator PlayFootstepSounds()
    {
        while(true)
        {
            if((xInput != 0 || yInput != 0) && IsGrounded && !IsCrouching && !isPlayerInVehicle)
            {
                playerSource.PlayOneShot(footstepSounds[actualFootstepSoundIndex]);
                actualFootstepSoundIndex++;

                if (actualFootstepSoundIndex > footstepSounds.Length - 1)
                    actualFootstepSoundIndex = 0;

                yield return new WaitForSeconds(1 / speed * 5);
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
            if (IsGrounded && !hasPlayedLandSoundPlayed && !IsCrouching)
            {
                playerSource.PlayOneShot(landSound);
                hasPlayedLandSoundPlayed = true;
                StopCoroutine(landSoundCoroutine);
            }
            yield return null;
        }
    }

    /// <summary> Set all variables needed to change Player size with provide for weapon position.</summary>
    /// <param name="startPlayerScale"></param>
    /// <param name="endPlayerScale"></param>
    /// <param name="startWeaponPos"></param>
    /// <param name="endWeaponPos"></param>
    void ResizePlayer(Vector3 startPlayerScale, Vector3 endPlayerScale, Vector3 startWeaponPos, Vector3 endWeaponPos)
    {
        startScale = startPlayerScale;
        endScale = endPlayerScale;
        weaponStartPosition = startWeaponPos;
        weaponEndPosition = endWeaponPos;
        elapsedTime = 0;
        isResizing = true;
    }

    /// <summary> Make player can move or not. Also works with movement effects. </summary>
    /// <param name="value"></param>
    public static void ChangeMovementPossibility(bool value)
    {
        canMove = value;
        playerSource.enabled = value;
        // walk effects must be turned off when movement is
        FindObjectOfType<HeadBobbing>().enabled = value;
        FindObjectOfType<WeaponSway>().enabled = value;
        FindObjectOfType<MouseLook>().enabled = value;
    }
}