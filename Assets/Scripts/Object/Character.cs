using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static SkillDataManager;

public enum CharacterStatus
{//       �������
    //���,  ������, �´� ��, ��, ���� �� , ���� ����
    Normal, OnAttack, OnHit, KO, Guard, GuardCrush
}

public class Character : MonoBehaviour
{
    ComboKey currentKey; // ���� Ű.
    [SerializeField] ComboSystem comboComponent; // �޺� �ý����� �갡 �������ֵ���.
    SkillDataManager.SkillData currentSkill; // ���� �ߵ��� ��ų. �̰� DamageBox�� �Ѱ��ٰ�.
    SkillDataManager.SkillData activeSkill; // �ߵ� �� ��ų
    public SkillDataManager.SkillData ActiveSkill { get { return activeSkill; } set { activeSkill = value; } }
    SkillDataManager.SkillData animationSkill; // �ߵ� ���� ��ų
    [SerializeField]List<SkillDataManager.SkillData> skillDataList = new List<SkillDataManager.SkillData>();
    [SerializeField]List<int> skillLevelList = new List<int>();
    public List<SkillDataManager.SkillData> SkillDataList { get { return skillDataList; } set { skillDataList = value; } }
    public List<int> SkillLevelList { get { return skillLevelList; } set { skillLevelList = value;} }


    float comboTime; // �޺� �ð�
    float inputTime; // ���� �ð�

    [SerializeField] float jumpPower = 10;

    [SerializeField] float tetanyTime;
    public float TetanyTime { get { return tetanyTime; } protected set { tetanyTime = value; } }

    // ü��
    [SerializeField] int maxHitPoint = 100;
    [SerializeField] int currentHitPoint;
    public int CurrentHitPoint { get { return currentHitPoint; } protected set { currentHitPoint = value; } }
    public float HitPointPercent { get { return (float)currentHitPoint / maxHitPoint; } }

    // �ǵ�
    [SerializeField] int shieldBallAmount; // �ִ� �ǵ� ����
    public int ShieldBallAmount { get { return shieldBallAmount; } protected set { shieldBallAmount = value; } }
    public int shieldBallCurrent { get; set; }  // ���� � ������ �ֳ�.

    
    [SerializeField] float shieldGaugeMax = 100;
    public float ShieldGaugeMax { get { return shieldGaugeMax; } set { shieldGaugeMax = value; } }
    [SerializeField] float shieldGauge;
    public float ShieldGauge { get { return shieldGauge; } set { shieldGauge = value; } }
    public float ShieldGaugePercent { get { return shieldGauge / shieldGaugeMax; } }



    public Character otherCharacter; // ��� �÷��̾� ����. ���� ������ �����Ҷ� ����� ��.
    [SerializeField]float otherPlayerVectorNorm;
    public float OtherPlayerVectorNorm { get { return otherPlayerVectorNorm; } }
    [SerializeField]private int sortingLayer; // ĳ���Ͱ� ���� ��Ʈ�ڽ��� sortingLayer
    public int SortingLayer { get { return sortingLayer; } set { sortingLayer = value; } }
    

    [SerializeField] CharacterStatus currentState = CharacterStatus.Normal; // ���� ����
    [SerializeField] float moveSpeed; // �ִ� �̼� 
    protected BattleRigidbody2D battleRigidbody; // �ӵ��� ��ġ �׷� ������ ó������ �༮.
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    [SerializeField] Vector2 preferedDirection;
    public Vector2 PreferedDirection { get { return preferedDirection; } set { preferedDirection = value; } } // �������ϴ� ����
    public Vector2 moveDirection { get; set; } // ���� ���� ����
    
    [SerializeField] bool isGround; // ���� �ִ�?
    [SerializeField] bool isGuard = false;
    bool isDead = false;
    public bool IsDead { get { return isDead; } }

    [SerializeField] Animator stickmanAnimator;
    public Animator StickmanAnimator { get { return stickmanAnimator; } }

    [SerializeField] SkeletonMecanim stickmanBone;
    public SkeletonMecanim StickmanBone { get { return stickmanBone; } }
    


