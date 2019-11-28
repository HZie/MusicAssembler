using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public static ChangeScene instance;

    void Awake()
    {
        if (!instance)  instance = this;
    }

    public void SC_PnFree()
    {
        SceneManager.LoadScene("Pn_FreeMode");
    }
    public void SC_PnPrac()
    {
        SceneManager.LoadScene("3-2 Pn_PracMode");
    }

    public void SC_InstSel()
    {
        SceneManager.LoadScene("2 Inst_Select");

    }

    public void SC_KbdFree()
    {
        SceneManager.LoadScene("5-2 Kbd_FreeMode");
    }
    public void SC_StrFree()
    {
        SceneManager.LoadScene("6-2 Str_FreeMode");
    }

}
