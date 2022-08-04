using System;
using UnityEngine;

public class ButtonScript : ObjectID 
{
    public BState bState;

    public Animator buttonAnimotor;
    public Collider _collider;
    public Outline _outline;
    private Camera _camera;
    
    
    public bool active = false;
    public bool complete = false;
    private bool once = false;

    private void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active && bState != BState.Start)
        {
            ButtonState(BState.None);
            once = true;
        }
        
        if (active && bState == BState.None)
        {
            if (once)
            {
                ButtonState(BState.Start);
                once = false;
            }
        }

        if (Input.touchCount > 0)
        {
            TouchControl();
        }
        else
        {
            MouseControl();
        }
    }

    private void ButtonState(BState state)
    {
        switch (state)
        {
            case BState.None:
                _collider.enabled = false;
                _outline.enabled = false;
                break;
            case BState.Start:
                _collider.enabled = true;
                _outline.enabled = true;
                break;
            case BState.Complete:
                _collider.enabled = false;
                _outline.enabled = false;
                complete = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    private void MouseControl()
    {
        if (Input.GetMouseButton(0))
        {
            if (_camera != null)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray,out hit))
                {
                    var hitItem = hit.collider.GetComponent<ObjectID>();
                    if (hitItem != null && hitItem.Type==ObjectType.Button)
                    {
                        buttonAnimotor.SetTrigger("Click");
                        ButtonState(bState = BState.Complete);
                    }
                }
            }
        }
    }

    private void TouchControl()
    {
        Touch touch = Input.GetTouch(0);
        
        if (touch.phase == TouchPhase.Began)
        {
            if (_camera != null)
            {
                Ray ray = _camera.ScreenPointToRay(touch.position);
                RaycastHit hit;
                
                if (Physics.Raycast(ray,out hit))
                {
                    var hitItem = hit.collider.GetComponent<ObjectID>();
                    if (hitItem != null && hitItem.Type==ObjectType.Button)
                    {
                        buttonAnimotor.SetTrigger("Click");
                        ButtonState(bState = BState.Complete);
                    }
                }
            }
        }
    }
    
    public enum BState
    {
        None,
        Start,
        Complete,
    }
}