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

// 트라이 노드 구조.
public class Trienode
{
    // 현재 입력 받은 키.
    ComboKey currentKey;
    // 일단 여태까지 중 완성된 커맨드가 있으면 표기.
    public SkillDataManager.SkillData currentSkill;

    //이 노드의 상위 노드.
    Trienode parent = null;

    //이 노드의 하위 리스트 노드
    List<Trienode> childList = new List<Trienode>();

    // 이 노드를 생성할때 생성자로 노드에 입력받은 키를 생성한다.
    public Trienode(ComboKey inputKey)
    {
        currentKey = inputKey;
    }

    // 해당하는 자식을 찾는 함수.
    public Trienode FindChild(ComboKey inputKey)
    {
        for (int i = 0; i < childList.Count; i++)
        {
            if (childList[i].currentKey == inputKey) { return childList[i]; }
        }
        return null;
    }

    // 새로운 자식을 생성하는 함수.
    public Trienode NewChild(ComboKey inputKey)
    {
        // 자식을 찾아본다.
        Trienode returnNode = FindChild(inputKey);
        // 자식이 없다면 만든다.
        if (returnNode == null)
        {
            // 새로운 노드를 만듬. returnNode에 그걸 넣어줌
            returnNode = new Trienode(inputKey);

            //  returnNode의 부모를 "이" 노드로 한다.
            //  returnNode는 자식인 트라이노드 이기 때문에 현재 노드를 returnNode의 부모로 넣어주는것.
            // 이 노드가 Trienode를 생성해서 이걸 returnNode라고 함. 생성 당시에는 parent가 null이기 때문에 이 parent에 이 노드를 넣어줌.
            returnNode.parent = this;
            // 만들어진 노드를 childList에 넣어줌.
            childList.Add(returnNode);
        }
        return returnNode;
    }

    // 받은 입력값의 배열을 넣고, 노드 리스트에서 
    public Trienode Find(ComboKey[] inputArray, int index)
    {
        // 다음 위치를 찾기위해 +1
        index++;
        // index가 배열 길이보다 작을때 = 다음 위치가 남았다면
        if (index < inputArray.Length)
        {
            // index+1의 위치에 해당하는 키를 가진 하위 노드를 찾아옴.
            Trienode nextChild = FindChild(inputArray[index]);

            // 있다면
            if (nextChild != null)
            {
                // 해당 노드를 또 Find함수를 돌려서 계속 찾음.
                // 만약 노드에 계속 하위노드가 있다면 여기까지 계속 돌것임.
                return nextChild.Find(inputArray, index);
            }
            // 없으면 null 반환.
            else { return null; }
        }
        // 배열 길이랑 같다면.
        else
        {// 이 노드랑 inputArry 의 마지막 칸을 비교해서 동일하면
            if (inputArray[inputArray.Length - 1] == currentKey)
            {
                // 이것을 반환
                return this;
            }
            else
            { // 아니면 null 반환.
                return null;
            }
        }
    }
    // 새로운 배열 순서에 따라 새로운 노드를 만듬.
    public void Insert(SkillDataManager.SkillData skill, int index)
    {
        // 일단 다음 칸을 찾음.
        index++;

        //인덱스가 배열 길이보다 작으면(지정된 콤보 수보다 작으면) 자식을 생산할것.
        if (index < skill.ComboCommand.Length)
        {
            Trienode nextChild = NewChild(skill.ComboCommand[index]); // 자식 생성해서 새로운 자식으로 함.
            nextChild.Insert(skill, index); //이 자식에서 다시 이 함수를 불러와서 콤보를 입력함.
        }
        else if (currentSkill == null) // 배열 크기랑 동일해졌다면
        {
            currentSkill = skill; // 현재 이름에 스킬 이름 저장.
        }
    }

    // 특정 노드 삭제
    public void Delete()
    {
        currentSkill = null; // 현재 노드 이름을 삭제.
        if (childList.Count <= 0) // 자식 노드가 없으면 실행.
        {
            if (parent != null) // 부모가 있다면
            {
                if (parent.childList.Count <= 1 && parent.currentSkill == null) // 부모의 자식 개수가 1개 혹은 그 이하 이면서, 부모 이름이 없다면
                {
                    parent.Delete(); // 부모를 지운다. (1개라면 본인이기 때문.)
                }
                else
                {
                    parent.childList.Remove(this); // 부모의 자식이 여러개라면 이 노드와 연결만 해제한다.
                }
                parent = null; // 이 노드의 부모 정보를 없앤다.
            }
            childList.Clear(); // 이 노드의 자식 리스트를 없앤다.
        }
    }
    public void Destroy() // 콤보 시스템에 등록된 모든 콤보 정보를 파괴.
    {
        if (parent != null) // 부모가 있다면
        {
            for (int i = 0; i < childList.Count; i++)// 자식 리스트를 돌면서
            {
                childList[i].Destroy(); // 자식 노드들에게 전부 이 함수를 하게 시킴.
            }
            parent = null; // 부모 연결 해제
            childList.Clear(); // 자식을 다 없앤다.
        }
    }
}

