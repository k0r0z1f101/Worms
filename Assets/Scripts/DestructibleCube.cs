using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCube : MonoBehaviour
{
    private float startTime = 0;
    // Start is called before the first frame update
    void Awake()
    {
        startTime = Time.time;
        ApplyForce();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time - startTime > 3.0f)
        {
            Destroy(gameObject);
        }
    }

    public void ApplyForce()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 10.0f, ForceMode.Impulse);

    }
    
}
