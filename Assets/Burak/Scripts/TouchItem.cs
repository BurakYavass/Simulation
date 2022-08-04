using System;
using UnityEngine;

public class TouchItem : ObjectID
{
   
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    
}
