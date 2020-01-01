using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowfiedParticle : MonoBehaviour
{
    public float moveSpeed;
    public int audioBand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveParticles = transform.forward * moveSpeed * Time.deltaTime;
        if (!float.IsNaN(moveParticles.x) && !float.IsNaN(moveParticles.y) && !float.IsNaN(moveParticles.z)) { 
            this.transform.position += moveParticles;
        }
    }

    public void ApplyRotation(Vector3 rotation, float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(rotation.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
