using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;

public enum SkillType
{
    Attack, Move, Guard
}
public static class SkillDataManager // 콤보 리스트.
{
    [Serializable] // 해줘야지 유니티 창에서 보임
    public class SkillData
    {
        Action<Character> Casting;
        public Action<Character, Character> HitAction { get; protected set; }
        public Action<Character, Character> DefenceAction { get; protected set; }

        // 일단 기술들에는 이름, 커맨드, 데미지, 히트시 미는 정도, 히트시 경직 시간이 필요.
        // 띄우거나 날리는것은 어떻게하지.
        [SerializeField]string name; // 이름
        string animationName;
        public string AnimationName { get { return animationName; }}
        public string Name { get { return name; } } // get 만 있는 이유 : 한번 등록하면 수정 안할 예정이기 때문.
        SkillType skillType;
        public SkillType SkillType { get { return skillType; } }

        ComboKey[] comboCommand; // 커맨드 어떻게 눌러야하는지.
        public ComboKey[] ComboCommand { get { return comboCommand; } }
        int damage;// 데미지
        public int Damage { get { return damage; } }
        
        public void CastSkill(Character player)
        {  Casting.Invoke(player);   }
        

        public SkillData(string name, SkillType skillType, ComboKey[] combos, string animationName, Action<Character> behaviour, Action<Character, Character> Attackbehaviour = null, Action<Character, Character> Defencebehaviour = null, int damage = 0)
        {
            this.name = name;
            this.skillType= skillType;
            this.animationName= animationName;
            comboCommand = combos;
            
            
            this.damage = damage;
             
            Casting = null;
            Casting += behaviour;

            HitAction = null;
            HitAction += Attackbehaviour;

            DefenceAction = null;
            DefenceAction += Defencebehaviour;
        }
    }
    // 기본 기능
    public static SkillData Jump_Up = new SkillData("Jump", SkillType.Move,  new ComboKey[] { ComboKey.UP, ComboKey.Neutral }, "", (player) => { player.Jump(Vector2.up); });
    public static SkillData Jump_UpLeft = new SkillData("Jump", SkillType.Move, new ComboKey[] { ComboKey.UP_LEFT, ComboKey.Neutral }, "",(player) => { player.Jump(new Vector2(-player.OtherPlayerVectorNorm, 1).normalized); });
    public static SkillData Jump_UpRight = new SkillData("Jump", SkillType.Move, new ComboKey[] { ComboKey.UP_RIGHT, ComboKey.Neutral },"" ,(player) => { player.Jump(new Vector2(player.OtherPlayerVectorNorm, 1).normalized); });
    public static SkillData Guard = new SkillData("방어", SkillType.Guard,new ComboKey[] { ComboKey.GUARD }, "block center", (player) => { player.GuardOn(); });
    public static SkillData Punch = new SkillData("펀치", SkillType.Attack, new ComboKey[] { ComboKey.NORMAL_ATTACK }, "jab single",(player) => { player.Attack(0); }, (player, other) => { other.PushThis(30); other.Tetany(0.6f); }, (player, other) => { }, 10);
    public static SkillData Kick = new SkillData("발차기", SkillType.Attack, new ComboKey[] { ComboKey.POWER_ATTACK }, "kick low", (player) => { player.Attack(1); }, (player, other) => { other.Tetany(0.6f); }, (player, other) => { }, 10);
    public static SkillData UpperPunch = new SkillData("어퍼컷", SkillType.Attack, new ComboKey[] { ComboKey.RIGHT, ComboKey.POWER_ATTACK, ComboKey.NORMAL_ATTACK }, "uppercut", (player) => { player.Attack(2); }, (player, other) => { other.FloatingAir(10); other.Tetany(1); }, (player, other) => {  other.PushThis(30); }, 20);
    //

    public static SkillData CutKick = new SkillData("컷킥", SkillType.Attack, new ComboKey[] { ComboKey.UP_RIGHT, ComboKey.POWER_ATTACK }, "salto kick", (player) => { player.Attack(3); },(player, other) => {  other.Tetany(1); other.PushThis(50); }, (player, other) => { other.PushThis(30); }, 10);
    public static SkillData ReverseKick = new SkillData("뒤돌려차기", SkillType.Attack, new ComboKey[] { ComboKey.RIGHT, ComboKey.ALL_ATTACK}, "reverse kick", (player) => { player.Attack(4); }, (player, other) => { other.Tetany(1); other.PushThis(5); other.FloatingAir(5); }, (player, other) => { other.PushThis(30); }, 30);
    public static SkillData StampingPunch = new SkillData("분쇄", SkillType.Attack, new ComboKey[] { ComboKey.RIGHT, ComboKey.NORMAL_ATTACK, ComboKey.NORMAL_ATTACK }, "__jump attack",(player) => { player.Attack(5); }, (player, other) => { other.Tetany(1); other.PushThis(50); }, (player, other) => { other.PushThis(30); }, 10);


    // 스킬 레벨 시스템 생성.
    // 스킬의 레벨을 매개변수로 받아올 예정.
    // 

    // 움직임.
    /*
     * 0 : 잽
     * 1 : 아래 킥
     * 2 : 어퍼컷 
     * 3 : 컷킥 
     * 4 : 리버스 킥 
     * 5 : 내려찍기
     */
}
