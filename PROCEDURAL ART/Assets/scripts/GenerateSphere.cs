using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSphere : MonoBehaviour
{
    [Range(2, 64)]
    public int resolution = 10;

    [SerializeField, HideInInspector]
    MeshFilter meshFilter;

    Mesh mesh;

    private void OnValidate()
    {
        Initialise();
    }

    private void Initialise()
    {

        mesh = new Mesh();

        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        GenerateMesh();
        meshFilter.mesh = mesh;

    }

    void GenerateMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution * 6];
        Vector3[] normals = new Vector3[resolution * resolution * 6];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6 * 6];
        int triIndex = 0;


        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        for (int d = 0; d < 6; d++)
        {
            Vector3 direction = directions[d];
            Vector3 axisA = new Vector3(direction.y, direction.z, direction.x);
            Vector3 axisB = Vector3.Cross(direction, axisA);

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int i = x + (y * resolution) + (d * resolution * resolution);

                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vector3 pointOnUnitCube = direction + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                    vertices[i] = pointOnUnitCube.normalized;
                    normals[i] = vertices[i];

                    if (x != resolution - 1 && y != resolution - 1)
                    {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + resolution + 1;
                        triangles[triIndex + 2] = i + resolution;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + 1;
                        triangles[triIndex + 5] = i + resolution + 1;

                        triIndex += 6;
                    }
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
    }
}