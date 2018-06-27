using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameController : MonoBehaviour
{
    public float milkPerSquirt = 0.1f;

    public float StrokeThreshold = 0.01f;
    public float SecondsToEaseIntoIt = 3f;
    public float AssumeThePositionSpeed = 1f;
    public float UdderMouseHoverAdjustmentMultiplier = 3f;
    public float InGameMilkingSpeed = 15f;
    public float MilkingSpeedDelay = 1f;
    public float HoofPinchSpeed = 15f;
    private bool inPosition = false;

    public float BucketsPerMinuteUpdateInterval = 0.3f;

    public GameObject TopLeftUdder;
    public GameObject TopRightUdder;

    public GameObject BucketPrefab;
    private Bucket currentBucketScript;
    private GameObject currentBucketGameObject;
    public Transform MilkCatchingPosition;
    public Vector3 BucketGetMilkedOnPosition;
    public Vector3 BucketGoOffscreenPosition;

    public Text BucketsPerMinuteText;

    public ParticleSystem UpperLeftMilkParticles;
    public ParticleSystem UpperRightMilkParticles;

    public Transform LeftArm;
    public Transform RightArm;

    public Transform LeftArmUpperHoof;
    public Transform LeftArmLowerHoof;

    public Transform RightArmUpperHoof;
    public Transform RightArmLowerHoof;


    public Vector3 LeftArmStartPosition;
    public Vector3 RightArmStartPosition;

    public Vector3 MilkUpperLeftBasePosition;
    public Vector3 MilkUpperLeftJustTheTipPosition;

    public Vector3 MilkLowerLeftBasePosition;
    public Vector3 MilkLowerLeftJustTheTipPosition;

    public Vector3 MilkUpperRightBasePosition;
    public Vector3 MilkUpperRightJustTheTipPosition;

    public Vector3 MilkLowerRightRBasePosition;
    public Vector3 MilkLowerRightJustTheTipPosition;


    public Vector3 UpperHoofRelaxedRotation;
    public Vector3 UpperHoofPinchedRotation;

    public Vector3 LowerHoofRelaxedRotation;
    public Vector3 LowerHoofPinchedRotation;

    public bool IsLeftStroke;
    public Vector3 LeftStrokeStartPos;

    public bool IsRightStroke;
    public Vector3 RightStrokeStartPos;

    public AudioSource UpperLeftUdderAudioSource;
    public AudioSource UpperRightUdderAudioSource;

    public List<FilledBucketDataPoint> bucketsPerMinuteData;
    private float bucketsMilked;

    // Use this for initialization
    void Start()
    {
        bucketsPerMinuteData = new List<FilledBucketDataPoint>();
        BucketsPerMinuteText.text = "0";
        CreateNewBucket();
        StartCoroutine(ContinuouslyUpdateBucketsPerMinute());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > SecondsToEaseIntoIt)
        {
            inPosition = true;
        }

        var movmentSpeed = inPosition ? InGameMilkingSpeed : AssumeThePositionSpeed;

        var mouseInput = GetMouseInput();
        var joystickInput = GetPs4ControllerInput();

        var leftArmPositionInput = 0f;
        leftArmPositionInput = mouseInput.LeftArmPosition != 0f ? mouseInput.LeftArmPosition : joystickInput.LeftArmPosition;
        var rightArmPositionInput = 0f;
        rightArmPositionInput = mouseInput.RightArmPosition != 0f ? mouseInput.RightArmPosition : joystickInput.RightArmPosition;

        var leftHoofPinchInput = mouseInput.LeftHoofPinch ? mouseInput.LeftHoofPinch : joystickInput.LeftHoofPinch;
        var rightHoofPinchInput = mouseInput.RightHoofPinch ? mouseInput.RightHoofPinch : joystickInput.RightHoofPinch;

        var leftArmTargetPosition = Vector3.Lerp(MilkUpperLeftBasePosition, MilkUpperLeftJustTheTipPosition, leftArmPositionInput);
        var rightArmTargetPosition = Vector3.Lerp(MilkUpperRightBasePosition, MilkUpperRightJustTheTipPosition, rightArmPositionInput);

        LeftArm.position = Vector3.Lerp(LeftArm.position, leftArmTargetPosition, Time.deltaTime * movmentSpeed);
        RightArm.position = Vector3.Lerp(RightArm.position, rightArmTargetPosition, Time.deltaTime * movmentSpeed);

        var leftUpperHoofTargetRotation = leftHoofPinchInput ? UpperHoofPinchedRotation : UpperHoofRelaxedRotation;
        var leftLowerHoofTargetRotation = leftHoofPinchInput ? LowerHoofPinchedRotation : LowerHoofRelaxedRotation;

        var rightUpperHoofTargetRotation = rightHoofPinchInput ? UpperHoofPinchedRotation : UpperHoofRelaxedRotation;
        var rightLowerHoofTargetRotation = rightHoofPinchInput ? LowerHoofPinchedRotation : LowerHoofRelaxedRotation;

        LeftArmUpperHoof.localRotation = Quaternion.Lerp(LeftArmUpperHoof.localRotation, Quaternion.Euler(leftUpperHoofTargetRotation), Time.deltaTime * HoofPinchSpeed);
        LeftArmLowerHoof.localRotation = Quaternion.Lerp(LeftArmLowerHoof.localRotation, Quaternion.Euler(leftLowerHoofTargetRotation), Time.deltaTime * HoofPinchSpeed);

        RightArmUpperHoof.localRotation = Quaternion.Lerp(RightArmUpperHoof.localRotation, Quaternion.Euler(rightUpperHoofTargetRotation), Time.deltaTime * HoofPinchSpeed);
        RightArmLowerHoof.localRotation = Quaternion.Lerp(RightArmLowerHoof.localRotation, Quaternion.Euler(rightLowerHoofTargetRotation), Time.deltaTime * HoofPinchSpeed);

        if (!IsLeftStroke && leftHoofPinchInput)
        {
            IsLeftStroke = true;
            LeftStrokeStartPos = LeftArm.position;
        }
        else if (!IsLeftStroke && !leftHoofPinchInput)
        {
            // now you're just playing with it
        }
        else if (IsLeftStroke && leftHoofPinchInput)
        {
            // keep strokin'
        }
        else if (IsLeftStroke && !leftHoofPinchInput)
        {
            IsLeftStroke = false;
            var strokeMagnitude = (LeftArm.position - LeftStrokeStartPos).sqrMagnitude;
            if (LeftArm.position.z > LeftStrokeStartPos.z)
            {
                UpperLeftMilkParticles.Play();
                UpperLeftUdderAudioSource.Play();

                if (currentBucketGameObject.transform.position == MilkCatchingPosition.position)
                {
                    bucketsMilked += milkPerSquirt;
                    var bucketFull = (currentBucketScript.AddPercentFullToBucketReturnNewPercentFull(milkPerSquirt) >= 1f);
                    if (bucketFull)
                    {
                        CreateNewBucket();
                        //RecordBucketFilled();
                        UpdateCurrentBucketsPerMinute();
                    }
                }
            }
        }

        if (!IsRightStroke && rightHoofPinchInput)
        {
            IsRightStroke = true;
            RightStrokeStartPos = RightArm.position;
        }
        else if (!IsRightStroke && !rightHoofPinchInput)
        {
            // now you're just playing with it
        }
        else if (IsRightStroke && rightHoofPinchInput)
        {
            // keep strokin'
        }
        else if (IsRightStroke && !rightHoofPinchInput)
        {
            IsRightStroke = false;
            var strokeMagnitude = (RightArm.position - RightArmStartPosition).sqrMagnitude;
            if (RightArm.position.z > RightStrokeStartPos.z)
            {
                UpperRightUdderAudioSource.Play();
                UpperRightMilkParticles.Play();

                if (currentBucketGameObject.transform.position == MilkCatchingPosition.position)
                {
                    bucketsMilked += milkPerSquirt;
                    var bucketFull = (currentBucketScript.AddPercentFullToBucketReturnNewPercentFull(milkPerSquirt) >= 1f);
                    if (bucketFull)
                    {
                        CreateNewBucket();
                        //RecordBucketFilled();
                        UpdateCurrentBucketsPerMinute();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    GoMilkYourselfInputDTO GetMouseInput() {
        //var mouseDown = Input.GetMouseButton(0);
        //var mouseOnLeftHalfOfScreen = Input.mousePosition.x <= (Screen.width / 2);
        //var mouseVerticalPosition =
        //    ((Input.mousePosition.y / Screen.height) - 0.5f) * 2;

        //var leftArmMousePositionInput = mouseOnLeftHalfOfScreen ? mouseVerticalPosition : 0;
        //var rightArmMousePositionInput = !mouseOnLeftHalfOfScreen ? mouseVerticalPosition : 0;
        //var leftHoofPinchMouseInput = mouseOnLeftHalfOfScreen && mouseDown;
        //var rightHoofPinchMouseInput = !mouseOnLeftHalfOfScreen && mouseDown;

        var leftArmMousePositionInput = 0f;
        var rightArmMousePositionInput = 0f;
        var leftHoofPinchMouseInput = false;
        var rightHoofPinchMouseInput = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray);

        foreach (var hit in hits)
        {
            if (hit.transform.gameObject.name == "TopLeftUdder")
            {
                var colliderCollided = hit.transform.gameObject.GetComponent<BoxCollider>();
                var backMiddle = (colliderCollided.center - new Vector3(0, 0, colliderCollided.size.z / 2));
                var frontMiddle = backMiddle + new Vector3(0, 0, colliderCollided.size.z);

                var hitPositionLocal = hit.transform.InverseTransformPoint(hit.point);
                var calculatedPosition = hit.transform.TransformPoint(backMiddle) + new Vector3(0, 0, hitPositionLocal.z);

                var hitPositionColliderVerticalPercentage = (calculatedPosition - backMiddle).sqrMagnitude / (frontMiddle - backMiddle).sqrMagnitude;
                hitPositionColliderVerticalPercentage *= UdderMouseHoverAdjustmentMultiplier;

                leftArmMousePositionInput = hitPositionColliderVerticalPercentage;
                leftHoofPinchMouseInput = Input.GetMouseButton(0);
            }

            if (hit.transform.gameObject.name == "TopRightUdder")
            {
                var colliderCollided = hit.transform.gameObject.GetComponent<BoxCollider>();
                var backMiddle = (colliderCollided.center - new Vector3(0, 0, colliderCollided.size.z / 2));
                var frontMiddle = backMiddle + new Vector3(0, 0, colliderCollided.size.z);

                var hitPositionLocal = hit.transform.InverseTransformPoint(hit.point);
                var calculatedPosition = hit.transform.TransformPoint(backMiddle) + new Vector3(0, 0, hitPositionLocal.z);

                var hitPositionColliderVerticalPercentage = (calculatedPosition - backMiddle).sqrMagnitude / (frontMiddle - backMiddle).sqrMagnitude;
                hitPositionColliderVerticalPercentage *= UdderMouseHoverAdjustmentMultiplier;

                rightArmMousePositionInput = hitPositionColliderVerticalPercentage;
                rightHoofPinchMouseInput = Input.GetMouseButton(0);
            }
        }

        return new GoMilkYourselfInputDTO
        {
            LeftArmPosition = leftArmMousePositionInput,
            RightArmPosition = rightArmMousePositionInput,
            LeftHoofPinch = leftHoofPinchMouseInput,
            RightHoofPinch = rightHoofPinchMouseInput
        };
    }

    GoMilkYourselfInputDTO GetPs4ControllerInput() {
        var leftArmJoystickPositionInput = (Input.GetAxis("Horizontal") + 1) / 2;
        var rightArmJoystickPositionInput = (Input.GetAxis("Vertical") + 1) / 2;

        var leftHoofPinchJoystickInput = Input.GetKey(KeyCode.JoystickButton6);
        var rightHoofPinchJoystickInput = Input.GetKey(KeyCode.JoystickButton7);

        return new GoMilkYourselfInputDTO
        {
            LeftArmPosition = leftArmJoystickPositionInput,
            RightArmPosition = rightArmJoystickPositionInput,
            LeftHoofPinch = leftHoofPinchJoystickInput,
            RightHoofPinch = rightHoofPinchJoystickInput
        };
    }


    void CreateNewBucket()
    {
        var bucketGameObject = Instantiate(BucketPrefab, MilkCatchingPosition.position, MilkCatchingPosition.rotation);
        currentBucketScript = bucketGameObject.GetComponent<Bucket>();
        currentBucketGameObject = bucketGameObject;
    }

    void RecordBucketFilled()
    {
        bucketsPerMinuteData.Add(new FilledBucketDataPoint { TimeFilled = Time.time });
    }

    void UpdateCurrentBucketsPerMinute()
    {
        //var bucketsFilledOverTheLastMinute = bucketsPerMinuteData.Where((dataPoint) =>
        //{
        //    return dataPoint.TimeFilled > Time.time - 60f;
        //});

        //BucketsPerMinuteText.text = bucketsFilledOverTheLastMinute.Count().ToString();

        var minutesOfGameplay = Time.time / 60;
        var bucketsPerMinute = bucketsMilked / minutesOfGameplay;// # knowing things is for losers

        BucketsPerMinuteText.text = ((int)bucketsPerMinute).ToString();
    }

    IEnumerator ContinuouslyUpdateBucketsPerMinute()
    {
        while (true)
        {
            yield return new WaitForSeconds(BucketsPerMinuteUpdateInterval);
            UpdateCurrentBucketsPerMinute();
        }
    }
}

public class GoMilkYourselfInputDTO {
    public float LeftArmPosition { get; set; }
    public float RightArmPosition { get; set; }

    public bool LeftHoofPinch { get; set; }
    public bool RightHoofPinch { get; set; }
}

public class FilledBucketDataPoint
{
    public float TimeFilled { get; set; }
}
