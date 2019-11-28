using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using System;
using UnityEngine.Events;

namespace KBD
{
    public class Pn_NotePreview : MonoBehaviour
    { }
        /*
        public static float Speed = 15f;
        public Camera Cam;
        public MidiFilePlayer midiFilePlayer;
        public MidiStreamPlayer midiStreamPlayer;
        public static Color ButtonColor = new Color(.7f, .9f, .7f, 1f);
        public NoteView NoteDisplay;
        public Collide Collider;
        public GameObject Plane;
        public float minZ, maxZ, minX, maxX;
        public float LastTimeCollider;
        public float DelayCollider = 25;
        public Material MatNewNote;
        public Material MatNewController;
        int[] countZ;

        public void EndLoadingSF()
        {
            Debug.Log("End loading SF, MPTK is ready to play");
            Debug.Log("   Time To Load SoundFont: " + Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadSoundFont.TotalSeconds, 3).ToString() + " second");
            Debug.Log("   Time To Load Waves: " + Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadWave.TotalSeconds, 3).ToString() + " second");
            Debug.Log("   Presets Loaded: " + MidiPlayerGlobal.MPTK_CountPresetLoaded);
            Debug.Log("   Waves Loaded: " + MidiPlayerGlobal.MPTK_CountWaveLoaded);
        }

        void Start()
        {
            if (!HelperDemo.CheckSFExists()) return;

            // Default size of a Unity Plan
            float planSize = 10f;

            minZ = Plane.transform.localPosition.z - Plane.transform.localScale.z * planSize / 2f;
            maxZ = Plane.transform.localPosition.z + Plane.transform.localScale.z * planSize / 2f;

            minX = Plane.transform.localPosition.x - Plane.transform.localScale.x * planSize / 2f;
            maxX = Plane.transform.localPosition.x + Plane.transform.localScale.x * planSize / 2f;


            // If call is already set from the inspector there is no need to set another listeneer
            if (!midiFilePlayer.OnEventNotesMidi.HasEvent())
            {
                // No listener defined, set now by script. NotesToPlay will be called for each new notes read from Midi file
                Debug.Log("No OnEventNotesMidi defined, set by script");
                midiFilePlayer.OnEventNotesMidi.AddListener(NotesToPlay);
            }
        }

        /// <summary>
        /// Call when a group of midi events is ready to plays from the the midi reader.
        /// Playing the events are delayed until they "fall out"
        /// </summary>
        /// <param name="notes"></param>
        public void NotesToPlay(List<MPTKEvent> notes)
        {
            countZ = new int[Convert.ToInt32(maxZ - minZ) + 1];

            //Debug.Log(notes.Count);
            foreach (MPTKEvent note in notes)
            {
                switch (note.Command)
                {
                    case MPTKCommand.NoteOn:
                        if (note.Value > 40 && note.Value < 100)// && note.Channel==1)
                        {
                            // Axis Z for the note value
                            float z = Mathf.Lerp(minZ, maxZ, (note.Value - 40) / 60f);
                            countZ[Convert.ToInt32(z - minZ)]++;
                            Vector3 position = new Vector3(maxX, 2 + countZ[Convert.ToInt32(z - minZ)] * 4f, z);
                            NoteView n = Instantiate<NoteView>(NoteDisplay, position, Quaternion.identity);
                            n.gameObject.SetActive(true);
                            n.midiStreamPlayer = midiStreamPlayer;
                            n.note = note;
                            n.gameObject.GetComponent<Renderer>().material = MatNewNote;
                            // See noteview.cs: update() move the note along the plan until they fall out, then they are played
                            n.zOriginal = position.z;

                            PlaySound();
                        }
                        break;

                    case MPTKCommand.PatchChange:
                        {
                            // See noteview.cs: update() move the note along the plan until they fall out, then they are played
                            float z = Mathf.Lerp(minZ, maxZ, note.Value / 127f);
                            countZ[Convert.ToInt32(z - minZ)]++;
                            Vector3 position = new Vector3(maxX, 8f + countZ[Convert.ToInt32(z - minZ)] * 4f, z);
                            n.gameObject.SetActive(true);
                            n.midiStreamPlayer = midiStreamPlayer;
                            n.note = note;
                            n.gameObject.GetComponent<Renderer>().material = MatNewController;
                            n.zOriginal = position.z;
                        }
                        break;
                }
            }
        }

        private void PlaySound()
        {
            // Some sound for waiting the notes ...
            if (!NoteView.FirstNotePlayed)
                //! [Example PlayNote]
                midiStreamPlayer.MPTK_PlayEvent
                (
                    new MPTKEvent()
                    {
                        Channel = 9,
                        Duration = 0.2f,
                        Value = 60,
                        Velocity = 100
                    }
                );
            //! [Example PlayNote]
        }

        
        

        public void Clear()
        {
            NoteView[] components = GameObject.FindObjectsOfType<NoteView>();
            foreach (NoteView noteview in components)
            {
                if (noteview.enabled)
                    //Debug.Log("destroy " + ut.name);
                    DestroyImmediate(noteview.gameObject);
            }
        }

        void Update()
        {
            if (midiFilePlayer != null && midiFilePlayer.MPTK_IsPlaying)
            {
                // Generate random collider
                float time = Time.realtimeSinceStartup - LastTimeCollider;
                if (time > DelayCollider)
                {
                    LastTimeCollider = Time.realtimeSinceStartup;

                    float zone = 10;
                    Vector3 position = new Vector3(UnityEngine.Random.Range(minX + zone, maxX - zone), -5, UnityEngine.Random.Range(minZ + zone, maxZ - zone));
                    Collide n = Instantiate<Collide>(Collider, position, Quaternion.identity);
                    n.gameObject.SetActive(true);
                }
            }
        }
    }*/
}