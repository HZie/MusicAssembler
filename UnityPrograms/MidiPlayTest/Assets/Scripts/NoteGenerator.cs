using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

public class NoteGenerator : MonoBehaviour
{
    
    public GameObject note1;
    public int objCnt = 0; // obj count

    //private GameObject[] tempObj; // 임시 오브젝트 저장
    private GameObject[] tempObj = new GameObject[256]; // 임시 오브젝트 저장

    private int[] count = new int[128]; // 총 128음 표현가능할것. 표 참고

    public int currentTrack;   // 출력할 악기 트랙
    private float ax, ay;





    void Start()
    {
       // tempObj = new GameObject[256]; 
        currentTrack = 0;
        //Debug.Log(MIDIPlayer.midiSequencer);


      //  MIDIPlayer.midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler(NoteOnHandler);
       // MIDIPlayer.midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler(NoteOffHandler);
    }

    /*
    void Update()
    {
        //Debug.Log("current note " + MIDIPlayer.curNote + "current chan " + MIDIPlayer.curChan);

        if (MIDIPlayer.curChan == currentTrack)
        {
            Debug.Log("current note " + MIDIPlayer.curNote + "current chan " + MIDIPlayer.curChan);
            Debug.Log("current noteOff " + MIDIPlayer.curNoteOff + "current chanOff " + MIDIPlayer.curChanOff);

            Destroy_Obj(MIDIPlayer.curNoteOff);
        }
        
        
    }
    */

    public void NoteOnHandler(int channel, int note, int velocity)
    {
        //if (channel == currentTrack)
        Debug.Log("NoteOn: " + note.ToString() + " channel: " + channel.ToString());
        CalcFret(note);
        Create_Obj(note, ax, ay);
        // Debug.Log("current note " + curNote + "current chan " + curChan);
    }

    public void NoteOffHandler(int channel, int note)
    {
        Debug.Log("NoteOff: " + note.ToString());
       // Destroy_Obj(note);

    }
    
    public void Create_Obj(int i, float x, float y)
    {
        tempObj[i] = Instantiate(note1, new Vector3(x, y, 0), Quaternion.identity);

        //objCnt++;
        // if (objCnt > 256) objCnt %= 256;
    }

    public void Destroy_Obj(int i)
    {
        Destroy(tempObj[i]);
    }
    

    public void CalcFret(int i)
    {
        switch(i)
        {
           // case 64:
           //     ax = -5f;   ay = -2f;   break;
            case 60:
                ax = -7f; ay = -0.5f; break;
            case 52:
                ax = -6f; ay = -1.5f; break;
            case 48:
                ax = -5f; ay = -2f; break;

            default:
                //ax = 0; ay = 0;
                break;
        }
    }



    





            /*
            if (int.Parse(MIDIPlayer.notevalue_on) == 64)
        {
            if(count[64] == 0)
            {
                Create_Obj(64);
                count++;
            }
        }
        if (int.Parse(MIDIPlayer.notevalue_off) == 64)
        {
            if(count == 1)
            {
                Destroy_Obj();
                count--;

            }

        }

        
        if (Input.GetButtonDown("Fire1"))
        {
            Create_Obj();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            Destroy_Obj();
        }*/

        // Debug.Log(MIDIPlayer.notevalue_on);


    
}
