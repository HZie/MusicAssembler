using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;
using System.IO.Ports;

[RequireComponent(typeof(AudioSource))]
public class PracticeModeGuitar : MonoBehaviour
{
    public static PracticeModeGuitar instance;

    //Public
    //Check the Midi's file folder for different songs
    private string midiFilePath = "Midis/N64ZeldaOcarinaEnding.mid";
    public bool ShouldPlayFile = true;

    //Can do "FM Bank/fm", "Analog Bank/analog", "GM bank/gm"
    private string bankFilePath = "GM Bank/gm";
    public int bufferSize = 1024;
    public int midiNote = 60;   // 음계 바꿀 수 있음
    public int midiNoteVolume = 100;
    [Range(0, 128)] //From Piano to Gunshot
    public int midiInstrument = 24; // Nylon Guitar

    //Private 
    private float[] sampleBuffer;
    private float gain = 1f;
    public static MidiSequencer midiSequencer;
    private StreamSynthesizer midiStreamSynthesizer;

    private float sliderValue = 1.0f;
    private float maxSliderValue = 127.0f;
    
    //Arduino
    //SerialPort sp = new SerialPort("COM3", 9600);
    string val="";
    private string[] tempval = new string[130]; 
    private int[] sensorInput = new int[130];
    private int[] inputFlag = new int[130];
    private int[] strokeFlag = new int[130];

    private int i = 0, j=0;

    // default 
    int[] defGtrLine = new int[7];
    bool[] defGtr_isEmpty = new bool[7];

    bool stroke_true = false;

