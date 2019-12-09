using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGeneratorStartWithScale : MonoBehaviour{

    Mesh mesh;

    Vector3[] vertecies;
    int[] triangles;
    Vector2[] uvs;

    [SerializeField]

    private int xSize = 32;
    [SerializeField]
    private int zSize = 32;
    [SerializeField]
    private Vector2 terrainOffset;

    [SerializeField]
    private int Scale = 1;
    

    void Start()
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;
        if (GetComponent<MeshCollider>() != null)
        {
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        else { Debug.LogWarning("There is no collider on this object!"); }

        CreateShape();
        UpdateMesh();

        if (GetComponent<MeshCollider>() != null)
        {
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }


    void CreateShape()
    {
        vertecies = new Vector3[(xSize + 1) * (zSize + 1)];
        uvs = new Vector2[vertecies.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = TerrainNoise(((float)x / Scale + (float)transform.position.x), ((float)z / Scale + (float)transform.position.z));
                vertecies[i] = new Vector3(x, y, z);

                uvs[i] = new Vector2((float)x / (float)xSize, (float)z / (float)zSize);
                i++;
            }
        }
            
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertecies;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
    }

    float TerrainNoise(float x, float z)
    {
        float y = Mathf.PerlinNoise(x * .7f, z * .7f) * .5f;
        y += Mathf.PerlinNoise(x * .08f, z * .08f) * 2.5f * 1.5f;
        return y * Scale;
    }
}