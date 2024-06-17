using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ComboKey
{
    Neutral,
    DOWN_RIGHT, RIGHT, UP_RIGHT, UP,
    UP_LEFT, LEFT, DOWN_LEFT, DOWN,
    NORMAL_ATTACK, POWER_ATTACK, GUARD, ALL_ATTACK, NORMAL_GUARD, POWER_GUARD, ALL_BUTTON, length
}

// Ʈ���� ��� ����.
public class Trienode
{
    // ���� �Է� ���� Ű.
    ComboKey currentKey;
    // �ϴ� ���±��� �� �ϼ��� Ŀ�ǵ尡 ������ ǥ��.
    public SkillDataManager.SkillData currentSkill;

    //�� ����� ���� ���.
    Trienode parent = null;

    //�� ����� ���� ����Ʈ ���
    List<Trienode> childList = new List<Trienode>();

    // �� ��带 �����Ҷ� �����ڷ� ��忡 �Է¹��� Ű�� �����Ѵ�.
    public Trienode(ComboKey inputKey)
    {
        currentKey = inputKey;
    }

    // �ش��ϴ� �ڽ��� ã�� �Լ�.
    public Trienode FindChild(ComboKey inputKey)
    {
        for (int i = 0; i < childList.Count; i++)
        {
            if (childList[i].currentKey == inputKey) { return childList[i]; }
        }
        return null;
    }

    // ���ο� �ڽ��� �����ϴ� �Լ�.
    public Trienode NewChild(ComboKey inputKey)
    {
        // �ڽ��� ã�ƺ���.
        Trienode returnNode = FindChild(inputKey);
        // �ڽ��� ���ٸ� �����.
        if (returnNode == null)
        {
            // ���ο� ��带 ����. returnNode�� �װ� �־���
            returnNode = new Trienode(inputKey);

            //  returnNode�� �θ� "��" ���� �Ѵ�.
            //  returnNode�� �ڽ��� Ʈ���̳�� �̱� ������ ���� ��带 returnNode�� �θ�� �־��ִ°�.
            // �� ��尡 Trienode�� �����ؼ� �̰� returnNode��� ��. ���� ��ÿ��� parent�� null�̱� ������ �� parent�� �� ��带 �־���.
            returnNode.parent = this;
            // ������� ��带 childList�� �־���.
            childList.Add(returnNode);
        }
        return returnNode;
    }

    // ���� �Է°��� �迭�� �ְ�, ��� ����Ʈ���� 
    public Trienode Find(ComboKey[] inputArray, int index)
    {
        // ���� ��ġ�� ã������ +1
        index++;
        // index�� �迭 ���̺��� ������ = ���� ��ġ�� ���Ҵٸ�
        if (index < inputArray.Length)
        {
            // index+1�� ��ġ�� �ش��ϴ� Ű�� ���� ���� ��带 ã�ƿ�.
            Trienode nextChild = FindChild(inputArray[index]);

            // �ִٸ�
            if (nextChild != null)
            {
                // �ش� ��带 �� Find�Լ��� ������ ��� ã��.
                // ���� ��忡 ��� ������尡 �ִٸ� ������� ��� ������.
                return nextChild.Find(inputArray, index);
            }
            // ������ null ��ȯ.
            else { return null; }
        }
        // �迭 ���̶� ���ٸ�.
        else
        {// �� ���� inputArry �� ������ ĭ�� ���ؼ� �����ϸ�
            if (inputArray[inputArray.Length - 1] == currentKey)
            {
                // �̰��� ��ȯ
                return this;
            }
            else
            { // �ƴϸ� null ��ȯ.
                return null;
            }
        }
    }
    // ���ο� �迭 ������ ���� ���ο� ��带 ����.
    public void Insert(SkillDataManager.SkillData skill, int index)
    {
        // �ϴ� ���� ĭ�� ã��.
        index++;

        //�ε����� �迭 ���̺��� ������(������ �޺� ������ ������) �ڽ��� �����Ұ�.
        if (index < skill.ComboCommand.Length)
        {
            Trienode nextChild = NewChild(skill.ComboCommand[index]); // �ڽ� �����ؼ� ���ο� �ڽ����� ��.
            nextChild.Insert(skill, index); //�� �ڽĿ��� �ٽ� �� �Լ��� �ҷ��ͼ� �޺��� �Է���.
        }
        else if (currentSkill == null) // �迭 ũ��� ���������ٸ�
        {
            currentSkill = skill; // ���� �̸��� ��ų �̸� ����.
        }
    }

