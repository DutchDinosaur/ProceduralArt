using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTreeMesh : MonoBehaviour
{
    [SerializeField] bool onValidate;

    public Branch branchh;

    [SerializeField, HideInInspector]
    MeshFilter meshFilter;

    Mesh mesh;

    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;
    
    private void OnValidate() {
        if (onValidate) {
            Initialise();
        }
    }

    public void Initialise() {
        mesh = new Mesh();
        if (meshFilter == null) {
            meshFilter = GetComponent<MeshFilter>();
        }

        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        TreeGen(branchh);

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        //mesh.normals = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void TreeGen(Branch branch) {
        for (int i = 0; i < branch.subBranches.Count; i++) { //generate subbranches
            TreeGen(branch.subBranches[i]);
        }

        int Length = branch.Positions.Count;
        if (Length < 3 || branch.Widths.Count != Length || branch.resolution < 2) { //check branch data
            //Debug.LogError("incorrect branch data");
            return;
        }


        for (int d = 0; d < Length - 2; d++) {
            Vector3[] tempPosses = new Vector3[branch.resolution * 2]; //calculate points for each piece
            Vector2[] tempUvs = new Vector2[branch.resolution * 2];
            float pieceLength = Dist(branch.Positions[d + 1], branch.Positions[d]);
            Vector3 pieceTopOffset = ((branch.Positions[d + 1] - branch.Positions[d]).normalized * pieceLength * branch.overshoot);

            for (int i = 0; i < branch.resolution; i++) {
                Vector2 p = posOnCircle(i, branch.resolution, branch.Widths[d]);
                tempPosses[i] = new Vector3(p.x + branch.Positions[d].x, p.y + branch.Positions[d].y, branch.Positions[d].z);

                Vector3 dir = (branch.Positions[d] - branch.Positions[d + 1]).normalized;
                tempPosses[i] = RotatePointAroundPivot(tempPosses[i], branch.Positions[d], Quaternion.LookRotation(dir));
                tempPosses[i + branch.resolution] = tempPosses[i] + pieceTopOffset;

                float uvx = 1f / branch.resolution * i;
                tempUvs[i] = new Vector2(uvx, 0);
                tempUvs[i + branch.resolution] = new Vector2(uvx, pieceLength);
            }

            for (int i = 0; i < tempPosses.Length; i++) {
                vertices.Add(tempPosses[i]);
                uvs.Add(tempUvs[i]);
            }

            int v = vertices.Count - 1;
            for (int i = 0; i < branch.resolution - 1; i++) { //add quads inbetween points
                int[] verts = new int[] { v - branch.resolution - 1 - i
                                        , v - i - 1
                                        , v - i
                                        , v - branch.resolution - i
                }; makeQuad(verts);
            }
            int[] Verts = new int[] { v - branch.resolution //fill in last quad
                                    , v 
                                    , v - branch.resolution + 1
                                    , v - branch.resolution - branch.resolution + 1
            }; makeQuad(Verts);

            //add end to each piece
            vertices.Add(branch.Positions[d] + pieceTopOffset * branch.overshootPoint);
            uvs.Add(new Vector2(.5f, branch.overshootPoint));

            for (int i = 0; i < branch.resolution - 1; i++) {
                triangles.Add(vertices.Count - 1);
                triangles.Add(vertices.Count - 2 - i);
                triangles.Add(vertices.Count - 3 - i);
            }

            triangles.Add(vertices.Count - 1);
            triangles.Add(vertices.Count - branch.resolution - 1);
            triangles.Add(vertices.Count - 2);
        }

        GenerateEnd(); //generate last piece
        void GenerateEnd() {
            Vector3[] tempPosses = new Vector3[branch.resolution];
            for (int i = 0; i < branch.resolution; i++) {
                Vector2 p = posOnCircle(i, branch.resolution, branch.Widths[branch.Widths.Count - 2]);
                tempPosses[i] = new Vector3(p.x + branch.Positions[branch.Positions.Count - 2].x, p.y + branch.Positions[branch.Positions.Count - 2].y, branch.Positions[branch.Positions.Count - 2].z);

                Vector3 dir = (branch.Positions[branch.Positions.Count - 1] - branch.Positions[branch.Positions.Count - 2]).normalized;
                tempPosses[i] = RotatePointAroundPivot(tempPosses[i], branch.Positions[branch.Positions.Count - 2], Quaternion.LookRotation(dir));
                vertices.Add(tempPosses[i]);
                uvs.Add(new Vector2(1f / branch.resolution * i, 0));
            }
            vertices.Add(branch.Positions[branch.Positions.Count - 1]);
            uvs.Add(new Vector2(.5f, 1));

            for (int i = 0; i < branch.resolution - 1; i++) {
                triangles.Add(vertices.Count - 3 - i);
                triangles.Add(vertices.Count - 2 - i);
                triangles.Add(vertices.Count - 1);
            }

            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - branch.resolution - 1);
            triangles.Add(vertices.Count - 1);
        }

        void makeQuad(int[] vertecies) {
            triangles.Add(vertecies[0]);
            triangles.Add(vertecies[1]);
            triangles.Add(vertecies[2]);

            triangles.Add(vertecies[0]);
            triangles.Add(vertecies[2]);
            triangles.Add(vertecies[3]);
        }
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angles) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = angles * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    Vector2 posOnCircle(int i, int totalPosses, float radius) {
        float dir = ((2 * Mathf.PI) / totalPosses) * i;
        return new Vector2(Mathf.Cos(dir) * radius, Mathf.Sin(dir) * radius);
    }

    float Dist(Vector3 firstPosition, Vector3 secondPosition) {
        Vector3 heading;
        float distanceSquared;
        float distance;

        heading = firstPosition - secondPosition;

        distanceSquared = heading.x * heading.x + heading.y * heading.y + heading.z * heading.z;
        distance = Mathf.Sqrt(distanceSquared);

        return distance;
    }
}