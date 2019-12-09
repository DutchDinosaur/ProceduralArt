using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform camera;
    [SerializeField] ProceduralTree tre;

    public Vector3 CamPosOffset;

    Vector3 camVelocity;
    public float smoothTime;

    void Update() {
        Vector3 desiredPos = tre.trackPos + CamPosOffset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(camera.transform.position, desiredPos, ref camVelocity, smoothTime);
        camera.transform.position = smoothedPosition;

        camera.transform.LookAt(tre.trackPos);
    }
}