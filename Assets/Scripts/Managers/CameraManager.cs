using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject[] characterArray = new GameObject[2]; // ĳ���͸� �޾ƿ�.
    Vector3 characterStageColliderSize;

    Vector3 characterDistance; // ĳ���Ͱ��� �Ÿ�.
    float screenRatio;
    float orthographicSize;
    float stageSize; // �������� ũ�� �޾ƿ�.
    Camera mainCamera;
    Rect cameraRect;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        characterArray = GameManager.Instance.CharacterArray; // �����ϸ� ���� �Ŵ������� ĳ���͵��� �޾ƿ�.
        characterStageColliderSize = characterArray[0].GetComponent<BattleRigidbody2D>().StageCollider;
        screenRatio = (float)Screen.height / Screen.width; // screen�� ���� int��.
        stageSize = GameManager.Instance.PhysicsManager.FullStageSize; // �������� ������� physics �Ŵ������� �޾ƿ�.
        CameraRectCalc();
    }
    private void Update()
    {

        orthographicSize = mainCamera.orthographicSize = CameraScale(); // ���ұ׷��� ����� CameraScale() �Լ����� ���� ���� �־���.

        CameraRectCalc();

        CameraPosition(); // �� ������Ʈ������ ī�޶� �������� ��������. ���� �Ƚ��忡�� �Ҹ�ŭ �߿��Ѱ� �ƴ϶� �׳� ������Ʈ���� �ص� ��.

    }
    private void CameraRectCalc() // ī�޶��� �ð��� ��� �ִ��� �󸶳� ū�� �˷��ִ� �Լ�.
    {
        Vector3 leftUp = mainCamera.ViewportToWorldPoint(Vector3.up);
        Vector3 rightDown = mainCamera.ViewportToWorldPoint(Vector3.right);
        cameraRect.x = leftUp.x;
        cameraRect.y = leftUp.y;
        cameraRect.width = rightDown.x - leftUp.x;
        cameraRect.height = leftUp.y - rightDown.y;
    }
    private void CameraPosition() // ī�޶� ��ġ�� �����.
    {
        Vector3 cameraTransform; // ī�޶� ��ġ�� �� ����.
        characterDistance = characterArray[0].transform.position - characterArray[1].transform.position; // ĳ���� ���� �Ÿ�
       
        cameraTransform = (characterDistance) * 0.5f + characterArray[1].transform.position;
        //if(cameraTransform.x <
        
        cameraTransform.y += orthographicSize-1; // �ϴ� UIǥ�ø� ���� ���� ���
        cameraTransform.z = -10;
        
        cameraTransform.x = Mathf.Clamp(cameraTransform.x, -stageSize + cameraRect.width * 0.5f, stageSize - cameraRect.width * 0.5f);

        // ī�޶��� ���κ��� �������� ���� ��Ҵٸ� ī�޶� �� �������� ����. Max�� Min�� ����ϸ� ����������?
        // ���밪�� ���������� x���� ���� �̻��̶�� �������� x�� ������ ��ȯ, �׸��� Extension.Normalize�� ������ ���� �޾ƿͼ� �����ָ� ��ȣ�� �ùٸ��� ����.
        
        transform.position = cameraTransform; // ī�޶� ���� ����
    }
    private float CameraScale()
    {
        
        return Mathf.Max((Mathf.Abs(characterDistance.x-1) + characterStageColliderSize.x)* screenRatio * 0.5f, 5);
           
    }
}
