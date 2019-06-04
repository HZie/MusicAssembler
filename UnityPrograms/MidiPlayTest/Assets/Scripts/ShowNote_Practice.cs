using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNote_Practice : MonoBehaviour
{
    public static ShowNote_Practice instance;
    int notes_num = 90;
    public GameObject[] playing_notes = new GameObject[128];
    string num_str = "";



    void Awake()
    {
        if (!instance)
            instance = this;

        for (int i = 40; i<= notes_num; i++)
        {
            try
            {
                num_str = (i).ToString();
                playing_notes[i] = GameObject.Find("mark (" + num_str + ")");
                //Debug.Log("not empty " + num_str);
                //print(playing_notes[i]);
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
                playing_notes[i].SetActive(false);
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
            
            playing_notes[note_num].SetActive(true);
            //Debug.Log("yes " + note_num);
        }
        catch
        {
            //Debug.Log("no " + note_num);
        }
    }


    public void NoteOff(int note_num)
    {
        try
        {
            playing_notes[note_num].SetActive(false);
        }
        catch
        {
            //Debug.Log("empty");
        }
    }

}
