using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InstSelect : MonoBehaviour
{
    //[Header("Dropdown")]
    public Dropdown dropdown;


    public void ClickButton()
    {
        //Debug.Log("val " + dropdown.value);

        switch (dropdown.value)
        {
            case 0: 
                SceneManager.LoadScene("3-1 Pn_FreeMode");
                break;
            case 1:
                SceneManager.LoadScene("3-2 Pn_PracMode");
                break;
            case 2:
                SceneManager.LoadScene("4-1 Gt_FreeMode");
                break;
            case 3:
                SceneManager.LoadScene("4-2 Gt_PracMode");
                break;
            case 4:
                SceneManager.LoadScene("5-1 kbd_UserInput");
                break;
            case 5:
                SceneManager.LoadScene("6-1 str_UserInput");
                break;
        }
    }

}
