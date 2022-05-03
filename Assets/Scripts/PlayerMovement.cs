using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _runSpeed = 1f;

    [SerializeField] private float _climbSpeed = 1f;
    private float _initialAnimatorSpeed;
    private float _initialGravity;

    [SerializeField] private float _jumpForce = 1f;
    [SerializeField] private int _numOfExtraJumps = 1;
    private int _extraJumps = 0;
    [SerializeField] private float _coyoteTime = 0.2f;
    private float _coyoteTimeCounter;




    private Vector2 _moveInput;
    private Rigidbody2D _myRigidBody2D;
    private Animator _myAnimator;
    private BoxCollider2D _myBoxCollider;
    

    private void Start()
    {
        _myRigidBody2D = GetComponent<Rigidbody2D>();
        _myAnimator = GetComponent<Animator>();
        _myBoxCollider = GetComponent<BoxCollider2D>();

        _coyoteTimeCounter = _coyoteTime;
        _initialAnimatorSpeed = _myAnimator.speed;
        _initialGravity = _myRigidBody2D.gravityScale;

    }
    private void FixedUpdate()
    {
        _Run();
        _Climb();
    }
    private void Update() 
    {
        if (IsClimbing())
        {
             _myAnimator.SetBool("isJumping", false);
        }

        if (IsGrounded() && !IsClimbing()) 
        {
            _coyoteTimeCounter = _coyoteTime; 
            _extraJumps = _numOfExtraJumps;
            _myAnimator.SetBool("isJumping", false);
        }
        else if (!IsGrounded() && !IsClimbing())
        {
            _coyoteTimeCounter -= Time.deltaTime;
            _myAnimator.SetBool("isJumping", true);
        }
    }

    private void OnMove(InputValue value) => _moveInput = value.Get<Vector2>();

    private void OnJump(InputValue value)
    {
        if (!value.isPressed) 
        {   
            return;
        }        

        if (!IsClimbing())
        {
            if (_coyoteTimeCounter > 0f) 
            {
                _myRigidBody2D.velocity = Vector2.up * _jumpForce;
                
            } 
            else if (_extraJumps > 0)
            {
                _myRigidBody2D.velocity = new Vector2(0f,_jumpForce);
                _extraJumps --;
            }
            else if (_extraJumps == 0 && IsGrounded())
            {
                _myRigidBody2D.velocity = Vector2.up * _jumpForce;
            }
            
        }

    }

    private void _Run()
    {
        _FlipSprite();
        Vector2 newSpeed = new Vector2 (_moveInput.x * _runSpeed, _myRigidBody2D.velocity.y);
        _myRigidBody2D.velocity = newSpeed;
        if (_moveInput.x != 0)
        {
            _myAnimator.SetBool("isRunning", true);
        }
        else
        {
            _myAnimator.SetBool("isRunning", false);
        }
    }

    private void _FlipSprite()
    {
        if (_moveInput.x != 0)
        {
            transform.localScale = new Vector2 (Mathf.Sign(_moveInput.x),1f);
        }
    }
    private void _Climb()
    {
        if (!IsClimbing())
        {
            _myAnimator.SetBool("isClimbing", false);
            _myAnimator.speed = _initialAnimatorSpeed;
            _myRigidBody2D.gravityScale = _initialGravity;
            return;
        }

        Vector2 newClimbSpeed = new Vector2(_myRigidBody2D.velocity.x, _moveInput.y*_climbSpeed);
        _myRigidBody2D.velocity = newClimbSpeed;
        _myAnimator.SetBool("isClimbing", true);
        if (_moveInput.y == 0)
        {
            _myAnimator.speed = 0;
            _myRigidBody2D.gravityScale = 0;
        }
        else
        {
            _myAnimator.speed = _initialAnimatorSpeed;

        }
    }

    private bool IsGrounded()
    {
        return _myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    private bool IsClimbing()
    {
        return _myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
    }

    
}
