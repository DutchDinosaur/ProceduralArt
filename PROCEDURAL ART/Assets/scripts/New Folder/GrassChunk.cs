using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class GrassChunk : MonoBehaviour
{

    Mesh mesh;

    Vector3[] vertecies;
    int[] triangles;
    Vector2[] uvs;
    Vector2[] uvs2;

    [SerializeField]
    private float GrassSize;
    [SerializeField]
    private int grassChunkSize;
    [SerializeField]
    private float distanceBetweenGrass;

    private MeshRenderer renderer;

    //[SerializeField]
    //private GameObject grassPrefab;
    //[SerializeField]
    //private int GenSpeed = 1;

    void Start()
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;
        renderer = GetComponent<MeshRenderer>();

        GenerateGrassMeshTris();
        UpdateMesh();

        //renderer.material.SetTexture(,);
        
        
        //StartCoroutine(placeStuff());

        

        //StartCoroutine(MowAllGrass());

        //for (int x = 0; x < grassChunkSize; x++)
        //{
        //    for (int z = 0; z < grassChunkSize; z++)
        //    {
        //        float xpos = transform.position.x + (x * distanceBetweenGrass);
        //        float zpos = transform.position.z + (z * distanceBetweenGrass);
        //        PlaceGrass(new Vector3(xpos, TerrainNoise(xpos, zpos), zpos));
        //    }
        //}
    }

    //IEnumerator placeStuff()
    //{

    //    isMowed = new bool[grassChunkSize * grassChunkSize];

    //    for (int x = 0; x < grassChunkSize; x++)
    //    {
    //        for (int z = 0; z < grassChunkSize; z++)
    //        {
    //            float xpos = transform.position.x + (x * distanceBetweenGrass);
    //            float zpos = transform.position.z + (z * distanceBetweenGrass);
    //            PlaceGrass(new Vector3(xpos, TerrainNoise(xpos, zpos), zpos));

    //            yield return new WaitForSeconds(Time.deltaTime / GenSpeed);
    //        }
    //    }

    //    yield return 0;
    //}

    //void GenerateGrassMeshQuads()
    //{
    //    vertecies = new Vector3[grassChunkSize * grassChunkSize * 4];

    //    for (int i = 0, x = 0; x < grassChunkSize; x++)
    //    {
    //        for (int z = 0; z < grassChunkSize; z++)
    //        {
    //            Vector3 pos = new Vector3(x * distanceBetweenGrass, TerrainNoise(x * distanceBetweenGrass + transform.position.x, z * distanceBetweenGrass + transform.position.z), z * distanceBetweenGrass);

    //            vertecies[i + 0] = new Vector3(pos.x - GrassSize, pos.y, pos.z);
    //            vertecies[i + 1] = new Vector3(pos.x + GrassSize, pos.y, pos.z);
    //            vertecies[i + 2] = new Vector3(pos.x + GrassSize, pos.y + GrassSize * 2, pos.z + GrassSize / 2);
    //            vertecies[i + 3] = new Vector3(pos.x - GrassSize, pos.y + GrassSize * 2, pos.z + GrassSize / 2);
    //            i += 4;
    //        }
    //    }

    //    triangles = new int[grassChunkSize * grassChunkSize * 6];

    //    int vert = 0;
    //    int tris = 0;

    //    for (int z = 0; z < grassChunkSize; z++)
    //    {
    //        for (int x = 0; x < grassChunkSize; x++)
    //        {
    //            triangles[tris + 0] = vert + 0;
    //            triangles[tris + 1] = vert + 3;
    //            triangles[tris + 2] = vert + 2;
    //            triangles[tris + 3] = vert + 2;
    //            triangles[tris + 4] = vert + 1;
    //            triangles[tris + 5] = vert + 0;

    //            vert+= 4;
    //            tris += 6;
    //        }

    //    }
    //}

    void GenerateGrassMeshTris()
    {
        vertecies = new Vector3[grassChunkSize * grassChunkSize * 3];
        uvs2 = new Vector2[vertecies.Length];
        for (int i = 0, x = 0; x < grassChunkSize; x++)
        {
            for (int z = 0; z < grassChunkSize; z++)
            {
                

                float r1 = Random.Range(-GrassSize, GrassSize);
                float r2 = Random.Range(-GrassSize, GrassSize);

                Vector3 pos = new Vector3(x * distanceBetweenGrass + r1, TerrainNoise(x * distanceBetweenGrass + transform.position.x, z * distanceBetweenGrass + transform.position.z), z * distanceBetweenGrass + r2);

                vertecies[i + 0] = new Vector3(pos.x, pos.y, pos.z);
                vertecies[i + 1] = new Vector3(pos.x + r1 /4, pos.y + GrassSize + r1, pos.z + r2 / 4);
                vertecies[i + 2] = new Vector3(pos.x, pos.y + (GrassSize * 2 )+ r1, pos.z);

                Vector2 uv2 = new Vector2(pos.x, pos.z);

                uvs2[i + 0] = uv2;
                uvs2[i + 1] = uv2;
                uvs2[i + 2] = uv2;

                i += 3;
            }
        }

        triangles = new int[grassChunkSize * grassChunkSize * 3];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < grassChunkSize; z++)
        {
            for (int x = 0; x < grassChunkSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + 1;
                triangles[tris + 2] = vert + 2;

                vert += 3;
                tris += 3;
            }

        }

        uvs = new Vector2[vertecies.Length];
        for (int i = 0; i < uvs.Length; i+=3)
        {
            uvs[i + 0] = new Vector2(0, 0);
            uvs[i + 1] = new Vector2(1, .5f);
            uvs[i + 2] = new Vector2(0, 1);
        }
    }

    public void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertecies;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.uv2 = uvs2;


        mesh.RecalculateNormals();

        //for (int i = 0; i < mesh.normals.Length; i++)
          // mesh.normals[i] = Vector3.up;
    }

    //void PlaceGrass(Vector3 pos)
    //{
    //    Instantiate(grassPrefab, pos, Quaternion.identity, transform);
    //}

    //public void MowGrass(int x, int z)
    //{
    //    int grassi = x * grassChunkSize + z;

    //    if (isMowed[grassi])
    //    {
    //        return;
    //    }

    //    float r1 = Random.Range(-GrassSize, GrassSize);
    //    float r2 = Random.Range(-GrassSize, GrassSize);

    //    Vector3 pos = new Vector3(x * distanceBetweenGrass + r1, TerrainNoise(x * distanceBetweenGrass + transform.position.x, z * distanceBetweenGrass + transform.position.z), z * distanceBetweenGrass + r2);

    //    vertecies[grassi + 0] = new Vector3(pos.x, pos.y, pos.z);
    //    vertecies[grassi + 1] = new Vector3(pos.x + r1 / 4, pos.y + GrassSize /3 + r1, pos.z + r2 / 4);
    //    vertecies[grassi + 2] = new Vector3(pos.x, pos.y + GrassSize/2 + r1, pos.z);
    //}

    //public void DeleteGrass(int x, int z)
    //{
    //    int grassi = x * grassChunkSize + z;

    //    if (isMowed[grassi])
    //    {
    //        return;
    //    }
    //    vertecies[grassi + 0] = new Vector3(0, -10, 0);
    //    vertecies[grassi + 1] = new Vector3(0, -10, 0);
    //    vertecies[grassi + 2] = new Vector3(0, -10, 0);
    //}

    //public void UpdateVertecies()
    //{
    //    mesh.vertices = vertecies;
    //}


    //IEnumerator MowAllGrass()
    //{
    //    for (int x = 0; x < grassChunkSize; x++)
    //    {
    //        for (int z = 0; z < grassChunkSize; z++)
    //        {
    //            DeleteGrass(x,z);
    //            UpdateVertecies();
    //            //yield return new WaitForSeconds(Time.deltaTime);
    //        }
    //    }


    //    yield return 0;
    //}

    float TerrainNoise(float x, float z)
    {
        float y = Mathf.PerlinNoise(x * .7f, z * .7f) * .5f;
        y += Mathf.PerlinNoise(x * .08f, z * .08f) * 2.5f * 1.5f;
        return y;
    }
}