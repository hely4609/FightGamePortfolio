using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    // �갡 ������ �־���� �� :
    // �̳༮�� �θ� ������Ʈ. (���̵� �÷��̾�� ������ ���⶧���� ���� �Ҽ����� Ȯ���� �ؾ���.)
    // ��Ʈ�ڽ� 3���� �ø�������� �ʵ�. �̰Ŵ� �迭��.
    // ��Ʈ�ڽ� 3���� ũ��.
    // �� ��Ʈ�ڽ��� ũ�� �� ��ġ.
    // ��Ʈ�ڽ��� �Ѱ����.
    // ��Ʈ�ڽ��� ��� ���� ������ ���ƴ��� Ȯ��
    // ���ƴٸ�(�ǰ� �Ǿ��ٸ�) ��ü�� ������ ��������.

    // ��� ���ݹ������� �Ǵ� = �� ���ݹ����� �θ��� �Ҽ����� Ȯ���ϱ�. �ƴϸ� ������.

    // ���̾ �̿��Ͽ� ���� �Ǵ�. 
    // ex) ĳ���� 1�� 1�� ���̾ ���. ĳ���� 2�� 2�� ���̾ ���.
    // ��Ʈ�ڽ��� ������ �ڽ� ��� ������ ���̾�� ����.
    // ĳ���� 1�� 2�� ���̾ �浹������ ����. ĳ���� 2�� 1�� ���̾ �浹������ ����.

    [SerializeField] Transform parent;
    [SerializeField] Transform[] hitBoxesTransformArray = new Transform[10];
    [SerializeField] List<DamageBox> enteredDamageBoxes = new List<DamageBox>();
    public Transform[] HitBoxesTransformArray { get { return hitBoxesTransformArray; } }
    // ���̾� ����
    [SerializeField] int sortingLayer;
    [SerializeField] Animator stickmanAnimator;
    [SerializeField] SkeletonMecanim stickmanBone;

    public int SortingLayer { get { return sortingLayer; } set { sortingLayer = value; } }

    // �׽�Ʈ�� ����
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

        // �ڽ��� ��ġ���.
        for(int i = 0; i< hitBoxesTransformArray.Length; i++)
        {
            // 0 : ������ 1: ���� 2: �Ӹ� 3: ���� ��� 4 : �Ͽ�
            // 5: ������ ��� 6: �Ͽ� 7: �޴ٸ� ����� 8: ������ 9: �����ٸ� ����� 10 : ������

            Spine.Bone currentBone = stickmanBone.Skeleton.Bones.Items[i + 2];//���� ������ ������ ����.  2���� �ǳʶٴ� ���� : 1�� = ��Ʈ ��ǥ, 2�� : Constraint Rubber body��� �� ��� Ư���� ��ǥ�� �ϳ� ����.
            hitBoxesTransformArray[i].localRotation =  Quaternion.Euler(0,0,currentBone.WorldRotationX); // �ش� ��Ʈ�ڽ��� ���� ������ ȸ�������� ����.
            if(i == 2) // �Ӹ���
            {
                hitBoxesTransformArray[i].localScale = new Vector3(currentBone.ScaleX, 1.2f, 0); // �� ũ�� ����.
            }
            else
            { 
               hitBoxesTransformArray[i].localScale = new Vector3(currentBone.ScaleX, 0.5f,0); //�׿� ������ �� ��� ����
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
        // ���̾� �����ϱ�.
        for (int i = 0; i < hitBoxesTransformArray.Length; i++)
        {
            hitBoxesTransformArray[i].GetComponent<SpriteRenderer>().sortingOrder = SortingLayer;
        }

        GameManager.Instance.PhysicsManager.InsertHitBox(this);
    }

    // OnCollisionEnter, Stay, Exit �ø���.
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
    // ������ �ڽ��� üũ�ϴ°�.
    private void OnDestroy()
    {
        GameManager.Instance.PhysicsManager.DeleteHitBox(this);
    }
    // ������ �¾Ҵ��� �ƴ��� Ȯ��
    public bool IsAttackHit(DamageBox damageBox)
    {
        if (damageBox.SortingLayer != SortingLayer) // ���÷��̾ ���Ͽ� �ٸ��� ����. �ڽ� ������ ����.
        {
            for (int i = 0; i < HitBoxesTransformArray.Length; i++) // ��Ʈ�ڽ��� ������ŭ �ݺ�
            {
                //������ �ڽ��� ��Ʈ�ڽ��� Extension.IsOverlapSqaure()�Ͽ� ���̸� ������
                if (Extension.IsOverlapSquare(damageBox.transform.position, damageBox.transform.localScale, HitBoxesTransformArray[i].transform.position, HitBoxesTransformArray[i].transform.localScale))
                {
                    return true;
                }
            }
        }
        // �ƹ�ư �ƴϸ� �ȸ�������.
        return false;
    }

    public void CheckDamageBox(DamageBox damageBox)
    {
        // �ϴ� ����Ʈ�� �ִ��� ���� üũ. List.Contains(�ش� ����) = �ش� ������ ����Ʈ�� �ִ��� Ȯ��.
        bool inListBox = enteredDamageBoxes.Contains(damageBox);
        // �¾Ҵ��� üũ
        if (IsAttackHit(damageBox))
        {
            // ���� ����ִ� ����Ʈ�� �ִ��� üũ.
            if(inListBox)
            {
                // �־��ٸ� CollisionStay �۵�
                HitBoxCollisionStay(damageBox);
            }
            else
            {
                // ������ ����Ʈ�� �ִ´�.
                // ColliderEnter �۵�
                Debug.Log($"��Ʈ {SortingLayer}");
                enteredDamageBoxes.Add(damageBox);
                HitBoxCollisionEnter(damageBox);
            }
        }
        //�ȸ¾Ҵٸ�
        else
        {
            // ����Ʈ�� �־��� �༮�̸�(����Ʈ�� �־��µ� ���� �ȸ°����� = ������.)
            if(inListBox)
            {
                // CollisionExit�Լ� ����. ����Ʈ���� ������.
                HitBoxCollisionExit(damageBox);
            }
        }
    }
       
}
