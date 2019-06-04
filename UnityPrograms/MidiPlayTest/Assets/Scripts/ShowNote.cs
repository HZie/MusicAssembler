using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNote : MonoBehaviour
{
    public static ShowNote instance;
    int notes_num = 90;
    GameObject[] notes = new GameObject[128];
    string num_str = "";


    void Awake()
    {
        if (!instance)
            instance = this;
    }



    void Start()
    {
        for(int i = 40; i<= notes_num; i++)
        {
            try
            {
                num_str = (i).ToString();
                notes[i] = GameObject.Find("mark (" + num_str + ")");
            }
            catch
            {
                //Debug.Log("empty "+num_str);

            }
        }

        AllNoteOff();
    }




    void AllNoteOff()
    {
        for (int i = 40; i <= notes_num; i++)
        {
            try
            {
                notes[i].SetActive(false);
            }
            catch
            {
                //Debug.Log("empty");
            }
        }
    }

    public void NoteOn(int note_num)
    {
        try
        {
            notes[note_num].SetActive(true);
        }
        catch
        {
            //Debug.Log("empty");
        }
    }


    public void NoteOff(int note_num)
    {
        try
        {
            notes[note_num].SetActive(false);
        }
        catch
        {
            //Debug.Log("empty");
        }
    }

}
