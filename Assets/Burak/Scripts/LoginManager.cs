using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    private EventSystem system;
    public Selectable firstinput;
    public GameObject LoginPanel;
    public GameObject TasksPanel;
    public GameObject WarningPanel;
    
    [Header("Login Panel Input Fields")]
    public InputField userName;
    public InputField passWord;

    [Header("Task panel Texts")] 
    public Text loginUserNameTxt;
    public Text loginUserSurNameTxt;
    public Text loginUserFacilityNameTxt;
    public Text loginUserJobTxt;
    
    public Animation WarningPanelAnimation;

    [Tooltip("Warning panel gösterim süresi")]
    public int logWarningTime;
    
    // Start is called before the first frame update
    void Start()
    {
        WarningPanel.SetActive(false);
        //PlayerPrefs.GetString("user");
        if (PlayerPrefs.HasKey("user"))
        {
            LoginPanel.SetActive(false);
            TasksPanel.SetActive(true);
        }
        else
        {
            LoginPanel.SetActive(true);
            TasksPanel.SetActive(false);
        }
        
        system = EventSystem.current;
        firstinput.Select();
    }

    public void LogOut()
    {
        PlayerPrefs.DeleteKey("user");
        LoginPanel.SetActive(true);
        TasksPanel.SetActive(false);
    }

    public void LoginButton()
    {
        if (userName.text == "amazoi" && passWord.text == "2022")
        {
            LoginPanel.SetActive(false);
            TasksPanel.SetActive(true);
            PlayerPrefs.SetString("user",userName.text);
        }
        else if (userName.text != "amazoi" || passWord.text != "2022" )
        {
            WarningPanelAnimation.Play("Log_War_OpenAnim");
            WarningPanel.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(Timer());
            //PlayerPrefs.SetString("user",userName.text);
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1.0f);
        WarningPanelAnimation.Play("Log_War_CloseAnim");
        
        yield return new WaitForSeconds(logWarningTime);
        
        WarningPanel.SetActive(false);
    }

    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void ShowPass()
    {
        Debug.Log("Change Content Type");
        var ContentType = InputField.ContentType.Standard;

        if (passWord.contentType == InputField.ContentType.Password)
        {
            passWord.contentType = ContentType;
            passWord.ForceLabelUpdate();
        }
        else
        {
            passWord.contentType = InputField.ContentType.Password;
            passWord.ForceLabelUpdate();
        }
    }

    public void ChangeLine()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next!=null)
            {
                next.Select();
            }
        }
        
    }
}
