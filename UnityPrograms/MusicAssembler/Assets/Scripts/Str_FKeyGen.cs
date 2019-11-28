using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Str_FKeyGen : MonoBehaviour
{
    public static Str_FKeyGen instance;
    public GameObject fret, line, mark, playmark, background, midimark;
    public GameObject strNote;
    
    GameObject[] frets = new GameObject[30];
    GameObject[] strs = new GameObject[30];
    GameObject[] strback = new GameObject[30];

    GameObject[] keyMarks = new GameObject[128];
    GameObject[] playMarks = new GameObject[128];
    GameObject[] midiMarks = new GameObject[128];

    GameObject[] noteName;
    InputField[] if_strNote;

    // 개수 (ex - 기타면 10/6, 바이올린은 0/4 처럼 쓰면 됨)
    int fretNum, strNum;
    //int fretNum = Str_UserInput.str_fretNum;
    //int strNum = Str_UserInput.str_strNum;
    public int minNote; // 낼 수 있는 가장 낮은 소리.
    int countNote = 1;    // 마크 생성용

    // minimum possible for each string
    public int[] strMinNote;

    float fminx, fminy, fcurx, fcury, fdist,    //fret
          sminx, sminy, scurx, scury, sdist,    //str
          mminx, mminy, mcurx, mcury, mdistx, mdisty,   //mark
          pminx, pminy, pcurx, pcury, pdist,    // play note mark
          nx, ny, nd;   //notename

    int i, j = 0;

    void Awake()
    {
        if (!instance) instance = this;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Gt_FreeMode")
        {
            fretNum = Gt_SetVal.str_fretNum;
            strNum = Gt_SetVal.str_strNum;

        }
        else
        {
            fretNum = Str_UserInput.str_fretNum;
            strNum = Str_UserInput.str_strNum;
        }


        // initialize values (min, dist, current)
        fminx = -6f;
        fminy = -1.75f;
        fcurx = fminx;
        fcury = fminy;
        fdist = 1f;

        sminx = 0f;
        sminy = -3f;
        scurx = sminx;
        scury = sminy;
        sdist = 0.5f;

        mminx = -6.5f;
        mminy = -3f;
        mcurx = mminx;
        mcury = mminy;
        mdistx = 1f;
        mdisty = 0.5f;

        pminx = 7.7f;
        pminy = -3f;
        pcurx = pminx;
        pcury = pminy;
        pdist = 0.5f;

        nx = -700f;
        ny = -270f;
        nd = 45f;

        //for(i = 1; i <= strNum; i++)
        strMinNote = new int[(strNum + 1)];
        noteName = new GameObject[(strNum + 1)];
        if_strNote = new InputField[(strNum + 1)];

        EachStrMin();

        minNote = strMinNote[0];

        Str_gen();
    }


    void EachStrMin()
    {
        if (SceneManager.GetActiveScene().name == "4-1 Gt_FreeMode" || SceneManager.GetActiveScene().name == "4-2 Gt_PracMode")
        {
            strMinNote[0] = 40;
            strMinNote[1] = 45;
            strMinNote[2] = 50;
            strMinNote[3] = 55;
            strMinNote[4] = 59;
            strMinNote[5] = 64;
        }
        else
        {
            for (i = 0; i < strNum; i++)
                strMinNote[i] = 40 + 5 * i;   //default
        }

        for (i=0; i<strNum;i++)
        {
            //strMinNote[i] = 40 + 5*i;   //default

            noteName[i] = Instantiate(strNote, new Vector3(nx, ny, 0), Quaternion.identity);
            if_strNote[i] = noteName[i].GetComponent<InputField>();
            if_strNote[i].text = Decide_key(strMinNote[i]);
            ny += nd;

            noteName[i].name = "noteName (" + i + ")";
            noteName[i].transform.SetParent(GameObject.Find("Canvas").transform, false);
        }

    }

    public void Decision()
    {
        for (i = 0; i < strNum; i++)
        {
            //Debug.Log(if_strNote[i].text);
            strMinNote[i] = Decide_noteNum(if_strNote[i].text);
            //Debug.Log(Decide_noteNum(if_strNote[i].text));
        }
        Destroy_Marks();
        Create_Marks();
        AllMarkOff();
    }


    void Str_gen()
    {
        for (i = 0; i < fretNum; i++)
        {
            frets[i] = Instantiate(fret, new Vector3(fcurx, fcury, 0), Quaternion.identity);
            fcurx += fdist;
            frets[i].name = "fret (" + i + ")";
        }

        for (i = 0; i < strNum; i++)
        {
            strs[i] = Instantiate(line, new Vector3(scurx, scury, 0), Quaternion.identity);
            strback[i] = Instantiate(background, new Vector3(scurx, scury, 0), Quaternion.identity);
            scury += sdist;
            strs[i].name = "str (" + i + ")";
        }

        Create_Marks();
        Create_PlayMarks();
        AllMarkOff();
    }


    void Create_Marks()
    {
        for (i = minNote; i < 128; i++)
        {
            // if next line, change x, y value to go next
            if (strMinNote[countNote] == i)
            {
                countNote++;
                mcury += mdisty;
                mcurx = mminx;
            }

            keyMarks[i] = Instantiate(mark, new Vector3(mcurx, mcury, 0), Quaternion.identity);
            midiMarks[i] = Instantiate(midimark, new Vector3(mcurx, mcury, 0), Quaternion.identity);

            mcurx += mdistx;
            keyMarks[i].name = "mark (" + i + ")";
            midiMarks[i].name = "midi mark (" + i + ")";

            keyMarks[i].transform.SetParent(GameObject.Find("Str_KeyGen").transform);
            midiMarks[i].transform.SetParent(GameObject.Find("Str_KeyGen").transform);
        }
    }


    void Create_PlayMarks()
    {
        for(i = 0; i<strNum; i++)
        {
            playMarks[i] = Instantiate(playmark, new Vector3(pcurx, pcury, 0), Quaternion.identity);
            pcury += pdist;
            playMarks[i].name = "playMark (" + i + ")";
            playMarks[i].transform.SetParent(GameObject.Find("Str_KeyGen").transform);
        }
    
    }

    void Destroy_Marks()
    {
        for (i = minNote; i < 128; i++)
        {
            Destroy(keyMarks[i]);
        }

        mcurx = mminx;
        mcury = mminy;
        countNote = 1;
    }



    void AllMarkOff()
    {
        for (int i = minNote; i < 128; i++)
        {
            try
            {
                keyMarks[i].SetActive(false);
                midiMarks[i].SetActive(false);

            }
            catch { }
        }
        for (int i = 0; i < 128; i++)
        {
            try
            {
                playMarks[i].SetActive(false);
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


    public void ShowPlayOn(int note_num)
    {
        try
        {
            playMarks[note_num].SetActive(true);
        }
        catch
        {
        }

    }
    public void ShowPlayOff(int note_num)
    {
        try
        {
            playMarks[note_num].SetActive(false);

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
            //StartCoroutine(ShowNoteFade(note_num));
            midiMarks[note_num].SetActive(true);
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




    // note name -> num
    int Decide_noteNum(string note)
    {
        char[] result = new char[4];
        int k = 0;
        int nn = 0; // note num;

        foreach (var item in note)
        {
            result[k] = item;
            k++;
            //Debug.Log(item);
        }

        // 
        if (result[1] == '#')
        {
            nn = 12 * ((int)Char.GetNumericValue(result[2])+1);
            nn++;   // +1 for sharp
            //Debug.Log(nn);
        }
        else
        {
            nn = 12 * ((int)Char.GetNumericValue(result[1])+1);
            //Debug.Log(nn);
        }
        nn += Tmp_decide(result[0]);

        //Debug.Log(note + " is " + nn);
        return nn;
    }

    int Tmp_decide(char note)
    {
        switch (note)
        {
            case 'C':
                return 0;
            case 'D':
                return 2;
            case 'E':
                return 4;
            case 'F':
                return 5;
            case 'G':
                return 7;
            case 'A':
                return 9;
            case 'B':
                return 11;
        }

        return 0;
    }


    // note num -> name
    string Decide_key(int note)
    {
        string kn = "";
        int otv = (note / 12) - 1;

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


}