    // Ư�� ��� ����
    public void Delete()
    {
        currentSkill = null; // ���� ��� �̸��� ����.
        if (childList.Count <= 0) // �ڽ� ��尡 ������ ����.
        {
            if (parent != null) // �θ� �ִٸ�
            {
                if (parent.childList.Count <= 1 && parent.currentSkill == null) // �θ��� �ڽ� ������ 1�� Ȥ�� �� ���� �̸鼭, �θ� �̸��� ���ٸ�
                {
                    parent.Delete(); // �θ� �����. (1����� �����̱� ����.)
                }
                else
                {
                    parent.childList.Remove(this); // �θ��� �ڽ��� ��������� �� ���� ���Ḹ �����Ѵ�.
                }
                parent = null; // �� ����� �θ� ������ ���ش�.
            }
            childList.Clear(); // �� ����� �ڽ� ����Ʈ�� ���ش�.
        }
    }
    public void Destroy() // �޺� �ý��ۿ� ��ϵ� ��� �޺� ������ �ı�.
    {
        if (parent != null) // �θ� �ִٸ�
        {
            for (int i = 0; i < childList.Count; i++)// �ڽ� ����Ʈ�� ���鼭
            {
                childList[i].Destroy(); // �ڽ� ���鿡�� ���� �� �Լ��� �ϰ� ��Ŵ.
            }
            parent = null; // �θ� ���� ����
            childList.Clear(); // �ڽ��� �� ���ش�.
        }
    }
}

public class ComboSystem : MonoBehaviour
{
    // Ʈ���� ����� ����. 0�� ���. �� ���� �ƹ� ������ ���� ������µ� �츮�� �޺�Ű�� 0��(Neutral)�� �ִ´�.
    private Trienode rootTrie = new Trienode(0);
    
    // ������ üũ�� ���� ����
    private Trienode lastCheck = null;
    
    // �� Ŭ������ ��ٷ� �ִ� �Ѱ� �ð�. �� ���� �Է½ð� + limitInputTime�� �����Ұ�. ����� ���� ����.
    private float expireTime = 0.0f;
    public float ExpireTime { get { return expireTime; } set { expireTime = value; } }

    // �Է��� �ް� ���� ���� �Է±��� ��ٷ��ִ� �ð�. �� �ð� �Ѿ�� �޺��� ������.
    private float limitInputTime = 0.2f;
    public float LimitInputTime { get { return limitInputTime; } }


    // ��带 ã�°��� ����. ��Ʈ ��忡�� �����ϵ����Ѵ�. ���� ���� �ڽ��� ã��.
    private Trienode FindNodeStart(ComboKey inputKey)
    {
        return rootTrie.FindChild(inputKey);
    }

    // ��Ʈ ã�°��� ��.
    private Trienode FindNodeEnd(ComboKey[] inputArray) // �Է� �迭�� �޾ƿ�
    {
        Trienode targetNode = FindNodeStart(inputArray[0]); // �Է� �迭�� ù��° ���� ��Ʈ���� �����ϴ� Ž�� �Լ��� �־���. �׸��� �� �ڽ� ���� ��ȯ��.
        if (targetNode != null) // ã�� ��尡 �ִ�
        {
            targetNode = targetNode.Find(inputArray, 0); // ������ ã��. ����ã��

            if (targetNode != null)// ����� �ִٸ�
            {
                return targetNode; // Ÿ�� ��带 ��ȯ
            }
        }
        return null; // ����� ���ų�, ã�� ��尡 ������ null ����.
    }

