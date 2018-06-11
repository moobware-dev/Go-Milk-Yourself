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
    public float InGameMilkingSpeed = 15f;
    public float MilkingSpeedDelay = 1f;
    public float HoofPinchSpeed = 15f;
    private bool inPosition = false;

    public float BucketsPerMinuteUpdateInterval = 0.3f;

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
        var mouseDown = Input.GetMouseButton(0);
        var mouseOnLeftHalfOfScreen = Input.mousePosition.x <= (Screen.width / 2);
        var mouseVerticalPosition =
            ((Input.mousePosition.y / Screen.height) - 0.5f) * 2;

        var leftArmMousePositionInput = mouseOnLeftHalfOfScreen ? mouseVerticalPosition : 0;
        var rightArmMousePositionInput = !mouseOnLeftHalfOfScreen ? mouseVerticalPosition : 0;
        var leftHoofPinchMouseInput = mouseOnLeftHalfOfScreen && mouseDown;
        var rightHoofPinchMouseInput = !mouseOnLeftHalfOfScreen && mouseDown;

        var leftArmJoystickPositionInput = (Input.GetAxis("Horizontal") + 1) / 2;
        var rightArmJoystickPositionInput = (Input.GetAxis("Vertical") + 1) / 2;

        var leftArmTargetPosition = Vector3.Lerp(MilkUpperLeftBasePosition, MilkUpperLeftJustTheTipPosition, leftArmJoystickPositionInput);
        var rightArmTargetPosition = Vector3.Lerp(MilkUpperRightBasePosition, MilkUpperRightJustTheTipPosition, rightArmJoystickPositionInput);

        var leftHoofPinchJoystickInput = Input.GetKey(KeyCode.JoystickButton6);
        var rightHoofPinchJoystickInput = Input.GetKey(KeyCode.JoystickButton7);

        if (Time.time > SecondsToEaseIntoIt)
        {
            inPosition = true;
        }

        var movmentSpeed = inPosition ? InGameMilkingSpeed : AssumeThePositionSpeed;

        LeftArm.position = Vector3.Lerp(LeftArm.position, leftArmTargetPosition, Time.deltaTime * movmentSpeed);
        RightArm.position = Vector3.Lerp(RightArm.position, rightArmTargetPosition, Time.deltaTime * movmentSpeed);

        var leftUpperHoofTargetRotation = leftHoofPinchJoystickInput ? UpperHoofPinchedRotation : UpperHoofRelaxedRotation;
        var leftLowerHoofTargetRotation = leftHoofPinchJoystickInput ? LowerHoofPinchedRotation : LowerHoofRelaxedRotation;

        var rightUpperHoofTargetRotation = rightHoofPinchJoystickInput ? UpperHoofPinchedRotation : UpperHoofRelaxedRotation;
        var rightLowerHoofTargetRotation = rightHoofPinchJoystickInput ? LowerHoofPinchedRotation : LowerHoofRelaxedRotation;

        LeftArmUpperHoof.localRotation = Quaternion.Lerp(LeftArmUpperHoof.localRotation, Quaternion.Euler(leftUpperHoofTargetRotation), Time.deltaTime * HoofPinchSpeed);
        LeftArmLowerHoof.localRotation = Quaternion.Lerp(LeftArmLowerHoof.localRotation, Quaternion.Euler(leftLowerHoofTargetRotation), Time.deltaTime * HoofPinchSpeed);

        RightArmUpperHoof.localRotation = Quaternion.Lerp(RightArmUpperHoof.localRotation, Quaternion.Euler(rightUpperHoofTargetRotation), Time.deltaTime * HoofPinchSpeed);
        RightArmLowerHoof.localRotation = Quaternion.Lerp(RightArmLowerHoof.localRotation, Quaternion.Euler(rightLowerHoofTargetRotation), Time.deltaTime * HoofPinchSpeed);

        if (!IsLeftStroke && leftHoofPinchJoystickInput)
        {
            IsLeftStroke = true;
            LeftStrokeStartPos = LeftArm.position;
        }
        else if (!IsLeftStroke && !leftHoofPinchJoystickInput)
        {
            // now you're just playing with it
        }
        else if (IsLeftStroke && leftHoofPinchJoystickInput)
        {
            // keep strokin'
        }
        else if (IsLeftStroke && !leftHoofPinchJoystickInput)
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

        if (!IsRightStroke && rightHoofPinchJoystickInput)
        {
            IsRightStroke = true;
            RightStrokeStartPos = RightArm.position;
        }
        else if (!IsRightStroke && !rightHoofPinchJoystickInput)
        {
            // now you're just playing with it
        }
        else if (IsRightStroke && rightHoofPinchJoystickInput)
        {
            // keep strokin'
        }
        else if (IsRightStroke && !rightHoofPinchJoystickInput)
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

public class FilledBucketDataPoint
{
    public float TimeFilled { get; set; }
}
