using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleRigidbody2D : MonoBehaviour
{
    // ���� �Ϸ��� �� ���� PhysicsManager�� �˰��־���Ѵ�.
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
        // physicsManager�� �־���.
        GameManager.Instance.PhysicsManager.InsertRigidbody(this);
        // ĳ������ ũ�� ����. ���������� ��� �κ�, ĳ���ͳ��� ��� �κ�.
        CharacterCollider = new Vector3(2, 4, 0);
        StageCollider = new Vector3(3, 2, 0);

        fieldSize = GameManager.Instance.PhysicsManager.StageSize; // �ʵ� ũ��
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
        // ĳ���� �浹�����ϱ����� �������� Ȯ���ϱ�!
        // �����̸� �۵����� ����.
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

    private bool StageBounds() // ������������ ��ȣ�ۿ�
    {
        bool result = false;
        Vector3 currentFieldPosition = FieldPosition();
        previousFieldPosition= currentFieldPosition;

        Vector3 characterPosition = gameObject.transform.position.PivotCenterBottom(StageCollider); // ĳ������ �߽���(transform.Pos)�� �簢���� �عٴ��� �ǵ��� ���� ����� ����.
        //currentFieldPosition.x = 20;
        Vector3 outboundOverlapVector = Extension.OutboundSquare(characterPosition, StageCollider, currentFieldPosition, fieldSize); // �ʵ忡�� �󸶳� ������� ���
        
        if (outboundOverlapVector != Vector3.zero)
        {
            characterPosition -= outboundOverlapVector; // ���ġ ��ŭ �ٷ� ���༭ ĳ���͸� �̵���Ŵ.
            result = true;
        }
       
        gameObject.transform.position = characterPosition.PivotCenterBottom2Center(StageCollider); // ������ ���ؼ� �߽����� �ٽ� ���󺹱�.

        if (GroundCheck()) ResetGravity();
        return result;
    }

    public void ResetGravity()
    {
        velocity.y = Mathf.Max(velocity.y, 0);
    }
    private void CharacterCollisionEnter(BattleRigidbody2D other) // ĳ���Ͱ��� ��ȣ�ۿ�
    {
        Vector3 characterPosition = gameObject.transform.position.PivotCenterBottom(CharacterCollider); // �� ĳ������ �簢��
        Vector3 otherPosition = other.transform.position.PivotCenterBottom(other.CharacterCollider); // �浹 ������ ����� �簢��

        Vector3 currentFieldPosition = FieldPosition();
        previousFieldPosition= currentFieldPosition;

        Vector3 inboundOverlapVector = Extension.InboundSquare(characterPosition, CharacterCollider, otherPosition, other.CharacterCollider); // �󸶳� ���ƴ��� üũ
        
        characterPosition.x += inboundOverlapVector.x * 0.75f; //0.75 = �ӽ�. ���� ������ ���߿�.
        otherPosition.x -= inboundOverlapVector.x * 0.25f;     // 0.25 = �ӽ�.
        float inboundOther = Extension.firstDimensionOverlap(otherPosition.x, other.StageCollider.x, currentFieldPosition.x, GameManager.Instance.PhysicsManager.StageSize.x);
        //Debug.Log(inboundOther);

        if (Mathf.Abs(inboundOther) < StageCollider.x)
        {
             inboundOther = (other.StageCollider.x - Mathf.Abs(inboundOther)) *inboundOther.Normalize(); // ���� ������ �з��� ��
        }
        else inboundOther = 0; // �ȹз������� 0
        characterPosition.x -= inboundOther;
        otherPosition.x -= inboundOther;
                               


        gameObject.transform.position = characterPosition.PivotCenterBottom2Center(CharacterCollider);
        other.transform.position = otherPosition.PivotCenterBottom2Center(other.CharacterCollider);

    }
    // ����
    public void AddForce(Vector3 direction)
    {
        Velocity += direction;
    }

    // �ӵ� �ذ�.
    private void SolveVelocity(float deltaTime)
    {
        gameObject.transform.position += Velocity * deltaTime; // �ش� ���� ������Ʈ�� ��ġ���� �ӵ��� ��ŸŸ���� ���Ѱ� ������.
        StageBounds(); // ó�� ��, �������� �ȿ� �־����.
    }

    // �߷��� �����̶�� ���ǵ��� �Ұ�.
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
