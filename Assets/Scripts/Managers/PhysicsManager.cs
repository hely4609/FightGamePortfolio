using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class PhysicsManager : MonoBehaviour
{
    // battle rigidbody�� �����Ǹ� physicsManager�� ����.
    // ���� �Ϸ��� �˰��־���Ѵ�.
    // �Դٰ� �˷��� �� �ִ� ���.
    // battleRigidbody�� ���������� �� ����Ʈ�� ����ϱ�.
    Action<float> physicsUpdate;
    Action<BattleRigidbody2D> characterCollision;
    [SerializeField] List<BattleRigidbody2D> battleRigidbodies;
    [SerializeField] Vector3 stageSize;
    [SerializeField] float fullStageSize;
   
    public float FullStageSize { get { return fullStageSize; } }
    [SerializeField] float maxCharacterDistance;
    public const float gravityScale = 9.8f;
    public Vector3 StageSize { get { return stageSize; } set { stageSize = value; } } // �ʵ������� ���߿� ���� �ʵ� �Ŵ����� �������ҵ�.
    public float MaxCharacterDistance { get { return maxCharacterDistance; } }
    // ��Ʈ�ڽ��� ��������.
    [SerializeField]private List<HitBox> hitBoxList;

    // ������ �ڽ��� ��������.
    [SerializeField]private List<DamageBox> damageBoxList;


    private void FixedUpdate()
    {
        physicsUpdate?.Invoke(Time.fixedDeltaTime);
        int damageBoxCount = damageBoxList.Count;
        int hitBoxCount = hitBoxList.Count;
        // ���� �����Ǿ��ִ� ������ �ڽ���ŭ �ݺ��� ����.
        foreach(DamageBox damageBox in damageBoxList)
        //for(int i = 0; i < damageBoxList.Count; i++)
        {
            // ��Ʈ�ڽ��� �¾Ҵ��� �ȸ¾Ҵ��� üũ. �� ��Ʈ�ڽ��� ���������� ��Ʈ�ڽ�.
            foreach (HitBox hitBox in hitBoxList)
            //for(int j = 0; j < hitBoxList.Count; j++)
            {
                //hitBoxList[j].CheckDamageBox(damageBoxList[i]);

                hitBox.CheckDamageBox(damageBox);
            }
        }
        // ��ϵǾ��ִ� battleRigidBody2D���� �Ű������� �ְ� characterCollision�� �۵���Ŵ.
        foreach (BattleRigidbody2D current in battleRigidbodies)
        {
            characterCollision?.Invoke(current);
        }
    }


    public void Initialize()
    {
        battleRigidbodies = new List<BattleRigidbody2D>();
        damageBoxList = new List<DamageBox>();
        hitBoxList = new List<HitBox>();
        maxCharacterDistance = 20;
        fullStageSize = 30;
        StageSize = new Vector3(maxCharacterDistance, 10, 0);
        
    }
    public void InsertRigidbody(BattleRigidbody2D rigidbody)
    {
        battleRigidbodies.Add(rigidbody);
        physicsUpdate += rigidbody.PhysicsUpdate;
        characterCollision += rigidbody.CharacterCollision;
    }
    public void DeleteRigidbody(BattleRigidbody2D rigidbody)
    {
        battleRigidbodies.Remove(rigidbody);
        physicsUpdate -= rigidbody.PhysicsUpdate;
        characterCollision -= rigidbody.CharacterCollision;
    }
    public void InsertHitBox(HitBox hitBox)
    {
        hitBoxList.Add(hitBox);
    }
    public void DeleteHitBox(HitBox hitBox)
    {
        hitBoxList.Remove(hitBox);
    }

    public void InsertDamageBox(DamageBox damageBox)
    {
        damageBoxList.Add(damageBox);
    }
    public void DeleteDamageBox(DamageBox damageBox)
    {
        damageBoxList.Remove(damageBox);
    }

}
