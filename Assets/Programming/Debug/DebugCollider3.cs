using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugCollider3 : MonoBehaviour {
    BoxCollider _collider;

	// Use this for initialization
	void Start () {
        _collider = GetComponent<BoxCollider>();	
	}

	private void OnDrawGizmos()
	{
        var min = _collider.bounds.min;
        var max = _collider.bounds.max;
        var point1 = transform.TransformPoint(new Vector3(min.x, min.y, min.z));
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(point1, 0.1f);
        var point2 = transform.TransformPoint(new Vector3(min.x, min.y, max.z));
        var point3 = transform.TransformPoint(new Vector3(min.x, max.y, min.z));
        var point4 = transform.TransformPoint(new Vector3(min.x, max.y, max.z));
        var point5 = transform.TransformPoint(new Vector3(max.x, min.y, min.z));
        var point6 = transform.TransformPoint(new Vector3(max.x, min.y, max.z));
        var point7 = transform.TransformPoint(new Vector3(max.x, max.y, min.z));
        var point8 = transform.TransformPoint(new Vector3(max.x, max.y, max.z));
	}

	// Update is called once per frame
	void Update () {

	}
}
