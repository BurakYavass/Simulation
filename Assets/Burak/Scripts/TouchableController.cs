using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchableController : MonoBehaviour
{
    public TouchableState TouchableTaskState;

    public List<TouchCheckItem> TouchCheckItems = new List<TouchCheckItem>();

    private TouchItem touchItemSc;
    private TouchItem touchItemSc2;
    private TouchCheckItem _TouchPlaceSc;
    private Transform toDrag;
    private Vector3 toDragFirstPos;
    
    public Camera Camera;
    private Vector3 camPos;
    private Vector3 touchPosition;
    private Vector3 offSet;

    //private bool once = false;
    private bool dragging = false;
    public bool active = false;
    public bool complete = false;
    private bool itemsComplete = false;
    private bool dropPlace = false;
    
    [Header("Touch Item")]
    public TouchItem touchItem;
    public Outline touchItemOutline;
    public Collider touchItemCollider;
    
    [Header("Touch Item Place")]
    public GameObject touchObjectPlace;
    public Outline touchObjectPlaceOutline;
    public Collider touchObjectPlaceCollider;

    private int index=0;
    public float rayMaxDistance=0;
    public int TaskSuccesfullItemNumber;

    private Vector3 screenPoint;

    private void Start()
    {
        TaskTouchableState(TouchableTaskState = TouchableState.None); 
    }

    void Update()
    {
        if (!active)
            return;

        if (active && TouchableTaskState != TouchableState.Complete && !itemsComplete)
        {
            TaskTouchableState(TouchableTaskState = TouchableState.Start);
            
        }
        ObjectMovement();
    }

    private void ObjectMovement()
    {
        if (Input.touchCount > 0)
        {
            //TouchController();
        }
        else
        {
            MouseController();
            
            if (dragging)
            {
                Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                Vector3 cursorPosition = Camera.ScreenToWorldPoint(cursorPoint) + offSet;

                toDrag.position = cursorPosition;
            }

            if (dropPlace)
            {
                TaskTouchableState(TouchableTaskState = TouchableState.Complete);
            }
        }
    }
    
    private void TaskTouchableState(TouchableState touchableState)
    {
        TouchableTaskState = touchableState;
        switch (touchableState)
        {
            case TouchableState.None:
                Debug.Log(touchableState);
                //DragItem
                touchItem.enabled = false;
                
                if (TaskSuccesfullItemNumber == 0)
                {
                    TaskSuccesfullItemNumber = TouchCheckItems.Count;
                }

                for (int i = 0; i < TouchCheckItems.Count; i++)
                {
                    TouchCheckItems[i].active = false;
                }

                touchObjectPlace.SetActive(false);
                touchObjectPlaceCollider.enabled = true;
                touchObjectPlaceOutline.enabled = true;
                break;
            
            case TouchableState.Start:
                
                //DragItem
                if (!dragging)
                {
                    touchItem.gameObject.SetActive(true);
                    touchItem.enabled = true;
                    touchItemCollider.enabled = true;
                    touchItemOutline.enabled = true;
                }

                if (dragging && !complete)
                {
                    touchItemOutline.enabled = false;
                    TouchCheckItems[index].active = true;
                    TouchCheckItems[index].enabled = true;
                }
                break;
            case TouchableState.ItemsComplete:
                
                for (int i = 0; i < TouchCheckItems.Count; i++)
                {
                    TouchCheckItems[i].enabled = false;
                    TouchCheckItems[i].active = false;
                }

                // touchItem.enabled = true;
                // touchItemCollider.enabled = true;
                // touchItemOutline.enabled = true;

                touchObjectPlace.SetActive(true);
                touchObjectPlaceCollider.enabled = true;
                touchObjectPlaceOutline.enabled = true;
                touchObjectPlaceOutline.OutlineColor = Color.red;
                break;
            
            case TouchableState.Complete:

                dragging = false;
                //DragItem
                touchItem.enabled = false;
                touchItemCollider.enabled = false;
                touchItemOutline.enabled = false;
                
                //DropPlace
                touchObjectPlace.SetActive(false);
                touchObjectPlaceCollider.enabled = false;
                touchObjectPlaceOutline.enabled = false;
                touchObjectPlaceOutline.OutlineColor = Color.red;

                complete = true;
                
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(touchableState), touchableState, null);
        }
    }
    
    #region Mouse Control
    
    private void MouseController()
    {
        Vector3 mousePos;
        camPos = Camera.transform.position;
        if (!dragging)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit1;
                
                toDrag = null;
        
                if (Physics.Raycast(ray,out hit1,rayMaxDistance))
                {
                    if (touchItemSc == null)
                        touchItemSc = hit1.transform.GetComponent<TouchItem>();
                    
                    if (touchItemSc != null && touchItemSc.Type == ObjectID.ObjectType.TouchObject)
                    {
                        //Hit ettigimiz objeyi toDrag objesine atiyoruz
                        toDrag = hit1.transform;
                        toDragFirstPos = toDrag.position;
                        dragging = true;
                    
                        //Obje alindiginda collideri kapaniyor
                        //touchItemSc.DragState(true);
                        touchItemCollider.enabled = false;

                        screenPoint = Camera.WorldToScreenPoint(hit1.point);
                        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                        mousePos = Camera.ScreenToWorldPoint(mousePos);
                        offSet = mousePos - hit1.point;
                    }
                    else
                    {
                        dragging = false;
                        touchItemSc = null;
                    }
                }
            }
        }
        
        if (dragging && itemsComplete)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray2 = Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit2;
                
                if (Physics.Raycast(ray2,out hit2,rayMaxDistance))
                {
                    if (touchItemSc2 == null)
                        touchItemSc2 = hit2.transform.GetComponent<TouchItem>();
                    
                    if(touchItemSc2 != null && touchItemSc2.Type == ObjectID.ObjectType.TouchObjectPlace)
                    {
                        dragging = false;
                        if (toDrag != null) 
                            toDrag.position = toDragFirstPos;
                        dropPlace = true;
                        TaskTouchableState(TouchableTaskState = TouchableState.Complete);
                    }
                }
            }
        }
    }
    #endregion

   /* #region Touch Countrol
    
    private void TouchController()
    {
        Vector3 touchpos;
        Touch touch = Input.GetTouch(0);
        touchPosition = touch.position;
        camPos = Camera.main.transform.position;
        //Debug.Log(touch.phase+"TouchController");
        if (touch.phase == TouchPhase.Began)
        {

            Ray ray = Camera.ScreenPointToRay(touch.position);
            RaycastHit hit1;
            Debug.Log(touch.position);
            if (Physics.Raycast(ray, out hit1))
            {
                Debug.Log("ray hit" + hit1.transform.name);
                if(hit1.transform.GetComponent<DragItem>()!=null)
                    touchItemSc = hit1.transform.GetComponent<TouchItem>();

                if (touchItemSc != null && touchItemSc.Type == ObjectID.ObjectType.TouchObject)
                {
                    Debug.Log("Obje Alindi");
                    //Hit ettigimiz objeyi toDrag objesine atiyoruz
                    toDrag = hit1.transform;
                    dragging = true;
                    
                    //Alinan objeye alindigini bildiriyor
                    touchItemSc.DragState(true);
                    
                    Vector3 screenPoint = Camera.WorldToScreenPoint(hit1.point);
                    touchpos = new Vector3(touch.position.x, touch.position.y, screenPoint.z);
                    touchpos = Camera.ScreenToWorldPoint(touchpos);

                    offSet = touchpos - hit1.point;

                    //collideArea = false;
                    //_touchablePlace = null;
                    touchItemSc.CollidePlaceCheck(false);
                }
                else
                {
                    dragging = false;
                    touchItemSc = null;
                }
            }
            
        }

        if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
        {
            // Drag Item move
            if (dragging)
            {
                Vector3 screenPoint = Camera.WorldToScreenPoint(toDrag.transform.position);
                touchpos = new Vector3(touch.position.x, touch.position.y, screenPoint.z);
                touchpos = Camera.ScreenToWorldPoint(touchpos);

                //Drag object localposition change
                toDrag.localPosition = touchpos - offSet;
            }

            Ray ray2 = Camera.ScreenPointToRay(touch.position);
            RaycastHit hitTest2;

            //Objeyi birakacagimiz alandamiyiz diye RAY ile kontrol ediyoruz
            //if (Physics.Raycast(ray2, out hitTest2))
            //{
                // _touchablePlace = hitTest2.transform.GetComponent<TouchCheckItem>();

                // if (_touchablePlace != null && _touchablePlace.Type == ObjectID.ObjectType.CollidePlace)
                // {
                //     collideArea = true;
                // }
                // else
                // {
                //     collideArea = false;
                //     _touchablePlace = null;
                // }
                // }
                // else
                // {
                //     _touchablePlace = null;
                //     collideArea = false;
                // }


            //}
        }

        if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            // if (_touchablePlace == null && _brushItemSc == null)
            //     return;

            // if (collideArea)
            // {
            //     collideArea = false;
            //     dragging = false;
            //     
            //     if (_brushItemSc != null)
            //     {
            //         //toDrag.transform.localPosition = _CollidePlace.transform.localPosition;
            //         _brushItemSc.CollidePlaceCheck(true);
            //         _brushItemSc.DragState(false);
            //     }
            //
            //     _brushItemSc = null;
            //     //_touchablePlace = null;
            // }
            // else if(!collideArea && dragging)
            // {
            //     dragging = false;
            //     collideArea = false;
            //     
            //     //toDrag.transform.localPosition = Vector3.Lerp(toDrag.transform.localPosition,toDragFirstPos,1);
            //
            //     if (_brushItemSc != null)
            //     {
            //         
            //         _brushItemSc.CollidePlaceCheck(false);
            //         _brushItemSc.DragState(false);
            //     }
            //
            //     _brushItemSc = null;
            //     //_touchablePlace = null;
            // }
            
        
    }
    #endregion
    */

    public IEnumerator IndexChanger()
    {
        Debug.Log("indexchanger");
        yield return new WaitForSeconds(1.0f);
        index++;
        
        if (index >= TouchCheckItems.Count)
        {
            TaskTouchableState(TouchableTaskState = TouchableState.ItemsComplete);
            itemsComplete = true;   
        }
        else if(index<= TouchCheckItems.Count)
        {
            TaskTouchableState(TouchableTaskState = TouchableState.Start); 
        }
    }
    
    public enum TouchableState
    {
        None,
        Start,
        ItemsComplete,
        Complete,
    }
    
}

