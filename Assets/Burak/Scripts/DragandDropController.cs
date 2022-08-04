using System;
using UnityEngine;

public class DragandDropController : MonoBehaviour
{
    public DragDropState Dr_DropState;
    
    private DragItem _dragItemSc;
    private DropPlace _dropPlaceSc;
    private Transform toDrag;
    private Vector3 toDragFirstPos;
    
    private Camera Camera;
    private Vector3 camPos;
    private Vector3 touchPosition;
    private Vector3 offSet;

    private bool once = false;
    private bool dragging = false;
    private bool dropArea = false;
    public bool active = false;
    public bool complete = false;
    
    // Drag Item 
    public DragItem _dragItem;
    public Outline _dragItemOutline;
    public Collider _dragItemCollider;
                     
    //Drop Place     
    public DropPlace _dropPlace;
    public Outline _dropPlaceOutline;
    public Collider _dropPlaceCollider;
    public MeshRenderer _dropPlaceRenderer;
    
    private void Awake()
    {
        Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active && Dr_DropState != DragDropState.Start )
        {
            DragDropTaskState(DragDropState.None);
            once = true;
        }
        
        if (active && Dr_DropState == DragDropState.None)
        {
            if (once)
            {
                DragDropTaskState(DragDropState.Start);
                once = false;
            }
            ObjectMovement();
        }
        