    bool[] curNoteOn = new bool[130];
    //bool[] curNoteOff = new bool[130];







    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        if (!instance)
            instance = this;

        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];
        
        midiStreamSynthesizer.LoadBank(bankFilePath);

        midiSequencer = new MidiSequencer(midiStreamSynthesizer);

        // Arduino
        //sp.Open();
        //sp.ReadTimeout=1;

        for(i=0; i< 130; i++)
        {
            tempval[i] = "0";
            sensorInput[i] = 0;
            inputFlag[i] = 0;
            strokeFlag[i] = 0;
            curNoteOn[i] = false;
            //curNoteOff[i] = false;

        }

        for (i = 1; i < 7; i++)
        {
            defGtr_isEmpty[i] = true;
        }

        defGtrLine[1] = 40;
        defGtrLine[2] = 45;
        defGtrLine[3] = 50;
        defGtrLine[4] = 55;
        defGtrLine[5] = 59;
        defGtrLine[6] = 64;

        sensorInput[40] = 1;
        sensorInput[45] = 1;
        sensorInput[50] = 1;
        sensorInput[55] = 1;
        sensorInput[59] = 1;
        sensorInput[64] = 1;

        //These will be fired by the midiSequencer when a song plays. Check the console for messages if you uncomment these
        midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler (MidiNoteOnHandler);
        midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler (MidiNoteOffHandler);			


    }

    void LoadSong(string midiPath)
    {
        midiSequencer.LoadMidi(midiPath, false);
        midiSequencer.Play();
    }


    // Update is called every frame, if the MonoBehaviour is enabled.
    void Update()
    {
        if (!midiSequencer.isPlaying)
        {
            //if (!GetComponent<AudioSource>().isPlaying)
            if (ShouldPlayFile)
            {  
                LoadSong(midiFilePath);
            }
        }
        else if (!ShouldPlayFile)
        {
            midiSequencer.Stop(true);
        }

        // Free_Guitar();
        Practice_Guitar();
    }









    void Free_Str_All_On()
    {
        sensorInput[defGtrLine[1]] = 1;
        sensorInput[defGtrLine[2]] = 1;
        sensorInput[defGtrLine[3]] = 1;
        sensorInput[defGtrLine[4]] = 1;
        sensorInput[defGtrLine[5]] = 1;
        sensorInput[defGtrLine[6]] = 1;

    }

    void Check_Free_Str(int strNum)
    {

        if (strNum > 40 && strNum < 45)
        {
            sensorInput[defGtrLine[1]] = 0;
        }
        else if (strNum > 45 && strNum < 50)
        {
            sensorInput[defGtrLine[2]] = 0;
        }
        else if (strNum > 50 && strNum < 55)
        {
            sensorInput[defGtrLine[3]] = 0;
        }
        else if (strNum > 55 && strNum < 59)
        {
            sensorInput[defGtrLine[4]] = 0;
        }
        else if (strNum > 59 && strNum < 56)
        {
            sensorInput[defGtrLine[5]] = 0;
        }
        else if (strNum > 64)
        {
            sensorInput[defGtrLine[6]] = 0;
        }
    }

    void Free_Guitar()
    {
        // Temp keyboard input
        if (Input.GetKeyDown(KeyCode.A))
        {
            tempval[43] = "1";

        }
        if (Input.GetKeyUp(KeyCode.A))
            tempval[43] = "0";

        if (Input.GetKeyDown(KeyCode.S))
            tempval[47] = "1";
        if (Input.GetKeyUp(KeyCode.S))
            tempval[47] = "0";

        if (Input.GetKeyDown(KeyCode.D))
            tempval[67] = "1";
        if (Input.GetKeyUp(KeyCode.D))
            tempval[67] = "0";


        for (i = 40; i < 130; i++)
        {
            sensorInput[i] = int.Parse(tempval[i]);

            //print(sensorInput[i]);
        }
        /*Free_Str_All_On();

        for (i = 40; i < 130; i++)
        {
            if (sensorInput[i] == 1)
                Check_Free_Str(i);
        }*/

        // Arduino
        /*
        if (sp.IsOpen)
        {
            try
            {
                val = sp.ReadLine();
                //print(val);
                tempval = val.Split(' ');

                for(i = 0; i<130; i++)
                {
                    sensorInput[i] = int.Parse(tempval[i]);
                }

            }
            catch (System.Exception)
            {

            }
        }*/



        for (j = 40; j < 130; j++)
        {
            if (sensorInput[j] == 1)
            {
                if (inputFlag[j] == 0)
                {
                    ShowNote.instance.NoteOn(j);
                    Debug.Log("on " + j);
                    inputFlag[j]++;
                }
            }

            if (sensorInput[j] == 0)
            {
                if (inputFlag[j] == 1)
                {
                    ShowNote.instance.NoteOff(j);
                    Debug.Log("off " + j);
                    inputFlag[j]--;
                }
            }

        }


        // Arduino input 받아올때 맨 처음에 받아오는 값이 stroke이도록 (나중에 각 현 입력으로 바꾸기)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            sensorInput[0] = 1;

        }
        if (Input.GetKeyUp(KeyCode.Return))
        {
            sensorInput[0] = 0;
        }


        if (sensorInput[0] == 1)
        {
            stroke_true = true;
        }
        if (sensorInput[0] == 0)
        {
            stroke_true = false;
        }

        /*
        if(Input.GetKeyDown(KeyCode.Return))
        {
            stroke_true = true;
        }
        if (Input.GetKeyUp(KeyCode.Return))
        {
            stroke_true = false;
        }
        */

        if (stroke_true)
        {
            for (j = 40; j < 130; j++)
            {
                if (sensorInput[j] == 1)
                {
                    if (strokeFlag[j] == 0)
                    {
                        midiStreamSynthesizer.NoteOn(0, j, midiNoteVolume, midiInstrument);
                       /* midiStreamSynthesizer.NoteOn(0, 50, midiNoteVolume , midiInstrument);
                        midiStreamSynthesizer.NoteOn(0, 55, midiNoteVolume, midiInstrument);
                        midiStreamSynthesizer.NoteOn(0, 59, midiNoteVolume , midiInstrument);

                        midiStreamSynthesizer.NoteOn(0, 47, midiNoteVolume, midiInstrument);
                        midiStreamSynthesizer.NoteOn(0, 43, midiNoteVolume, midiInstrument);
                        midiStreamSynthesizer.NoteOn(0, 67, midiNoteVolume, midiInstrument);
                        */

                        strokeFlag[j]++;
                    }
                }

                if (sensorInput[j] == 0)
                {
                    if (strokeFlag[j] == 1)
                    {
                          midiStreamSynthesizer.NoteOff(0, j);
                       /*   midiStreamSynthesizer.NoteOff(0, 50);
                          midiStreamSynthesizer.NoteOff(0, 55);
                          midiStreamSynthesizer.NoteOff(0, 59);

                          midiStreamSynthesizer.NoteOff(0, 47);
                          midiStreamSynthesizer.NoteOff(0, 43);
                          midiStreamSynthesizer.NoteOff(0, 67);*/

                      
                        strokeFlag[j]--;
                    }
                }

            }
        }
        else
        {
            for (j = 40; j < 130; j++)
            {
                if (strokeFlag[j] == 1)
                {
                    midiStreamSynthesizer.NoteOff(0, j);
                    /*midiStreamSynthesizer.NoteOff(0, 50);
                    midiStreamSynthesizer.NoteOff(0, 55);
                    midiStreamSynthesizer.NoteOff(0, 59);
                    midiStreamSynthesizer.NoteOff(0, 47);
                    midiStreamSynthesizer.NoteOff(0, 43);
                    midiStreamSynthesizer.NoteOff(0, 67);*/

                    strokeFlag[j]--;
                }

            }
        }
    }








    // See http://unity3d.com/support/documentation/ScriptReference/MonoBehaviour.OnAudioFilterRead.html for reference code
    //	If OnAudioFilterRead is implemented, Unity will insert a custom filter into the audio DSP chain.
    //
    //	The filter is inserted in the same order as the MonoBehaviour script is shown in the inspector. 	
    //	OnAudioFilterRead is called everytime a chunk of audio is routed thru the filter (this happens frequently, every ~20ms depending on the samplerate and platform). 
    //	The audio data is an array of floats ranging from [-1.0f;1.0f] and contains audio from the previous filter in the chain or the AudioClip on the AudioSource. 
    //	If this is the first filter in the chain and a clip isn't attached to the audio source this filter will be 'played'. 
    //	That way you can use the filter as the audio clip, procedurally generating audio.
    //
    //	If OnAudioFilterRead is implemented a VU meter will show up in the inspector showing the outgoing samples level. 
    //	The process time of the filter is also measured and the spent milliseconds will show up next to the VU Meter 
    //	(it turns red if the filter is taking up too much time, so the mixer will starv audio data). 
    //	Also note, that OnAudioFilterRead is called on a different thread from the main thread (namely the audio thread) 
    //	so calling into many Unity functions from this function is not allowed ( a warning will show up ). 	
    private void OnAudioFilterRead(float[] data, int channels)
    {
        //This uses the Unity specific float method we added to get the buffer
        midiStreamSynthesizer.GetNext(sampleBuffer);

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = sampleBuffer[i] * gain;
        }
    }


    void Practice_Guitar()
    {
        for(i=40; i< 130; i++)
        {
            if (curNoteOn[i])
                ShowNote_Practice.instance.NoteOn(i);
            else
                ShowNote_Practice.instance.NoteOff(i);

        }
    }

    
    public void MidiNoteOnHandler(int channel, int note, int velocity)
    {
        if(channel == 0)
        {
            curNoteOn[note] = true;
            //ShowNote_Practice.instance.NoteOn(note);
        }
        //curNote = (int)midiStreamSynthesizer.CurrentNote;
        //curChan = (int)midiStreamSynthesizer.CurrentChannel;
        // Debug.Log("current note " + curNote + "current chan " + curChan);
        Debug.Log("NoteOn: " + note.ToString() + " chanel: " + channel.ToString());

    }

    public void MidiNoteOffHandler(int channel, int note)
    {

        if (channel == 0)
        {
            curNoteOn[note] = false;

            //ShowNote_Practice.instance.NoteOff(note);
            //Debug.Log("NoteOff: " + note.ToString());

        }
        //curNoteOff = (int)midiStreamSynthesizer.CurrentNoteOff;
        //curChanOff = (int)midiStreamSynthesizer.CurrentChannelOff;

    }
    







}
