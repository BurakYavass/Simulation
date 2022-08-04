using UnityEngine;
using UnityEngine.UI;

public class EyeCheckItem : ObjectID
{
    public TaskEyeCheckControll EyeCheckControll;
    public EyeCheckitem EyeCheckitemState;

    public Outline Outline;
    public Collider collider;

    public bool active = false;
    public bool complete = false;
    private bool once = false;

    public float TotalTime= 0;
    private float count= 0;
    
    private void Update()
    {
        if (!active)
            return;

        if (active)
        {
            if (!once && !complete)
            {
                EyeCheckitemTaskState(state: EyeCheckitem.Start);
                once = true;
            }

            if (complete)
            {
                EyeCheckitemTaskState(state: EyeCheckitem.Complete);
            }
            ObjectCheck();
        }
    }

    public void EyeCheckitemTaskState(EyeCheckitem state)
    {
        EyeCheckitemState = state;
        switch (state)
        {
            case EyeCheckitem.None:
                break;
            case EyeCheckitem.Start:
                collider.enabled = true;
                complete = false;
                break;
            case EyeCheckitem.Complete:
                collider.enabled = false;
                Outline.enabled = false;
                complete = true;
                
                break;
        }
    }

    private void ObjectCheck()
    {
        int x = Screen.width / 2;
        int y = Screen.height / 2;
        
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x,y));
        RaycastHit hitObject;
        if (Physics.Raycast(ray,out hitObject))
        {
            if (hitObject.transform.gameObject == this.gameObject && complete == false)
            {
                // Objeye bakmaya devam ettiğimiz sürece outline mavi oluyor ve bakma süremizi tamamlıyoruz
                Outline.OutlineColor = Color.blue;

                if (count<TotalTime)
                {
                    count += Time.deltaTime;
                }
                else if(count>=TotalTime)
                {
                    if (once)
                    {
                        EyeCheckitemTaskState(state: EyeCheckitem.Complete);
                        EyeCheckControll.StateUpdate();
                        once = false;
                    }
                }
            }
            else
            {
                Outline.OutlineColor = Color.green;
            }
        }
    }
    
    public enum EyeCheckitem
    {
        None,
        Start,
        Complete,
    }
}