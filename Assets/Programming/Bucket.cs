using UnityEngine;
using System.Collections;

public class Bucket : MonoBehaviour
{
    [Range(0f, 1f)]
    public float percentFull;
    private float _lastFramePercentFullTarget;
    private float _fillLerpStartTime;
    private float _fillLerpStartPercentFull;
    private float _runningPercentFull = 0f;
    private float _fillSpeed = 0.5f;

    public AnimationCurve bucketFillingCurve;

    public Transform milkPlane;

    public Vector3 emptyMilkPosition;
    public Vector3 fullMilkPosition;

    public Vector3 emptyMilkScale;
    public Vector3 fullMilkScale;

    public float destroyBucketEalierSeconds = 0.2f;

    private Animator animator;

    private bool markedForDeath = false;

    void Start()
    {
        _lastFramePercentFullTarget = percentFull;
        _runningPercentFull = percentFull;
        _fillLerpStartPercentFull = percentFull;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_lastFramePercentFullTarget == percentFull
            && _runningPercentFull == percentFull)
        {
            // we are at target
            return;
        }

        if (_lastFramePercentFullTarget != percentFull)
        {
            // target changed
            // set new target
            _fillLerpStartTime = Time.time;
            _fillLerpStartPercentFull = _runningPercentFull;
            _lastFramePercentFullTarget = percentFull;
        }

        // lerp start percentage to target (percentfull) TODO https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
        _runningPercentFull = Mathf.Lerp(_fillLerpStartPercentFull, percentFull, ((Time.time - _fillLerpStartTime) * _fillSpeed) / (percentFull - _fillLerpStartPercentFull));


        // TODO set target/state above, and lerp awayyyyy
        milkPlane.localPosition = Vector3.Lerp(emptyMilkPosition, fullMilkPosition, bucketFillingCurve.Evaluate(_runningPercentFull));
        milkPlane.localScale = Vector3.Lerp(emptyMilkScale, fullMilkScale, bucketFillingCurve.Evaluate(_runningPercentFull));
    }

    public float AddPercentFullToBucketReturnNewPercentFull(float percentToAdd) {
        percentFull += percentToAdd;

        if (percentFull >= 1f) {
            if (!markedForDeath) {
                markedForDeath = true;
                animator.SetTrigger("BucketFilled");
                var timeToKill = animator.GetCurrentAnimatorStateInfo(0).length - destroyBucketEalierSeconds;
                StartCoroutine(DestroyAfterSeconds(timeToKill));
            }
        }

        return percentFull;
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(transform.gameObject);
    }
}
