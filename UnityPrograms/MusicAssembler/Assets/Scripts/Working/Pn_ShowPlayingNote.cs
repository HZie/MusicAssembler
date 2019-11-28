using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pn_ShowPlayingNote : MonoBehaviour
{
    public static Pn_ShowPlayingNote instance;
    int notes_num = 128;

    GameObject[] notes = new GameObject[128];
    
    void Awake()
    {
        if (!instance)  instance = this;
    }
        
    void Start()
    {
        for (int i = 0; i < notes_num; i++)
        {
            try
            {
                notes[i] = GameObject.Find("mark (" + i + ")");
            }
            catch {  }
        }

        AllNoteOff();
    }




    void AllNoteOff()
    {
        for (int i = 0; i < notes_num; i++)
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

    public void ShowNoteOn(int note_num)
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


    public void ShowNoteOff(int note_num)
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
