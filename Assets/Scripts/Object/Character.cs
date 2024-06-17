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
{//       가드깨짐
    //평소,  공격중, 맞는 중, 짐, 가드 중 , 가드 깨짐
    Normal, OnAttack, OnHit, KO, Guard, GuardCrush
}

public class Character : MonoBehaviour
{
    ComboKey currentKey; // 현재 키.
    [SerializeField] ComboSystem comboComponent; // 콤보 시스템을 얘가 가지고있도록.
    SkillDataManager.SkillData currentSkill; // 현재 발동된 스킬. 이걸 DamageBox에 넘겨줄것.
    SkillDataManager.SkillData activeSkill; // 발동 할 스킬
    public SkillDataManager.SkillData ActiveSkill { get { return activeSkill; } set { activeSkill = value; } }
    SkillDataManager.SkillData animationSkill; // 발동 중인 스킬
    [SerializeField]List<SkillDataManager.SkillData> skillDataList = new List<SkillDataManager.SkillData>();
    [SerializeField]List<int> skillLevelList = new List<int>();
    public List<SkillDataManager.SkillData> SkillDataList { get { return skillDataList; } set { skillDataList = value; } }
    public List<int> SkillLevelList { get { return skillLevelList; } set { skillLevelList = value;} }


    float comboTime; // 콤보 시간
    float inputTime; // 누른 시간

    [SerializeField] float jumpPower = 10;

    [SerializeField] float tetanyTime;
    public float TetanyTime { get { return tetanyTime; } protected set { tetanyTime = value; } }

    // 체력
    [SerializeField] int maxHitPoint = 100;
    [SerializeField] int currentHitPoint;
    public int CurrentHitPoint { get { return currentHitPoint; } protected set { currentHitPoint = value; } }
    public float HitPointPercent { get { return (float)currentHitPoint / maxHitPoint; } }

    // 실드
    [SerializeField] int shieldBallAmount; // 최대 실드 개수
    public int ShieldBallAmount { get { return shieldBallAmount; } protected set { shieldBallAmount = value; } }
    public int shieldBallCurrent { get; set; }  // 현재 몇개 가지고 있나.

    
    [SerializeField] float shieldGaugeMax = 100;
    public float ShieldGaugeMax { get { return shieldGaugeMax; } set { shieldGaugeMax = value; } }
    [SerializeField] float shieldGauge;
    public float ShieldGauge { get { return shieldGauge; } set { shieldGauge = value; } }
    public float ShieldGaugePercent { get { return shieldGauge / shieldGaugeMax; } }



    public Character otherCharacter; // 상대 플레이어 정보. 현재 방향을 측정할때 사용할 것.
    [SerializeField]float otherPlayerVectorNorm;
    public float OtherPlayerVectorNorm { get { return otherPlayerVectorNorm; } }
    [SerializeField]private int sortingLayer; // 캐릭터가 가진 히트박스의 sortingLayer
    public int SortingLayer { get { return sortingLayer; } set { sortingLayer = value; } }
    

    [SerializeField] CharacterStatus currentState = CharacterStatus.Normal; // 현재 상태
    [SerializeField] float moveSpeed; // 최대 이속 
    protected BattleRigidbody2D battleRigidbody; // 속도나 위치 그런 물리를 처리해줄 녀석.
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    [SerializeField] Vector2 preferedDirection;
    public Vector2 PreferedDirection { get { return preferedDirection; } set { preferedDirection = value; } } // 가고자하는 방향
    public Vector2 moveDirection { get; set; } // 실제 가는 방향
    
    [SerializeField] bool isGround; // 땅에 있니?
    [SerializeField] bool isGuard = false;
    bool isDead = false;
    public bool IsDead { get { return isDead; } }

    [SerializeField] Animator stickmanAnimator;
    public Animator StickmanAnimator { get { return stickmanAnimator; } }

    [SerializeField] SkeletonMecanim stickmanBone;
    public SkeletonMecanim StickmanBone { get { return stickmanBone; } }
    


