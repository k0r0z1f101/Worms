using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Missile : MonoBehaviour
{
   private Rigidbody _rb;
   [SerializeField]private float _missileSpeed;

   private void OnEnable()
   {
      _rb.AddForce(Vector3.up*_missileSpeed,ForceMode.Force);
   }
}
