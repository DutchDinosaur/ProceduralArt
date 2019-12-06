using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBranch : MonoBehaviour
{
    public bool UPDATE;

    [SerializeField] Branch branchh;

    [System.Serializable]
    class Branch {
        public List<Vector3> Positions;
        public List<float> Widths;
        public List<Branch> subBranches;
        public int resolution;
    }

    [SerializeField, HideInInspector]
    MeshFilter meshFilter;

    Mesh mesh;

    private void OnValidate()
    {
        Initialise();
    }

    private void Initialise() {
        mesh = new Mesh();
        if (meshFilter == null) {
            meshFilter = GetComponent<MeshFilter>();
        }

        BranchGen(branchh);
        meshFilter.mesh = mesh;

    }

    void BranchGen(Branch branch) {
        int Length = branch.Positions.Count;
        if (Length < 3 || branch.Widths.Count != Length) {
            return;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        ends(true);

        for (int d = 1; d < Length - 2; d++) {
            Vector3[] tempPosses = new Vector3[branch.resolution];
            for (int i = 0; i < branch.resolution; i++) {
                Vector2 p = posOnCircle(i, branch.resolution, branch.Widths[d]);
                tempPosses[i] = new Vector3(p.x + branch.Positions[d].x, p.y + branch.Positions[d].y, branch.Positions[d].z);

                Vector3 dir = (branch.Positions[d] - branch.Positions[d + 1]).normalized;
                tempPosses[i] = RotatePointAroundPivot(tempPosses[i], branch.Positions[d], Quaternion.LookRotation(dir));
                vertices.Add(tempPosses[i]);
            }

            int v = vertices.Count - 1;

            for (int i = 0; i < branch.resolution - 1; i++) {
                int[] verts = new int[] { v - branch.resolution - 1 - i, v - i - 1, v  - i, v - branch.resolution - i };
                makeQuad(verts);
            }
        }

        int vc = vertices.Count - 1 + branch.resolution;
        for (int i = 0; i < branch.resolution - 1; i++) {
            int[] verts = new int[] { vc - branch.resolution - 1 - i, vc - i - 1, vc - i, vc - branch.resolution - i };
            makeQuad(verts);
        }

        ends(false);

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        void ends(bool begin) {
            if (begin) {
                vertices.Add(branch.Positions[0]);

                Vector3[] tempPosses = new Vector3[branch.resolution];
                for (int i = 0; i < branch.resolution; i++)  {
                    Vector2 p = posOnCircle(i, branch.resolution, branch.Widths[0]);
                    tempPosses[i] = new Vector3(p.x + branch.Positions[0].x, p.y + branch.Positions[0].y, branch.Positions[0].z);

                    Vector3 dir = (branch.Positions[0] - branch.Positions[1]).normalized;
                    tempPosses[i] = RotatePointAroundPivot(tempPosses[i], branch.Positions[0], Quaternion.LookRotation(dir));
                    vertices.Add(tempPosses[i]);
                }

                for (int i = 0; i < branch.resolution - 1; i++) {
                    triangles.Add(0);
                    triangles.Add(i + 1);
                    triangles.Add(i + 2);
                }

                triangles.Add(0);
                triangles.Add(branch.resolution);
                triangles.Add(1);
            } else {

                Vector3[] tempPosses = new Vector3[branch.resolution];
                for (int i = 0; i < branch.resolution; i++) {
                    Vector2 p = posOnCircle(i, branch.resolution, branch.Widths[branch.Widths.Count -2]);
                    tempPosses[i] = new Vector3(p.x + branch.Positions[branch.Positions.Count - 2].x, p.y + branch.Positions[branch.Positions.Count - 2].y, branch.Positions[branch.Positions.Count - 2].z);
                    
                    Vector3 dir = (branch.Positions[branch.Positions.Count -1] - branch.Positions[branch.Positions.Count - 2]).normalized;
                    tempPosses[i] = RotatePointAroundPivot(tempPosses[i], branch.Positions[branch.Positions.Count - 2], Quaternion.LookRotation(dir));
                    vertices.Add(tempPosses[i]);
                }
                vertices.Add(branch.Positions[branch.Positions.Count - 1]);

                for (int i = 0; i < branch.resolution - 1; i++) {
                    triangles.Add(vertices.Count - 3 - i);
                    triangles.Add(vertices.Count - 2 - i);
                    triangles.Add(vertices.Count - 1);
                }

                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - branch.resolution - 1);
                triangles.Add(vertices.Count - 1);
            }
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

        heading.x = firstPosition.x - secondPosition.x;
        heading.y = firstPosition.y - secondPosition.y;
        heading.z = firstPosition.z - secondPosition.z;

        distanceSquared = heading.x * heading.x + heading.y * heading.y + heading.z * heading.z;
        distance = Mathf.Sqrt(distanceSquared);

        return distance;
    }
}