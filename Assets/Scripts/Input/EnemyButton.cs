using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyButton : MonoBehaviour
{
    public GameObject[] buttons = new GameObject[3];
    private bool[] clearData;

    private void Start()
    {
        clearData = PlayerData.Instance.ClearData;
        
        for (int i = 0; i < clearData.Length; i++)
        {
            if (clearData[i])
            {
                DisableButton(buttons[i]);
            }
        }
    }
    public void SelectButton(int number)
    {
        if (!clearData[number])
        {
            PlayerData.Instance.selectBattle = number;
            SceneManager.LoadScene("MainScene");
        }
    }
    public void DisableButton(GameObject buttonImage)
    {
        buttonImage.GetComponent<Poster>().Cleared();
    }

}
