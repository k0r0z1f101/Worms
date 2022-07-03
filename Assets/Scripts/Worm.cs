using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using System;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class Worm : MonoBehaviour
{
    private string _wormName;
    private int _wormHitPoints = 100;
    private int _teamNumber;
    private float _wormJumpStrength;
    [SerializeField] private InputActionAsset wormInput;
    private InputAction _move;
    private InputAction _jump;
    private InputAction _fireWeapon;
    private InputAction _targeting;
    private float _direction;
    private float _walkSpeed;
    private Rigidbody _rb;
    private bool _isFacingLeft;
    private ForceMode _wormForce;
    private bool _isMoving;
    private bool _isJumping;
    private bool _isGrounded;
    private List<AudioClip> _listOfWormSounds = new List<AudioClip>();
    private AudioSource _wormsSounds;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        SetActions();
        _walkSpeed = 7.0f;
        _wormJumpStrength = 25.0f;
        _wormForce = ForceMode.Force;
        _wormsSounds = GetComponent<AudioSource>();
        LoadAudioClips();
    }

    private void LoadAudioClips()
    {
        AudioClip wormJump = Resources.Load<AudioClip>("jump1") as AudioClip;
        //AssetDatabase.LoadAssetAtPath("Assets/Sounds/jump1.wav", typeof(AudioClip));
        _listOfWormSounds.Add(wormJump);
    }

    private void Start()
    {
        
    }

    private void SetActions()
    {
        if (wormInput != null)
        {
            var actionMap = wormInput.FindActionMap("Worms");
            _move = actionMap.FindAction("Move");
            _move.performed += OnMove;
        }
        if (wormInput != null)
        {
            var actionMap = wormInput.FindActionMap("Worms");
            _jump = actionMap.FindAction("Jump");
            _jump.performed += OnJump;
        }
        //if (_wormInput != null)
        //{
        //    _fireWeapon = _wormInput.actions["Fire"];
        //}
        //if (_wormInput != null)
        //{
        //    _targeting = _wormInput.actions["Targeting"];
        //}
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            _isGrounded = true;
            
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            _isJumping = false;
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            _isJumping = false;
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        
        //Debug.Log(context.action);
        if (context.performed)
        {
            _isJumping = true;
        }
        else
        {
            _isJumping = false;
        }
        
       

    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<float>();
        if (!context.performed)
        {
            _isMoving = false;
        }
        else
        {
            _isMoving = true;
        }
       
    }
    

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            Move();
        }
        
        if (_isJumping && _isGrounded)
        {
            Jump();
            PlayAudioFile(0);

        }
        
    }

    private void PlayAudioFile(int index)
    {
        _wormsSounds.clip = _listOfWormSounds[index];
        if (!_wormsSounds.isPlaying)
        {
            _wormsSounds.Play();
        }
    }

    private void Jump()
    {
        Vector3 jumpingVector = new Vector3(0.0f, _wormJumpStrength, 0.0f);
        _rb.AddForce(jumpingVector,ForceMode.Force);
    }
    
    private void Move()
    {
        if (_direction < 0.0f)
        {
            _isFacingLeft = false;
            if (!_isFacingLeft)
            {
                transform.LookAt(new Vector3(transform.position.x + _direction, transform.position.y,
                    transform.position.z));
            }
        }
        else if (_direction >= 0.0f)
        {
            _isFacingLeft = true;
            if (_isFacingLeft)
            {
                transform.LookAt(new Vector3(transform.position.x + _direction, transform.position.y,
                    transform.position.z));
            }
        }
        Vector3 v2 = new Vector3(_direction * _walkSpeed, 0.0f, 0.0f);
        _rb.AddForce(v2, _wormForce);
    }

    public void SetHealth(int healthChange)
    {
        _wormHitPoints += healthChange;
    }

    public void SetName(string name)
    {
        _wormName = name;
    }
    public void SetTeam(int teamNumber)
    {
        _teamNumber = teamNumber;
    }

    public string GetName()
    {
        return _wormName;
    }
}