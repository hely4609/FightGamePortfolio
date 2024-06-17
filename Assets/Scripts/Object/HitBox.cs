using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    // 얘가 가지고 있어야할 것 :
    // 이녀석의 부모 오브젝트. (적이든 플레이어든 구조가 같기때문에 누구 소속인지 확실히 해야함.)
    // 히트박스 3개의 시리얼라이즈 필드. 이거는 배열로.
    // 히트박스 3개의 크기.
    // 각 히트박스의 크기 및 위치.
    // 히트박스를 켜고끄기.
    // 히트박스가 상대 공격 범위와 겹쳤는지 확인
    // 겹쳤다면(피격 되었다면) 본체에 경직을 내려야함.

    // 상대 공격범위인지 판단 = 내 공격범위는 부모의 소속인지 확인하기. 아니면 상대공격.

    // 레이어를 이용하여 공격 판단. 
    // ex) 캐릭터 1은 1번 레이어만 사용. 캐릭터 2는 2번 레이어만 사용.
    // 히트박스와 데미지 박스 모두 각각의 레이어에서 생성.
    // 캐릭터 1은 2번 레이어만 충돌판정을 진행. 캐릭터 2는 1번 레이어만 충돌판정을 진행.

    [SerializeField] Transform parent;
    [SerializeField] Transform[] hitBoxesTransformArray = new Transform[10];
    [SerializeField] List<DamageBox> enteredDamageBoxes = new List<DamageBox>();
    public Transform[] HitBoxesTransformArray { get { return hitBoxesTransformArray; } }
    // 레이어 설정
    [SerializeField] int sortingLayer;
    [SerializeField] Animator stickmanAnimator;
    [SerializeField] SkeletonMecanim stickmanBone;

    public int SortingLayer { get { return sortingLayer; } set { sortingLayer = value; } }

    // 테스트용 빌드
    private void Update()
    {
        //if(parent.GetComponent<Character>().IsGuard)
        //{
        //    HitBoxesTransformArray[2].GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 0.3f);
        //}
        //else
        //{
        //    HitBoxesTransformArray[2].GetComponent<SpriteRenderer>().color = new Color(0, 255, 34, 0.3f);
        //}

        // 박스들 위치잡기.
        for(int i = 0; i< hitBoxesTransformArray.Length; i++)
        {
            // 0 : 엉덩이 1: 가슴 2: 머리 3: 왼팔 상완 4 : 하완
            // 5: 오른팔 상완 6: 하완 7: 왼다리 허벅지 8: 정강이 9: 오른다리 허벅지 10 : 정강이

            Spine.Bone currentBone = stickmanBone.Skeleton.Bones.Items[i + 2];//현재 참고할 관절을 선택.  2개를 건너뛰는 이유 : 1번 = 루트 좌표, 2번 : Constraint Rubber body라는 이 어셋 특유의 좌표가 하나 있음.
            hitBoxesTransformArray[i].localRotation =  Quaternion.Euler(0,0,currentBone.WorldRotationX); // 해당 히트박스를 현재 관절의 회전방향을 적용.
            if(i == 2) // 머리면
            {
                hitBoxesTransformArray[i].localScale = new Vector3(currentBone.ScaleX, 1.2f, 0); // 좀 크게 만듬.
            }
            else
            { 
               hitBoxesTransformArray[i].localScale = new Vector3(currentBone.ScaleX, 0.5f,0); //그외 파츠는 좀 얇게 만듬
            }

            hitBoxesTransformArray[i].position = currentBone.GetWorldPosition(transform);

            Vector3 scalingVector =  hitBoxesTransformArray[i].localPosition;
            scalingVector.x *= stickmanBone.transform.localScale.x;
            scalingVector.y *= stickmanBone.transform.localScale.y;
            hitBoxesTransformArray[i].localPosition = scalingVector;
            hitBoxesTransformArray[i].position += hitBoxesTransformArray[i].right*0.5f;
        }


    }
    

    void Start()
    {
        stickmanAnimator = parent.GetComponent<Character>().StickmanAnimator;
        stickmanBone = parent.GetComponent<Character>().StickmanBone;
        int index = 0;
        foreach (Transform hitboxTransform in transform)
        {
            HitBoxesTransformArray[index] = hitboxTransform;
            index++;
        }
        // 레이어 설정하기.
        for (int i = 0; i < hitBoxesTransformArray.Length; i++)
        {
            hitBoxesTransformArray[i].GetComponent<SpriteRenderer>().sortingOrder = SortingLayer;
        }

        GameManager.Instance.PhysicsManager.InsertHitBox(this);
    }

    // OnCollisionEnter, Stay, Exit 시리즈.
    public void HitBoxCollisionEnter(DamageBox damageBox)
    {
        damageBox.OnHit(damageBox.GetComponentInParent<Character>(), this.GetComponentInParent<Character>());
    }
    private void HitBoxCollisionStay(DamageBox damageBox)
    {

    }
    private void HitBoxCollisionExit(DamageBox damageBox)
    {
        enteredDamageBoxes.Remove(damageBox);
    }
    // 데미지 박스를 체크하는것.
    private void OnDestroy()
    {
        GameManager.Instance.PhysicsManager.DeleteHitBox(this);
    }
    // 공격이 맞았는지 아닌지 확인
    public bool IsAttackHit(DamageBox damageBox)
    {
        if (damageBox.SortingLayer != SortingLayer) // 소팅레이어를 비교하여 다르면 진행. 자신 때리기 방지.
        {
            for (int i = 0; i < HitBoxesTransformArray.Length; i++) // 히트박스의 개수만큼 반복
            {
                //데미지 박스와 히트박스를 Extension.IsOverlapSqaure()하여 참이면 맞은것
                if (Extension.IsOverlapSquare(damageBox.transform.position, damageBox.transform.localScale, HitBoxesTransformArray[i].transform.position, HitBoxesTransformArray[i].transform.localScale))
                {
                    return true;
                }
            }
        }
        // 아무튼 아니면 안맞은거임.
        return false;
    }

    public void CheckDamageBox(DamageBox damageBox)
    {
        // 일단 리스트에 있는지 먼저 체크. List.Contains(해당 변수) = 해당 변수가 리스트에 있는지 확인.
        bool inListBox = enteredDamageBoxes.Contains(damageBox);
        // 맞았는지 체크
        if (IsAttackHit(damageBox))
        {
            // 현재 닿아있는 리스트에 있는지 체크.
            if(inListBox)
            {
                // 있었다면 CollisionStay 작동
                HitBoxCollisionStay(damageBox);
            }
            else
            {
                // 없으면 리스트에 넣는다.
                // ColliderEnter 작동
                Debug.Log($"히트 {SortingLayer}");
                enteredDamageBoxes.Add(damageBox);
                HitBoxCollisionEnter(damageBox);
            }
        }
        //안맞았다면
        else
        {
            // 리스트에 있었던 녀석이면(리스트에 있었는데 이제 안맞고있음 = 나갔음.)
            if(inListBox)
            {
                // CollisionExit함수 실행. 리스트에서 제거함.
                HitBoxCollisionExit(damageBox);
            }
        }
    }
       
}
