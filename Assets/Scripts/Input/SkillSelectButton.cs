using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectButton : MonoBehaviour
{
    UIManager uiManager;
    Button button;
    SkillDataManager.SkillData skillData;
    Transform rewardCard;
    void Start()
    {
        rewardCard = transform.parent;
        skillData = rewardCard.GetComponentInChildren<Card>().SkillData;
        uiManager = GameObject.Find("MainUI").GetComponent<UIManager>();
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(ClickButton);
    }
    private void ClickButton()
    {
        uiManager.SelectSkillButton(skillData);
    }
}
