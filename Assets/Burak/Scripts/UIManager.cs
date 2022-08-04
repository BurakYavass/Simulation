using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TaskManager TaskManager;
    
    public bool active = false;

    [Header("Panels")]
    public GameObject startInfoPanel;
    public GameObject taskInfoPanel;
    public GameObject gamePanel;
    //public GameObject gameInfoPanel;
    public GameObject taskIndexPanel;

    [Header("Task Info Text")]
    [Tooltip("Info panelin altındaki , TaskInfo buraya atılır")]
    public Text _taskInfoText;
    
    [Header("Task Index Texts")]
    [Tooltip("Task Index Panel altındaki text içeren objeler atılır")]
    public Text _totalIndexText;
    public Text _indexLeftTxt1;
    public Text _indexLeftTxt2;
    public Text _indexCenterTxt;
    public Text _indexRightTxt1;
    public Text _indexRightTxt2;
    public Text _indexRightSmall;
    
    [Header("Animator")] 
    public Animator startPanelAnimator;
    public Animator taskInfoPanelAnimator;
    //public Animator gameInfoPanelAnimator;
    
    [Header("UI TaskIndex Animation")]
    public Animation TaskIndexAnimation;
    
    //Audios
    public AudioSource audioSource;
    public Sound[] UISounds;
    
    //Booleans
    private bool StartPanelOn = false;
    private bool startButton = false;
    private bool once = false;
    //public bool InfoPanelOn = false;

    private void Start()
    {
        startInfoPanel.SetActive(false);
        gamePanel.SetActive(false);
        //gameInfoPanel.SetActive(false);
        taskInfoPanel.SetActive(false);

        _indexLeftTxt2.text = "";
        _indexLeftTxt1.text = "";
        var index = TaskManager.currentIndex;
        index += 1;
        _indexCenterTxt.text = index.ToString();
        index += 1;
        _indexRightTxt1.text = index.ToString();
        index += 1;
        _indexRightTxt2.text = index.ToString();
    }

    private void Update()
    {
        if (active)
        {
            if (!once)
            {
                startInfoPanel.SetActive(true);
                startPanelAnimator.SetBool("Open",true);
                StartPanelOn = true;
                _totalIndexText.text = TaskManager.TaskItems.Count.ToString();
                once = true;
            }
            
            if (TaskManager.managerState == TaskManager.ManagerState.Start)
            {
                startInfoPanel.SetActive(false);
                StartPanelOn = false;
                gamePanel.SetActive(true);
            }
        }
    }

    public void InfoTextSetter(string Text , bool Open)
    {
        _taskInfoText.text = Text;
        taskInfoPanelAnimator.SetBool("Open",Open);
        taskInfoPanel.SetActive(Open);
    }
    
    public void LoginMenuButton(string SceneName)
    {
        TaskManager.LogOutCurrentScene(SceneName);
    }
    
    public void InfoLink()
    {
        Debug.Log("ImageLink");
        Application.OpenURL("www.google.com");
    }

    public void IndexChanger()
    {
        if (!gamePanel)
        {
            gamePanel.SetActive(enabled);
        }
        
        int currentIndex = TaskManager.currentIndex;
        if (currentIndex==1)
        {
            _indexLeftTxt1.text = "";
            
            _indexCenterTxt.text = (currentIndex).ToString();

            _indexRightTxt1.text = (currentIndex+1).ToString();
            
            _indexRightTxt2.text = (currentIndex+2).ToString();
            
            _indexRightSmall.text = (currentIndex+3).ToString();
        }
        else if (currentIndex>=2)
        {
            _indexLeftTxt1.text = (currentIndex-1).ToString("D");

            _indexCenterTxt.text = (currentIndex).ToString();

            _indexRightTxt1.text = (currentIndex+1).ToString();
            
            _indexRightTxt2.text = (currentIndex+2).ToString();
            
            _indexRightSmall.text = "";
            
        }
        // else if(currentIndex ==3)
        // {
        //     _indexLeftTxt1.text = (currentIndex-1).ToString("D");
        //
        //     _indexCenterTxt.text = (currentIndex).ToString();
        //
        //     _indexRightTxt1.text = (currentIndex+1).ToString();
        //
        //     _indexRightTxt2.text = "";
        //     
        //     _indexRightSmall.text = "";
        // }
      
    }
}
