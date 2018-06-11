using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPs4Controller : MonoBehaviour
{
    public Button L2_button;
    public Button L1_button;

    public Button R2_button;
    public Button R1_button;

    public Slider LeftJoystick_slider;
    public Slider RightJoystick_slider;

    public Color ButtonHeldColor;
    public Color ButtonNeutralColor;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var leftJoyStickUpDown = Input.GetAxis("Horizontal");
        //Debug.Log("Left joystick input: " + leftJoyStickUpDown);
        LeftJoystick_slider.value = leftJoyStickUpDown;

        var rightJoyStickUpDown = Input.GetAxis("Vertical");
        //Debug.Log("Right Joystick input: " + rightJoyStickUpDown);
        RightJoystick_slider.value = rightJoyStickUpDown;

        // L1 = joystick button 4
        // R1 = joystick button 5
        // L2 = joystick button 6
        // R2 = joystick button 7
        var L1_heldDown = Input.GetKey(KeyCode.JoystickButton4);
        var R1_heldDown = Input.GetKey(KeyCode.JoystickButton5);
        var L2_heldDown = Input.GetKey(KeyCode.JoystickButton6);
        var R2_heldDown = Input.GetKey(KeyCode.JoystickButton7);

        L1_button.image.color = L1_heldDown ? ButtonHeldColor : ButtonNeutralColor;
        R1_button.image.color = R1_heldDown ? ButtonHeldColor : ButtonNeutralColor;
        L2_button.image.color = L2_heldDown ? ButtonHeldColor : ButtonNeutralColor;
        R2_button.image.color = R2_heldDown ? ButtonHeldColor : ButtonNeutralColor;
    }
}
