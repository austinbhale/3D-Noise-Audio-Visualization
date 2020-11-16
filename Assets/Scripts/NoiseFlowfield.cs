//Copyright (c) 2018 Peter Olthof, Peer Play
//http://www.peerplay.nl, info AT peerplay.nl 
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowfield : MonoBehaviour
{
    FastNoise fastNoise;
    public Vector3Int gridSize;
    public float cellSize;
    public Vector3[,,] flowfieldDirection; // 2 commas for 3d grid
    public float increment;
    public Vector3 offset, offsetSpeed;
    // For the particles...
    public GameObject particlePrefab;
    public int amountOfParticles;
    [HideInInspector]
    public List<FlowfiedParticle> particles;
    public List<MeshRenderer> particleMeshRenderer;
    public float spawnRadius;
    public float particleScale, particleMoveSpeed, particleRotateSpeed;

    // Ensure that particles do not spawn inside one another.
    bool particlesSpawnValidation(Vector3 position)
    {
        bool valid = true;
        foreach (FlowfiedParticle particle in particles)
        {
            if (Vector3.Distance(position, particle.transform.position) < spawnRadius)
            {
                valid = false;
                break;
            }
        }
        return valid;
    }
    // Start is called before the first frame update
    void Awake()
    {
        flowfieldDirection = new Vector3[gridSize.x, gridSize.y, gridSize.z];
        fastNoise = new FastNoise();
        particles = new List<FlowfiedParticle>();
        particleMeshRenderer = new List<MeshRenderer>();
        for (int i = 0; i < amountOfParticles; i++)
        {
            int attempt = 0;
            while (attempt < 100)
            {
                Vector3 randomPos = new Vector3(
                    Random.Range(this.transform.position.x, this.transform.position.x + gridSize.x * cellSize),
                    Random.Range(this.transform.position.y, this.transform.position.y + gridSize.y * cellSize),
                    Random.Range(this.transform.position.z, this.transform.position.z + gridSize.z * cellSize));
                bool isValid = particlesSpawnValidation(randomPos);

                if (isValid)
                {
                    GameObject particleInstance = (GameObject)Instantiate(particlePrefab);
                    particleInstance.transform.position = randomPos;
                    particleInstance.transform.parent = this.transform;
                    particleInstance.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
                    particles.Add(particleInstance.GetComponent<FlowfiedParticle>());
                    particleMeshRenderer.Add(particleInstance.GetComponent<MeshRenderer>());
                    break;
                }
                if (!isValid)
                {
                    attempt++;
                }
            }

        }
        //Debug.Log(particles.Count);
    }

    // Update is called once per frame
    void Update()
    {
        calculateFlowfieldDirections();
        ParticleBehaviour();
    }

    void calculateFlowfieldDirections()
    {
        offset = new Vector3(
            offset.x + (offsetSpeed.x * Time.deltaTime),
            offset.y + (offsetSpeed.y * Time.deltaTime),
            offset.z + (offsetSpeed.z * Time.deltaTime)
            );

        float xOffset = 0f;
        for (int x = 0; x < gridSize.x; x++)
        {
            float yOffset = 0f;
            for (int y = 0; y < gridSize.y; y++)
            {
                float zOffset = 0f;
                for (int z = 0; z < gridSize.z; z++)
                {
                    // Noise value is between 0 and 2, so we multiply it by 0.5 in the new Color.
                    float noise = fastNoise.GetSimplex(xOffset + offset.x, yOffset + offset.y, zOffset + offset.z) + 1;
                    Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));
                    flowfieldDirection[x, y, z] = Vector3.Normalize(noiseDirection);
                    zOffset += increment;
                }
                yOffset += increment;
            }
            xOffset += increment;
        }
    }

    void ParticleBehaviour()
    {
        foreach(FlowfiedParticle p in particles)
        {
            // To have the particles move to the opposite edges of the flowfield.
            // Check x bounds.
            if (p.transform.position.x > this.transform.position.x + (gridSize.x * cellSize))
            {
                p.transform.position = new Vector3(this.transform.position.x, p.transform.position.y, p.transform.position.z);
            }
            if (p.transform.position.x < this.transform.position.x)
            {
                p.transform.position = new Vector3(this.transform.position.x + (gridSize.x * cellSize), p.transform.position.y, p.transform.position.z);
            }
            // Check y bounds.
            if (p.transform.position.y > this.transform.position.y + (gridSize.y * cellSize))
            {
                p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y, p.transform.position.z);
            }
            if (p.transform.position.y < this.transform.position.y)
            {
                p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y + (gridSize.y * cellSize), p.transform.position.z);
            }
            // Check z bounds.
            if (p.transform.position.z > this.transform.position.z + (gridSize.z * cellSize))
            {
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, this.transform.position.z);
            }
            if (p.transform.position.z < this.transform.position.z)
            {
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, this.transform.position.z + (gridSize.z * cellSize));
            }
            Vector3Int particlePos = new Vector3Int(
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.x - this.transform.position.x) / cellSize, 0, gridSize.x - 1)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.y - this.transform.position.y) / cellSize, 0, gridSize.y - 1)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.z - this.transform.position.z) / cellSize, 0, gridSize.z - 1))
                );
            p.ApplyRotation(flowfieldDirection[particlePos.x, particlePos.y, particlePos.z], particleRotateSpeed);
            p.moveSpeed = particleMoveSpeed;
            //p.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(this.transform.position + new Vector3(((gridSize.x * cellSize) * 0.5f), ((gridSize.y * cellSize) * 0.5f), (gridSize.z * cellSize) * 0.5f),
            new Vector3(gridSize.x * cellSize, gridSize.y * cellSize, gridSize.z * cellSize));
    }
}