using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static float Normalize(this float value)
    {
        return (value == 0 ? 0 : (value > 0 ? 1 : -1));
    }
    public static Vector3 PivotCenterBottom(this Vector3 playerPos, Vector3 squareSize)
    { // ����Ҷ� �Ǻ��� ������ũ���� ���ڸ�ŭ �÷��� ����.
        return playerPos + squareSize.y * 0.5f * Vector3.up;
    }
    public static Vector3 PivotCenterBottom2Center(this Vector3 playerPos, Vector3 squareSize)
    {// ������ �� �ش� ��ǥ�� ���ؼ� �Ѱ���. �ݵ�� PivotCenterBottom�̶� ���̽����.
        return playerPos - squareSize.y * 0.5f * Vector3.up;
    }
    public static Vector3 SquareDistance(Vector3 characterPos, Vector3 otherPos)
    {// �� ��ü ������ �Ÿ��� ���Ѵ�. ���� �Ÿ��� �ƴ�, x����, y���� ��ŭ�� ���� ������.)
        return new Vector3(characterPos.x - otherPos.x, characterPos.y - otherPos.y, 0);
    }

    public static Vector3 StageOutboundOverlapAmount(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)    
    {
        // ĳ���Ͱ� �ʵ带 �󸶳� ������ �� ����Ѵ�. otherSize = �ʵ� ũ��. 
        Vector3 overlap = Overlap(characterPos, characterSize, otherPos, otherSize);
        
        return characterSize - overlap;
    }
    public static Vector3 CharacterInboundOverlapAmount(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)
    {
        Vector3 overlap = Overlap(characterPos, characterSize, otherPos, otherSize);

        return characterSize - overlap;
    }
    public static Vector3 OutboundSquare(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)
    {// �ʿ��� ���� ĳ���Ͱ� �ʵ带 �󸶳� ������. �ٵ� +- ��ȣ�� ���ܾ���.
        // + : ��, ���������� ����.   - : �Ʒ�, �������� ����.
        Vector3 result = Vector3.zero;
        Vector3 characterPeak = characterPos; // ������ ���� �� ����.
        Vector3 otherPeak = otherPos;
        Vector3 characterDistance;

        if (IsOverlapField(characterPos, characterSize, otherPos, otherSize))
        {
            characterDistance = SquareDistance(characterPos, otherPos); // �ϴ� ���͸� ����. ����� : ĳ���Ϳ� �ʵ��� ��ġ ����.

            if (characterDistance.x > 0)
            {
                characterPeak.x += characterSize.x * 0.5f;
                otherPeak.x += otherSize.x * 0.5f;
                if (characterPeak.x < otherPeak.x) { characterPeak.x = otherPeak.x; } // ���� �ȳѾ���� 0���� ������ֱ� ����.
            } // �������� ���ݸ�ŭ ũ�⸦ ������. ����� : ĳ���Ϳ� �ʵ��� �Ÿ��� + ũ��.
            else
            {
                characterPeak.x -= characterSize.x * 0.5f;
                otherPeak.x -= otherSize.x * 0.5f;
                if (characterPeak.x > otherPeak.x) { characterPeak.x = otherPeak.x; }
            }
            
            if (characterDistance.y > 0)
            {
                characterPeak.y += characterSize.y * 0.5f;
                otherPeak.y += otherSize.y * 0.5f;
                if( characterPeak.y < otherPeak.y) { characterPeak.y = otherPeak.y;}
            }
            else
            {
                characterPeak.y -= characterSize.y * 0.5f;
                otherPeak.y -= otherSize.y * 0.5f;
                if(characterPeak.y > otherPeak.y) { characterPeak.y = otherPeak.y;}
            }
            result = characterPeak - otherPeak;
        }
        return result;
    }
    public static Vector3 InboundSquare(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)
    {
        Vector3 result = Vector3.zero;

        //
        if (IsOverlapSquare(characterPos, characterSize, otherPos, otherSize))
        {
            // ����θ� �޾ƿ´�.
            result = Overlap(characterPos, characterSize, otherPos, otherSize);
            if(SquareDistance(characterPos, otherPos).x < 0)
            {
                result.x = -result.x; // ��ü���� ��ġ���� ĳ���Ͱ� �����ʿ� ������ - ������ ��ȯ
            }
            if(SquareDistance(characterPos, otherPos).y < 0)
            {
                result.y = -result.y;
            }
        }
        return result;
    }

    public static bool IsOverlapSquare(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)
    { // �簢���̶� ���ƴ�?
        Vector3 result = characterPos.Overlap(characterSize, otherPos, otherSize);
        return result.x > 0 && result.y > 0;
    }

    public static bool IsOverlapField(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)
    { // �ʵ� ���̶� ���ƴ�?
        Vector3 result = characterPos.StageOutboundOverlapAmount(characterSize, otherPos, otherSize);
        return result.x > 0 || result.y > 0;
    }
    public static Vector3 CharacterOverlap(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)
    {
        Vector3 result = Vector3.zero;
        Vector3 overlap = Overlap(characterPos, characterSize, otherPos, otherSize);
        if (overlap.y > 0 && overlap.x >0)
        {
            result = overlap;
        }

        return result;
    }
    public static float firstDimensionOverlap(this float myPos, float myLength, float otherPos, float otherLength)
    {
        float difference = myPos - otherPos;
        float distance = Mathf.Abs(difference);
        float minLength = (myLength + otherLength) * 0.5f;
        
        return (minLength - distance)*difference.Normalize();
    }


    public static Vector3 Overlap(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)
    {// �� ĳ���� �簢���� �󸶳� �����ִ��� ���Ѵ�.
        // ���������� ���� : �������� ����, �������� : ������� ����.
        Vector3 distance = SquareDistance(characterPos, otherPos); // �Ÿ��� ������ ���.
        distance.y = Mathf.Abs(distance.y);
        distance.x = Mathf.Abs(distance.x);
        Vector3 minDistance = (characterSize + otherSize) * 0.5f; // ĳ���� ������ �� �ε��� ���� �������� ��. / 2

        return minDistance - distance;
    }

    public static Vector3 OtherPlayerVector(this Vector3 player, Vector3 other) // �ٸ��÷��̾�� ���ϴ� ����.
    {
        return SquareDistance(other, player);
    }
}
