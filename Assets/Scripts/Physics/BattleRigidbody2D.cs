using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleRigidbody2D : MonoBehaviour
{
    // 관리 하려면 이 것이 PhysicsManager가 알고있어야한다.
    [SerializeField] Vector3 velocity = Vector3.zero;
    [SerializeField] Vector3 stageCollider;
    [SerializeField] Vector3 characterCollider;
    public Vector3 CharacterCollider { get { return characterCollider; } set { characterCollider = value; } }
    public Vector3 StageCollider { get { return stageCollider; } set { stageCollider = value; } }
    public Vector3 Velocity { get { return velocity; }
set { velocity = value; } }

    [SerializeField] bool isInbound;
    [SerializeField] Vector3 overlap;
    [SerializeField] Vector3 distance;


    Vector3 previousFieldPosition;
    Vector3 fieldSize;
    float fieldPositionLimit;

    private void Start()
    {
        // physicsManager에 넣었다.
        GameManager.Instance.PhysicsManager.InsertRigidbody(this);
        // 캐릭터의 크기 설정. 스테이지에 닿는 부분, 캐릭터끼리 닿는 부분.
        CharacterCollider = new Vector3(2, 4, 0);
        StageCollider = new Vector3(3, 2, 0);

        fieldSize = GameManager.Instance.PhysicsManager.StageSize; // 필드 크기
        fieldPositionLimit = GameManager.Instance.PhysicsManager.FullStageSize - fieldSize.x * 0.5f;

    }
    //private void OnDisable()
    //{
    //    GameManager.Instance.PhysicsManager.DeleteRigidbody(this);
    //}
    private void OnDestroy()
    {
        GameManager.Instance.PhysicsManager.DeleteRigidbody(this);

    }
    public void PhysicsUpdate(float deltaTime)
    {
        Gravity(deltaTime, PhysicsManager.gravityScale);
        StageBounds();
        SolveVelocity(deltaTime);
    }
    public void CharacterCollision(BattleRigidbody2D rigidbody)
    {
        // 캐릭터 충돌판정하기전에 본인인지 확인하기!
        // 본인이면 작동하지 않음.
        if (rigidbody != this)
        {
            CharacterCollisionEnter(rigidbody);
        }
    }
    private Vector3 FieldPosition()
    {

        GameObject[] characterArray = GameManager.Instance.CharacterArray;
        Vector3 characterMiddlePoint = (characterArray[0].transform.position + characterArray[1].transform.position) * 0.5f;
        if (Mathf.Abs(characterArray[0].transform.position.x - characterArray[1].transform.position.x) < fieldSize.x-stageCollider.x)
        {
            return new Vector3(Mathf.Clamp(characterMiddlePoint.x, -fieldPositionLimit, fieldPositionLimit), 0, characterMiddlePoint.z);
        }
        return previousFieldPosition;
    }

    private bool StageBounds() // 스테이지와의 상호작용
    {
        bool result = false;
        Vector3 currentFieldPosition = FieldPosition();
        previousFieldPosition= currentFieldPosition;

        Vector3 characterPosition = gameObject.transform.position.PivotCenterBottom(StageCollider); // 캐릭터의 중심점(transform.Pos)이 사각형의 밑바닥이 되도록 설정 계산을 위함.
        //currentFieldPosition.x = 20;
        Vector3 outboundOverlapVector = Extension.OutboundSquare(characterPosition, StageCollider, currentFieldPosition, fieldSize); // 필드에서 얼마나 벗어났는지 계산
        
        if (outboundOverlapVector != Vector3.zero)
        {
            characterPosition -= outboundOverlapVector; // 계산치 만큼 바로 빼줘서 캐릭터를 이동시킴.
            result = true;
        }
       
        gameObject.transform.position = characterPosition.PivotCenterBottom2Center(StageCollider); // 적용을 위해서 중심점을 다시 원상복구.

        if (GroundCheck()) ResetGravity();
        return result;
    }

    public void ResetGravity()
    {
        velocity.y = Mathf.Max(velocity.y, 0);
    }
    private void CharacterCollisionEnter(BattleRigidbody2D other) // 캐릭터간의 상호작용
    {
        Vector3 characterPosition = gameObject.transform.position.PivotCenterBottom(CharacterCollider); // 내 캐릭터의 사각형
        Vector3 otherPosition = other.transform.position.PivotCenterBottom(other.CharacterCollider); // 충돌 판정할 대상의 사각형

        Vector3 currentFieldPosition = FieldPosition();
        previousFieldPosition= currentFieldPosition;

        Vector3 inboundOverlapVector = Extension.InboundSquare(characterPosition, CharacterCollider, otherPosition, other.CharacterCollider); // 얼마나 겹쳤는지 체크
        
        characterPosition.x += inboundOverlapVector.x * 0.75f; //0.75 = 임시. 비율 조정은 나중에.
        otherPosition.x -= inboundOverlapVector.x * 0.25f;     // 0.25 = 임시.
        float inboundOther = Extension.firstDimensionOverlap(otherPosition.x, other.StageCollider.x, currentFieldPosition.x, GameManager.Instance.PhysicsManager.StageSize.x);
        //Debug.Log(inboundOther);

        if (Mathf.Abs(inboundOther) < StageCollider.x)
        {
             inboundOther = (other.StageCollider.x - Mathf.Abs(inboundOther)) *inboundOther.Normalize(); // 적이 벽으로 밀려난 값
        }
        else inboundOther = 0; // 안밀려났으면 0
        characterPosition.x -= inboundOther;
        otherPosition.x -= inboundOther;
                               


        gameObject.transform.position = characterPosition.PivotCenterBottom2Center(CharacterCollider);
        other.transform.position = otherPosition.PivotCenterBottom2Center(other.CharacterCollider);

    }
    // 점프
    public void AddForce(Vector3 direction)
    {
        Velocity += direction;
    }

    // 속도 해결.
    private void SolveVelocity(float deltaTime)
    {
        gameObject.transform.position += Velocity * deltaTime; // 해당 게임 오브젝트의 위치값에 속도와 델타타임을 곱한걸 더해줌.
        StageBounds(); // 처리 후, 스테이지 안에 있어야함.
    }

    // 중력은 공중이라면 사용되도록 할것.
    private void Gravity(float deltaTime, float gravityScale)
    {
        velocity.y -= gravityScale * deltaTime;
    }
    public bool GroundCheck()
    {
        if (transform.position.y <= -GameManager.Instance.PhysicsManager.StageSize.y * 0.5f)
            return true;
        else 
            return false;
    }

    private void OnDrawGizmos()
    {   
        Vector3 colliderCenter = gameObject.transform.position;
        Vector3 colliderSize = StageCollider;
        Vector3 colliderStage = fieldSize;


        Gizmos.DrawWireCube(colliderCenter + colliderSize.y * 0.5f * Vector3.up, colliderSize);

        colliderSize = CharacterCollider;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(colliderCenter + colliderSize.y * 0.5f * Vector3.up, colliderSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(Mathf.Clamp(GameManager.Instance.mainCamera.transform.position.x, -30+(fieldSize.x*0.5f),30-(fieldSize.x * 0.5f)), 0), fieldSize);

    }

    
}
