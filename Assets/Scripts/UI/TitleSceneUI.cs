using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneUI : MonoBehaviour
{
    private void Update()
    {
        if(Input.anyKeyDown)
        {
            StartButton();
        } 
    }
    public void StartButton()
    {
        SceneManager.LoadScene("SelectScene");
    }
}
