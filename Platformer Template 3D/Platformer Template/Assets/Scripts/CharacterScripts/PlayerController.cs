using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Actor
{
    // Serialized simply means that the field will be shown in the inspector. Fields that are public will automatically be shown, without the need for the [SerializeField] attribute.
    // #region allows you to collapse a section of code by clicking the arrow to the left of it.
    #region Serailized Fields
    [Space]
    [SerializeField] private PlayerMovementSettings _movementSettings = new();
    [Space]
    [SerializeField] private PlayerLookSettings _lookSettings = new();
    [Space]
    [SerializeField] private PlayerGroundDetectionSettings _groundDetectionSettings = new();
    [Space]
    [SerializeField] private PlayerControlSettings _controlSettings = new();

    #region Debug Settings
    [Header("Debug Settings")]
    [SerializeField]
    private bool _showGroundRaycast = true;

    [SerializeField]
    private bool _showVelocityVector = true;

    [SerializeField]
    private bool _showFallingGravityVector = true;

    [SerializeField]
    private bool _showFacingDirectionVector = true;

    [SerializeField]
    private bool _showMoveDirVector = true;
    #endregion
    #endregion

    private float _landingJumpInputTimer = 0;
    public float LandingJumpInputTimer
    {
        get => _landingJumpInputTimer; set
        {
            if (value < 0)
            { // if the value trying to be set to LandingJumpInputTimer is below 0 it will be replaced with 0. In other words, _landingJumpInputTimer can never be less than 0.
                _landingJumpInputTimer = 0;
            }
            else
            {
                _landingJumpInputTimer = value;
            }
        }
    }

    [ReadOnly, SerializeField] private Vector3 _originalMoveDir = Vector3.zero;

    public override void Awake()
    {
        base.Awake(); // this calls the Awake method in the base class, "Actor"

        if (_controlSettings.fallingGravityForce != null)
        {
            _controlSettings.fallingGravityForce.enabled = false;
        }
    }

    // Physics-related code, such as adding force, should be executed in FixedUpdate for consistent results.
    public override void FixedUpdate()
    {
        Vector3 trueMoveDir = transform.forward * _originalMoveDir.z + transform.right * _originalMoveDir.x;
        MoveDir = trueMoveDir;

        base.FixedUpdate();

        #region IsGrounded
        // This line of code can be explained as follows
        _groundDetectionSettings.isGrounded = Physics.Raycast( // A Raycast is a line that detects if something traveled through it.
            transform.position + _groundDetectionSettings.groundCastPosition,// this is the position of the raycast
                                                              // (the transform.position makes it
                                                              // relative to the player)
            _groundDetectionSettings.groundCastLength.normalized, // This shows direction of where to cast the ray
            out _, // This just shows that we do not care about the output of this Raycast except for the bool
            _groundDetectionSettings.groundCastLength.magnitude, // this shows for how long to cast the ray
            _groundDetectionSettings.groundLayer); // this shows what layer(s) will be counted as ground.
        #endregion

        #region Horizontal Velocity Clamping
        Vector2 horizontalVelocity = new Vector2(_body.velocity.x, _body.velocity.z);
        horizontalVelocity = Vector2.ClampMagnitude(horizontalVelocity, _movementSettings.maxSpeed);
        _body.velocity = new Vector3(horizontalVelocity.x, _body.velocity.y, horizontalVelocity.y);
        #endregion

        if (_controlSettings.fallingGravityForce != null)
        {
            // if the body's vertical velocity is less than 0 (the object is falling), enable the falling gravity component.
            _controlSettings.fallingGravityForce.enabled = _body.velocity.y < 0;
        }

        if (LandingJumpInputTimer > 0 && _groundDetectionSettings.isGrounded)
        { // This is to ensure that inputs just before a jump are not wasted
            Jump();
        }

        // The timer goes down the same time that this method is called.
        // This will count down in seconds.
        LandingJumpInputTimer -= Time.fixedDeltaTime;
    }

    // Non-Physics-related code, such as timers or checks should be executed in Update as FixedUpdate is reserved for physics
    public void Update()
    {
        if (_lookSettings.cameraRotateWithMouse)
        {
            Look(_lookSettings.lookDelta);
            if (_lookSettings.playerRotateWithCamera)
            {
                _lookSettings.lookTarget.localEulerAngles = new Vector3(
                    _lookSettings.lookTarget.localEulerAngles.x,
                    Mathf.LerpAngle(
                        _lookSettings.lookTarget.localEulerAngles.y,
                        0, _lookSettings.cameraLerp),
                    Mathf.LerpAngle(
                        _lookSettings.lookTarget.localEulerAngles.z,
                        0, _lookSettings.cameraLerp));
            }
        }
    }

    private void Jump()
    {
        LandingJumpInputTimer = 0;
        _body.velocity = new Vector3(_body.velocity.x, 0, _body.velocity.z);
        _body.AddForce(new Vector3(0, _movementSettings.jumpForce, 0), ForceMode.Impulse);
    }

    #region Movement
    public void MoveControl(InputAction.CallbackContext context)
    {
        Vector2 inputDirection = context.ReadValue<Vector2>();
        _originalMoveDir = new Vector3(inputDirection.x, 0, inputDirection.y);
    }

    public void JumpControl(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LandingJumpInputTimer = _controlSettings.landingJumpInputTime;
        }

        if (context.canceled)
        {
            LandingJumpInputTimer = 0;

            if (_controlSettings.holdForHigherJumps)
            {
                // Set player's vertical velocity to 0 when the button is released.
                if (_body.velocity.y > 0)
                {
                    _body.velocity = new Vector3(_body.velocity.x, 0, _body.velocity.z);
                }
            }
        }
    }
    #endregion

    #region Looking
    public void LookControl(InputAction.CallbackContext context)
    {
        _lookSettings.lookDelta = context.ReadValue<Vector2>();
    }
    public void Look(Vector2 delta)
    {
        if (_lookSettings.lookTarget == null)
        {
            Debug.LogError("You must set a Look target for camera movement to function.");
            return;
        }

        if (_lookSettings.playerRotateWithCamera)
        {
            transform.rotation *= Quaternion.AngleAxis(delta.x * _lookSettings.rotationPower, Vector3.up);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        else
        {
            _lookSettings.lookTarget.rotation *= Quaternion.AngleAxis(delta.x * _lookSettings.rotationPower, Vector3.up);
        }

        if (_lookSettings.planeYControls)
        {
            _lookSettings.lookTarget.rotation *= Quaternion.AngleAxis(-delta.y * _lookSettings.rotationPower, Vector3.right);
        }
        else
        {
            _lookSettings.lookTarget.rotation *= Quaternion.AngleAxis(delta.y * _lookSettings.rotationPower, Vector3.right);
        }

        //clamp the up/down axis
        Vector3 angles = _lookSettings.lookTarget.eulerAngles;
        angles.z = 0;

        if (angles.x > 180 && angles.x < 360 + _lookSettings.maxLookUpAngle)
        {
            angles.x = 360 + _lookSettings.maxLookUpAngle;
        }
        else if (angles.x < 180 && angles.x > _lookSettings.maxLookDownAngle)
        {
            angles.x = _lookSettings.maxLookDownAngle;
        }
        _lookSettings.lookTarget.eulerAngles = angles;
    }
    #endregion
    private void OnDrawGizmos()
    {
        if (_showGroundRaycast)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + _groundDetectionSettings.groundCastPosition, _groundDetectionSettings.groundCastLength); 
        }

        if (_showVelocityVector && _body != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_body.position + _body.centerOfMass, _body.velocity);
        }

        if (_showFallingGravityVector && _controlSettings.fallingGravityForce != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, _controlSettings.fallingGravityForce.force + _controlSettings.fallingGravityForce.relativeForce);
        }

        if (_showFacingDirectionVector)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * 2);
        }

        if (_showMoveDirVector)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawRay(transform.position, MoveDir * 3);
        }
    }
}
