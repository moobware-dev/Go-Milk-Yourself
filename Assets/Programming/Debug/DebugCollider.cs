using UnityEngine;

[ExecuteInEditMode]
public class DebugCollider : MonoBehaviour
{
    public bool MatchRotation = false;
    Collider _collider;

    void Start()
    {
        _collider = GetComponent<Collider>();
    }

    void OnDrawGizmos()
    {
        // TODO figure out wtf is going on here, when I rotate, the extents and the size change,
        //      in my mind the size sholdn't change, I'm not scaling the shape just rotating it fml
        var topMiddle = new Vector3(_collider.bounds.center.x, (_collider.bounds.center.y + (_collider.bounds.size.y / 2f)), _collider.bounds.center.z);
        if (MatchRotation) {
            topMiddle = transform.rotation * topMiddle;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(topMiddle, 0.1f);
    }
}