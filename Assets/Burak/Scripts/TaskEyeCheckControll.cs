using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskEyeCheckControll : MonoBehaviour
{
    public List<EyeCheckItem> EyeCheckObject = new List<EyeCheckItem>();

    public EyeCheckState CheckState;
    public Image fillBar;

    private bool forOnce = false;
    private bool once = false;
    private bool checking = false;
    public bool active = false;
    public bool complete = false;

    private int index = 0;
    private int counter;

    public int TaskSuccesfullItemNumber;

    void Update()
    {
        if (!active && CheckState != EyeCheckState.Start && !once)
        {
            //CheckState = EyeCheckState.None;
            EyeCheckTaskState(CheckState = EyeCheckState.None);
            once = true;
        }

        if (active && !complete)
        {
            //CheckState =
            EyeCheckTaskState(CheckState = EyeCheckState.Start);
        }
    }

    private void EyeCheckTaskState(EyeCheckState state)
    {
        CheckState = state;
        switch (state)
        {
            case EyeCheckState.None:
                if (TaskSuccesfullItemNumber == 0)
                {
                    TaskSuccesfullItemNumber = EyeCheckObject.Count;
                }

                fillBar.enabled = false;
                
                for (int i = 0; i < EyeCheckObject.Count; i++)
                {
                    EyeCheckObject[i].GetComponent<Outline>().enabled = false;
                    EyeCheckObject[i].enabled = false;
                    EyeCheckObject[i].active = false;
                }
                break;

            case EyeCheckState.Start:
                for (int i = index; i < EyeCheckObject.Count; i++)
                {
                    fillBar.enabled = true;
                    EyeCheckObject[i].enabled = true;
                    EyeCheckObject[i].active = true;
                    EyeCheckObject[i].GetComponent<Outline>().enabled = true;
                    EyeCheckObject[i].GetComponent<Outline>().OutlineColor = Color.green;
                }
                break;
            
            case EyeCheckState.Complete:
                for (int i = 0; i < EyeCheckObject.Count; i++)
                {
                    EyeCheckObject[i].enabled = false;
                    EyeCheckObject[i].GetComponent<Outline>().enabled = false;
                    EyeCheckObject[i].active = false;
                }
                complete = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void StateUpdate()
    {
        counter++;
        fillBar.fillAmount += 1.0f / TaskSuccesfullItemNumber;
        if (counter >=  TaskSuccesfullItemNumber)
        {
            //CheckState = EyeCheckState.Complete;
            EyeCheckTaskState(CheckState = EyeCheckState.Complete);
        }
        else
        {
            //CheckState = EyeCheckState.Start;
            EyeCheckTaskState(CheckState = EyeCheckState.Start);
        }
    }

    public enum EyeCheckState
    {
        None,
        Start,
        Complete,
    }
}
