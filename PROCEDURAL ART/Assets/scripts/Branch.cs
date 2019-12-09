using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Branch {
    //MeshGen
    public List<Vector3> Positions;
    public List<float> Widths;
    public List<Branch> subBranches;
    public int resolution;
    public float overshoot = 1;
    public float overshootPoint = 1;

    //Gen
    public List<Vector3> growthVector;


    [Range(0, .2f)] public float GrowthSpeed = .019f;
    [Range(0, 1)] public float GrowthFallOf = .96f;

    [Range(0, 1)] public float newSectionReq = .11f;
    [Range(0, 1)] public float newSectionFallOff = .03f;
    public float newSectionDirInfluence = .5f;
    public float newSectionGrowthMultiplier = 5f;
    public int ParentIndex;
}