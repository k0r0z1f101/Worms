using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class onCollision : MonoBehaviour
{
    private Collider _missileBody;
    private AudioSource _explosionSource;

    private Rigidbody _rb;
    // Start is called before the first frame update
    private void Awake()
    {
        _missileBody = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        _explosionSource = GetComponent<AudioSource>();
        _explosionSource.clip = Resources.Load<AudioClip>("Explosion");
        
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(_rb.velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            var test=ExplosionForce(collision.relativeVelocity);
            ContactPoint contactWithGround = collision.contacts[0];
            //Debug.Log(collision.contacts[0].point);
            GetComponent<AudioSource>().clip = _explosionSource.clip;
            GetComponent<AudioSource>().Play();
            Destroy(this.gameObject);
        }
    }

    public Vector3 ExplosionForce(Vector3 relativeVelocity)
    {
        
        float m = _rb.mass;
        Vector3 a = relativeVelocity / Time.deltaTime;
        Vector3 force = m * a;
        return force;

    }

    public float ExplosionRadius(float force)
    {
        float baseRadius = 0.5f;
        return baseRadius * force;
    }
}
