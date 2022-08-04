using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TaskActions
{
    [System.Serializable]
    public class AnimationClass
    {
        [Header("Animations")] 
        public Animation animations;
        public TaskItem.AnimationsTaskState animationsTaskState;
        public bool playAnimationSynchron;
    }
    
    [System.Serializable]
    public class AnimatorClass
    {
        public Animator animators;
        public string AnimatorParameter;
        public bool AnimatorParameterBool;
    }
    
    [System.Serializable]
    public class SoundClass
    {
        public AudioSource Sounds;
        public TaskItem.SoundTaskState SoundTaskState;
    }

    [System.Serializable]
    public class ObjectClass
    {
        public GameObject objectT;
        public bool closeOpen;
    }

    public ObjectClass[] Objects;
    public SoundClass[] Sounds;
    public AnimationClass[] Animations;
    public AnimatorClass[] Animators;
    public UnityEvent UnityEvent;
}
