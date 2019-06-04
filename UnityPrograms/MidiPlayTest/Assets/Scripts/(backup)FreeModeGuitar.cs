using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;
using System.IO.Ports;

[RequireComponent(typeof(AudioSource))]
public class MIDIPlayer : MonoBehaviour
{
    public static MIDIPlayer instance;

    //Public
    //Check the Midi's file folder for different songs
    private string midiFilePath = "Midis/airplane.mid";
    public bool ShouldPlayFile = true;

    //Can do "FM Bank/fm", "Analog Bank/analog", "GM bank/gm"
    private string bankFilePath = "GM Bank/gm";
    public int bufferSize = 1024;
    public int midiNote = 60;   // 음계 바꿀 수 있음
    public int midiNoteVolume = 100;
    [Range(0, 128)] //From Piano to Gunshot
    public int midiInstrument = 24;
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

    int parcesuccess;


    bool stroke_true = false;

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

        }

        sensorInput[40] = 1;
        sensorInput[45] = 1;
        sensorInput[50] = 1;
        sensorInput[55] = 1;
        sensorInput[59] = 1;
        sensorInput[64] = 1;

        //These will be fired by the midiSequencer when a song plays. Check the console for messages if you uncomment these
        //midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler (MidiNoteOnHandler);
        // midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler (MidiNoteOffHandler);			


    }

    void LoadSong(string midiPath)
    {
        midiSequencer.LoadMidi(midiPath, false);
        //midiSequencer.Play();
    }


    // Update is called every frame, if the MonoBehaviour is enabled.
    void Update()
    {/*
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
        }*/



        // Temp keyboard input
        if (Input.GetKeyDown(KeyCode.A))
        {
            tempval[6] = "1";
            
        }
        if (Input.GetKeyUp(KeyCode.A))
            tempval[6] = "0";

        if (Input.GetKeyDown(KeyCode.S))
            tempval[3] = "1";
        if (Input.GetKeyUp(KeyCode.S))
            tempval[3] = "0";

        if (Input.GetKeyDown(KeyCode.D))
            tempval[22] = "1";
        if (Input.GetKeyUp(KeyCode.D))
            tempval[22] = "0";

        for (i = 1; i < 130; i++)
        {
            sensorInput[i] = int.Parse(tempval[i]);
            //print(sensorInput[i]);
        }



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


        for (j=1; j<130; j++)
        {
            if(sensorInput[j] == 1)
            {
                if(inputFlag[j] == 0)
                {
                    //midiStreamSynthesizer.NoteOn(0, j, midiNoteVolume, midiInstrument);
                    //midiStreamSynthesizer.NoteOn(0, j+61, midiNoteVolume+500, midiInstrument);
                    ShowNote.instance.NoteOn(j);
                    Debug.Log("on " + j);
                    inputFlag[j]++;
                }
            }

            if (sensorInput[j] == 0)
            {
                if (inputFlag[j] == 1)
                {
                    //midiStreamSynthesizer.NoteOff(0, j + 61);
                    ShowNote.instance.NoteOff(j);

                    Debug.Log("off " + j);
             
                    inputFlag[j]--;
                }
            }

        }

        
        if(Input.GetKeyDown(KeyCode.Return))
        {
            stroke_true = true;
        }
        if (Input.GetKeyUp(KeyCode.Return))
        {
            stroke_true = false;
        }


        if (stroke_true)
        {
            for (j = 1; j < 130; j++)
            {
                if (sensorInput[j] == 1)
                {
                    if (strokeFlag[j] == 0)
                    {
                        midiStreamSynthesizer.NoteOn(0, j+40, midiNoteVolume, midiInstrument);
                        /*midiStreamSynthesizer.NoteOn(0, 50, midiNoteVolume , midiInstrument);
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
                        //midiStreamSynthesizer.NoteOff(0, j + 40);
                        midiStreamSynthesizer.NoteOff(0, 50);
                        midiStreamSynthesizer.NoteOff(0, 55);
                        midiStreamSynthesizer.NoteOff(0, 59);

                        midiStreamSynthesizer.NoteOff(0, 47);
                        midiStreamSynthesizer.NoteOff(0, 43);
                        midiStreamSynthesizer.NoteOff(0, 67);


                        strokeFlag[j]--;
                    }
                }

            }
        }
        else
        {
            for (j = 1; j < 130; j++)
            {
                if (strokeFlag[j] == 1)
                {
                    //midiStreamSynthesizer.NoteOff(0, j + 40);
                    midiStreamSynthesizer.NoteOff(0, 50);
                    midiStreamSynthesizer.NoteOff(0, 55);
                    midiStreamSynthesizer.NoteOff(0, 59);
                    midiStreamSynthesizer.NoteOff(0, 47);
                    midiStreamSynthesizer.NoteOff(0, 43);
                    midiStreamSynthesizer.NoteOff(0, 67);

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


    /*
    public void MidiNoteOnHandler(int channel, int note, int velocity)
    {
        Debug.Log("NoteOn: " + note.ToString() + " chanel: " + channel.ToString());
        //curNote = (int)midiStreamSynthesizer.CurrentNote;
        //curChan = (int)midiStreamSynthesizer.CurrentChannel;
        Instantiate(note1, new Vector3(-7f, -0.5f, 0), Quaternion.identity);

        // Debug.Log("current note " + curNote + "current chan " + curChan);
    }

    public void MidiNoteOffHandler(int channel, int note)
    {
         Debug.Log("NoteOff: " + note.ToString());

        curNoteOff = (int)midiStreamSynthesizer.CurrentNoteOff;
        curChanOff = (int)midiStreamSynthesizer.CurrentChannelOff;

    }
    */







}
