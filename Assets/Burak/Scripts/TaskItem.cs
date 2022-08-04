using System;
using UnityEngine;

public class TaskItem : MonoBehaviour
{
    public event Action TaskItemComplete;
    public TaskState TaskItemState;

    public bool active;
    public bool complete=false;
    private bool once = false;

    [Header("Texts")]
    public string infoText;
    public string warningText;
    
    [Header("Sounds")]
    public AudioClip infoSound;
    public AudioClip correctSound;
    
    [Header("Tasks")]
    public TaskType Type;
    public ButtonScript taskButton;
    public DragandDropController taskDragDrop;
    public TaskEyeCheckControll taskEyeCheckControll;
    public TouchableController taskTouchableController;
    
    [Header("UI Progres Bar")]
    [Tooltip("Touchable taskı için , UI Game Panel altında bulunan EyeCheckProgresBar Objesi buraya atılır")]
    public GameObject EyeCheckProgresBar;

    [Header("Actions")]
    [Tooltip("Task başladığında gerçekleşecek aksiyonlar")]
    public TaskActions taskStartAction;
    [Tooltip("Task tamamlandığında gerçekleşecek aksiyonlar")]
    public TaskActions taskCompleteAction;

    private void Update()
    {
        if(!active)
            return;
        if (TaskItemState != TaskState.Complete)
        {
            CurrentTaskState(TaskState.Start);
        }
    }

    public void CurrentTaskState(TaskState TaskItem)
    {
        TaskItemState = TaskItem;
        
        switch (TaskItem)
        {
            case TaskState.Start:
                if (taskStartAction != null && !once)
                {
                    foreach (TaskActions.ObjectClass o in taskStartAction.Objects)
                    {
                        o.objectT.SetActive(o.closeOpen);
                    }
                    foreach (TaskActions.SoundClass s in taskStartAction.Sounds)
                    {
                        if (s.SoundTaskState == SoundTaskState.playSound)
                        {
                            s.Sounds.Play();
                        }
                        if (s.SoundTaskState == SoundTaskState.stopSound)
                        {
                            s.Sounds.Stop();
                        }
                    }

                    foreach (TaskActions.AnimatorClass animator in taskStartAction.Animators)
                    {
                        animator.animators.SetBool(animator.AnimatorParameter, animator.AnimatorParameterBool);
                    }

                    foreach (TaskActions.AnimationClass animation in taskStartAction.Animations)
                    {
                        if (animation.animationsTaskState == AnimationsTaskState.playAnimation)
                        {
                            animation.animations.Play();
                        }
                        if (animation.animationsTaskState == AnimationsTaskState.stopAnimation)
                        {
                            animation.animations.Stop();
                        }
                    }
                    if (taskStartAction.UnityEvent != null)
                        taskStartAction.UnityEvent.Invoke();

                    once = true;
                }
                
                if (Type == TaskType.Button)
                {
                    TypeState(TaskType.Button);
                }
                
                if (Type == TaskType.DragDrop)
                {
                    TypeState(TaskType.DragDrop);
                }

                if (Type == TaskType.EyeCheck)
                {
                    TypeState(TaskType.EyeCheck);
                }

                if (Type == TaskType.TouchableObjects)
                {
                    TypeState(TaskType.TouchableObjects);
                }

                if (Type == TaskType.PushAndPull)
                {
                    TypeState(TaskType.PushAndPull);
                }
                break;
            
            case TaskState.Complete:
                TaskItemComplete?.Invoke();
                if (taskCompleteAction != null)
                {
                    foreach (TaskActions.ObjectClass o in taskCompleteAction.Objects)
                    {
                        o.objectT.SetActive(o.closeOpen);
                    }
                    foreach (TaskActions.SoundClass s in taskCompleteAction.Sounds)
                    {
                        if (s.SoundTaskState == SoundTaskState.playSound)
                        {
                            s.Sounds.Play();
                        }
                        if (s.SoundTaskState == SoundTaskState.stopSound)
                        {
                            s.Sounds.Stop();
                        }
                    }

                    foreach (TaskActions.AnimatorClass animator in taskCompleteAction.Animators)
                    {
                        animator.animators.SetBool(animator.AnimatorParameter, animator.AnimatorParameterBool);
                    }

                    foreach (TaskActions.AnimationClass animation in taskCompleteAction.Animations)
                    {
                        if (animation.animationsTaskState == AnimationsTaskState.playAnimation)
                        {
                            animation.animations.Play();
                        }
                        if (animation.animationsTaskState == AnimationsTaskState.stopAnimation)
                        {
                            animation.animations.Stop();
                        }
                    }

                    if (taskCompleteAction.UnityEvent != null)
                        taskCompleteAction.UnityEvent.Invoke();
                }
                complete = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(TaskItem), TaskItem, null);
        }
    }

    // Task tiplerine gore gecis yapiyoruz
    private void TypeState(TaskType TypeState)
    {
        //Type = TypeState;
        switch (TypeState)
        {
            case TaskType.DragDrop:
                taskDragDrop.enabled = true;
                taskDragDrop.active = true;
                
                if (taskDragDrop.complete)
                {
                    active = false;
                    taskDragDrop.active = false;
                    taskDragDrop.enabled = false;
                    CurrentTaskState(TaskState.Complete);
                }
                break;
            
            case TaskType.Button:
                taskButton.enabled = true;
                taskButton.active = true;

                if (taskButton.complete)
                {
                    active = false;
                    taskButton.active = false;
                    taskButton.enabled = false;
                    CurrentTaskState(TaskState.Complete);
                    once = false;
                }
                break;
            
            case TaskType.EyeCheck:
                taskEyeCheckControll.enabled = true;
                taskEyeCheckControll.active = true;
                EyeCheckProgresBar.SetActive(true);
                
                if (taskEyeCheckControll.complete)
                {
                    active = false;
                    taskEyeCheckControll.active = false;
                    //askEyeCheckControll. = false;
                    taskEyeCheckControll.enabled = false;
                    CurrentTaskState(TaskState.Complete);
                    EyeCheckProgresBar.SetActive(false);
                }
                break;
            
            case TaskType.TouchableObjects:
                taskTouchableController.enabled = true;
                taskTouchableController.active = true;

                if (taskTouchableController.complete)
                {
                    active = false;
                    taskTouchableController.active = false;
                    taskTouchableController.enabled = false;
                    CurrentTaskState(TaskState.Complete);
                }
                break;
            
            case TaskType.PushAndPull:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(TypeState), TypeState, null);
        }
    }
    public enum TaskState
    {
        Start,
        Complete,
    }

    public enum TaskType
    {
        Button,
        DragDrop,
        EyeCheck,
        TouchableObjects,
        PushAndPull,
    }
    
    public enum SoundTaskState
    {
        playSound,
        stopSound,
    }

    public enum AnimationsTaskState
    {
        playAnimation,
        stopAnimation,
    }
}




 
    
    
