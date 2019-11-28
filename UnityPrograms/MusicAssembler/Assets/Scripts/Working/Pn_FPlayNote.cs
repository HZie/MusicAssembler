using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using System;
using System.IO.Ports;

namespace MidiPlayerTK
{
    public class Pn_FPlayNote : MonoBehaviour
    {

        // MPTK component able to play a stream of midi events
        public MidiStreamPlayer midiStreamPlayer;

        [Range(0.1f, 10f)]
        public float DelayTimeChange = 1;

        [Range(0, 127)]
        public int Velocity = 100;

        [Range(0, 16)]
        public int StreamChannel = 0;

        [Range(0, 127)]
        public int CurrentNote;

        [Range(0, 127)]
        public int CurrentPatchInstrument;

        [Range(0, 127)]
        public int CurrentPatchDrum;

        [Range(0, 127)]
        public int PanChange;

        [Range(0, 127)]
        public int ModChange;

        [Range(0, 127)]
        public int ExpChange;

        /// <summary>
        /// Current note playing
        /// </summary>
        private MPTKEvent NotePlaying;
        private float LastTimeChange;


        // GUI
        private PopupListItem PopPatchInstrument;
        private PopupListItem PopBankInstrument;
        private PopupListItem PopPatchDrum;
        private PopupListItem PopBankDrum;

        public CustomStyle myStyle;
        private Vector2 scrollerWindow = Vector2.zero;
        private int buttonWidth = 250;
        private int sliderwidth = 150;
        private float spaceVertival = 8;
        private float spaceHorizontal = 5;
        private float widthLabel = 120;


        // private vars
        bool[] curNote = new bool[128];
        bool[] pressed = new bool[128];
        int i;

        //Arduino
        string val;
        SerialPort sp = new SerialPort("COM3", 9600);

        void Awake()
        {
            // Arduino
            sp.Open();
            sp.ReadTimeout=1;
        }

        void Start()
        {
            if (midiStreamPlayer != null)
            {
                if (!midiStreamPlayer.OnEventSynthStarted.HasEvent())
                    midiStreamPlayer.OnEventSynthStarted.AddListener(StartLoadingSynth);
                if (!midiStreamPlayer.OnEventSynthStarted.HasEvent())
                    midiStreamPlayer.OnEventSynthStarted.AddListener(EndLoadingSynth);
            }
            else
                Debug.LogWarning("No Stream Midi Player associed to this game object");

            LastTimeChange = Time.realtimeSinceStartup;
            ///midiStreamPlayer.MPTK_Play(new MPTKEvent() { Command = MPTKCommand.PatchChange, Patch = CurrentPatchInstrument, Channel = StreamChannel, });
            PanChange = 64;
            LastTimeChange = -9999999f;

            PopBankInstrument = new PopupListItem() { Title = "Select A Bank", OnSelect = BankPatchChanged, Tag = "BANK_INST", ColCount = 5, ColWidth = 150, };
            PopPatchInstrument = new PopupListItem() { Title = "Select A Patch", OnSelect = BankPatchChanged, Tag = "PATCH_INST", ColCount = 5, ColWidth = 150, };
            PopBankDrum = new PopupListItem() { Title = "Select A Bank", OnSelect = BankPatchChanged, Tag = "BANK_DRUM", ColCount = 5, ColWidth = 150, };
            PopPatchDrum = new PopupListItem() { Title = "Select A Patch", OnSelect = BankPatchChanged, Tag = "PATCH_DRUM", ColCount = 5, ColWidth = 150, };



            for (i = 0; i < 128; i++)
            {
                curNote[i] = false;
                pressed[i] = false;
            }

        }

