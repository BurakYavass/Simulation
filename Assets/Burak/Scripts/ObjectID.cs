using UnityEngine;

public class ObjectID : MonoBehaviour
{
    // GameObject ID
    public enum ObjectType
    {
        DragItem,
        CollidePlace,
        Button,
        EyeCheckItem,
        TouchObject,
        TouchObjectPlace,
        TouchCheckObject,
    }

    public ObjectType Type;
}