    // 테스트용 빌드
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
        battleRigidbody = gameObject.GetComponent<BattleRigidbody2D>(); // 물리 코드 넣어주기.

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
        // 테스트용 빌드
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

            // 테스트용 빌드
            //printText();
            //

            // 콤보 실질적인 실행
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

            // 아래 코드는 방향키 관련 문제였는데, 캐릭터 컨트롤러쪽에서 해결함.
            //// 캐릭터가 정상적인 상황일때.
            //if (currentState == CharacterStatus.Normal)
            //{
            //    // 중립으로 돌리기 위한 콤보 타임.
            //    comboTime = Time.time;
            //    // 키가 중립이 아닌경우
            //    if (currentKey != ComboKey.Neutral)
            //    {// 콤보타임 - 입력시간이 제한시간보다 엄청 살짝보다 크거나 같으면 입력하도록.
            //        if (comboTime - inputTime >= comboComponent.LimitInputTime - 0.05f)
            //        {
            //            // 현재 키를 중립으로 바꾸고 입력한다.
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

            // 방향 키 입력이 없을때
            if (moveDirection.magnitude == 0)
            {
                if (isGround)
                {

                    // 못움직일때의 움직임. ex)피격당함.
                    speedLimitVelocity = battleRigidbody.Velocity;
                    speedLimitVelocity.x = Mathf.Lerp(speedLimitVelocity.x, 0, 0.3f);

                    battleRigidbody.Velocity = speedLimitVelocity;



                }
            }
            else // 방향 키 입력이 있음
            {
                // 캐릭터가 정상적인 상황
                if (currentState == CharacterStatus.Normal)
                {
                    if (isGround)
                    {
                        if (IsOtherOnRight()) // 방향전환. 나중에 키입력을 하게되면 상대 캐릭터를 바라본다 로 변경할것.
                        {
                            transform.rotation = Quaternion.identity; //원래대로 전환
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(0, 180, 0);// 180도 회전

                        }
                        // 이동하는 방향으로 힘을줌.
                    }
                }
                battleRigidbody.AddForce((Vector3)moveDirection * 30);
                // 캐릭터가 정상이 아니더라도 일단 물리는 계산.
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
            // 땅에 있는지 체크
            isGround = battleRigidbody.GroundCheck();
            stickmanAnimator.SetBool("isGround", isGround);
            GuardAble();// 가드 가능한지 체크

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
        {// 아무튼 뭔가 처맞고있거나 게임끝났으면 이동못함.
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
            // 방향을 각도로 바꾸어서 각도를 나누기를 해서 360도를 45도로 나눈 몫을 enum화 시킴.
            currentKey = (ComboKey)(Quaternion.FromToRotation(preferedDirection, Vector2.up).eulerAngles.z / 45 +1) ;
            //InputKey(currentKey);
        }
    }
    
