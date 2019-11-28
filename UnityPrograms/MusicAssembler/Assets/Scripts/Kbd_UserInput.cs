using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kbd_UserInput : MonoBehaviour
{
    public InputField if_minOct,if_numOct;

    // default values given - piano
    public static int kbd_minOct = 0;
    public static int kbd_numOct = 9; 

    public static int kbd_minNote = 33;
    public static int kbd_num = 88; 

    public void Decision()
    {
        kbd_minOct = Int32.Parse(if_minOct.text);
        kbd_numOct = Int32.Parse(if_numOct.text);
    }

}
