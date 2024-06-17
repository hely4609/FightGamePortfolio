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
    float inputDirectionKeyCompleteTime; // Ȯ������ ���� �ð�.
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


        //����� 
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


    // Time.time �������� 0.02 ���Ϸ� �Է½� ����Ű �Է�
    protected void OnMove(InputValue value) // �̵� Ű�Է�
    {
        if (character.ActionAble()&&!character.StickmanAnimator.GetBool("isGuard"))
        {


            ComboKey currentDirectionKey;
            currentInputVector = value.Get<Vector2>(); // �Է°��� ����2�� ����
            Vector2 comboInputVector = currentInputVector;

            if (currentInputVector == Vector2.zero) // ����Ű �Է��� ���� �߸��Է��� ����.
            {
                currentDirectionKey = ComboKey.Neutral;
            }
            else // ������ �Է��ϸ� �Է°� �Է�
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
                if (currentInput) // ��� ������ �� �� �۵��ϵ��� ��.
                {
                    inputAttackKeyCompleteTime = completeTime;
                    attackKeyList.Add(ComboKey.POWER_ATTACK);
                }
            }
        }
    }

    protected void OnGuard(InputValue value)  // ���� Ű�� ����
    {
       
            bool currentInput = (value.Get<float>() > inputPress); // ���� Ű�� 0.5�з� �̻����� ���� ������ ��, �Էµ��ִ°����� ���.
            if (keyInputs[(int)ComboKey.GUARD] != currentInput) // ���� Ű�� ����� Ű�� ���°� �ٸ���. (������ �ȴ��Ȱų�, ���� ������ Ǯ�����)
            {
                keyInputs[(int)ComboKey.GUARD] = currentInput; // ���� Ű�� �������ִ� Ű �迭�� ���.
                Debug.Log(keyInputs[(int)ComboKey.GUARD]);
                if (currentInput) // Ű�� ��ݴ����ٸ�.
                {
                    inputAttackKeyCompleteTime = completeTime;
                    attackKeyList.Add(ComboKey.GUARD); // ���� Ű�� ����.
                }
                else // ��� �ôٸ�
                {
                    attackKeyList.Remove(ComboKey.GUARD); // ���� Ű�� ����.

                    character.GuardOff(); // ���带 ����
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
            // 1. ���������� ����,���������� �ѱ�.
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

    //�� ���� �ൿ

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
    // VŰ
    private void OnEnemyAttack()
    {
        
        EnemyBehavior();
    }
}
