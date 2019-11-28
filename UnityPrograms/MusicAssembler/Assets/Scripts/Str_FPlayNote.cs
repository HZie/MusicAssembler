using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using System;
using System.IO.Ports;

namespace MidiPlayerTK
{
    public class Str_FPlayNote : MonoBehaviour
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

        //[Range(0, 127)]
        int CurrentPatchInstrument = 24;

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
        private MPTKEvent[] NotePlaying = new MPTKEvent[128];
        private float LastTimeChange;

        // private vars
        bool[] curNote = new bool[128];
        bool[] pressed = new bool[128];
        bool[] played = new bool[128];  // check if each string is played or not
        bool[] playedPressed = new bool[128];

        int strNum = Str_UserInput.str_strNum;  // user's pick of one line 
        int hwStrNum = 6;
        int sensorPerStr;   // how many lines of sensor?

        int curSensorPerStr = 0;    // now how many?
        int i;
        //int minNote;



        /// <summary>
        /// Popup to select an instrument
        /// </summary>
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


        //Arduino
        string val;
        SerialPort sp = new SerialPort("COM3", 9600);

        int[] numSensorArray = new int[256];  // number of sensors, element will show which key
        int totalSensorNum;  // number of hw sensor in plate used


        void Awake()
        {
            // Arduino
            sp.Open();
            sp.ReadTimeout = 60;    // need for arduino time match
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
                played[i] = false;
                playedPressed[i] = false;
            }

