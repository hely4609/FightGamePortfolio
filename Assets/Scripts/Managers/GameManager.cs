using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    PhysicsManager physicsManager;
    GameObject[] characterArray = new GameObject[2];
    public GameObject[] CharacterArray { get { return characterArray; } }
    [SerializeField] float startPoint = 3;
    [SerializeField] GameObject emptyCharacter;
    [SerializeField] InputActionAsset action;
    [SerializeField] bool isPlayerLeft = true;
    bool isGameOver = false;
    public bool IsGameOver { get { return isGameOver; } }

    [SerializeField] RectTransform mainUI;
    UIManager uiManager;
    Character player;
    Character enemy;
    public Character Player { get { return player; } }
    public Character Enemy { get { return enemy; } }
    public CameraManager mainCamera; 

    EnemyData enemyData;

    // 테스트용 빌드
    public TextMeshProUGUI commandText;
    public string[] combokeys = new string[5];
    public void InputComboKey(string inputKey)
    {
        string prompt = "";
        for(int i = 4; i > 0; i--)
        {
            combokeys[i] = combokeys[i - 1];
            
            prompt = combokeys[i] + "\n"+prompt;
        }
        combokeys[0] = inputKey;

        prompt = combokeys[0] + "\n" + prompt;
        commandText.text = prompt;
    }

    //

    public static GameManager Instance { get { return instance; } }
    public PhysicsManager PhysicsManager { get { return physicsManager; } }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            physicsManager = instance.AddComponent<PhysicsManager>();
            enemyData = PlayerData.Instance.GetComponent<EnemyData>();
            physicsManager.Initialize();
            uiManager = GameObject.Find("MainUI").GetComponent<UIManager>();
            
        }
        else Destroy(gameObject);
    }
    private void OnEnable()
    {
        enemyData.EnemySkillInitialize();
        PlayerEntry();
    }

    void PlayerEntry()
    {
        Character[] characterScriptArray = new Character[2]; // 임시로 만든 캐릭터 스크립트 보관소
        for (int i = 0; i < characterArray.Length; i++)
        {
            // 캐릭터를 생성. 위치는 스타팅 위치 기준으로 
            characterArray[i] = Instantiate(emptyCharacter, new Vector2(i == 0 ? startPoint : -startPoint, -physicsManager.StageSize.y * 0.5f), Quaternion.Euler(0, i == 0 ? 180 : 0, 0));
            //각 캐릭터의 히트박스 레이어를 설정.
            characterArray[i].GetComponentInChildren<HitBox>().SortingLayer = i;

            characterScriptArray[i] = characterArray[i].GetComponent<Character>(); // 캐릭터 스크립트 보관소에 저장함
            characterScriptArray[i].SortingLayer = i; // sortingLayer 지정
        }
        characterScriptArray[0].otherCharacter = characterScriptArray[1];
        characterScriptArray[1].otherCharacter = characterScriptArray[0];

        GameObject playerUIPrefab = Resources.Load<GameObject>("Prefabs/PlayerUI");
        GameObject rightPlayerUI = Instantiate(playerUIPrefab, mainUI);
        GameObject leftPlayerUI = Instantiate(playerUIPrefab, mainUI);
        rightPlayerUI.transform.SetAsFirstSibling();
        leftPlayerUI.transform.SetAsFirstSibling();

        rightPlayerUI.GetComponent<PlayerUI>().Holder = characterScriptArray[0];
        leftPlayerUI.GetComponent<PlayerUI>().Holder = characterScriptArray[1];


        RectTransform rightPlayerRect = rightPlayerUI.GetComponent<RectTransform>();
        rightPlayerRect.anchorMin = Vector2.right;
        rightPlayerRect.anchorMax = Vector2.one;
        Vector3 rightPlayerScale = rightPlayerRect.localScale;
        rightPlayerScale.x *= -1;
        rightPlayerRect.localScale = rightPlayerScale;


        // 이유는 모르겠는데, 입력이 작동안함. 다 잘들어갔는데 키를 못받음. <- 프리팹에 등록한 캐릭터에 Character 스크립트를 안넣었음. 해결
        if (isPlayerLeft)
        {
            PlayerInput input = characterArray[1].AddComponent<PlayerInput>();
            characterArray[1].AddComponent<ControllManager>();
            input.actions = action;
            input.SwitchCurrentActionMap("GamePlay");

            player = characterScriptArray[1];
            player.SkillDataList = PlayerData.Instance.SkillDataList;
            player.SkillLevelList = PlayerData.Instance.SkillLevelDataList;
            enemy = characterScriptArray[0];
            enemy.SkillDataList = enemyData.EnemySkillData(PlayerData.Instance.selectBattle);
            enemy.SkillLevelList = enemyData.EnemySkillLevelData(PlayerData.Instance.selectBattle);
            enemy.GetComponentInChildren<SkeletonMecanim>().skeleton.SetSkin(SkinString(PlayerData.Instance.selectBattle));
        }
        else
        {
            PlayerInput input = characterArray[0].AddComponent<PlayerInput>();
            characterArray[0].AddComponent<ControllManager>();
            input.actions = action;
            input.SwitchCurrentActionMap("GamePlay");

            player = characterScriptArray[0];
            player.SkillDataList = PlayerData.Instance.SkillDataList;
            player.SkillLevelList = PlayerData.Instance.SkillLevelDataList;

            enemy = characterScriptArray[1];
            enemy.SkillDataList = enemyData.EnemySkillData(PlayerData.Instance.selectBattle);
            enemy.SkillLevelList = enemyData.EnemySkillLevelData(PlayerData.Instance.selectBattle);

            enemy.GetComponentInChildren<SkeletonMecanim>().skeleton.SetSkin(SkinString(PlayerData.Instance.selectBattle));

        }
    }

    public void GameOver()
    {
        isGameOver = true;
        if (player.IsDead)
        {
            uiManager.ResultScene();
        }
        else
        {
            PlayerData.Instance.ClearData[PlayerData.Instance.selectBattle] = true;

            if (Array.FindIndex(PlayerData.Instance.ClearData, isWin => isWin == false)<0) // 아직 게임이 진행중이다.
            {
                uiManager.ResultScene();
            }
            else
            {
                uiManager.WinResult();
            }
        }

    }

    private string SkinString(int enemyData)
    {
        string skinName = "black";
        switch(enemyData)
        {
            case 0:
                skinName = "purple";
                break; 
            case 1:
                skinName = "color presets/french";
                break;
            case 2:
                skinName = "color presets/fox";
                break;
        }


        return skinName;
    }
}
