using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;
using System;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;
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
    private bool _isCurrentWorm = false;
    private float _timeToDeactivate = 99999999.0f;
    private float _startTime;
    private bool _loaded = false;
    
    //not yet integrated
    public Inventory _thisWormsStuff = new Inventory();
    private InputAction _swapWeapon;
    private Vector3 WeaponPosition;
    private GameObject _bazooka;
    private GameObject _holyHandGrenade;
    private GameObject _ninjaRope;
    private GameObject _projectile;
    private InputAction _moveTarget;
    private bool _isTargeting;
    private GameObject _target;
    private Vector2 _targetDirection;
    private bool _rotateLeft;
    private InputAction _fire;
    private Vector3 _missileFlightVector;
    private GameObject _missile;
    private GameObject _missilePrefab;
    private bool _missileIsFlying;
    private float _missileForce;
    private float _grenadeForce;
    private float _chargeTime;
    private GameObject _grenadePrefab;
    private GameObject _grenade;
    private bool _grenadeFlying;
    private GameObject _chargeCapsule;
    //
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        SetActions();
        _walkSpeed = 7.0f;
        _wormJumpStrength = 25.0f;
        _wormForce = ForceMode.Force;
        _wormsSounds = GetComponent<AudioSource>();
        LoadAudioClips();
        
        //to integrate
        _thisWormsStuff.SetAmmo(10,3);
        _thisWormsStuff.SelectInitialWeapon();
        _target= transform.Find("Target").gameObject;
        _missileForce = 100.0f;
        _grenadeForce = 75.0f;
        _chargeCapsule = transform.Find("Charge%").gameObject;
        //
    }

    private void LoadAudioClips()
    {
        AudioClip wormJump = Resources.Load<AudioClip>("jump1");
        _listOfWormSounds.Add(wormJump);
        AudioClip wormWalk = Resources.Load<AudioClip>("SqueakyWalk");
        _listOfWormSounds.Add(wormWalk);
        AudioClip wormBounce = Resources.Load<AudioClip>("SqueakyBounce");
        _listOfWormSounds.Add(wormBounce);
        AudioClip explosion =  Resources.Load<AudioClip>("Explosion");
        _listOfWormSounds.Add(explosion);
    }

    private void Start()
    {
        _startTime = Time.time;
        _bazooka = transform.Find("Bazooka(Clone)").gameObject;
        _holyHandGrenade = transform.Find("Grenade(Clone)").gameObject;
        _ninjaRope= transform.Find("Bow(Clone)").gameObject;
        _projectile = transform.Find("Bow(Clone)/Arrow").gameObject;
        _projectile.GetComponent<MeshRenderer>().enabled = false;
        _missilePrefab = Resources.Load<GameObject>("missile");
        _grenadePrefab = Resources.Load<GameObject>("Grenade1");
    }

    private void SetActions()
    {
        if (wormInput != null)
        {
            var actionMap = wormInput.FindActionMap("Worms");
            _move = actionMap.FindAction("Move");
            _move.performed += OnMove;
            _jump = actionMap.FindAction("Jump");
            _jump.performed += OnJump;
            _swapWeapon = actionMap.FindAction("SwapWeapons");
            _swapWeapon.performed += SwapWeapon;
            _moveTarget = actionMap.FindAction("Targeting");
            _moveTarget.performed += RotateTarget;
            _fire = actionMap.FindAction("Fire");
            _fire.performed += FireBazooka =>
            {
                if (_thisWormsStuff.ReturnSelectedWeapon() == 0)
                {
                    if (FireBazooka.interaction is SlowTapInteraction)
                    {
                        _chargeTime -= Time.time;
                        //_chargeCapsule.transform.localScale.Set(_chargeCapsule.transform.localScale.x,_chargeCapsule.transform.localScale.y*_chargeTime,_chargeCapsule.transform.localScale.z);
                        _missileForce *= Mathf.Abs(_chargeTime);
                        if (Mathf.Abs(_chargeTime) >= 0.2f)
                        {
                            _missileIsFlying = true;
                            _chargeTime = 0.0f;
                            return;
                        }
                        else
                        {
                            //Debug.Log("weak");
                            _chargeTime = 0.0f;
                        }
                    }
                    else
                    {
                        //Debug.Log("normal fire");
                    }

                }
                else if (_thisWormsStuff.ReturnSelectedWeapon() == 1)
                {
                    if (FireBazooka.interaction is SlowTapInteraction)
                    {
                        //grenade charge
                        _chargeTime -= Time.time;
                        _grenadeForce *= Mathf.Abs(_chargeTime);
                        if (Mathf.Abs(_chargeTime) >= 0.2f)
                        {
                            _grenadeFlying = true;
                            _chargeTime = 0.0f;
                            return;
                        }
                        else
                        {
                            //Debug.Log("weak");
                            _chargeTime = 0.0f;
                        }
                    }
                    else
                    {
                        //Debug.Log("normal fire");
                    }
                }
            };

            _fire.started += FireBazooka =>
            {

                if (FireBazooka.started)
                {
                    _chargeTime = Time.time;
                }
            };


            _fire.canceled += FireBazooka =>
            {
                if (FireBazooka.interaction is SlowTapInteraction)
                {
                    //Debug.Log("Canceled");
                }

            };
        }
    }

    private void FireBazooka(InputAction.CallbackContext context)
    {
        
        
        if (context.started)
        {
            if (context.canceled)
            {
            }
        }
        else
        {
            _missileIsFlying = false;
        }
    }

    private void TossedGrenade()
    {
        _missileFlightVector = _target.transform.position - this.gameObject.transform.position;
        _missileFlightVector /= _missileFlightVector.magnitude;
        _grenade = Instantiate(_grenadePrefab);
        _grenade.transform.position = WeaponPosition;
        _grenade.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        _grenade.GetComponent<Rigidbody>().AddForce(_missileFlightVector*_grenadeForce,ForceMode.Force);
        _grenadeForce = 100.0f;
    }
    private void MissileInFlight()
    {
        _missileFlightVector = _target.transform.position - this.gameObject.transform.position;
        _missileFlightVector /= _missileFlightVector.magnitude;
        _missile = Instantiate(_missilePrefab);
        _missile.transform.position = _rb.transform.position;
        _missile.transform.LookAt(_target.transform);
        _missile.GetComponent<Rigidbody>().AddForce(_missileFlightVector*_missileForce,ForceMode.Force);
        _missileForce = 100.0f;

    }

    private void RotateTarget(InputAction.CallbackContext context)
    {
        _targetDirection = context.ReadValue<Vector2>();
        if (_targetDirection.y < 0)
        {
            _rotateLeft = true;
        }
        else
        {
            _rotateLeft = false;
        }
    }

    private void SwapWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _thisWormsStuff.WeaponSwap();
            
        }
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

        if (_isCurrentWorm)
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


    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (_isCurrentWorm)
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

    }
    
    private void Update()
    {
        //ChargeUi();
        //_chargeCapsule.transform.localScale += new Vector3(_chargeCapsule.transform.localScale.x,
        //    _chargeCapsule.transform.localScale.y + _chargeTime, _chargeCapsule.transform.localScale.z);
        WeaponPosition=transform.Find("WeaponPos").transform.position;
        if (_thisWormsStuff.ReturnSelectedWeapon() == 0)
        {
            _ninjaRope.GetComponent<MeshRenderer>().enabled=false;
            _projectile.GetComponent<MeshRenderer>().enabled=false;
            _bazooka.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            _bazooka.transform.position = WeaponPosition;
        }
        else if (_thisWormsStuff.ReturnSelectedWeapon() == 1)
        {
            _bazooka.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            _holyHandGrenade.GetComponent<MeshRenderer>().enabled=true;
            _holyHandGrenade.transform.position = WeaponPosition;
        }
        else if(_thisWormsStuff.ReturnSelectedWeapon()==2)
        {
            _holyHandGrenade.GetComponent<MeshRenderer>().enabled=false;
            _ninjaRope.GetComponent<MeshRenderer>().enabled=true;
            _projectile.GetComponent<MeshRenderer>().enabled=true;
            _ninjaRope.transform.position = WeaponPosition;
        }
    }

    private void FixedUpdate()
    {
        if (_isMoving && _isCurrentWorm)
        {
            Move();
            if(_direction!=0){PlayAudioFile(1);}
        }
        
        if (_isJumping && _isGrounded && _isCurrentWorm)
        {
            Jump();
            PlayAudioFile(0);

        }

        if (_isTargeting && _isCurrentWorm)
        {
            MoveTarget();
        }

        if (_missileIsFlying && _isCurrentWorm)
        {
            MissileInFlight();
            _missileIsFlying = false;
        }

        if (_grenadeFlying && _isCurrentWorm)
        {
            TossedGrenade();
            _grenadeFlying = false;
        }

        if ((transform.position.y < 0.0f || _wormHitPoints <= 0) && _timeToDeactivate > 9999999.0f)
        {
            PlayDeath();
        }

        if (_timeToDeactivate < Time.time)
        {
            gameObject.SetActive(false);
        }
    }
    
    private void MoveTarget()
    {
        if (_rotateLeft)
        {
            _target.transform.RotateAround(this.gameObject.transform.position,Vector3.forward, 1.0f);
        }
        else
        {
            _target.transform.RotateAround(this.gameObject.transform.position,Vector3.forward, -1.0f);
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
                transform.LookAt(new Vector3(transform.position.x + -(_direction), transform.position.y,
                    transform.position.z));
            }
        }
        else if (_direction >= 0.0f)
        {
            _isFacingLeft = true;
            if (_isFacingLeft)
            {
                transform.LookAt(new Vector3(transform.position.x + -(_direction), transform.position.y,
                    transform.position.z));
            }
        }
        Vector3 v2 = new Vector3(-(_direction) * _walkSpeed, 0.0f, 0.0f);
        _rb.AddForce(v2, _wormForce);
    }
    
    GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

    public int GetHealth()
    {
        return _wormHitPoints;
    }

    public void SetHealth(int healthChange)
    {
        _wormHitPoints -= healthChange;
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

    public void SetCurrent(bool isCurrent)
    {
        _isCurrentWorm = isCurrent;
    }

    public int GetTeam()
    {
        return _teamNumber;
    }

    void PlayDeath()
    {
        AudioClip death = Resources.Load<AudioClip>("DEATH");
        _wormsSounds.PlayOneShot(death);
        _timeToDeactivate = Time.time + 1.0f;
    }
}