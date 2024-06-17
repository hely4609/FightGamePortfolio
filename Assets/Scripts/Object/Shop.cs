using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]List<SkillDataManager.SkillData> shopList = new List<SkillDataManager.SkillData>();
    public List<SkillDataManager.SkillData> ShopList { get { return shopList; } }

    // test
    public string skillName;

    private void Start()
    {
        
        //shopList.Add(SkillDataManager.Punch);
        //shopList.Add(SkillDataManager.Kick);
        
        shopList.Add(SkillDataManager.UpperPunch);
        
        shopList.Add(SkillDataManager.ReverseKick);
        shopList.Add(SkillDataManager.CutKick);
        shopList.Add(SkillDataManager.StampingPunch);
        
    }

    public SkillDataManager.SkillData DrawSkill()
    {
        SkillDataManager.SkillData skill = shopList[Random.Range(0, shopList.Count)];
        Debug.Log(skill.Name);
        return skill;
    }

}
