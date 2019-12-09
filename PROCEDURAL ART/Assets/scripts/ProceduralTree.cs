using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTree : MonoBehaviour
{
    [SerializeField] GameObject branchPrefab;
    [SerializeField] GameObject mainStum;
    [SerializeField] Transform Parent;
    [SerializeField] bool useRandomSeed;
    [SerializeField] string seed;

    GenerateTreeMesh mainStump;
    List<GenerateTreeMesh> branches;

    bool b;
    System.Random rdm;

    private void Start() {
        if (useRandomSeed) {
            seed = System.DateTime.Now.ToString();
        }
        rdm = new System.Random(seed.GetHashCode());

        branches = new List<GenerateTreeMesh>();

        mainStump = newBranch(transform.position,Vector3.up, mainStum);
        GenerateGrowthVectors(mainStump.branchh, Vector3.up);
    }

    private void FixedUpdate() {
        if (b) {
            grow();
            mainStump.Initialise();
            b = false;
        }
        else {
            b = true;
        }
    }

    void grow() {
        List<Vector3> cuml = GrowBranch(mainStump.branchh, Vector3.zero);

        for (int i = 0; i < branches.Count; i++) {
            GrowBranch(branches[i].branchh, cuml[branches[i].branchh.ParentIndex]);
        }

        List<int> subBranchParentIndexes = new List<int>();
        foreach (var subBranch in branches) {
            subBranchParentIndexes.Add(subBranch.branchh.ParentIndex);
        }

        if (mainStump.branchh.growthVector[mainStump.branchh.growthVector.Count - 1].magnitude < mainStump.branchh.newSectionReq) {
            addNewBranchSection(mainStump.branchh, generateRandomGrowthVector(Vector3.up));
            mainStump.branchh.newSectionReq -= mainStump.branchh.newSectionFallOff;
        }
    }

    List<Vector3> GrowBranch(Branch br, Vector3 cumulativeOffset) {
        List<Vector3> offsets = new List<Vector3>();

        Vector3 cumulativeVector = cumulativeOffset;
        for (int i = 0; i < br.Positions.Count; i++) {
            cumulativeVector += br.growthVector[i] * br.GrowthSpeed;
            offsets.Add(cumulativeOffset);

            br.Positions[i] += cumulativeVector;
            br.growthVector[i] *= br.GrowthFallOf;
        }

        return offsets;
    }

    GenerateTreeMesh newBranch(Vector3 pos, Vector3 dir, GameObject prefab) {
        GenerateTreeMesh tm = GameObject.Instantiate(prefab, Parent).GetComponent<GenerateTreeMesh>();
        //for (int i = 0; i < b.Positions.Count; i++) {
        //    b.Positions[i] += dir * i;
        //}
        tm.branchh.growthVector = new List<Vector3>();
        tm.Initialise();
        return tm;
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

    void GenerateGrowthVectors(Branch br , Vector3 BranchDirOffset) {
        br.growthVector.Add(Vector3.zero);

        for (int i = br.growthVector.Count; i < br.Positions.Count - 1; i++) {
            br.growthVector.Add((br.Positions[i + 1] - br.Positions[i]).normalized);
        }

        br.growthVector.Add(BranchDirOffset);
    }

    void addNewBranchSection(Branch br, Vector3 GrowthVector) {
        br.Positions.Add(br.Positions[br.Positions.Count - 1]);
        br.Widths.Add(br.Widths[br.Widths.Count - 1]);
        br.growthVector.Add((GrowthVector / br.newSectionDirInfluence + br.growthVector[br.growthVector.Count - 1]).normalized * br.newSectionGrowthMultiplier);
    }

    Vector3 generateRandomGrowthVector(Vector3 dir) {
        Vector3 v = dir;
        
        v += new Vector3(rdm.Next(-100, 100) / 100f, rdm.Next(0, 100) / 100f, rdm.Next(-100, 100) / 100f);
        return v;
    }
}