using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class PhysicsManager : MonoBehaviour
{
    // battle rigidbody가 생성되면 physicsManager가 관리.
    // 관리 하려면 알고있어야한다.
    // 왔다고 알려줄 수 있는 기능.
    // battleRigidbody가 생성됬을때 이 리스트에 등록하기.
    Action<float> physicsUpdate;
    Action<BattleRigidbody2D> characterCollision;
    [SerializeField] List<BattleRigidbody2D> battleRigidbodies;
    [SerializeField] Vector3 stageSize;
    [SerializeField] float fullStageSize;
   
    public float FullStageSize { get { return fullStageSize; } }
    [SerializeField] float maxCharacterDistance;
    public const float gravityScale = 9.8f;
    public Vector3 StageSize { get { return stageSize; } set { stageSize = value; } } // 필드사이즈는 나중에 따로 필드 매니저를 만들어야할듯.
    public float MaxCharacterDistance { get { return maxCharacterDistance; } }
    // 히트박스를 가져오기.
    [SerializeField]private List<HitBox> hitBoxList;

    // 데미지 박스를 가져오기.
    [SerializeField]private List<DamageBox> damageBoxList;


    private void FixedUpdate()
    {
        physicsUpdate?.Invoke(Time.fixedDeltaTime);
        int damageBoxCount = damageBoxList.Count;
        int hitBoxCount = hitBoxList.Count;
        // 현재 생성되어있는 데미지 박스만큼 반복을 돌것.
        foreach(DamageBox damageBox in damageBoxList)
        //for(int i = 0; i < damageBoxList.Count; i++)
        {
            // 히트박스에 맞았는지 안맞았는지 체크. 이 히트박스는 묶음단위의 히트박스.
            foreach (HitBox hitBox in hitBoxList)
            //for(int j = 0; j < hitBoxList.Count; j++)
            {
                //hitBoxList[j].CheckDamageBox(damageBoxList[i]);

                hitBox.CheckDamageBox(damageBox);
            }
        }
        // 등록되어있는 battleRigidBody2D들을 매개변수로 넣고 characterCollision을 작동시킴.
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
