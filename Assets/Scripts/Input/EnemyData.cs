using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum EnemyType
{
    Alpha, Bravo, Charlie
}


public class EnemyData : MonoBehaviour
{
    struct EnemySkillStruct
    {
        public EnemyType enemyType;
        public List<SkillDataManager.SkillData> enemySkillList;
        public List<int> enemySkillLevelList;
        
    }
    List<EnemySkillStruct> enemySkillStruct= new List<EnemySkillStruct>();

    // = new Dictionary<EnemyType, List<SkillDataManager.SkillData>>()
    private int shieldBallMax;
    private int shieldBallCurrent;
    private bool isReady = false;
    
    public int ShieldBallCurrent { get { return shieldBallCurrent; } }
    public int ShieldBallMax { get { return shieldBallMax; } }

    private EnemyType enemyType;
    public EnemyType EnemyType { get { return enemyType; } set { enemyType = value; } }

    public void Initialize(int ballMax, int ballCurrent)
    {
        shieldBallMax = ballMax;
        shieldBallCurrent = ballCurrent;
    }
    public List<SkillDataManager.SkillData> EnemySkillData(int enemyNumber)
    {
        return enemySkillStruct[enemyNumber].enemySkillList;
    }
    public List<int> EnemySkillLevelData(int enemyNumber)
    {
        return enemySkillStruct[enemyNumber].enemySkillLevelList;
    }

    public void EnemySkillInitialize()
    {
        if (!isReady)
        {
            EnemySkillStruct Alpha;
            Alpha.enemyType = EnemyType.Alpha;
            Alpha.enemySkillList = new List<SkillDataManager.SkillData>
            {
                SkillDataManager.Guard,
                SkillDataManager.Punch, 
                SkillDataManager.Kick,
                SkillDataManager.UpperPunch
            };
            Alpha.enemySkillLevelList = new List<int>
            {
                1,1,1,1
            };
            enemySkillStruct.Add(Alpha);


            EnemySkillStruct Bravo;
            Bravo.enemyType = EnemyType.Bravo;
            Bravo.enemySkillList = new List<SkillDataManager.SkillData>
            {
                SkillDataManager.Guard,
                SkillDataManager.Punch,
                SkillDataManager.Kick,
                SkillDataManager.UpperPunch
            };
            Bravo.enemySkillLevelList = new List<int>
            {
                1,1,1,1
            };
            enemySkillStruct.Add(Bravo);


            EnemySkillStruct Charlie;
            Charlie.enemyType = EnemyType.Charlie;
            Charlie.enemySkillList = new List<SkillDataManager.SkillData>
            {
                SkillDataManager.Guard,
                SkillDataManager.Punch,
                SkillDataManager.Kick,
                SkillDataManager.UpperPunch
            };
            Charlie.enemySkillLevelList = new List<int>
            {
                1,1,1,1
            };
            enemySkillStruct.Add(Charlie);

            isReady = true;
        }
    }
}
