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

                var colliderCollided = hit.transform.gameObject.GetComponent<BoxCollider>();
                var backMiddle = (colliderCollided.center - new Vector3(0, 0, colliderCollided.size.z / 2));
                var frontMiddle = backMiddle + new Vector3(0, 0, colliderCollided.size.z);

                Debug.DrawLine(hit.transform.TransformPoint(backMiddle),
                               hit.transform.TransformPoint(frontMiddle), Color.black);

                var hitPositionLocal = hit.transform.InverseTransformPoint(hit.point);
                var calculatedPosition = hit.transform.TransformPoint(backMiddle) + new Vector3(0, 0, hitPositionLocal.z);
                Debug.DrawLine(hit.transform.TransformPoint(backMiddle),
                               hit.transform.TransformPoint(calculatedPosition), Color.white);

                var hitPositionColliderVerticalPercentage = (calculatedPosition - backMiddle).sqrMagnitude / (frontMiddle - backMiddle).sqrMagnitude;
                Debug.Log("percentage: " + hitPositionColliderVerticalPercentage);
                MousePositionLeftHalfOfScreen.value =(hitPositionColliderVerticalPercentage * 2 ) - 1;
            }

            if (hit.transform.gameObject.name == "TopRightUdder")
            {
                hitDetected = true;
                hitPoint = hit.point;
                MousePositionRightHalfOfScreen.value = (Mathf.InverseLerp(rightUdderZMin, rightUdderZMax, hitPoint.z) * 2) - 1;
            }
        }
    }
}
