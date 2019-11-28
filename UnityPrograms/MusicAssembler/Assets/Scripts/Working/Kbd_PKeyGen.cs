using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kbd_PKeyGen : MonoBehaviour
{
    public static Kbd_PKeyGen instance;
    public GameObject whiteKey, blackKey, whiteKeyMark, blackKeyMark, wMidiMark, bMidiMark;
    //public Text octaveCheck;

    GameObject[] keys = new GameObject[128];
    GameObject[] keyMarks = new GameObject[128];
    GameObject[] midiMarks = new GameObject[128];

    int minNote = Kbd_UserInput.kbd_minNote;
    int noteNum = Kbd_UserInput.kbd_num;
    int i, j = 0;
    int octave, tempNote;
    float wminx, wminy, bminx, bminy, wdist, bdist, wcurx, wcury, bcurx, bcury;
    int whosefirst = 0; // 검은건반이 먼저 나오면 wdist++
    int keycount = 0, maxKeycount = 36;    // 40개 이상이면 다음 줄로 -> w는 0 b는 0.6
    int skipNextBlack = 0;  // 온음계 스킵

    void Awake()
    {
        if (!instance) instance = this;
    }

    void Start()
    {
        // initialize values (min, dist, current)
        wminx = -8.2f;
        wminy = 3.2f;
        bminx = -7.8f;
        bminy = 3.82f;
        wdist = 0.7f;
        bdist = 0.7f;
        wcurx = wminx;
        wcury = wminy;
        bcurx = bminx;
        bcury = bminy;

        Kbd_gen();
    }


    void Kbd_gen()
    {
        for (i = minNote; i < minNote + noteNum; i++, j++)
        {
            try
            {
                if(Decide_key(i) == 0)
                {            
                    keys[i] = Instantiate(whiteKey, new Vector3(wcurx, wminy, 0), Quaternion.identity);
                    keyMarks[i] = Instantiate(whiteKeyMark, new Vector3(wcurx, wminy, 0), Quaternion.identity);
                    midiMarks[i] = Instantiate(wMidiMark, new Vector3(wcurx, wminy, 0), Quaternion.identity);

                    wcurx += wdist;
                    if (whosefirst == 0)
                        whosefirst++;
                }
                else
                {
                    keys[i] = Instantiate(blackKey, new Vector3(bcurx, bminy, 0), Quaternion.identity);
                    keyMarks[i] = Instantiate(blackKeyMark, new Vector3(bcurx, bminy, 0), Quaternion.identity);
                    midiMarks[i] = Instantiate(bMidiMark, new Vector3(bcurx, bminy, 0), Quaternion.identity);

                    bcurx += bdist;
                    // 다음이 온음계면 추가 dist (검은 건반 건너뛰기)
                    if (tempNote == 3 || tempNote == 10)
                    {
                        skipNextBlack++;
                        bcurx += bdist;
                    }
                    else
                    {
                        skipNextBlack = 0;
                    }

                    if (whosefirst == 0)
                    {
                        wcurx += wdist;
                        whosefirst++;
                    }
                }

                // Name Instantiated Objs
                keys[i].name = "keys (" + i + ")";
                keyMarks[i].name = "mark (" + i + ")";
                midiMarks[i].name = "midiMark (" + i + ")";

                // markDefault = 0ff
                AllMarkOff();

                keycount++;
                if(keycount > maxKeycount)
                {
                    wminy -= 3.2f;
                    bminy -= 3.2f;
                    wcurx = wminx;
                    bcurx = bminx;
                    keycount = 0;
                    whosefirst = 0;
                    if(skipNextBlack == 1)
                    {
                        bcurx += bdist;
                    }
                }
            }
            catch { }
        }
    }

    // 흰건반(0)/ 검은건반(1) 구분하기
    int Decide_key(int note)
    {
        //1. /12로 옥타브 어딘지 알아내기 (실제 기준은 이 값의 -1)
        octave = (note / 12);
        //2. %12로 어느 건반인지 구분
        tempNote = (note % 12);
        
        // 나중에 수정... 기준점 C에서 옥타브 표시 보여주기
        //if(tempNote == 0)
          // octaveCheck.text = "C" + (octave-2);

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

    void AllMarkOff()
    {
        for (int i = minNote; i < minNote+noteNum; i++)
        {
            try {
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

    public void ShowMidiNoteOn(int note_num)
    {
        //midiMarks[note_num].SetActive(true);

        StartCoroutine(ShowNoteFade(note_num));

        /*
        try
        {
            midiMarks[note_num].SetActive(true);
            Debug.Log("yes");
        }
        catch
        {
            Debug.Log("why not " + note_num);
        }*/
    }


    IEnumerator ShowNoteFade(int note_num)
    {
        midiMarks[note_num].SetActive(true);
        //Color fadecolor = midiMarks[note_num].GetComponent<SpriteRenderer>().color;
        float fadecolor = 0.0f;
        midiMarks[note_num].GetComponent<SpriteRenderer>().color = new Color(0,0,1,0);

        while (fadecolor < 1.0f)
        {

            fadecolor += 0.1f;
            //midiMarks[note_num].SpriteRenderer.color = fadecolor;
            midiMarks[note_num].GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, fadecolor);
            //Debug.Log(fadecolor);

            yield return new WaitForSeconds(0.01f);
        }

    }




    public void ShowMidiNoteOff(int note_num)
    {
        try
        {
            midiMarks[note_num].SetActive(false);
        }
        catch
        {
        }
    }


}
