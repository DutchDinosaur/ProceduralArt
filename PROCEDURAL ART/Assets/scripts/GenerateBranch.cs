using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBranch : MonoBehaviour
{
    public bool UPDATE;

    public Quaternion rot;

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

        for (int d = 0; d < Length - 3; d++) {
            Vector3[] tempPosses = new Vector3[branch.resolution];
            for (int i = 0; i < branch.resolution; i++) {
                Vector2 p = posOnCircle(i, branch.resolution, branch.Widths[d]);
                tempPosses[i] = new Vector3(p.x + branch.Positions[d].x, p.y + branch.Positions[d].y, branch.Positions[d].z);

                Vector3 dir = (branch.Positions[d] - branch.Positions[d + 1]).normalized;
                tempPosses[i] = RotatePointAroundPivot(tempPosses[i], branch.Positions[d], Quaternion.LookRotation(dir));
                vertices.Add(tempPosses[i]);
            }

            //int v = vertices.Count;

            //for (int i = 0; i < branch.resolution - 1; i++)
            //{
            //    triangles.Add(v - branch.resolution - i);
            //    triangles.Add(v - 1 - i);
            //    triangles.Add(v - branch.resolution - 1 - i);

            //    triangles.Add(v - branch.resolution - i);
            //    triangles.Add(v - 1 - i);
            //    triangles.Add(v - i);
            //}
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
                    Vector2 p = posOnCircle(i, branch.resolution, branch.Widths[branch.Widths.Count -1]);
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
    }

    //private void OnDrawGizmos()
    //{
    //    Branch branch = branchh;
    //    Gizmos.DrawSphere(branch.Positions[0], 0.1f);
    //    Gizmos.DrawSphere(branch.Positions[1], 0.1f);
    //    Gizmos.DrawSphere(branch.Positions[2], 0.1f);



    //    Vector3[] tempPosses = new Vector3[branch.resolution];
    //    for (int i = 0; i < branch.resolution; i++) {
    //        Vector2 p = posOnCircle(i,branch.resolution,branch.Widths[0]);
    //        tempPosses[i] = new Vector3(p.x + branch.Positions[0].x, p.y + branch.Positions[0].y, branch.Positions[0].z);

    //        Vector3 dir =(branch.Positions[0] - branch.Positions[1]).normalized;
    //        Quaternion quatDir = Quaternion.LookRotation(dir);
    //        //float step = 1 * Time.deltaTime;

    //        //tempPosses[i] = Vector3.RotateTowards(tempPosses[i],dir,step, benis);
    //        tempPosses[i] = RotatePointAroundPivot(tempPosses[i],branch.Positions[0], quatDir);




    //        //Gizmos.DrawSphere(new Vector3(p.x, branch.Positions[0].y, p.y), .1f);

    //        Gizmos.DrawSphere(tempPosses[i], .1f);

    //        //float dist = Dist(branch.Positions[0],branch.Positions[1]);

    //    }
    //}

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angles) {
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