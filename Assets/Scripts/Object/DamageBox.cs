using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageBox : MonoBehaviour
{
    Action<Character, Character> OnHitAction;
    Action<Character, Character> OnDefenceAction;

    [SerializeField] private int sortingLayer;
    public int SortingLayer { get { return sortingLayer; } set { sortingLayer = value; } }


    [SerializeField]SkillDataManager.SkillData skillData;
    public SkillDataManager.SkillData SkillData { get { return skillData; } set { skillData = value; } }

    [SerializeField] int skillLevel;
    public int SkillLevel { get { return skillLevel; } set { skillLevel= value; } }

    public void InsertAction(Action<Character, Character> OnHitAction, Action<Character, Character> OnDefenceAction)
    {
        Debug.Log("액션 들어옴");
        this.OnHitAction = OnHitAction;
        this.OnDefenceAction = OnDefenceAction; 
    }
    public void OnHit(Character player, Character other)
    {
        if(other.GuardCheck())
        {
            other.TakeDefence(skillData.Damage + skillLevel*5);
            Debug.Log($"데미지 : {skillData.Damage + skillLevel * 5}");
            OnDefenceAction.Invoke(player, other);
        }
        else
        {
            other.TakeDamage(skillData.Damage + skillLevel * 5);
            Debug.Log($"데미지 : {skillData.Damage + skillLevel * 5}");

            OnHitAction.Invoke(player, other);
        }
    }
    //
    private void OnDestroy()
    {
        GameManager.Instance.PhysicsManager.DeleteDamageBox(this);
    }
}
