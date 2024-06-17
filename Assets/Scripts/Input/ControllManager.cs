using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllManager : MonoBehaviour
{
    bool[] keyInputs = new bool[(int)ComboKey.length]; 
    [SerializeField]List<ComboKey> attackKeyList = new List<ComboKey>();

    ComboKey lastDirectionKey;
    Vector2 currentInputVector;

    const float inputPress = 0.5f;
    const float completeTime = 0.02f;
    float inputDirectionKeyCompleteTime; // 확정까지 남은 시간.
    float inputAttackKeyCompleteTime;
    float enemyActiveTime = 3;
    [SerializeField] Character character;
    Character enemyCharacter = GameManager.Instance.Enemy;
    [SerializeField] GameObject pauseBack;

    private void Start()
    {
        character = GetComponent<Character>();
        enemyCharacter = GameManager.Instance.Enemy;
        if (pauseBack == null)
        {
            pauseBack = MenuManager.Instance.PauseBack;
        }
    }
    private void Update()
    {
        if(inputDirectionKeyCompleteTime > 0)
        {
            inputDirectionKeyCompleteTime -= Time.deltaTime;
            if(inputDirectionKeyCompleteTime < 0 )
            {
                inputDirectionKeyCompleteTime = 0;
                InputDirectionKey();
                
            }
        }
        if(inputAttackKeyCompleteTime > 0)
        {
            inputAttackKeyCompleteTime-= Time.deltaTime;
            if(inputAttackKeyCompleteTime < 0)
            {
                inputAttackKeyCompleteTime= 0;
                InputAttackKey();
                attackKeyList.Clear();
            }    
        }


        //빌드용 
        if (enemyCharacter.ActionAble())
        {
            if (enemyActiveTime > 0)
            {
                enemyActiveTime -= Time.deltaTime;
                if (enemyActiveTime < 0)
                {
                    //EnemyBehavior();
                    enemyActiveTime = 3;
                }
            }
        }
        //
    }


    // Time.time 기준으로 0.02 이하로 입력시 동시키 입력
    protected void OnMove(InputValue value) // 이동 키입력
    {
        if (character.ActionAble()&&!character.StickmanAnimator.GetBool("isGuard"))
        {


            ComboKey currentDirectionKey;
            currentInputVector = value.Get<Vector2>(); // 입력값을 벡터2로 받음
            Vector2 comboInputVector = currentInputVector;

            if (currentInputVector == Vector2.zero) // 방향키 입력을 떼면 중립입력이 나감.
            {
                currentDirectionKey = ComboKey.Neutral;
            }
            else // 방향을 입력하면 입력값 입력
            {
                comboInputVector.x *= Extension.OtherPlayerVector(character.transform.position, character.otherCharacter.transform.position).x.Normalize();
                int currentAngle = (int)((Vector2.SignedAngle(Vector2.up, comboInputVector) + 180) / 45);

                currentDirectionKey = (ComboKey)currentAngle;
            }


            keyInputs[(int)currentDirectionKey] = true;
            keyInputs[(int)lastDirectionKey] = false;

            inputDirectionKeyCompleteTime = completeTime;
            lastDirectionKey = currentDirectionKey;

        }

    }
    protected void OnNormalAttack(InputValue value) 
    {
        if (character.ActionAble())
        {
            bool currentInput = (value.Get<float>() > inputPress);
            if (keyInputs[(int)ComboKey.NORMAL_ATTACK] != currentInput)
            {
                keyInputs[(int)ComboKey.NORMAL_ATTACK] = currentInput;
                if (currentInput)
                {
                    inputAttackKeyCompleteTime = completeTime;
                    attackKeyList.Add(ComboKey.NORMAL_ATTACK);
                }

            }
        }
    }
    protected void OnPowerAttack(InputValue value) 
    {
        if (character.ActionAble())
        {
            bool currentInput = (value.Get<float>() > inputPress);
            if (keyInputs[(int)ComboKey.POWER_ATTACK] != currentInput)
            {
                keyInputs[(int)ComboKey.POWER_ATTACK] = currentInput;
                if (currentInput) // 방금 눌렀을 때 만 작동하도록 함.
                {
                    inputAttackKeyCompleteTime = completeTime;
                    attackKeyList.Add(ComboKey.POWER_ATTACK);
                }
            }
        }
    }

    protected void OnGuard(InputValue value)  // 가드 키를 누름
    {
       
            bool currentInput = (value.Get<float>() > inputPress); // 현재 키는 0.5압력 이상으로 값이 들어왔을 때, 입력되있는것으로 취급.
            if (keyInputs[(int)ComboKey.GUARD] != currentInput) // 현재 키와 저장된 키의 상태가 다를때. (이전에 안눌렸거나, 이제 눌린게 풀린경우)
            {
                keyInputs[(int)ComboKey.GUARD] = currentInput; // 현재 키를 누르고있는 키 배열에 등록.
                Debug.Log(keyInputs[(int)ComboKey.GUARD]);
                if (currentInput) // 키를 방금눌렀다면.
                {
                    inputAttackKeyCompleteTime = completeTime;
                    attackKeyList.Add(ComboKey.GUARD); // 가드 키를 누름.
                }
                else // 방금 뗐다면
                {
                    attackKeyList.Remove(ComboKey.GUARD); // 가드 키를 누름.

                    character.GuardOff(); // 가드를 내림
                }
            }
        
    }
    protected ComboKey Input2Key(List<ComboKey> comboKeys) 
    {
        if (comboKeys.Contains(ComboKey.NORMAL_ATTACK) && comboKeys.Contains(ComboKey.POWER_ATTACK) && comboKeys.Contains(ComboKey.GUARD))
        {
            return ComboKey.ALL_BUTTON;
        }
        else if (comboKeys.Contains(ComboKey.POWER_ATTACK) && comboKeys.Contains(ComboKey.NORMAL_ATTACK))
        {
            return ComboKey.ALL_ATTACK;
        }
        else if(comboKeys.Contains(ComboKey.NORMAL_ATTACK) && comboKeys.Contains(ComboKey.GUARD))
        {
            return ComboKey.NORMAL_GUARD;
        }
        else if (comboKeys.Contains(ComboKey.POWER_ATTACK) && comboKeys.Contains(ComboKey.GUARD))
        {
            return ComboKey.POWER_GUARD;
        }
        else return comboKeys[0];
    }
    private void InputDirectionKey()
    {
        character.InputKey(lastDirectionKey);
        character.Move(currentInputVector);
    }

    private void InputAttackKey()
    {
        ComboKey lastAttackKey = Input2Key(attackKeyList);
        
        character.InputKey(lastAttackKey);
    }
    protected void OnOpenMenu()
    { Pause(); }
    public virtual void Pause()
    {
        if (!GameManager.Instance.IsGameOver)
        {
            if (!pauseBack.activeInHierarchy)
            {
                MenuManager.Instance.PauseButton(pauseBack.activeInHierarchy);
                pauseBack.SetActive(!pauseBack.activeInHierarchy);
            }
            // 1. 켜져있으면 끄기,꺼져있으면 켜기.
            else
            {
                pauseBack.GetComponent<MenuManager>().EscapeMode();
            }
        }
    }
    protected void OnCommandList()
    {
        pauseBack.GetComponent<MenuManager>().OnCommandList();
    }

    //적 랜덤 행동

    public void EnemyBehavior()
    {
        
        if (enemyCharacter.CurrentHitPoint > 0)
        {
            if (enemyCharacter.StickmanAnimator.GetBool("isGuard"))
            {
                enemyCharacter.GuardOff();
            }
            else
            {
                List<SkillDataManager.SkillData> enemySkillList = enemyCharacter.SkillDataList;
                SkillDataManager.SkillData enemySkill = enemySkillList[Random.Range(0, enemySkillList.Count)];
                enemyCharacter.ActiveSkill = enemySkill;
            }
        }
    }
    // V키
    private void OnEnemyAttack()
    {
        
        EnemyBehavior();
    }
}
