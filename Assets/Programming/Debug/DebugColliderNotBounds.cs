using UnityEngine;

[ExecuteInEditMode]
public class DebugColliderNotBounds : MonoBehaviour {
    public Color color = Color.green;
    public float Z_Adjustment = 0f;
    public bool MatchRotation = false;
    public bool MatchScale = false;
    BoxCollider _collider;

	// Use this for initialization
	void Start () {
        _collider = GetComponent<BoxCollider>();	
	}

	void OnDrawGizmos()
	{
        var topMiddle = (_collider.center + new Vector3(0, _collider.size.y / 2, 0));

        if (MatchScale)
        {
            topMiddle = Vector3.Scale(topMiddle, transform.localScale);
        }

        if (MatchRotation)
        {
            topMiddle = transform.rotation * topMiddle;
        }

        topMiddle.z += Z_Adjustment;
        Gizmos.color = color;
        Gizmos.DrawSphere(topMiddle, 0.1f);
	}
}
