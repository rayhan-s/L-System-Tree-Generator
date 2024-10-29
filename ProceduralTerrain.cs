using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrain : MonoBehaviour
{
    public int width = 256;
    public int depth = 256;
    public float scale = 20f;
    public float height = 20f;
    public float lacunarity = 2.0f;
    public float persistence = 0.5f;
    public int octaves = 4;
    public float falloffStrength = 3.0f; 

    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }

        GenerateTerrain();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            height += 1f;
            GenerateTerrain();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            height -= 1f;
            GenerateTerrain();
        }
    }

    void GenerateTerrain()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(width + 1) * (depth + 1)];
        
        for (int i = 0, z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = CalculateHeight(x, z);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        mesh.vertices = vertices;

        int[] triangles = new int[width * depth * 6];
        for (int z = 0, vert = 0, tris = 0; z < depth; z++, vert++)
        {
            for (int x = 0; x < width; x++, vert++, tris += 6)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;
            }
        }
        
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

 float CalculateHeight(int x, int z)
{
    float y = 0f;
    float amplitude = 1f;
    float frequency = 1f;
    float centerX = width / 2f;
    float centerZ = depth / 2f;

    float distanceToCenter = Vector2.Distance(new Vector2(x, z), new Vector2(centerX, centerZ));

    float falloff = Mathf.Max(0, 1 - Mathf.Pow(distanceToCenter / Mathf.Max(width, depth), falloffStrength));

    for (int i = 0; i < octaves; i++)
    {
        float sampleX = x * scale * 0.1f * frequency;
        float sampleZ = z * scale * 0.1f * frequency;

        float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;

        float ridgedValue = 1 - Mathf.Abs(perlinValue);

        y += (perlinValue + ridgedValue) * amplitude;

        amplitude *= persistence;
        frequency *= lacunarity;
    }

    return y * height * falloff;
}
}
