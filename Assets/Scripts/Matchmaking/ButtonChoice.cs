using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChoice : MonoBehaviour
{
    public InputManager myInput { get; set; }
    public void SetButton(InputManager _myInput)
    {
        myInput = _myInput;
    }

    public void ButtonClick(string choice)
    {
        myInput.ChooseRPS(choice);
    }
}