            totalSensorNum = 24;
            for (i = 0; i <= totalSensorNum; i++)
            {
                numSensorArray[i] = 0;
            }
            SetSensorNum();
            sensorPerStr = totalSensorNum / hwStrNum;
           // sensorPerStr = 4;
           // Debug.Log(sensorPerStr);


        }


        void SetSensorNum()
        {
            numSensorArray[0] = 0;
            numSensorArray[1] = 6;
            numSensorArray[2] = 12;
            numSensorArray[3] = 3;
            numSensorArray[4] = 9;
            numSensorArray[5] = 15;
            numSensorArray[6] = 1;
            numSensorArray[7] = 7;
            numSensorArray[8] = 13;
            numSensorArray[9] = 4;
            numSensorArray[10] = 10;
            numSensorArray[11] = 16;
            numSensorArray[12] = 2;
            numSensorArray[13] = 8;
            numSensorArray[14] = 14;
            numSensorArray[15] = 11;
            numSensorArray[16] = 5;
            numSensorArray[17] = 17;

            numSensorArray[18] = 18;
            numSensorArray[19] = 19;
            numSensorArray[20] = 20;
            numSensorArray[21] = 21;
            numSensorArray[22] = 22;
            numSensorArray[23] = 23;


            /*numSensorArray[0] = 0;
            numSensorArray[1] = 2;
            numSensorArray[2] = 4;
            numSensorArray[3] = 1;
            numSensorArray[4] = 3;
            numSensorArray[5] = 5;
            numSensorArray[6] = 6;
            numSensorArray[7] = 8;
            numSensorArray[8] = 10;
            numSensorArray[9] = 7;
            numSensorArray[10] = 9;
            numSensorArray[11] = 11;
            numSensorArray[12] = 12;
            numSensorArray[13] = 14;
            numSensorArray[14] = 16;
            numSensorArray[15] = 13;
            numSensorArray[16] = 15;
            numSensorArray[17] = 17;
            numSensorArray[18] = 18;
            numSensorArray[19] = 19;
            numSensorArray[20] = 20;
            numSensorArray[21] = 21;
            numSensorArray[22] = 22;
            numSensorArray[23] = 23;*/
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
            GetArduinoInput();  // check for input
            PlayCheck();        // make sound for each str
        }
        //! [Example MPTK_PlayNotes]



        private void PlayCheck()
        {
            /// showing notes
            for (i = 0; i < 128; i++)
            {
                //Debug.Log(i + " playcheck " + curNote[i]);

                if (curNote[i] == true && pressed[i] == false)
                {
                    pressed[i] = true;
                    Str_FKeyGen.instance.ShowNoteOn(i);
                }
                else if (pressed[i] == true && curNote[i] == false)
                {
                    pressed[i] = false;
                    Str_FKeyGen.instance.ShowNoteOff(i);
                }
            }

            /// playing sound
            for (int j = 0; j < hwStrNum; j++)
            {
                int soundHere = Str_FKeyGen.instance.strMinNote[j];

                for (int k = 0; k < sensorPerStr; k++)
                {
                    if (curNote[soundHere + k] == true)
                    {
                        soundHere = soundHere + k;
                    }
                }
               // Debug.Log("sound here " + soundHere);

                if (played[j] == true && playedPressed[j] == false)
                {
                    PlayOneNote(soundHere);
                    //Debug.Log("sound here " + soundHere);
                    Str_FKeyGen.instance.ShowPlayOn(j);
                    playedPressed[j] = true;
                }
                else if(playedPressed[j] == true && played[j] == false)
                {
                    playedPressed[j] = false;
                    StopOneNote(soundHere);
                    //Debug.Log("sound bye " + soundHere);
                    Str_FKeyGen.instance.ShowPlayOff(j);
                }

            }
           
        }

        private void PlayOneNote(int val)
        {
            // Start playing a new note
            NotePlaying[val] = new MPTKEvent()
            {
                Command = MPTKCommand.NoteOn,
                Value = val,
                Channel = StreamChannel,
                Duration = 9999999, // 9999 seconds but stop by the new note. See before.
                Velocity = Velocity // Sound can vary depending on the velocity
            };
            midiStreamPlayer.MPTK_PlayEvent(NotePlaying[val]);
        }


        private void StopOneNote(int val)
        {
            //midiStreamPlayer.MPTK_StopEvent(NotePlaying[val]);
            StartCoroutine(StopFade(val));
        }


        IEnumerator StopFade(int note_num)
        {
            float fade = 0.0f;
            while (fade < 1.0f)
            {
                fade += 0.1f;
                yield return new WaitForSeconds(0.01f);
            }
            midiStreamPlayer.MPTK_StopEvent(NotePlaying[note_num]);

        }

        void GetArduinoInput()
        {
            if (sp.IsOpen)
            {
                try
                {
                    val = sp.ReadLine();
                    //print(val);
                }
                catch (System.Exception)
                {
                }
            }

            //MimicArduino();
            char[] instNum = val.ToCharArray();
            int noteIndex;
            int indexMinStr = 0;
            int minNote = Str_FKeyGen.instance.strMinNote[indexMinStr];
            int divideSPS, modSPS;


            //Debug.Log("minNote " + minNote);

            for (int i = 0; i < totalSensorNum; i++)
            {
                noteIndex = numSensorArray[i];
                divideSPS = noteIndex / (sensorPerStr - 1);
                modSPS = noteIndex % (sensorPerStr - 1);
                //Debug.Log(minNote + noteIndex);

                if(noteIndex % hwStrNum > strNum)
                {
                    continue;
                }

                // for string hit - does not count as curNote
                if(noteIndex/ hwStrNum == (sensorPerStr - 1))
                {
                    //Debug.Log(noteIndex + " noteindex " + noteIndex % strNum);
                    if (instNum[i] == '1')
                        played[noteIndex% hwStrNum] = true;
                    else
                        played[noteIndex % hwStrNum] = false;
                    continue;
                }

                try
                {
                    minNote = Str_FKeyGen.instance.strMinNote[divideSPS];

                }
                catch { }

                if (instNum[i] == '1')
                {
                    curNote[minNote + modSPS + 1] = true;
                }
                else
                    curNote[minNote + modSPS + 1] = false;

                curSensorPerStr++;



                /*

                Debug.Log(i % (sensorPerStr - 1));
                // for mark
                if (noteIndex%(sensorPerStr - 1) == 0 )
                    //|| curSensorPerStr == (sensorPerStr - 1))
                {
                    curSensorPerStr = 0;
                    //Debug.Log(indexMinStr);
                    minNote = Str_FKeyGen.instance.strMinNote[indexMinStr];
                    indexMinStr++;
                }

                if (instNum[i] == '1')
                {
                    curNote[minNote + (noteIndex % (sensorPerStr - 1)) + 1] = true;
                    //curNote[minNote + noteIndex + 1] = true;
                }
                else
                    curNote[minNote + (noteIndex % (sensorPerStr - 1)) + 1] = false;

                curSensorPerStr++;
                */


                //Debug.Log("minNote " + minNote + " curSPS " + curSensorPerStr);
                //Debug.Log("in " + curSensorPerStr + " with " + indexMinStr + " and " + noteIndex);

                //Debug.Log(i + " is " + (minNote + noteIndex + 1));
                //Debug.Log(i + " is "+ curSensorPerStr);

            }
        }








        void MimicArduino()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                val = "100000000000000000000000";
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                val = "000000000000000000000000";
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                val = "010000000000000000000000";
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                val = "000000000000000000000000";
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                val = "100000100000000000111110";
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
                val = "000000000000000000000000";
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                val = "000000000000000000111111";
            }
            if (Input.GetKeyUp(KeyCode.J))
            {
                val = "000000000000000000000000";
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
