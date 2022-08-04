using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour 
{
    public ManagerState managerState;

    [Tooltip("UI Manager Objesi atılır")]
    public UIManager UIManager;

    [Header("Task Itemlarin Listesi ")]
    public List<TaskItem> TaskItems = new List<TaskItem>();

    //TaskItems Sirasi
    [HideInInspector]
    public int currentIndex;
    
    //Timer
    [Header("Task tamamlanınca beklenen süre")]
    [Tooltip("Bir task tamamlandıktan sonra beklenen süre")]
    public int taskChangeTime;
    
    [Header("İlk taskı başlatmadan önce geçen süre")]
    [Tooltip("Eğitim ilk açıldığında info sound'un bitme süresi")]
    public int startTime;
    
    private float clicked =0f;
    private float clicktime =0f;
    private float clickdelay=0.5f;
    [Header("Mouse imlecinin kapanma süresi")]
    [Tooltip("Cursor bu süreden sonra kilitlenir")]
    public float lockTime;

    public AudioSource AudioSource;
    public AudioClip CorrectSound;

    //Booleans
    private bool once = false;
    private bool doubleClick = false;


    private void Start()
    {
        if (managerState != ManagerState.Start)
        {
            CurrentTaskState(ManagerState.None);
        }
    }

    private void Update()
    {
        DoubleClick();
        if (doubleClick)
        {
            Cursor.lockState = CursorLockMode.None;
            
        }
        else if (!doubleClick)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void LogOutCurrentScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    #region Manager State
    
    //Manager State
    private void CurrentTaskState(ManagerState newstate)
    {
        managerState = newstate;
       
        switch (newstate)
        {
            case ManagerState.None:
                UIManager.enabled = true;
                UIManager.active = true;
                UIManager.audioSource.clip = UIManager.UISounds[0].clip;
                StartCoroutine(ManagerStateUpdate());
                if (!once)
                {
                    if (UIManager.audioSource.isPlaying)
                    {
                        UIManager.audioSource.Pause();
                        //UIManager.audioSource.Stop();
                    }
                    else
                    {
                        UIManager.audioSource.Play();
                        once = true;
                    }
                }
                
                break;
            case ManagerState.Start:

                //Start panelini kapatiyoruz
                UIManager.startPanelAnimator.SetBool("Open",false);

                TaskItems[currentIndex].active = true;

                // UIManagerda Tasklarin info textlerini yazdiriyoruz
                if (TaskItems[currentIndex].infoText != null)
                {
                    UIManager.InfoTextSetter(TaskItems[currentIndex].infoText,true);
                }
                
                // UIManager audio source a task item soundlarini gonderiyoruz
                if (TaskItems[currentIndex].infoSound != null)
                {
                    UIManager.audioSource.clip = TaskItems[currentIndex].infoSound;
                    //UI Manager Audio Source
                    if (UIManager.audioSource.isPlaying)
                    {
                        UIManager.audioSource.Pause();
                    }
                    else
                    {
                        UIManager.audioSource.Play();
                    }
                }
                
                //Yeni geçtiğimiz tasktan Complete Eventini dinlemeye başlıyoruz
                TaskItems[currentIndex].TaskItemComplete += TaskItemUpdate;
                
                break;
            case ManagerState.Completed:
                
                //Kullaniciya Ui uzerinden Tasklarin bittigini bildiriyoruz
                UIManager.InfoTextSetter("Tebrikler. Görevleri başarı ile tamamladınız.",true);
                UIManager.audioSource.clip = UIManager.UISounds[1].clip;
                if (UIManager.audioSource.isPlaying)
                {
                    UIManager.audioSource.Pause();
                }
                else
                {
                    UIManager.audioSource.Play();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newstate), newstate, null);
        }
    }
    #endregion
    
    IEnumerator ManagerStateUpdate()
    {
        yield return new WaitForSeconds(startTime);
        CurrentTaskState(newstate: ManagerState.Start);
    }

    private void TaskItemUpdate()
    {
        //Gelen Event ile birlikte bir önceki Taskın bittiğini anlıyoruz ve
        //Tamamlanan tasktan event dinlemeyi bırakıyoruz
        TaskItems[currentIndex].TaskItemComplete -= TaskItemUpdate;

        AudioSource.PlayOneShot(CorrectSound);

        //UI Managerdaki methoda info texti degistiriyoruz
        if (TaskItems[currentIndex].infoText != null)
        {
            UIManager.taskInfoPanelAnimator.SetBool("Open",false);
        }
        
        // Coroutine çalıştırılarak Current indexi artırıyoruz
        StartCoroutine(Counter());
    }

    private IEnumerator Counter()
    {
        yield return new WaitForSeconds(taskChangeTime);
        //Current indexi arttırarak bir sonraki Taska geçiyoruz
        currentIndex++;
        
        //Başka Task yok ise Manager State i Complete e çekiyoruz
        if (currentIndex >= TaskItems.Count)
        {
            CurrentTaskState(ManagerState.Completed);
            UIManager.taskIndexPanel.SetActive(false);
        }
        else
        {
            CurrentTaskState(ManagerState.Start);
            
            //UIManager Task Index degistikce animasyonu calistiriyoruz
            UIManager.TaskIndexAnimation.Play();
            UIManager.IndexChanger();
        }
    }

    // Kullanıcı çift tıkladığı zaman clicktime kadar mouse görünür olur ve kilitli kalmaz 
    private void DoubleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clicked++;
            if (clicked == 1)
            {
                clicktime = Time.time;
            }
        }
    
        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            clicked = 0;
            clicktime = 0;
            doubleClick = true;
            StartCoroutine(Timer());
        }
        else if (clicked > 2 || Time.time - clicktime > 1) 
            clicked = 0;
        
    }
    
    public enum ManagerState
    {
        None,
        Start,
        Completed,
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(lockTime);
        doubleClick = false;
    }
}