    //public void Insert(ComboKey[] inputArray, string skillName)
    //{
    //    Trienode targetNode = rootTrie.NewChild(inputArray[0]);
    //    targetNode.Insert(inputArray, 0, skillName);
    //}
    // ���� �޺��� �־��ִ� ����. �̰��� �޺� ����Ʈ�� ����°��̶�� �����ϸ��.
    public void Insert(SkillDataManager.SkillData skill) // ��ų ������ �Ŵ����� �ִ� ������ ���� ����ؼ� �߰�����.
    {
        Trienode targetNode = rootTrie.NewChild(skill.ComboCommand[0]); // Ŀ�ǵ��� �� ó�� ���� ��Ʈ ����� �ڽ����� ����
        targetNode.Insert(skill, 0); // Insert������ ��ų 
        // �ٵ� �̹� �ִ°��, Insert�� if(currentSkill == "") �κ��� �ɷ��� ����� �ȵ�.
    }

    // ����. �Է� Ű �迭�� �޾ƿͼ� �ش� ��ų�� ������.
    public void Delete(ComboKey[] inputArray) 
    {
        Trienode targetNode = FindNodeEnd(inputArray);
        if (targetNode != null)
        {
            targetNode.Delete();
        }
    }
    public void ChangeSkill(ComboKey[] inputArray, SkillDataManager.SkillData skillData) // �ش� ��ų�� �̸��� ������.
    {
        Trienode targetNode = FindNodeEnd(inputArray); // ������ ����
        if (targetNode != null) // �� ��尡 null�� �ƴϸ�(���� �̹� ����������)
        {
            targetNode.currentSkill = skillData; // ��ų �̸��� ����.
        }
    }

    public SkillDataManager.SkillData Input(ComboKey inputKey, float inputTime) // ���� �޺� ����. ���� ���� Ű���ϰ�, ������ �ð��� �޾ƿ�.
    {
        if (expireTime >= inputTime) // ���� �ð��� inputTime���� �� ������(�ð� ���� �����ٸ�)
        {
            if (lastCheck != null) // üũ�� Ű�� null�� �ƴ϶��, �������� üũ�� Ű�� �ִٸ�
            {
                lastCheck = lastCheck.FindChild(inputKey);// ������ üũ�� Ű�� �̾ �̹��� �Է��� Ű�� �޺� ����Ʈ�� �ִٸ�, ���� Ű�� ��Ʈ üũ�� ����.
            }
        }
        else // �ð� ���� �������ٸ�
        {
            lastCheck = null; // null ���ؼ� ó������ ������.
            Debug.Log("reset");
        }


        ExpireTime = inputTime + limitInputTime; // �ϴ� ���� Ű �Է� ���ѽð� ����.

        if (lastCheck == null) // ���� ���� Ű�� �޺� ����Ʈ�� ���ٸ�
        {
            lastCheck = FindNodeStart(inputKey); // ���� ���� Ű�� ó���ΰ����� �Ǵ��Ͽ� �̰����� �����ϴ� �޺��� �ִ��� ã�ƺ�.
        }

        if (lastCheck != null) // Ű�� �ִٸ� 
        {
            if (lastCheck.currentSkill != null)
            {
                Debug.Log(lastCheck.currentSkill.Name); // ���� �̸��� ����Ƽ �α׿� ���..
                // �׽�Ʈ�� ����
                GameManager.Instance.InputComboKey(lastCheck.currentSkill.Name);
                //
            }
            return lastCheck.currentSkill; //�����.
        }
        else // ���� ���°��̶��
        {
            return null; // ����.
        }
    }
}