    // �׽�Ʈ�� ����
    //public bool IsGuard {  get { return isGuard; }  }
    //[SerializeField] TextMeshProUGUI hpText;
    //[SerializeField] TextMeshProUGUI shieldText;
    //public void printText()
    //{ 
    //    hpText.text = "HP : " + CurrentHitPoint.ToString();
    //    shieldText.text = "Sheild : " +  shieldGauge.ToString();
        
    //}
    
    //

    private void Awake()
    {
        stickmanAnimator = GetComponentInChildren<Animator>();
        stickmanBone= GetComponentInChildren<SkeletonMecanim>();
        CurrentHitPoint = maxHitPoint;
        ShieldGauge = ShieldGaugeMax;
        shieldBallCurrent = shieldBallAmount;
    }
    void Start()
    {
        battleRigidbody = gameObject.GetComponent<BattleRigidbody2D>(); // ���� �ڵ� �־��ֱ�.

        if (skillDataList.Count != 0)
        {
            for (int i = 0; i < skillDataList.Count; i++)
            {
                AddSkill(skillDataList[i]);
            }
            comboComponent.Insert(SkillDataManager.Jump_Up);
            comboComponent.Insert(SkillDataManager.Jump_UpRight);
            comboComponent.Insert(SkillDataManager.Jump_UpLeft);
        }
    }
    public void InputKey(ComboKey key)
    {
        inputTime = Time.time;
        Debug.Log(key);
        // �׽�Ʈ�� ����
        GameManager.Instance.InputComboKey(key.ToString());
        //
        currentSkill = comboComponent.Input(key, inputTime);
        if(currentSkill != null)
        {
            comboTime = comboComponent.ExpireTime;
            activeSkill = currentSkill;
            if(activeSkill.SkillType == SkillType.Guard) 
            { 
                activeSkill?.CastSkill(this);
                activeSkill = null;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsGameOver)
        {

            // �׽�Ʈ�� ����
            //printText();
            //

            // �޺� �������� ����
            if (comboTime < Time.time)
            {
                if (activeSkill != null)
                {
                    animationSkill = activeSkill;
                    activeSkill?.CastSkill(this);
                    activeSkill = null;
                }
            }

            //

            // �Ʒ� �ڵ�� ����Ű ���� �������µ�, ĳ���� ��Ʈ�ѷ��ʿ��� �ذ���.
            //// ĳ���Ͱ� �������� ��Ȳ�϶�.
            //if (currentState == CharacterStatus.Normal)
            //{
            //    // �߸����� ������ ���� �޺� Ÿ��.
            //    comboTime = Time.time;
            //    // Ű�� �߸��� �ƴѰ��
            //    if (currentKey != ComboKey.Neutral)
            //    {// �޺�Ÿ�� - �Է½ð��� ���ѽð����� ��û ��¦���� ũ�ų� ������ �Է��ϵ���.
            //        if (comboTime - inputTime >= comboComponent.LimitInputTime - 0.05f)
            //        {
            //            // ���� Ű�� �߸����� �ٲٰ� �Է��Ѵ�.
            //            currentKey = ComboKey.Neutral;
            //            InputKey(currentKey);
            //        }
            //    }
            //}
            Vector3 speedLimitVelocity;
            otherPlayerVectorNorm = Extension.OtherPlayerVector(transform.position, otherCharacter.transform.position).x.Normalize();
            switch (currentState)
            {
                case CharacterStatus.Normal:
                    moveDirection = (PreferedDirection * Vector2.right).normalized;
                    break;
                case CharacterStatus.OnHit:
                case CharacterStatus.KO:
                    moveDirection = Vector2.zero;
                    break;
            }

            // ���� Ű �Է��� ������
            if (moveDirection.magnitude == 0)
            {
                if (isGround)
                {

                    // �������϶��� ������. ex)�ǰݴ���.
                    speedLimitVelocity = battleRigidbody.Velocity;
                    speedLimitVelocity.x = Mathf.Lerp(speedLimitVelocity.x, 0, 0.3f);

                    battleRigidbody.Velocity = speedLimitVelocity;



                }
            }
            else // ���� Ű �Է��� ����
            {
                // ĳ���Ͱ� �������� ��Ȳ
                if (currentState == CharacterStatus.Normal)
                {
                    if (isGround)
                    {
                        if (IsOtherOnRight()) // ������ȯ. ���߿� Ű�Է��� �ϰԵǸ� ��� ĳ���͸� �ٶ󺻴� �� �����Ұ�.
                        {
                            transform.rotation = Quaternion.identity; //������� ��ȯ
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(0, 180, 0);// 180�� ȸ��

                        }
                        // �̵��ϴ� �������� ������.
                    }
                }
                battleRigidbody.AddForce((Vector3)moveDirection * 30);
                // ĳ���Ͱ� ������ �ƴϴ��� �ϴ� ������ ���.
                speedLimitVelocity = battleRigidbody.Velocity;
                speedLimitVelocity.x = Mathf.Clamp(speedLimitVelocity.x, -MoveSpeed, MoveSpeed);
                battleRigidbody.Velocity = speedLimitVelocity;

                //Debug.Log(battleRigidbody.Velocity);
            }
            stickmanAnimator.SetFloat("Move", battleRigidbody.Velocity.x * otherPlayerVectorNorm);
            stickmanAnimator.SetFloat("VerticalVelocity", battleRigidbody.Velocity.y * 0.1f);
            if (currentState == CharacterStatus.OnHit)
            {
                if (TetanyTime > 0)
                {
                    TetanyTime -= Time.deltaTime;
                }
                else
                {
                    currentState = CharacterStatus.Normal;
                    stickmanAnimator.SetTrigger("Recover");
                }

            }
            // ���� �ִ��� üũ
            isGround = battleRigidbody.GroundCheck();
            stickmanAnimator.SetBool("isGround", isGround);
            GuardAble();// ���� �������� üũ

        }
        else
        {
            battleRigidbody.Velocity = new Vector2(0, 0);
        }
    }
    private bool IsOtherOnRight()
    {
        return Extension.SquareDistance(transform.position, otherCharacter.transform.position).x < 0;
    }

    
    public DamageBox InstantiateDamageBox()
    {
        GameObject damageBoxPrefab = Resources.Load<GameObject>("Prefabs/DamageBox");
        GameObject damageBox = Instantiate(damageBoxPrefab, transform);
        DamageBox damageComponent = damageBox.GetComponent<DamageBox>();
        damageComponent.SortingLayer = this.SortingLayer;
        damageComponent.SkillData = animationSkill;

        int arrayNumber = skillDataList.FindIndex(data => data == animationSkill);
        damageComponent.SkillLevel = skillLevelList[arrayNumber];

        damageComponent.InsertAction(animationSkill.HitAction, animationSkill.DefenceAction);
        
        return damageComponent;
    }
    public void TakeDamage(int damage)
    {
        if (currentState != CharacterStatus.KO)
        {
            CurrentHitPoint -= damage;
            if (CurrentHitPoint <= 0)
            {
                currentState = CharacterStatus.KO;
                stickmanAnimator.SetTrigger("Die");
                isDead = true;
                GameManager.Instance.GameOver();
            }
        }
    }
    public void TakeDefence(int damage)
    {
        ShieldGauge -= damage;
    }

    //protected void OnMove(InputValue value) { Move(value.Get<Vector2>()); }
    public void Move(Vector2 direction)
    {
        switch (currentState)
        {// �ƹ�ư ���� ó�°��ְų� ���ӳ������� �̵�����.
            case CharacterStatus.OnAttack:
            case CharacterStatus.OnHit:
            case CharacterStatus.KO:
                PreferedDirection = Vector2.zero;
                return;
        }
        direction.Normalize();
        PreferedDirection = direction;
        if(direction.magnitude != 0)
        {
            // ������ ������ �ٲپ ������ �����⸦ �ؼ� 360���� 45���� ���� ���� enumȭ ��Ŵ.
            currentKey = (ComboKey)(Quaternion.FromToRotation(preferedDirection, Vector2.up).eulerAngles.z / 45 +1) ;
            //InputKey(currentKey);
        }
    }
    
    //protected void OnNormalAttack() { NormalAttack(); }
    public void NormalAttack() {
        switch (currentState)
        {// �ƹ�ư ���� ó�°��ְų� ���ӳ������� �׼Ǹ���.
            case CharacterStatus.OnHit:
            case CharacterStatus.KO:
                return;
        }
        currentKey = ComboKey.NORMAL_ATTACK;
        //InputKey(currentKey);
    }
    //protected void OnPowerAttack() { PowerAttack(); }
    public void PowerAttack() 
    {
        switch (currentState)
        {// �ƹ�ư ���� ó�°��ְų� ���ӳ������� �׼Ǹ���.
            case CharacterStatus.OnHit:
            case CharacterStatus.KO:
                return;
        }
        currentKey = ComboKey.POWER_ATTACK;
        
        //InputKey(currentKey);
        
    }

    // ����Ű�� ���� �Լ�.
    // ���� ������ �Է��ϸ� 0.02�� �̳��� �ٸ� ������ �ԷµǾ��ٸ� ����Ű�� �Է�.
    // ���� �ȸ������. ��������
    //protected void Input2Key()
    //{
    //    switch (currentState)
    //    {// �ƹ�ư ���� ó�°��ְų� ���ӳ������� �׼Ǹ���.
    //        case CharacterStatus.OnHit:            
    //        case CharacterStatus.KO:
    //            return;
    //    }
    //    currentKey = ComboKey.ALL_ATTACK;
    //    //InputKey(currentKey);
    //    if (currentSkill != null)
    //    {
    //        GameManager.Instance.PhysicsManager.InsertDamageBox(InstantiateDamageBox());
    //    }
    //}

    //protected void OnGuard(InputValue value) { Guard(); }
    
    public void GuardOn()
    {
        isGuard = true; // ���� ���·� ����
        
        stickmanAnimator.SetBool("isGuard", true);
    }
    public void GuardOff()
    {
        isGuard = false;
        stickmanAnimator.SetBool("isGuard", false);
        if(activeSkill != null)
        {
            if (activeSkill.SkillType == SkillType.Guard)
            { activeSkill = null; }
        }
    }
    public bool GuardAble()
    {
        // ��� ���¸� ����
        // ���带 �� �� �ִٸ�
        // T, F�� ������.
        switch (currentState)
        {// �ƹ�ư ���� ó�°��ְų� ���ӳ������� �׼Ǹ���.
            case CharacterStatus.OnHit:
            case CharacterStatus.KO:
            case CharacterStatus.OnAttack:
                stickmanAnimator.SetBool("CanGuard", false);
                return false;
        }
        if(shieldBallCurrent<=0)
        {
            stickmanAnimator.SetBool("CanGuard", false);
            return false;
        }
        if(!stickmanAnimator.GetBool("isGround"))
        {
            return false;
        }
        stickmanAnimator.SetBool("CanGuard", true);
        return true;
    }
    public bool ActionAble()
    {
        switch (currentState)
        {// �ƹ�ư ���� ó�°��ְų� ���ӳ������� �׼Ǹ���.
            case CharacterStatus.OnHit:
            case CharacterStatus.KO:
            case CharacterStatus.OnAttack:
                return false;
        }
        return true;

    }
    public bool GuardCheck() // ���带 �ϴ��߿� �ߵ��Ѵ�.
    {
        // �÷��̾ ���� Ű�� ������ �ִ�.
        if (isGuard)
        {
            return GuardAble();// ���尡 ������ ��Ȳ�̸� t, �ƴϸ� f
        }

        return false; // ���带 �ȴ����� = f
    }

    protected void OnOpenMenu() { OpenMenu(); }
    protected void OpenMenu() { }



    // �÷��̾� ��ų ���.
    // ������ �Ѵ�. ������ �ڽ� ����.
    public void Attack(int attackNumber)
    {
            if (activeSkill != null)
        {
            //GameManager.Instance.PhysicsManager.InsertDamageBox(InstantiateDamageBox());
            stickmanAnimator.SetTrigger("Attack");
            stickmanAnimator.SetInteger("AttackNumber", attackNumber);
        }
    }
    // ����
    public void Jump(Vector2 direction)
    {
        // 
        if (isGround)
        {
            battleRigidbody.AddForce(direction * jumpPower);
            //transform.position += new Vector3(0, 0.001f,0);
            
            isGround = false;
        }
    }
    // �о��
    public void PushThis(float pushLength)
    {
        if (currentState != CharacterStatus.KO)
        {
            Vector2 direction;
            // ��밡 �����ʿ� ������.(���� ���ʿ� ������, ���� �������� �ٶ󺸰� ������)
            if (!IsOtherOnRight())
            {
                direction = Vector2.right;
            }
            else
            {
                direction = Vector2.left;
            }
            battleRigidbody.AddForce(pushLength * direction);
        }
    }

    // ����
    public void Tetany(float tetanyTime)
    {
        if (currentState != CharacterStatus.KO)
        {
            TetanyTime = tetanyTime;
            currentState = CharacterStatus.OnHit;
            StickmanAnimator.SetTrigger("OnHit");

            stickmanAnimator.SetFloat("TetanyTime", tetanyTime);
        }
        // �ִϸ��̼��� �ǰ����� ��� �ٲ�����.
    }


    // ����
    public void FloatingAir(float floatingAmount)
    {
        if (currentState != CharacterStatus.KO)
        {
            stickmanAnimator.SetTrigger("AirHit");
            // �߷� �ϴ� �ʱ�ȭ�ѹ�.
            battleRigidbody.ResetGravity();
            // �̹� ���߿� ���ִٸ�. ���� ���� ����������.
            if (!isGround)
            { floatingAmount *= 0.5f; }
            Debug.Log(floatingAmount);
            // ĳ���� �ִϸ��̼��� ���� ���ΰɷ� ��.

            // floatingAmount��ŭ ����.
            battleRigidbody.AddForce(floatingAmount * Vector2.up);
        }
    }

    // ����ȸ��
    public void ScrewAir(float floatingAmount)
    {
        if (currentState != CharacterStatus.KO)
        {
            // �ƹ�ư ���߿� �׸�ŭ ���.
            battleRigidbody.AddForce(floatingAmount * Vector2.up);

            // ��۹�� ����.

            // ������ �� �߷°��� �������� ����.
        }
    }

    // �ٿ��
    public void BoundCharacter()
    {
        // ��븦 �ٴ����� ����.

        // ��븦 �ٽ� ����, ���ۺ��� ����.
        ScrewAir(5);
    }

    // ������ �ε������ϱ�
    public void DumpWall()
    {

    }
    // ���۾Ƹ�
    public void SuperArmor()
    {
        // �ش� ������ �ϴ� ��, ��� ��� ���� �Լ� ����.
    }
    // ���� �ı�
    public void GuardCrush()
    {
        // �ǵ�������� 0���ϰ� �Ǹ� Tenaty�� 3 �޴´�.
        // ���� �Ұ� ���°� �ȴ�.
        // ������� �ּ��� ��� �ؾ������� �ִϸ��̼� ����� �켱 ������ �޵��� ��.
        //TetanyTime = 1;
        //stickmanAnimator.SetTrigger("GuardCrush");
        //currentState = CharacterStatus.GuardCrush;


        Tetany(3);
    }

    // cŰ�� ������ ��븦 ������ ���带 �ø�����.
    // �����.
    private void OnEnemyDefence()
    {
        if(otherCharacter.isGuard)
        {
            otherCharacter.GuardOff();
        }
        else
        {
            otherCharacter.GuardOn();
        }
    }
    

    public void AddSkill(SkillDataManager.SkillData skillData)
    {
        comboComponent.Insert(skillData);
        //skillDataList.Add(skillData);
    }
}
