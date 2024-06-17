using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] protected static GameObject skillListWindow;
    [SerializeField] protected static GameObject pauseBack;

    
    public GameObject PauseBack { get { return pauseBack; } }
    protected float prevTimeScale;
    // 테스트용 빌드
    [SerializeField] GameObject commandList;
    public void OnCommandList()
    {
        commandList.SetActive(!commandList.activeInHierarchy);
    }
    //



    private void Start()
    {
        if (Instance == null)
            Instance = this;
        if (pauseBack == null)
        {
            pauseBack = GameObject.Find("BlackScreen");
            if (skillListWindow == null)
            {
                skillListWindow = GameObject.Find("SkillListPannel");
                skillListWindow.SetActive(false);
            }
            if(commandList == null)
            {
                commandList = GameObject.Find("Command");
                commandList.SetActive(false);
            }
            pauseBack.SetActive(false);
        }

    }
   
  
    public void PauseButton(bool switchBool)
    {
        if(!switchBool)
        {
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = prevTimeScale;
        }
    }


    public void ExitButton()
    {
        SceneManager.LoadScene("TitleScene");
        PlayerData.Instance.DestroyData();
    }

    public void SkillListButton()
    {
        skillListWindow.SetActive(!skillListWindow.activeInHierarchy);
        skillListWindow.GetComponent<SkillPannel>().Initialize();
    }

    public void EscapeMode()
    {
        if (skillListWindow.activeInHierarchy)
        {
            skillListWindow.SetActive(!skillListWindow.activeInHierarchy);
        }
        else
        {
            PauseButton(pauseBack.activeInHierarchy);
            pauseBack.SetActive(!pauseBack.activeInHierarchy);
            
        }
    }

   
}
