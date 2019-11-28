using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;
//using System.IO.Ports;

[RequireComponent(typeof(AudioSource))]
public class Pn_Prac : MonoBehaviour
{
    public static Pn_Prac instance;

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

    bool[] curNoteOn = new bool[128];
    int i = 0;
    

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
            curNoteOn[i] = false;
        }

        //These will be fired by the midiSequencer when a song plays.
        midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler(MidiNoteOnHandler);
        midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler(MidiNoteOffHandler);


    }

    void LoadSong(string midiPath)
    {
        midiSequencer.LoadMidi(midiPath, false);
        midiSequencer.Play();
    }


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

        Practice_Piano();
    }

    void Practice_Piano()
    {
        for (i = 0; i < 128; i++)
        {
            if (curNoteOn[i])
                Kbd_PKeyGen.instance.ShowMidiNoteOn(i);
            else
                Kbd_PKeyGen.instance.ShowMidiNoteOff(i);

        }
    }


    public void MidiNoteOnHandler(int channel, int note, int velocity)
    {
        if (channel == midiChannel)
        {
            curNoteOn[note] = true;
        }
    }

    public void MidiNoteOffHandler(int channel, int note)
    {
        if (channel == midiChannel)
        {
            curNoteOn[note] = false;
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

        //StartCoroutine(NotePreview());

        //This uses the Unity specific float method we added to get the buffer
        midiStreamSynthesizer.GetNext(sampleBuffer);

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = sampleBuffer[i] * gain;
        }
    }

    IEnumerator NotePreview()
    {
        yield return new WaitForSeconds(1);
    }



}
