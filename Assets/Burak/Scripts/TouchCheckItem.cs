using UnityEngine;

public class TouchCheckItem : ObjectID
{
    public TouchableController touchableController;
    public TouchCheckitem TouchCheckitemState;

    public Outline Outline;
    public Collider collider;
    
    public bool active = false;
    public bool complete = false;
    private bool once = false;
    
    public float TotalTime= 0;
    public float count= 0;
    
    private void Update()
    {
        if (!active)
            TouchCheckitemTaskState(TouchCheckitemState= TouchCheckitem.None); 

        if (active)
        {
            if (!complete && !once)
            {
                TouchCheckitemTaskState(TouchCheckitemState= TouchCheckitem.Start);
                once = true;
            }
            else if (complete)
            {
                TouchCheckitemTaskState(TouchCheckitemState= TouchCheckitem.Complete);
            }
            ObjectCheck();
            
        }
    }

    public void TouchCheckitemTaskState(TouchCheckitem state)
    {
        TouchCheckitemState = state;
        switch (state)
        {
            case TouchCheckitem.None:
                collider.enabled = false;
                Outline.enabled = false;
                break;
            case TouchCheckitem.Start:
                collider.enabled = true;
                Outline.enabled = true;
               
                break;
            case TouchCheckitem.Complete:
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
            if (hitObject.transform.gameObject == gameObject && complete == false)
            {
                Outline.OutlineColor = Color.blue;

                if (count<TotalTime)
                {
                    count += Time.deltaTime;
                }
                else if(count>=TotalTime)
                {
                    if (once && !complete)
                    {
                        TouchCheckitemTaskState(TouchCheckitemState= TouchCheckitem.Complete);
                        touchableController.StartCoroutine("IndexChanger");
                        once = false;
                    }
                }
            }
        }
    }
    
    public enum TouchCheckitem
    {
        None,
        Start,
        Complete,
    }
}