using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    Image frameImage;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI command;
    [SerializeField] SkeletonAnimation skeletonAnimation;
    SkillDataManager.SkillData skillData;
    public SkillDataManager.SkillData SkillData { get { return skillData; } }
    [SerializeField]string animationName;
    
    public void Initialize(SkillDataManager.SkillData skillData, bool isFullCard = true)
    {
        frameImage = GetComponent<Image>();
        this.skillData = skillData;
        skillName.text = skillData.Name;
        if(isFullCard)
        {
            command.text = ParsingCommand(skillData.ComboCommand);
            animationName = $"1_/{skillData.AnimationName}";
            skeletonAnimation.AnimationName = animationName;
        }

        
        if (skillData.HitAction != null)
        {
            frameImage.color = new Color(1, 0.7f, 0.66f);
            if (isFullCard)
            {
                skeletonAnimation.AnimationState.AddAnimation(0, animationName, true, 0f);
            }
        }
    }

    //DOWN_RIGHT, RIGHT, UP_RIGHT, UP,
    //UP_LEFT, LEFT, DOWN_LEFT, DOWN,
    //NORMAL_ATTACK, POWER_ATTACK, GUARD, ALL_ATTACK, NORMAL_GUARD, POWER_GUARD, ALL_BUTTON, length
    private string ParsingCommand(ComboKey[] combokeyList)
    {
        string commandText = "";
        for(int i =0; i< combokeyList.Length; i++)
        {
            switch (combokeyList[i])
            {
                case ComboKey.LEFT:
                    commandText += "¡ç ";
                    break;
                case ComboKey.UP_LEFT:
                    commandText += "¢Ø ";
                    break;
                case ComboKey.DOWN_LEFT:
                    commandText += "¢× ";
                    break;
                case ComboKey.RIGHT:
                    commandText += "¡æ ";
                    break;
                case ComboKey.DOWN_RIGHT:
                    commandText += "¢Ù ";
                    break;
                case ComboKey.UP_RIGHT:
                    commandText += "¢Ö ";
                    break;
                case ComboKey.UP:
                    commandText += "¡è ";
                    break;
                case ComboKey.DOWN:
                    commandText += "¡é ";
                    break;
                case ComboKey.NORMAL_ATTACK:
                    commandText += "U ";
                    break;
                case ComboKey.POWER_ATTACK:
                    commandText += "I ";
                    break;
                case ComboKey.ALL_ATTACK:
                    commandText += "U+I ";
                    break;
                case ComboKey.GUARD:
                    commandText += "O ";
                    break;
                case ComboKey.NORMAL_GUARD:
                    commandText += "U+O ";
                    break;
                case ComboKey.POWER_GUARD:
                    commandText += "I+O ";
                    break;
                case ComboKey.ALL_BUTTON:
                    commandText += "U+I+O ";
                    break;
            }
        }
        return commandText;
    }
}
