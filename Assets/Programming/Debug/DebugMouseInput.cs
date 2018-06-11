using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMouseInput : MonoBehaviour {

    public Button MouseDown;

    public Slider MousePositionLeftHalfOfScreen;
    public Slider MousePositionRightHalfOfScreen;

    public Color ButtonHeldColor;
    public Color ButtonNeutralColor;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var mouseOnLeftHalfOfScreen = Input.mousePosition.x <= (Screen.width / 2);
        var mouseVerticalPosition = 
            ((Input.mousePosition.y / Screen.height) - 0.5f) * 2;

        MousePositionLeftHalfOfScreen.value = mouseOnLeftHalfOfScreen ? mouseVerticalPosition : 0;

        MousePositionRightHalfOfScreen.value = !mouseOnLeftHalfOfScreen ? mouseVerticalPosition: 0;

        var mouseDown = Input.GetMouseButton(0);

        MouseDown.image.color = mouseDown ? ButtonHeldColor : ButtonNeutralColor;
    }
}
