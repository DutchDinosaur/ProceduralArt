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
        if (Length < 2 || branch.Widths.Count != Length) {
            return;
        }

        Vector3[] vertices = new Vector3[Length * branch.resolution + 2];
        Vector3[] normals = new Vector3[Length * branch.resolution + branch.resolution];

        int[] triangles = new int[3 *(2 * (branch.resolution) + Length * (branch.resolution * 2))];


        vertices[0] = branch.Positions[0];

        Vector3[] tempPosses = new Vector3[branch.resolution];
        for (int i = 0; i < branch.resolution; i++) {
            Vector2 p = posOnCircle(i, branch.resolution, branch.Widths[0]);
            tempPosses[i] = new Vector3(p.x, p.y, branch.Positions[0].z);

            Vector3 dir = (branch.Positions[0] - branch.Positions[1]).normalized;
            Quaternion quatDir = Quaternion.LookRotation(dir);
            tempPosses[i] = RotatePointAroundPivot(tempPosses[i], branch.Positions[0], quatDir);
            vertices[i + 1] = tempPosses[i];
        }




        //float dx = branch.Positions[1].x - branch.Positions[0].x;
        //float dy = branch.Positions[1].y - branch.Positions[0].y;
        //vertices[1] = new Vector3(dx, -dy, branch.Positions[0].z);//.normalized * branch.Widths[0];
        //vertices[2] = new Vector3(branch.Positions[0].x, -dy, dx);//.normalized * branch.Widths[0];
        //vertices[3] = new Vector3(-dx, dy, branch.Positions[0].z);//.normalized * branch.Widths[0];
        //vertices[4] = new Vector3(branch.Positions[0].x, dy, -dx);//.normalized * branch.Widths[0];



        triangles[0] = 0;
        triangles[1] = 1;
        triangles[3] = 2;

        triangles[0] = 0;
        triangles[1] = 2;
        triangles[3] = 3;

        triangles[0] = 0;
        triangles[1] = 3;
        triangles[3] = 4;

        triangles[0] = 0;
        triangles[1] = 4;
        triangles[3] = 1;

        for (int i = 1; i < Length; i++) {

        }


        //for (int d = 0; d < 6; d++)
        //{
        //    Vector3 direction = directions[d];
        //    Vector3 axisA = new Vector3(direction.y, direction.z, direction.x);
        //    Vector3 axisB = Vector3.Cross(direction, axisA);

        //    for (int y = 0; y < resolution; y++)
        //    {
        //        for (int x = 0; x < resolution; x++)
        //        {
        //            int i = x + (y * resolution) + (d * resolution * resolution);

        //            Vector2 percent = new Vector2(x, y) / (resolution - 1);
        //            Vector3 pointOnUnitCube = direction + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
        //            vertices[i] = pointOnUnitCube.normalized;
        //            normals[i] = vertices[i];

        //            if (x != resolution - 1 && y != resolution - 1)
        //            {
        //                triangles[triIndex] = i;
        //                triangles[triIndex + 1] = i + resolution + 1;
        //                triangles[triIndex + 2] = i + resolution;

        //                triangles[triIndex + 3] = i;
        //                triangles[triIndex + 4] = i + 1;
        //                triangles[triIndex + 5] = i + resolution + 1;

        //                triIndex += 6;
        //            }
        //        }
        //    }
        //}

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        //mesh.normals = normals;
    }

    private void OnDrawGizmos()
    {
        Branch branch = branchh;
        Gizmos.DrawSphere(branch.Positions[0], 0.1f);
        Gizmos.DrawSphere(branch.Positions[1], 0.1f);
        Gizmos.DrawSphere(branch.Positions[2], 0.1f);

        

        Vector3[] tempPosses = new Vector3[branch.resolution];
        for (int i = 0; i < branch.resolution; i++) {
            Vector2 p = posOnCircle(i,branch.resolution,branch.Widths[0]);
            tempPosses[i] = new Vector3(p.x, p.y, branch.Positions[0].z);

            Vector3 dir =(branch.Positions[0] - branch.Positions[1]).normalized;
            Quaternion quatDir = Quaternion.LookRotation(dir);
            //float step = 1 * Time.deltaTime;

            //tempPosses[i] = Vector3.RotateTowards(tempPosses[i],dir,step, benis);
            tempPosses[i] = RotatePointAroundPivot(tempPosses[i],branch.Positions[0], quatDir);




            //Gizmos.DrawSphere(new Vector3(p.x, branch.Positions[0].y, p.y), .1f);

            Gizmos.DrawSphere(tempPosses[i], .1f);

            //float dist = Dist(branch.Positions[0],branch.Positions[1]);

        }
    }

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