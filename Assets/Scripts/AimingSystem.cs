using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class AimingSystem : MonoBehaviour
{
    [SerializeField] private InputActionAsset wormInput;
    private InputAction _aim;
    private Vector2 _aimVector;
    private bool _weaponIsMoving;
    private GameObject _target;
    private GameObject _worm;
    [SerializeField]private float rotSpeed;
    private bool turnleft;

    private void Awake()
    {
        var actionMap = wormInput.FindActionMap("Worms");
        _aim = actionMap.FindAction("Targeting");
        _aim.performed += MoveTarget;
    }

    private void MoveTarget(InputAction.CallbackContext context)
    {
        _aimVector = context.ReadValue<Vector2>();
        if (context.ReadValue<Vector2>().y < 0)
        {
            turnleft = true;
        }
        else
        {
            turnleft = false;
        }
        if (context.performed)
        {
            _weaponIsMoving = true;
        }
        else
        {
            _weaponIsMoving = false;
        }

    }

    private void Update()
    {
        if (_weaponIsMoving)
        {
            if (turnleft)
            {
                _target = transform.parent.Find("Target").gameObject;
                _worm = transform.parent.gameObject;
                _target.transform.RotateAround(_worm.transform.position, Vector3.forward, rotSpeed);
                this.GameObject().transform.LookAt(_target.transform);
                _weaponIsMoving = false;
                //this.GameObject().transform.Rotate(new Vector3(_aimVector.x,_aimVector.y, 1.0f));
            }
            else
            {
                _target = transform.parent.Find("Target").gameObject;
                //print("ouin " + _target);
                _worm = transform.parent.gameObject;
                _target.transform.RotateAround(_worm.transform.position, Vector3.forward, -rotSpeed);
                this.GameObject().transform.LookAt(_target.transform);
                _weaponIsMoving = false;
            }
        }
        
    }
}
