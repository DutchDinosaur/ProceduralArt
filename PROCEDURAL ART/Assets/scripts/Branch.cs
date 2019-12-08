using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class Branch
{
    public List<Vector3> Positions;
    public List<float> Widths;
    public List<Branch> subBranches;
    public int resolution;
    public float overshoot = 1;
    public float overshootPoint = 1;
}