        if (active && _dragItem.dropPlace)
        {
            if (!once)
            {
                DragDropTaskState(DragDropState.Complete);
                once = true;
            }
        }
    }

    private void ObjectMovement()
    {
        if (Input.touchCount > 0)
        {
            TouchController();
        }
        else
        {
            MouseController();
        }
    }
    
    private void DragDropTaskState(DragDropState dragDropState)
    {
        switch (dragDropState)
        {
            case DragDropState.None:
                //DragItem
                _dragItem.enabled = false;
                _dragItemCollider.enabled = false;
                _dragItemOutline.enabled = false;
                //DropPlace
                _dropPlace.enabled = false;
                _dropPlaceCollider.enabled = false;
                _dropPlaceOutline.enabled = false;
                _dropPlaceRenderer.enabled = false;
                
                break;
            case DragDropState.Start:

                //DragItem
                _dragItem.enabled = true;
                _dragItemCollider.enabled = true;
                _dragItemOutline.enabled = true;
                
                //DropPlace
                _dropPlace.enabled = true;
                _dropPlaceCollider.enabled = true;
                _dropPlaceOutline.enabled = true;
                _dropPlaceRenderer.enabled = true;
                
                complete = false;
                break;
            case DragDropState.Complete:

                //DragItem
                _dragItem.enabled = false;
                _dragItemCollider.enabled = false;
                _dragItemOutline.enabled = false;
                
                //DropPlace
                _dropPlace.enabled = false;
                _dropPlaceCollider.enabled = false;
                _dropPlaceOutline.enabled = false;
                _dropPlaceRenderer.enabled = false;
                
                complete = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dragDropState), dragDropState, null);
        }
    }
    
    #region Mouse Control
    
    private void MouseController()
    {
        Vector3 mousePos;
        camPos = Camera.transform.position;
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1;
        
            if (Physics.Raycast(ray,out hit1))
            {
                _dragItemSc = hit1.transform.GetComponent<DragItem>();
        
                if (_dragItemSc != null && _dragItemSc.Type == ObjectID.ObjectType.DragItem)
                {
                    //Hit ettigimiz objeyi toDrag objesine atiyoruz
                    toDrag = hit1.transform;
                    toDragFirstPos = toDrag.transform.position;
                    dragging = true;
                    
                    //Alinan objeye alindigini bildiriyor
                    _dragItemSc.DragState(true);
                    
                    Vector3 screenPoint = Camera.WorldToScreenPoint(hit1.point);
                    mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                    mousePos = Camera.ScreenToWorldPoint(mousePos);

                    offSet = mousePos - hit1.point;
                    

                    dropArea = false;
                    _dropPlaceSc = null;
                    _dragItemSc.DropPlaceCheck(false);
                }
                else
                {
                    dragging = false;
                    _dragItemSc = null;
                }
            }
        }
        
        if (Input.GetMouseButton(0))
        {
            
            // Drag Item move
            if (dragging)
            {
                Vector3 screenPoint = Camera.WorldToScreenPoint(toDrag.transform.position);
                mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                mousePos = Camera.ScreenToWorldPoint(mousePos);
                
                //Drag object localposition change
         
                toDrag.position = mousePos - offSet;
            }
            
            //Objeyi birakabilecegimiz alandamiyiz diye RAY ile kontrol ediyoruz 
            Ray ray2 = Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit2;
            if (Physics.Raycast(ray2,out hit2))
            {
                if(_dropPlaceSc == null)
                    _dropPlaceSc = hit2.transform.GetComponent<DropPlace>();
                
                if (_dropPlaceSc != null && _dropPlaceSc.Type == ObjectID.ObjectType.CollidePlace)
                {
                    dropArea = true;
                }
                else
                {
                    dropArea = false;
                    _dropPlaceSc = null;
                }
            }
            else
            {
                _dropPlaceSc = null;
                dropArea = false;
            }
        
        }

        else if (Input.GetMouseButtonUp(0))
        {
            if (_dropPlaceSc == null && _dragItemSc == null)
                return;

            if (dropArea)
            {
                dropArea = false;
                dragging = false;
                
                if (_dragItemSc != null)
                {
                    toDrag.transform.position = _dropPlaceSc.transform.position;
                    _dragItemSc.DropPlaceCheck(true);
                    _dragItemSc.DragState(false);
                }

                _dragItemSc = null;
                _dropPlaceSc = null;
            }
            else if(!dropArea && dragging)
            {
                dragging = false;
                
                toDrag.transform.position = Vector3.Lerp(toDrag.transform.position,toDragFirstPos,1);

                if (_dragItemSc != null)
                {
                    _dragItemSc.DropPlaceCheck(false);
                    _dragItemSc.DragState(false);
                }

                _dragItemSc = null;
                _dropPlaceSc = null;
            }
        }
    }
    #endregion

    #region Touch Countrol
    
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
                    _dragItemSc = hit1.transform.GetComponent<DragItem>();

                if (_dragItemSc != null && _dragItemSc.Type == ObjectID.ObjectType.DragItem)
                {
                    Debug.Log("Obje Alindi");
                    //Hit ettigimiz objeyi toDrag objesine atiyoruz
                    toDrag = hit1.transform;
                    dragging = true;
                    
                    //Alinan objeye alindigini bildiriyor
                    _dragItemSc.DragState(true);
                    
                    Vector3 screenPoint = Camera.WorldToScreenPoint(hit1.point);
                    touchpos = new Vector3(touch.position.x, touch.position.y, screenPoint.z);
                    touchpos = Camera.ScreenToWorldPoint(touchpos);

                    offSet = touchpos - hit1.point;

                    dropArea = false;
                    _dropPlaceSc = null;
                    _dragItemSc.DropPlaceCheck(false);
                }
                else
                {
                    dragging = false;
                    _dragItemSc = null;
                }
            }
            
        }
    
        if (touch.phase == TouchPhase.Stationary||touch.phase == TouchPhase.Moved)
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
            if (Physics.Raycast(ray2, out hitTest2))
            {
                _dropPlaceSc = hitTest2.transform.GetComponent<DropPlace>();
    
                if (_dropPlaceSc != null && _dropPlaceSc.Type == ObjectID.ObjectType.CollidePlace)
                {
                    dropArea = true;
                }
                else
                {
                    dropArea = false;
                    _dropPlaceSc = null;
                }
            }
            else
            {
                _dropPlaceSc = null;
                dropArea = false;
            }
            
        }
    
        if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            if (_dropPlaceSc == null && _dragItemSc == null)
                return;

            if (dropArea)
            {
                dropArea = false;
                dragging = false;
                
                if (_dragItemSc != null)
                {
                    toDrag.transform.localPosition = _dropPlaceSc.transform.localPosition;
                    _dragItemSc.DropPlaceCheck(true);
                    _dragItemSc.DragState(false);
                }

                _dragItemSc = null;
                _dropPlaceSc = null;
            }
            else if(!dropArea && dragging)
            {
                dragging = false;
                dropArea = false;
                
                toDrag.transform.localPosition = Vector3.Lerp(toDrag.transform.localPosition,toDragFirstPos,1);

                if (_dragItemSc != null)
                {
                    
                    _dragItemSc.DropPlaceCheck(false);
                    _dragItemSc.DragState(false);
                }

                _dragItemSc = null;
                _dropPlaceSc = null;
            }
            
        }
    }
    #endregion
    
    public enum DragDropState
    {
        None,
        Start,
        Complete,
    }
    
}

