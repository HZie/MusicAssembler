using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Str_UserInput : MonoBehaviour
{
    public InputField if_fretNum, if_strNum;

    // default values given - guitar
    public static int str_fretNum = 10;
    public static int str_strNum = 6;

    public void Decision()
    {
        str_fretNum = Int32.Parse(if_fretNum.text);
        str_strNum = Int32.Parse(if_strNum.text);
    }

}
