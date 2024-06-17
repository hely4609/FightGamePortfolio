using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] protected static GameObject winBack;
    [SerializeField] protected static GameObject rewardPannel;

    [SerializeField] protected static GameObject resultBack;
    [SerializeField] protected static GameObject restartPannel;


    GameObject rewardCardPrefab;
    Transform shopArea; 
    public GameObject WinBack { get { return winBack; } }
    protected float prevTimeScale;
    // 테스트용 빌드
    [SerializeField] GameObject commandList;
    Shop shop;
    

    private void Start()
    {
        if (Instance == null)
            Instance = this;
            rewardCardPrefab = Resources.Load("Prefabs/RewardCard") as GameObject;
        
        if (winBack == null)
        {
            winBack = GameObject.Find("WinScreen");
            if (rewardPannel == null)
            {
                rewardPannel = GameObject.Find("RewardPannel");
                shopArea = rewardPannel.GetComponentInChildren<WinRewardSorting>().transform;
                shop = PlayerData.Instance.GetComponent<Shop>();
            }
            winBack.SetActive(false);

            resultBack = GameObject.Find("ResultScreen");
            if (commandList == null)
            {
                commandList = GameObject.Find("RestartPannel");
            }
            resultBack.SetActive(false);
        }

    }

    public void WinResult()
    {
        winBack.SetActive(true);
        for(int i = 0; i<3; i++)
        {
            ShopItemDraw();
        }
    }

    private void ShopItemDraw()
    {
        GameObject skillCardObject = Instantiate(rewardCardPrefab, shopArea);
        SkillDataManager.SkillData skillData = shop.DrawSkill();
        skillCardObject.GetComponentInChildren<Card>().Initialize(skillData);
        if(PlayerData.Instance.SkillDataList.Contains(skillData))
        {
            skillCardObject.GetComponent<RewardCard>().textBlock.text = "공격력 +5";
        }
    }


    public void ResultScene()
    {
        resultBack.SetActive(true);
        resultBack.GetComponent<ResultScreen>().Initialize();
    }

    public void SelectSceneButton()
    {
        SceneManager.LoadScene("SelectScene");
    }
    public void SelectSkillButton(SkillDataManager.SkillData skillData)
    {
        PlayerData.Instance.AddSkill(skillData);
        SelectSceneButton();
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("TitleScene");
        PlayerData.Instance.DestroyData();
    }
}
