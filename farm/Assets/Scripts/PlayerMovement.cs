using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 10;
    [SerializeField] float jumpHeight = 10;
    [SerializeField] float mass = 5;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    float gravity = -10;
    bool isGrounded;

    CharacterController characterController;
    Vector3 velocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        #region Movement

        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float y = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        Vector3 move = transform.right * x + transform.forward * y;

        // player do not speed up when key on horizontal and vertical axis are both pressed
        if(Mathf.Abs(x) > 0 && Mathf.Abs(y) > 0)
          move = move.normalized * speed * Time.deltaTime;

        characterController.Move(move);

        #endregion

        #region Jump

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

        #endregion

        #region Gravity

        // gravity from rigidbody does not work with character controller
        velocity.y += gravity * mass * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, groundLayer);

        // when player is on the ground, velocity restarts
        if (isGrounded && velocity.y < 0)
            velocity.y = -2;

        #endregion
    }
}
