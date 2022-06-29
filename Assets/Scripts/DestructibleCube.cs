using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCube : MonoBehaviour
{
    private float startTime = 0;

    private Renderer mesh;
    // Start is called before the first frame update
    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        startTime = Time.time;
        ApplyForce2();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time - startTime > 3.0f)
        {
            mesh.material.color = Color.Lerp(mesh.material.color, Color.clear, 2.0f * Time.deltaTime);
        }

        if (mesh.material.color == Color.clear)
        {
            Destroy(gameObject);
        }
    }

    public void ApplyForce2()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 30.0f, ForceMode.Impulse);

    }
    
}
