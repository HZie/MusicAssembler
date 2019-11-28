using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;
//using System.IO.Ports;

[RequireComponent(typeof(AudioSource))]
public class Pn_Free : MonoBehaviour
{
    public static Pn_Free instance;

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
    public int midiInstrument = 0; //Piano
    public int midiChannel = 0;

    //Private 
    private float[] sampleBuffer;
    private float gain = 1f;
    public static MidiSequencer midiSequencer;
    private StreamSynthesizer midiStreamSynthesizer;

    private float sliderValue = 1.0f;
    private float maxSliderValue = 127.0f;

    //Arduino
    //SerialPort sp = new SerialPort("COM3", 9600);

    private int i = 0, j = 0;
    int[] currentNote = new int [128];
    int[] pressed = new int[128];



    void Awake()
    {
        if (!instance)  instance = this;

        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];
        midiStreamSynthesizer.LoadBank(bankFilePath);
        midiSequencer = new MidiSequencer(midiStreamSynthesizer);

        // Arduino
        //sp.Open();
        //sp.ReadTimeout=1;
  
        for (i = 0; i < 128; i++)
        {
            currentNote[i] = 0;
            pressed[i] = 0;
        }

        //Debug.Log(Kbd_UserInput.kbd_num);

        //These will be fired by the midiSequencer when a song plays
        midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler (MidiNoteOnHandler);
        midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler (MidiNoteOffHandler);			
    }

    /*
    void Update()
    {
        FreeMode_Piano();
    }*/


    void GetInput()
    {
        // C4
        if (Input.GetKeyDown(KeyCode.A))
            currentNote[60] = 1;
        if (Input.GetKeyUp(KeyCode.A))
            currentNote[60] = 0;

        if (Input.GetKeyDown(KeyCode.W))
            currentNote[61] = 1;
        if (Input.GetKeyUp(KeyCode.W))
            currentNote[61] = 0;

        // D4
        if (Input.GetKeyDown(KeyCode.S))
            currentNote[62] = 1;
        if (Input.GetKeyUp(KeyCode.S))
            currentNote[62] = 0;
        if (Input.GetKeyDown(KeyCode.E))
            currentNote[63] = 1;
        if (Input.GetKeyUp(KeyCode.E))
            currentNote[63] = 0;
        // E4
        if (Input.GetKeyDown(KeyCode.D))
            currentNote[64] = 1;
        if (Input.GetKeyUp(KeyCode.D))
            currentNote[64] = 0;

        if (Input.GetKeyDown(KeyCode.F))
            currentNote[65] = 1;
        if (Input.GetKeyUp(KeyCode.F))
            currentNote[65] = 0;

        if (Input.GetKeyDown(KeyCode.T))
            currentNote[66] = 1;
        if (Input.GetKeyUp(KeyCode.T))
            currentNote[66] = 0;

        if (Input.GetKeyDown(KeyCode.G))
            currentNote[67] = 1;
        if (Input.GetKeyUp(KeyCode.G))
            currentNote[67] = 0;

        if (Input.GetKeyDown(KeyCode.Y))
            currentNote[68] = 1;
        if (Input.GetKeyUp(KeyCode.Y))
            currentNote[68] = 0;

        if (Input.GetKeyDown(KeyCode.H))
            currentNote[69] = 1;
        if (Input.GetKeyUp(KeyCode.H))
            currentNote[69] = 0;

    }

    public void FreeMode_Piano()
    {
        GetInput();

        for (i = 0; i < 128; i++)
        {
            if (currentNote[i] == 1 && pressed[i] == 0)
            {
               // MidiNoteOnHandler(midiChannel, i, midiNoteVolume);
                pressed[i] = 1;
                Pn_FKeyGen.instance.ShowNoteOn(i);
            }
            else if (currentNote[i] == 0)
            {
               // MidiNoteOffHandler(midiChannel, i);
                pressed[i] = 0;
                Pn_FKeyGen.instance.ShowNoteOff(i);

            }
        }
    }


    // note on/off event에 사용되는 함수들
    public void MidiNoteOnHandler(int channel, int note, int velocity)
    {
        midiStreamSynthesizer.NoteOn(channel, note, velocity, midiInstrument);
    }

    public void MidiNoteOffHandler(int channel, int note)
    {
        midiStreamSynthesizer.NoteOff(channel, note);
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
    //  소리 재생 함수
    private void OnAudioFilterRead(float[] data, int channels)
    {
        //This uses the Unity specific float method we added to get the buffer
        midiStreamSynthesizer.GetNext(sampleBuffer);

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = sampleBuffer[i] * gain;
        }
    }
    
}
