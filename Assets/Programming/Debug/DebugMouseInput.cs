using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMouseInput : MonoBehaviour
{
    public Button MouseDown;

    public Slider MousePositionLeftHalfOfScreen;
    public Slider MousePositionRightHalfOfScreen;

    public Color ButtonHeldColor;
    public Color ButtonNeutralColor;

    public float HitDebugSphereSize = 0.1f;

    bool hitDetected;
    Vector3 hitPoint;

    public float leftUdderZMax = 0.08f;
    public float leftUdderZMin = -0.02f;

    public float rightUdderZMax = 0.08f;
    public float rightUdderZMin = -0.02f;

    // Use this for initialization
    void Start()
    {

    }

    public void OnDrawGizmos()
    {
        if (hitDetected)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(hitPoint, HitDebugSphereSize);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MouseDown.image.color = Input.GetMouseButtonDown(0) ? ButtonHeldColor : ButtonNeutralColor;

        hitDetected = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.black);
        var hits = Physics.RaycastAll(ray);

        foreach (var hit in hits)
        {
            if (hit.transform.gameObject.name == "TopLeftUdder")
            {
                hitDetected = true;
                hitPoint = hit.point;
                //Debug.Log(string.Format("z: {0}", hitPoint.z));
                MousePositionLeftHalfOfScreen.value = (Mathf.InverseLerp(leftUdderZMin, leftUdderZMax, hitPoint.z) * 2) - 1;

                var colliderCollided = hit.transform.gameObject.GetComponent<BoxCollider>();
                var backMiddle = (colliderCollided.center - new Vector3(0, 0, colliderCollided.size.z / 2));
                var frontMiddle = backMiddle + new Vector3(0, 0, colliderCollided.size.z);



                Debug.DrawLine(hit.transform.TransformPoint(backMiddle),
                               hit.transform.TransformPoint(frontMiddle), Color.black);

                //var mousePositionOnLine = Vector3.Project(
                //(hit.transform.TransformPoint(backMiddle) + hit.transform.TransformPoint(frontMiddle)),
                //(ray.origin + ray.direction));

                //var mousePositionOnLine = ClosestPointOnLine(backMiddle, frontMiddle, hit.point);

                var mousePositionOnLine = ClosestPointOnLine(frontMiddle, backMiddle, hit.point);


                Debug.DrawLine(hit.transform.TransformPoint(backMiddle),
                               mousePositionOnLine, Color.white);



                //Debug.DrawLine(hit.transform.TransformPoint(backMiddle),
                //hit.transform.TransformPoint(colliderCollided.center), Color.white);
            }

            if (hit.transform.gameObject.name == "TopRightUdder")
            {
                hitDetected = true;
                hitPoint = hit.point;
                //Debug.Log(string.Format("z: {0}", hitPoint.z));
                MousePositionRightHalfOfScreen.value = (Mathf.InverseLerp(rightUdderZMin, rightUdderZMax, hitPoint.z) * 2) - 1;
            }
        }
    }

    private Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
    {
        var vVector1 = vPoint - vA;
        var vVector2 = (vB - vA).normalized;

        var d = Vector3.Distance(vA, vB);
        var t = Vector3.Dot(vVector2, vVector1);

        if (t <= 0)
            return vA;

        if (t >= d)
            return vB;

        var vVector3 = vVector2 * t;

        var vClosestPoint = vA + vVector3;

        return vClosestPoint;
    }

    //return Vector3.Project((pointNotOnLine - lineStart), (lineEnd - lineStart)) + lineStart;

    //        function Closest(vA : Vector3, vB: Vector3, vPoint: Vector3): Vector3


    //{

    //    return Vector3.Project((vPoint - vA), (vB - vA)) + vA;

    //}
}
