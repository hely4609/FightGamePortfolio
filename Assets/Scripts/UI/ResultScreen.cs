using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stateText;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] GameObject[] posterArray = new GameObject[3];
    [SerializeField] Transform skillArea;
    GameObject skillPrefab;
    private bool[] clearData = new bool[3];
    [SerializeField] private List<SkillDataManager.SkillData> skillDataList;

    // Start is called before the first frame update
    void Start()
    {
        skillPrefab = Resources.Load<GameObject>("Prefabs/ResultSkillCard");
    }

    public void Initialize()
    {
        clearData = PlayerData.Instance.ClearData;
        int clear = 0;
        for (int i = 0; i < clearData.Length; i++)
        {
            if (clearData[i])
            {
                DisableButton(posterArray[i]);
                clear++;
            }
        }
        if(clear == clearData.Length)
        {
            titleText.text = "Game Clear";
            stateText.text = "축하합니다!";
        }
        skillDataList = PlayerData.Instance.SkillDataList;
        for (int i = 0; i < skillDataList.Count; i++)
        {
            GameObject skillCardObject = Instantiate(skillPrefab, skillArea);
            skillCardObject.GetComponent<Card>().Initialize(skillDataList[i], false);
        }
        
    }

    public void DisableButton(GameObject buttonImage)
    {
        buttonImage.GetComponent<Poster>().Cleared();
    }
}
