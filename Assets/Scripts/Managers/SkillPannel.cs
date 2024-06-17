using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPannel : MonoBehaviour
{
    List<SkillDataManager.SkillData> skillList;
    [SerializeField]GameObject leftPage;
    [SerializeField]GameObject rightPage;
    GameObject skillcard;
    bool isActived = false;

    
    public void Initialize()
    {
        if (!isActived)
        {
            isActived = true;
            skillList = GameManager.Instance.Player.SkillDataList;
            skillcard = Resources.Load("Prefabs/SkillCard") as GameObject;
            SkillPageInstantiate();
        }
    }
    
    protected void SkillPageInstantiate()
    {
        for(int i = 0; i < skillList.Count; i++) 
        { 
            if(i < 5)
            {
                // 왼쪽 페이지
                GameObject skillCardObject = Instantiate(skillcard, leftPage.transform);
                skillCardObject.GetComponent<Card>().Initialize(skillList[i]);

            }
            else
            {
                //오른쪽 페이지

                GameObject skillCardObject = Instantiate(skillcard, rightPage.transform);
                skillCardObject.GetComponent<Card>().Initialize(skillList[i]);
            }
        }
    }
}
