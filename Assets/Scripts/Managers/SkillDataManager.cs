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
public static class SkillDataManager // �޺� ����Ʈ.
{
    [Serializable] // ������� ����Ƽ â���� ����
    public class SkillData
    {
        Action<Character> Casting;
        public Action<Character, Character> HitAction { get; protected set; }
        public Action<Character, Character> DefenceAction { get; protected set; }

        // �ϴ� ����鿡�� �̸�, Ŀ�ǵ�, ������, ��Ʈ�� �̴� ����, ��Ʈ�� ���� �ð��� �ʿ�.
        // ���ų� �����°��� �������.
        [SerializeField]string name; // �̸�
        string animationName;
        public string AnimationName { get { return animationName; }}
        public string Name { get { return name; } } // get �� �ִ� ���� : �ѹ� ����ϸ� ���� ���� �����̱� ����.
        SkillType skillType;
        public SkillType SkillType { get { return skillType; } }

        ComboKey[] comboCommand; // Ŀ�ǵ� ��� �������ϴ���.
        public ComboKey[] ComboCommand { get { return comboCommand; } }
        int damage;// ������
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
    // �⺻ ���
    public static SkillData Jump_Up = new SkillData("Jump", SkillType.Move,  new ComboKey[] { ComboKey.UP, ComboKey.Neutral }, "", (player) => { player.Jump(Vector2.up); });
    public static SkillData Jump_UpLeft = new SkillData("Jump", SkillType.Move, new ComboKey[] { ComboKey.UP_LEFT, ComboKey.Neutral }, "",(player) => { player.Jump(new Vector2(-player.OtherPlayerVectorNorm, 1).normalized); });
    public static SkillData Jump_UpRight = new SkillData("Jump", SkillType.Move, new ComboKey[] { ComboKey.UP_RIGHT, ComboKey.Neutral },"" ,(player) => { player.Jump(new Vector2(player.OtherPlayerVectorNorm, 1).normalized); });
    public static SkillData Guard = new SkillData("���", SkillType.Guard,new ComboKey[] { ComboKey.GUARD }, "block center", (player) => { player.GuardOn(); });
    public static SkillData Punch = new SkillData("��ġ", SkillType.Attack, new ComboKey[] { ComboKey.NORMAL_ATTACK }, "jab single",(player) => { player.Attack(0); }, (player, other) => { other.PushThis(30); other.Tetany(0.6f); }, (player, other) => { }, 10);
    public static SkillData Kick = new SkillData("������", SkillType.Attack, new ComboKey[] { ComboKey.POWER_ATTACK }, "kick low", (player) => { player.Attack(1); }, (player, other) => { other.Tetany(0.6f); }, (player, other) => { }, 10);
    public static SkillData UpperPunch = new SkillData("������", SkillType.Attack, new ComboKey[] { ComboKey.RIGHT, ComboKey.POWER_ATTACK, ComboKey.NORMAL_ATTACK }, "uppercut", (player) => { player.Attack(2); }, (player, other) => { other.FloatingAir(10); other.Tetany(1); }, (player, other) => {  other.PushThis(30); }, 20);
    //

    public static SkillData CutKick = new SkillData("��ű", SkillType.Attack, new ComboKey[] { ComboKey.UP_RIGHT, ComboKey.POWER_ATTACK }, "salto kick", (player) => { player.Attack(3); },(player, other) => {  other.Tetany(1); other.PushThis(50); }, (player, other) => { other.PushThis(30); }, 10);
    public static SkillData ReverseKick = new SkillData("�ڵ�������", SkillType.Attack, new ComboKey[] { ComboKey.RIGHT, ComboKey.ALL_ATTACK}, "reverse kick", (player) => { player.Attack(4); }, (player, other) => { other.Tetany(1); other.PushThis(5); other.FloatingAir(5); }, (player, other) => { other.PushThis(30); }, 30);
    public static SkillData StampingPunch = new SkillData("�м�", SkillType.Attack, new ComboKey[] { ComboKey.RIGHT, ComboKey.NORMAL_ATTACK, ComboKey.NORMAL_ATTACK }, "__jump attack",(player) => { player.Attack(5); }, (player, other) => { other.Tetany(1); other.PushThis(50); }, (player, other) => { other.PushThis(30); }, 10);


    // ��ų ���� �ý��� ����.
    // ��ų�� ������ �Ű������� �޾ƿ� ����.
    // 

    // ������.
    /*
     * 0 : ��
     * 1 : �Ʒ� ű
     * 2 : ������ 
     * 3 : ��ű 
     * 4 : ������ ű 
     * 5 : �������
     */
}
