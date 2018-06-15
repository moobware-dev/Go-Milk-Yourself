using UnityEngine;

[ExecuteInEditMode]
public class DebugCollider4 : MonoBehaviour {
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

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(topMiddle, 0.1f);
	}

	// Update is called once per frame
	void Update () {

	}
}
