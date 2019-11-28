using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Kbd_KeyGenByOctave : MonoBehaviour
{
    public static Kbd_KeyGenByOctave instance;
    public GameObject oneOctave, whiteKey, blackKey, whiteKeyMark, blackKeyMark, wMidiMark, bMidiMark;

    GameObject[] keys = new GameObject[128];
    GameObject[] keyMarks = new GameObject[128];
    GameObject[] midiMarks = new GameObject[128];

    GameObject[] octaves = new GameObject[10];

    float wminx, wminy, bminx, bminy, wdist, bdist, wcurx, wcury, bcurx, bcury;

    int i, j = 0;
    int tempOctave, tempNote;   // Decide_WB()

    // from Kbd_UserInput
    int minOctave, numOctave;

    

    void Awake()
    {
        if (!instance) instance = this;
    }

    void Start()
    {
        //Debug.Log(SceneManager.GetActiveScene().name);
        if(SceneManager.GetActiveScene().name == "Pn_FreeMode" || SceneManager.GetActiveScene().name == "Pn_PracMode")
        {
            minOctave = Pn_SetVal.kbd_minOct;
            numOctave = Pn_SetVal.kbd_numOct;
        }
        else
        {
            minOctave = Kbd_UserInput.kbd_minOct;
            numOctave = Kbd_UserInput.kbd_numOct;
        }

        // initialize values (min, dist, current)
        wminx = -8.0f;
        wminy = 5.5f;
        bminx = -7.7f;
        bminy = 6.0f;
        wdist = 0.7f;
        bdist = 0.7f;
        wcurx = wminx;
        wcury = wminy;
        bcurx = bminx;
        bcury = bminy;

        // create each octaves
        for (j = minOctave; j < minOctave + numOctave; j++)
        {
            if (j % 3 == 0)
            {
                wcurx = wminx;
                wcury -= wdist * 4;
                bcurx = bminx;
                bcury -= bdist * 4;
            }
            KBD_1Octave(j);

        }
    }


    void KBD_1Octave(int octaveNum)
    {
        octaves[octaveNum] = Instantiate(oneOctave, new Vector3(wcurx, wcury, 0), Quaternion.identity);
        octaves[octaveNum].name = "octave (" + octaveNum + ")";

        int curNoteNum = octaveNum * 12;

        for (i = curNoteNum; i < curNoteNum + 12; i++)
        {
            // white key
            if (Decide_WB(i) == 0)
            {
                keys[i] = Instantiate(whiteKey, new Vector3(wcurx, wcury, 0), Quaternion.identity);
                keyMarks[i] = Instantiate(whiteKeyMark, new Vector3(wcurx, wcury, 0), Quaternion.identity);
                midiMarks[i] = Instantiate(wMidiMark, new Vector3(wcurx, wcury, 0), Quaternion.identity);

                wcurx += wdist;
            }
            // black key
            else
            {
                keys[i] = Instantiate(blackKey, new Vector3(bcurx, bcury, 0), Quaternion.identity);
                keyMarks[i] = Instantiate(blackKeyMark, new Vector3(bcurx, bcury, 0), Quaternion.identity);
                midiMarks[i] = Instantiate(bMidiMark, new Vector3(bcurx, bcury, 0), Quaternion.identity);

                bcurx += bdist;
                // check for whole tone
                if ((i % 12) == 3 || (i % 12) == 10)
                    bcurx += bdist;
            }


            // show octave # on only C
            if (i % 12 == 0)
            {
                TextMesh temptxt = octaves[octaveNum].GetComponent<TextMesh>();
                temptxt.transform.position = new Vector3(wcurx - wdist, wcury - wdist, 0);
                temptxt.text = Decide_key(i);
                temptxt.GetComponent<MeshRenderer>().sortingOrder = 10;
            }

            // set for hierarchy
            keys[i].name = "keys (" + i + ")";
            keyMarks[i].name = "keys (" + i + ")";
            midiMarks[i].name = "midi mark (" + i + ")";

            keys[i].transform.SetParent(octaves[octaveNum].transform);
            keyMarks[i].transform.SetParent(octaves[octaveNum].transform);
            midiMarks[i].transform.SetParent(octaves[octaveNum].transform);

            //keyMarks[i].transform.SetParent(octaves[octaveNum].transform, false);

        }
        AllMarkOff();
    }



    // 흰건반(0)/ 검은건반(1) 구분하기
    int Decide_WB(int note)
    {
        //1. /12로 옥타브 어딘지 알아내기 (실제 기준은 이 값의 -1)
        tempOctave = (note / 12);
        //2. %12로 어느 건반인지 구분
        tempNote = (note % 12);

        // 나중에 수정... 기준점 C에서 옥타브 표시 보여주기
        //if (tempNote == 0)
        //    octaveCheck.text = "C" + (octave - 2);

        switch (tempNote)
        {
            case 0:
            case 2:
            case 4:
            case 5:
            case 7:
            case 9:
            case 11:
                return 0;
            default:
                return 1;
        }

    }

    string Decide_key(int note)
    {
        string kn = "";
        int otv = (note / 12);

        switch (note % 12)
        {
            case 0:
                kn = "C";
                break;
            case 1:
                kn = "C#";
                break;
            case 2:
                kn = "D";
                break;
            case 3:
                kn = "D#";
                break;
            case 4:
                kn = "E";
                break;
            case 5:
                kn = "F";
                break;
            case 6:
                kn = "F#";
                break;
            case 7:
                kn = "G";
                break;
            case 8:
                kn = "G#";
                break;
            case 9:
                kn = "A";
                break;
            case 10:
                kn = "A#";
                break;
            case 11:
                kn = "B";
                break;
        }

        kn += otv;
        //Debug.Log(kn);
        return kn;
    }




    void AllMarkOff()
    {
        for (i = 0; i < 128; i++)
        {
            try
            {
                keyMarks[i].SetActive(false);
                midiMarks[i].SetActive(false);
            }
            catch { }
        }
    }


    public void ShowNoteOn(int note_num)
    {
        try
        {
            keyMarks[note_num].SetActive(true);
            Debug.Log(note_num);
        }
        catch
        {
        }
    }


    public void ShowNoteOff(int note_num)
    {
        try
        {
            keyMarks[note_num].SetActive(false);
        }
        catch
        {
        }
    }

    public void ShowMidiOn(int note_num)
    {
        try
        {
            //Debug.Log(note_num);
            StartCoroutine(ShowNoteFade(note_num));
            //midiMarks[note_num].SetActive(true);
        }
        catch
        {
        }
    }


    public void ShowMidiOff(int note_num)
    {
        try
        {
            midiMarks[note_num].SetActive(false);
        }
        catch
        {
        }
    }


    IEnumerator ShowNoteFade(int note_num)
    {
        midiMarks[note_num].SetActive(true);
        //Color fadecolor = midiMarks[note_num].GetComponent<SpriteRenderer>().color;
        float fadecolor = 0.0f;
        midiMarks[note_num].GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, 0);

        while (fadecolor < 1.0f)
        {

            fadecolor += 0.1f;
            //midiMarks[note_num].SpriteRenderer.color = fadecolor;
            midiMarks[note_num].GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, fadecolor);
            //Debug.Log(fadecolor);

            yield return new WaitForSeconds(0.01f);
        }

    }
}
