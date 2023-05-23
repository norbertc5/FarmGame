using System.Collections;
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
    const float RUN_SPEED = 20;
    const float CROUCH_SPEED = 5;
    public static bool isWalking;
    public static bool isRunning;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 10;
    [SerializeField] float mass = 5;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckRadius = 0.6f;
    float gravity = -10;
    bool isGrounded;
    Vector3 velocity;
    const float DEFAULT_Y_VELOCITY = -2;

    [Header("Crouch")]
    [SerializeField] Transform ceilingCheck;
    [SerializeField] LayerMask ceilingLayer;
    bool isUnderCeiling;
    [HideInInspector] public static bool isCrouching;
    bool isResizing;
    float elapsedTime;
    Vector3 startScale;
    Vector3 endScale;
    Vector3 weaponStartPosition;
    Vector3 weaponEndPosition;
    Shooting shooting;


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
        shooting = FindObjectOfType<Shooting>();
    }

    private void Start()
    {
        StartCoroutine(PlayFootstepSounds());
    }

    void Update()
    {
        #region Movement

        if (isGrounded)
        {
            xInput = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            yInput = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        }
        Vector3 move = transform.right * xInput + transform.forward * yInput;

        // player do not speed up when key on horizontal and vertical axis are pressed at the same time
        if (xInput != 0 && yInput != 0)
          move = move.normalized * speed * Time.deltaTime;

            characterController.Move(move);

        #endregion

        #region Jump

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * DEFAULT_Y_VELOCITY * gravity);
            playerSource.PlayOneShot(jumpSound);
            Crosshair.spread += Crosshair.JUMP_SPREAD;
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
            velocity.y = DEFAULT_Y_VELOCITY;

        #endregion

        #region Run

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isCrouching && (xInput != 0 || yInput != 0))
        {
            speed = RUN_SPEED;
            isWalking = false;
            isRunning = true;
            Crosshair.baseSpread = Crosshair.RUN_SPREAD;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = DEFAULT_SPEED;
            isRunning = false;
            Crosshair.baseSpread = Crosshair.WALK_SPREAD;
        }

        #endregion

        #region Crouch

        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded)
        {
            ResizePlayer(transform.localScale, new Vector3(1, 0.5f, 1), shooting.transform.localPosition, new Vector3(0, -0.7f, 0));
            speed = CROUCH_SPEED;
            isCrouching = true;
            isWalking = false;
            Crosshair.baseSpread = Crosshair.CROUCH_SPREAD;
        }
        // point, wehen player released crouch button or when stop being under sth where he was crouching
        if(!Input.GetKey(KeyCode.LeftControl) && !isUnderCeiling && isCrouching)
        {
            ResizePlayer(transform.localScale, Vector3.one, shooting.transform.localPosition, Vector3.zero);
            speed = DEFAULT_SPEED;
            isCrouching = false;
            Crosshair.baseSpread = Crosshair.WALK_SPREAD;
        }
        // time after stop crouching, when player has time to resize
        if(isResizing)
        {
            elapsedTime += 2 * Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime);
            shooting.transform.localPosition = Vector3.Lerp(weaponStartPosition, weaponEndPosition, elapsedTime);

            // player smoothly stand up from crouching
            if (!isCrouching)
                velocity.y = 1;
            else // player can't fly when spam crouch button
                velocity.y = DEFAULT_Y_VELOCITY;

            if (transform.localScale == endScale)
                isResizing = false;
        }

        #endregion

        // if player is moving
        if(xInput != 0 || yInput != 0)
        {
            // isRunning can't be true together with isWalking
            // player can walk OR sprint
            if (!isRunning && !isCrouching)
                isWalking = true;
        }
        else
        {
            isWalking = false;
        }
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
            if (isGrounded && !hasPlayedLandSoundPlayed && !isCrouching)
            {
                playerSource.PlayOneShot(landSound);
                hasPlayedLandSoundPlayed = true;
                StopCoroutine(landSoundCoroutine);
            }
            yield return null;
        }
    }

    /// <summary>
    /// Set all variables needed to change player size with provide for weapon position.
    /// </summary>
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
}