        /// <summary>
        /// This call is defined from MidiPlayerGlobal event inspector. Run when SF is loaded.
        /// </summary>
        public void EndLoadingSF()
        {
            Debug.Log("End loading SF, MPTK is ready to play");

            //Debug.Log("List of presets available");
            //int i = 0;
            //foreach (string preset in MidiPlayerGlobal.MPTK_ListPreset)
            //    Debug.Log("   " + string.Format("[{0,3:000}] - {1}", i++, preset));
            //i = 0;
            //Debug.Log("List of drums available");
            //foreach (string drum in MidiPlayerGlobal.MPTK_ListDrum)
            //    Debug.Log("   " + string.Format("[{0,3:000}] - {1}", i++, drum));

            Debug.Log("Load statistique");
            Debug.Log("   Time To Load SoundFont: " + Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadSoundFont.TotalSeconds, 3).ToString() + " second");
            Debug.Log("   Time To Load Waves: " + Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadWave.TotalSeconds, 3).ToString() + " second");
            Debug.Log("   Presets Loaded: " + MidiPlayerGlobal.MPTK_CountPresetLoaded);
            Debug.Log("   Waves Loaded: " + MidiPlayerGlobal.MPTK_CountWaveLoaded);

        }
        public void StartLoadingSynth(string name)
        {
            //Debug.LogFormat("Synth {0} loading", name);
        }

        public void EndLoadingSynth(string name)
        {
           // Debug.LogFormat("Synth {0} loaded", name);
            midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent() { Command = MPTKCommand.PatchChange, Value = CurrentPatchInstrument, Channel = StreamChannel, });
        }

        private void BankPatchChanged(object tag, int index)
        {
            switch ((string)tag)
            {
                case "BANK_INST":
                    MidiPlayerGlobal.MPTK_SelectBankInstrument(index);
                    midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent() { Command = MPTKCommand.ControlChange, Controller = MPTKController.BankSelect, Value = index, Channel = StreamChannel, });
                    break;

                case "PATCH_INST":
                    CurrentPatchInstrument = index;
                    midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent() { Command = MPTKCommand.PatchChange, Value = index, Channel = StreamChannel, });
                    break;

                case "BANK_DRUM":
                    MidiPlayerGlobal.MPTK_SelectBankDrum(index);
                    midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent() { Command = MPTKCommand.ControlChange, Controller = MPTKController.BankSelect, Value = index, Channel = 9, });
                    break;

                case "PATCH_DRUM":
                    CurrentPatchDrum = index;
                    midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent() { Command = MPTKCommand.PatchChange, Value = index, Channel = 9 });
                    break;
            }

        }
        

        // Update is called once per frame
        //! [Example MPTK_PlayNotes]
        void Update()
        {
            // Checj that SoundFont is loaded and add a little wait (0.5 s by default) because Unity AudioSource need some time to be started
            if (!MidiPlayerGlobal.MPTK_IsReady())
                return;

            //GetInput();
            GetArduinoInput();
            PlayCheck();
        }
        //! [Example MPTK_PlayNotes]



        private void PlayCheck()
        {
            for(i = 0; i < 128; i++)
            {
                if(curNote[i] == true && pressed[i] == false)
                {
                    PlayOneNote(i);
                    pressed[i] = true;
                    //Kbd_KeyGenByOctave.instance.ShowNoteOn(i);

                    //Kbd_FKeyGen.instance.ShowNoteOn(i);

                }
                else if (pressed[i] == true && curNote[i] == false)
                {
                    StopOneNote(i);
                    pressed[i] = false;
                    //Debug.Log("Stop note " + i);
                    //Kbd_FKeyGen.instance.ShowNoteOff(i);
                    //Kbd_KeyGenByOctave.instance.ShowNoteOff(i);

                }
            }
        }

        private void PlayOneNote(int val)
        {
            // Start playint a new note
            NotePlaying = new MPTKEvent()
            {
                Command = MPTKCommand.NoteOn,
                Value = val,
                Channel = StreamChannel,
                Duration = 9999999, // 9999 seconds but stop by the new note. See before.
                Velocity = Velocity // Sound can vary depending on the velocity
            };
            midiStreamPlayer.MPTK_PlayEvent(NotePlaying);
        }


        private void StopOneNote(int val)
        {
            midiStreamPlayer.MPTK_StopEvent(NotePlaying);
        }


        void GetArduinoInput()
        {
            if(sp.IsOpen)
            {
                try
                {
                    Debug.Log("hi");
                    val = sp.ReadLine();
                    print(val);
                }
                catch(System.Exception) { Debug.Log("no"); }
            }
            if (val.Equals("100"))
            {
                curNote[60] = true;
                //CurrentNote = 60;
                //PlayOneNote();
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                curNote[60] = false;
                //StopOneNote();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                curNote[62] = true;

                //CurrentNote = 62;
                //PlayOneNote();
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                curNote[62] = false;
                // pressed[62] = false;

                //StopOneNote();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                curNote[64] = true;
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                curNote[64] = false;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                curNote[65] = true;
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                curNote[65] = false;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                curNote[67] = true;
            }
            if (Input.GetKeyUp(KeyCode.G))
            {
                curNote[67] = false;
            }
        }


        void GetInput()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                curNote[60] = true;
                //CurrentNote = 60;
                //PlayOneNote();
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                curNote[60] = false;
                //StopOneNote();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                curNote[62] = true;

                //CurrentNote = 62;
                //PlayOneNote();
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                curNote[62] = false;
                // pressed[62] = false;

                //StopOneNote();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                curNote[64] = true;
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                curNote[64] = false;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                curNote[65] = true;
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                curNote[65] = false;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                curNote[67] = true;
            }
            if (Input.GetKeyUp(KeyCode.G))
            {
                curNote[67] = false;
            }

        }

        /// <summary>
        /// Helper to create a note 
        /// </summary>
        /// <returns></returns>
        private MPTKEvent CreateNote(int key, float delay)
        {
            MPTKEvent note = new MPTKEvent()
            {
                Command = MPTKCommand.NoteOn,
                Value = key,
                Duration = DelayTimeChange * 1000f,
                Velocity = Velocity,
            };
            return note;
        }


        void OnGUI()
        {
            // Set custom Style. Good for background color 3E619800
            if (myStyle == null) myStyle = new CustomStyle();

            if (midiStreamPlayer != null)
            {
                scrollerWindow = GUILayout.BeginScrollView(scrollerWindow, false, false, GUILayout.Width(Screen.width));

                // If need, display the popup  before any other UI to avoid trigger it hidden
                PopBankInstrument.Draw(MidiPlayerGlobal.MPTK_ListBank, MidiPlayerGlobal.ImSFCurrent.DefaultBankNumber, myStyle);
                PopPatchInstrument.Draw(MidiPlayerGlobal.MPTK_ListPreset, CurrentPatchInstrument, myStyle);
                PopBankDrum.Draw(MidiPlayerGlobal.MPTK_ListBank, MidiPlayerGlobal.ImSFCurrent.DrumKitBankNumber, myStyle);
                PopPatchDrum.Draw(MidiPlayerGlobal.MPTK_ListPresetDrum, CurrentPatchDrum, myStyle);

                // MainMenu.Display("Test Midi Stream - A very simple Generated Music Stream ", myStyle);

                // Display soundfont available and select a new one
                //GUISelectSoundFont.Display(scrollerWindow, myStyle);

                // Select bank & Patch for Instrument
                // ----------------------------------
                //GUILayout.Space(spaceVertival);
                //GUILayout.Space(spaceVertival);
                GUILayout.BeginVertical(myStyle.BacgDemos);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Instrument: ", myStyle.TitleLabel3, GUILayout.Width(widthLabel));

                // Open the popup to select a bank
                if (GUILayout.Button(MidiPlayerGlobal.ImSFCurrent.DefaultBankNumber + " - Bank", GUILayout.Width(buttonWidth)))
                    PopBankInstrument.Show = !PopBankInstrument.Show;
                PopBankInstrument.Position(ref scrollerWindow);

                // Open the popup to select an instrument
                if (GUILayout.Button(
                    CurrentPatchInstrument.ToString() + " - " +
                    MidiPlayerGlobal.MPTK_GetPatchName(MidiPlayerGlobal.ImSFCurrent.DefaultBankNumber,
                    CurrentPatchInstrument),
                    GUILayout.Width(buttonWidth)))
                    PopPatchInstrument.Show = !PopPatchInstrument.Show;
                PopPatchInstrument.Position(ref scrollerWindow);

                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Space(spaceVertival);
                GUILayout.Label("MidiStreamPlayer not defined, check hierarchy.", myStyle.TitleLabel3);
            }

        }
    }
}