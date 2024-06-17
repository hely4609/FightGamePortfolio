using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private int shieldBallMax;
    private int shieldBallCurrent;
    [SerializeField] private List<SkillDataManager.SkillData> skillDataList = new List<SkillDataManager.SkillData>();
    public List<SkillDataManager.SkillData> SkillDataList { get { return skillDataList; } }

    [SerializeField] private List<int> skillLevelList = new List<int>();
    public List<int> SkillLevelDataList { get { return skillLevelList;} }

    public int ShieldBallCurrent { get { return shieldBallCurrent; } }
    public int ShieldBallMax { get { return shieldBallMax; } }

    private static PlayerData instance;
    public static PlayerData Instance { get { return instance; } }

    private bool[] clearData = new bool[3];
    public bool[] ClearData { get { return clearData; } set { clearData = value; } }


    public int selectBattle;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SkillInitialize();
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void Initialize(int ballMax, int ballCurrent, List<SkillDataManager.SkillData> skillDataList)
    {
        shieldBallMax = ballMax;
        shieldBallCurrent = ballCurrent;
        this.skillDataList = skillDataList;
    }

    private void SkillInitialize()
    {
        AddSkill(SkillDataManager.Guard);
        AddSkill(SkillDataManager.Punch);
        AddSkill(SkillDataManager.Kick);
        AddSkill(SkillDataManager.UpperPunch);
    }
    public void AddSkill(SkillDataManager.SkillData skillData)
    {
        if (SkillDataList.Contains(skillData))
        {
            int arrayNumber = skillDataList.FindIndex(data => data == skillData);
            skillLevelList[arrayNumber]++;
        }
        else
        {
            skillDataList.Add(skillData);
            skillLevelList.Add(1);
        }
    }

    public void DestroyData()
    {
        Destroy(gameObject);
    }
}
