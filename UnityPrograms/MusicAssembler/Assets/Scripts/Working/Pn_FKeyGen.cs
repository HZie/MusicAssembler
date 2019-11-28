using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pn_FKeyGen : MonoBehaviour
{
    public static Pn_FKeyGen instance;
    public GameObject oneOctave, whiteKey, blackKey, whiteKeyMark, blackKeyMark;

    GameObject[] keys = new GameObject[128];
    GameObject[] keyMarks = new GameObject[128];
    GameObject[] octaves = new GameObject[10];
    Text[] keyName = new Text[128];

    float wminx, wminy, bminx, bminy, wdist, bdist, wcurx, wcury, bcurx, bcury;

    int i, j = 0;
    int tempOctave, tempNote;   // Decide_WB()

    // from Kbd_UserInput
    int minOctave = Kbd_UserInput.kbd_minOct;
    int numOctave = Kbd_UserInput.kbd_numOct;



    // 필요없
    int minNote = Kbd_UserInput.kbd_minNote;
    int noteNum = Kbd_UserInput.kbd_num;
    int keycount = 0, maxKeycount = 32;    // 40개 이상이면 다음 줄로 -> w는 0 b는 0.6




    void Awake()
    {
        if (!instance) instance = this;
    }

    void Start()
    {

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

                //keyName[i] = keys[i].AddComponent<Text>();
                //keyName[i].text = Decide_key(i);
                //Debug.Log(keyName[i].text);
                wcurx += wdist;
            }
            // black key
            else
            {
                keys[i] = Instantiate(blackKey, new Vector3(bcurx, bcury, 0), Quaternion.identity);
                keyMarks[i] = Instantiate(blackKeyMark, new Vector3(bcurx, bcury, 0), Quaternion.identity);
                bcurx += bdist;
                // check for whole tone
                if ((i % 12) == 3 || (i % 12) == 10)
                    bcurx += bdist;
            }

            // set for hierarchy
            keys[i].name = "keys (" + i + ")";
            keyMarks[i].name = "keys (" + i + ")";
            keys[i].transform.SetParent(octaves[octaveNum].transform);
            keyMarks[i].transform.SetParent(octaves[octaveNum].transform);
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
        int otv = note / 12;

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
            }
            catch { }
        }
    }


    public void ShowNoteOn(int note_num)
    {
        try
        {
            keyMarks[note_num].SetActive(true);
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


}
