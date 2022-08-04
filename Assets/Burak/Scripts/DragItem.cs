using System;
using UnityEngine;

public class DragItem : ObjectID
{
   
    private Collider _collider;

    public bool dropPlace = false;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void DragState(bool dragging)
    {
        _collider.enabled = !dragging;
    }

    public void DropPlaceCheck(bool dropping)
    {
        dropPlace = dropping;
        _collider.enabled = dropping;
    }
}
