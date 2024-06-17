using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject[] characterArray = new GameObject[2]; // 캐릭터를 받아옴.
    Vector3 characterStageColliderSize;

    Vector3 characterDistance; // 캐릭터간의 거리.
    float screenRatio;
    float orthographicSize;
    float stageSize; // 스테이지 크기 받아옴.
    Camera mainCamera;
    Rect cameraRect;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        characterArray = GameManager.Instance.CharacterArray; // 시작하면 게임 매니저에서 캐릭터들을 받아옴.
        characterStageColliderSize = characterArray[0].GetComponent<BattleRigidbody2D>().StageCollider;
        screenRatio = (float)Screen.height / Screen.width; // screen의 값은 int다.
        stageSize = GameManager.Instance.PhysicsManager.FullStageSize; // 스테이지 사이즈는 physics 매니저에서 받아옴.
        CameraRectCalc();
    }
    private void Update()
    {

        orthographicSize = mainCamera.orthographicSize = CameraScale(); // 오소그래픽 사이즈에 CameraScale() 함수에서 계산된 값을 넣어줌.

        CameraRectCalc();

        CameraPosition(); // 매 업데이트때마다 카메라 포지션을 갱신해줌. 딱히 픽스드에서 할만큼 중요한건 아니라 그냥 업데이트에서 해도 됨.

    }
    private void CameraRectCalc() // 카메라의 시각이 어디에 있는지 얼마나 큰지 알려주는 함수.
    {
        Vector3 leftUp = mainCamera.ViewportToWorldPoint(Vector3.up);
        Vector3 rightDown = mainCamera.ViewportToWorldPoint(Vector3.right);
        cameraRect.x = leftUp.x;
        cameraRect.y = leftUp.y;
        cameraRect.width = rightDown.x - leftUp.x;
        cameraRect.height = leftUp.y - rightDown.y;
    }
    private void CameraPosition() // 카메라 위치를 잡아줌.
    {
        Vector3 cameraTransform; // 카메라 위치가 될 변수.
        characterDistance = characterArray[0].transform.position - characterArray[1].transform.position; // 캐릭터 간의 거리
       
        cameraTransform = (characterDistance) * 0.5f + characterArray[1].transform.position;
        //if(cameraTransform.x <
        
        cameraTransform.y += orthographicSize-1; // 하단 UI표시를 위해 조금 띄움
        cameraTransform.z = -10;
        
        cameraTransform.x = Mathf.Clamp(cameraTransform.x, -stageSize + cameraRect.width * 0.5f, stageSize - cameraRect.width * 0.5f);

        // 카메라의 끝부분이 스테이지 벽에 닿았다면 카메라가 더 움직이지 않음. Max나 Min을 사용하면 되지않을까?
        // 절대값이 스테이지의 x값의 절반 이상이라면 스테이지 x의 값으로 변환, 그리고 Extension.Normalize로 현재의 값을 받아와서 곱해주면 부호도 올바르게 들어갈것.
        
        transform.position = cameraTransform; // 카메라 정보 갱신
    }
    private float CameraScale()
    {
        
        return Mathf.Max((Mathf.Abs(characterDistance.x-1) + characterStageColliderSize.x)* screenRatio * 0.5f, 5);
           
    }
}