    //protected void OnNormalAttack() { NormalAttack(); }
    public void NormalAttack() {
        switch (currentState)
        {// 아무튼 뭔가 처맞고있거나 게임끝났으면 액션못함.
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
        {// 아무튼 뭔가 처맞고있거나 게임끝났으면 액션못함.
            case CharacterStatus.OnHit:
            case CharacterStatus.KO:
                return;
        }
        currentKey = ComboKey.POWER_ATTACK;
        
        //InputKey(currentKey);
        
    }

    // 동시키를 위한 함수.
    // 공격 한쪽을 입력하면 0.02초 이내에 다른 공격이 입력되었다면 동시키로 입력.
    // 아직 안만든거임. 만들어야함
    //protected void Input2Key()
    //{
    //    switch (currentState)
    //    {// 아무튼 뭔가 처맞고있거나 게임끝났으면 액션못함.
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
        isGuard = true; // 가드 상태로 변경
        
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
        // 모든 상태를 보고
        // 가드를 할 수 있다면
        // T, F를 돌려줌.
        switch (currentState)
        {// 아무튼 뭔가 처맞고있거나 게임끝났으면 액션못함.
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
        {// 아무튼 뭔가 처맞고있거나 게임끝났으면 액션못함.
            case CharacterStatus.OnHit:
            case CharacterStatus.KO:
            case CharacterStatus.OnAttack:
                return false;
        }
        return true;

    }
    public bool GuardCheck() // 가드를 하는중에 발동한다.
    {
        // 플레이어가 가드 키를 누르고 있다.
        if (isGuard)
        {
            return GuardAble();// 가드가 가능한 상황이면 t, 아니면 f
        }

        return false; // 가드를 안누른다 = f
    }

    protected void OnOpenMenu() { OpenMenu(); }
    protected void OpenMenu() { }



    // 플레이어 스킬 기능.
    // 공격을 한다. 데미지 박스 생성.
    public void Attack(int attackNumber)
    {
            if (activeSkill != null)
        {
            //GameManager.Instance.PhysicsManager.InsertDamageBox(InstantiateDamageBox());
            stickmanAnimator.SetTrigger("Attack");
            stickmanAnimator.SetInteger("AttackNumber", attackNumber);
        }
    }
    // 점프
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
    // 밀어내기
    public void PushThis(float pushLength)
    {
        if (currentState != CharacterStatus.KO)
        {
            Vector2 direction;
            // 상대가 오른쪽에 있으면.(내가 왼쪽에 있으면, 내가 오른쪽을 바라보고 있으면)
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

    // 경직
    public void Tetany(float tetanyTime)
    {
        if (currentState != CharacterStatus.KO)
        {
            TetanyTime = tetanyTime;
            currentState = CharacterStatus.OnHit;
            StickmanAnimator.SetTrigger("OnHit");

            stickmanAnimator.SetFloat("TetanyTime", tetanyTime);
        }
        // 애니메이션이 피격으로 즉시 바뀌어야함.
    }


    // 띄우기
    public void FloatingAir(float floatingAmount)
    {
        if (currentState != CharacterStatus.KO)
        {
            stickmanAnimator.SetTrigger("AirHit");
            // 중력 일단 초기화한번.
            battleRigidbody.ResetGravity();
            // 이미 공중에 떠있다면. 띄우는 양을 절반으로함.
            if (!isGround)
            { floatingAmount *= 0.5f; }
            Debug.Log(floatingAmount);
            // 캐릭터 애니메이션을 눕는 중인걸로 함.

            // floatingAmount만큼 띄운다.
            battleRigidbody.AddForce(floatingAmount * Vector2.up);
        }
    }

    // 공중회전
    public void ScrewAir(float floatingAmount)
    {
        if (currentState != CharacterStatus.KO)
        {
            // 아무튼 공중에 그만큼 띄움.
            battleRigidbody.AddForce(floatingAmount * Vector2.up);

            // 뱅글뱅글 돌림.

            // 떨어질 때 중력값을 절반으로 적용.
        }
    }

    // 바운드
    public void BoundCharacter()
    {
        // 상대를 바닥으로 내림.

        // 상대를 다시 띄우고, 빙글빙글 돌림.
        ScrewAir(5);
    }

    // 벽까지 부딪히게하기
    public void DumpWall()
    {

    }
    // 슈퍼아머
    public void SuperArmor()
    {
        // 해당 공격을 하는 중, 모든 상대 공격 함수 무시.
    }
    // 가드 파괴
    public void GuardCrush()
    {
        // 실드게이지가 0이하가 되면 Tenaty를 3 받는다.
        // 가드 불가 상태가 된다.
        // 원래라면 주석된 대로 해야하지만 애니메이션 부재로 우선 경직만 받도록 함.
        //TetanyTime = 1;
        //stickmanAnimator.SetTrigger("GuardCrush");
        //currentState = CharacterStatus.GuardCrush;


        Tetany(3);
    }

    // c키를 누르면 상대를 강제로 가드를 올리게함.
    // 빌드용.
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
