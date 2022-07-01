using UnityEngine;

public class DestructibleCube : MonoBehaviour
{
    private float startTime = 0;
    private bool stopApplying = false;
    private float awakeTime;

    private Renderer mesh;
    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        awakeTime = Time.time;
    }

    void Update()
    {
        if (Time.time - awakeTime > 0.0f && !stopApplying)
        {
            startTime = Time.time;
            ApplyForce();
            stopApplying = true;
        }
    }
    
    void FixedUpdate()
    {
        if (stopApplying)
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
    }

    public void ApplyForce()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 30.0f, ForceMode.Impulse);

    }
    
}