public class ComboSystem : MonoBehaviour
{
    // 트라이 노드의 시작. 0번 노드. 이 노드는 아무 정보가 들어가도 상관없는데 우리는 콤보키의 0번(Neutral)을 넣는다.
    private Trienode rootTrie = new Trienode(0);
    
    // 마지막 체크한 값은 없음
    private Trienode lastCheck = null;
    
    // 이 클래스가 기다려 주는 한계 시간. 이 값은 입력시간 + limitInputTime을 적용할것. 계산을 위한 변수.
    private float expireTime = 0.0f;
    public float ExpireTime { get { return expireTime; } set { expireTime = value; } }

    // 입력을 받고 나서 다음 입력까지 기다려주는 시간. 이 시간 넘어가면 콤보로 안쳐줌.
    private float limitInputTime = 0.2f;
    public float LimitInputTime { get { return limitInputTime; } }


    // 노드를 찾는것을 시작. 루트 노드에서 시작하도록한다. 들어온 값의 자식을 찾음.
    private Trienode FindNodeStart(ComboKey inputKey)
    {
        return rootTrie.FindChild(inputKey);
    }

    // 노트 찾는것의 끝.
    private Trienode FindNodeEnd(ComboKey[] inputArray) // 입력 배열을 받아옴
    {
        Trienode targetNode = FindNodeStart(inputArray[0]); // 입력 배열의 첫번째 값을 루트노드로 시작하는 탐색 함수에 넣어줌. 그리고 그 자식 값을 반환함.
        if (targetNode != null) // 찾은 노드가 있다
        {
            targetNode = targetNode.Find(inputArray, 0); // 다음을 찾음. 쭉쭉찾음

            if (targetNode != null)// 결과가 있다면
            {
                return targetNode; // 타겟 노드를 반환
            }
        }
        return null; // 결과가 없거나, 찾은 노드가 없으면 null 리턴.
    }

    //public void Insert(ComboKey[] inputArray, string skillName)
    //{
    //    Trienode targetNode = rootTrie.NewChild(inputArray[0]);
    //    targetNode.Insert(inputArray, 0, skillName);
    //}
    // 만든 콤보를 넣어주는 구문. 이것은 콤보 리스트를 만드는것이라고 생각하면됨.
    public void Insert(SkillDataManager.SkillData skill) // 스킬 데이터 매니저에 있는 지정된 값을 사용해서 추가해줌.
    {
        Trienode targetNode = rootTrie.NewChild(skill.ComboCommand[0]); // 커맨드의 맨 처음 것을 루트 노드의 자식으로 설정
        targetNode.Insert(skill, 0); // Insert에서도 스킬 
        // 근데 이미 있는경우, Insert의 if(currentSkill == "") 부분이 걸려서 등록이 안됨.
    }

    // 삭제. 입력 키 배열을 받아와서 해당 스킬을 제거함.
    public void Delete(ComboKey[] inputArray) 
    {
        Trienode targetNode = FindNodeEnd(inputArray);
        if (targetNode != null)
        {
            targetNode.Delete();
        }
    }
    public void ChangeSkill(ComboKey[] inputArray, SkillDataManager.SkillData skillData) // 해당 스킬의 이름을 변경함.
    {
        Trienode targetNode = FindNodeEnd(inputArray); // 끝까지 가서
        if (targetNode != null) // 그 노드가 null이 아니면(누가 이미 쓰고있으면)
        {
            targetNode.currentSkill = skillData; // 스킬 이름을 변경.
        }
    }

    public SkillDataManager.SkillData Input(ComboKey inputKey, float inputTime) // 드디어 콤보 실행. 현재 누른 키값하고, 눌러진 시간을 받아옴.
    {
        if (expireTime >= inputTime) // 남은 시간이 inputTime보다 더 많으면(시간 내에 눌렀다면)
        {
            if (lastCheck != null) // 체크한 키가 null이 아니라면, 지난번에 체크한 키가 있다면
            {
                lastCheck = lastCheck.FindChild(inputKey);// 지난번 체크한 키에 이어서 이번에 입력한 키가 콤보 리스트에 있다면, 현재 키를 라스트 체크에 넣음.
            }
        }
        else // 시간 내에 못눌렀다면
        {
            lastCheck = null; // null 로해서 처음부터 시작함.
            Debug.Log("reset");
        }


        ExpireTime = inputTime + limitInputTime; // 일단 다음 키 입력 제한시간 갱신.

        if (lastCheck == null) // 현재 누른 키가 콤보 리스트에 없다면
        {
            lastCheck = FindNodeStart(inputKey); // 지금 들어온 키가 처음인것으로 판단하여 이것으로 시작하는 콤보가 있는지 찾아봄.
        }

        if (lastCheck != null) // 키가 있다면 
        {
            if (lastCheck.currentSkill != null)
            {
                Debug.Log(lastCheck.currentSkill.Name); // 현재 이름을 유니티 로그에 출력..
                // 테스트용 빌드
                GameManager.Instance.InputComboKey(lastCheck.currentSkill.Name);
                //
            }
            return lastCheck.currentSkill; //출력함.
        }
        else // 만약 없는것이라면
        {
            return null; // 없음.
        }
    }
}
