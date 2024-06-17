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
    { // 계산할때 피봇을 사이즈크기의 상자만큼 올려서 생각.
        return playerPos + squareSize.y * 0.5f * Vector3.up;
    }
    public static Vector3 PivotCenterBottom2Center(this Vector3 playerPos, Vector3 squareSize)
    {// 적용할 때 해당 좌표를 구해서 넘겨줌. 반드시 PivotCenterBottom이랑 같이써야함.
        return playerPos - squareSize.y * 0.5f * Vector3.up;
    }
    public static Vector3 SquareDistance(Vector3 characterPos, Vector3 otherPos)
    {// 두 물체 사이의 거리를 구한다. 직선 거리가 아닌, x차이, y차이 만큼의 값만 돌려줌.)
        return new Vector3(characterPos.x - otherPos.x, characterPos.y - otherPos.y, 0);
    }

    public static Vector3 StageOutboundOverlapAmount(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)    
    {
        // 캐릭터가 필드를 얼마나 나갔냐 를 계산한다. otherSize = 필드 크기. 
        Vector3 overlap = Overlap(characterPos, characterSize, otherPos, otherSize);
        
        return characterSize - overlap;
    }
    public static Vector3 CharacterInboundOverlapAmount(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)
    {
        Vector3 overlap = Overlap(characterPos, characterSize, otherPos, otherSize);

        return characterSize - overlap;
    }
    public static Vector3 OutboundSquare(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)
    {// 필요한 것은 캐릭터가 필드를 얼마나 나갔냐. 근데 +- 부호를 남겨야함.
        // + : 위, 오른쪽으로 나감.   - : 아래, 왼쪽으로 나감.
        Vector3 result = Vector3.zero;
        Vector3 characterPeak = characterPos; // 원점과 가장 먼 정점.
        Vector3 otherPeak = otherPos;
        Vector3 characterDistance;

        if (IsOverlapField(characterPos, characterSize, otherPos, otherSize))
        {
            characterDistance = SquareDistance(characterPos, otherPos); // 일단 벡터를 구함. 결과값 : 캐릭터와 필드의 위치 차이.

            if (characterDistance.x > 0)
            {
                characterPeak.x += characterSize.x * 0.5f;
                otherPeak.x += otherSize.x * 0.5f;
                if (characterPeak.x < otherPeak.x) { characterPeak.x = otherPeak.x; } // 만약 안넘어갔으면 0으로 만들어주기 위함.
            } // 사이즈의 절반만큼 크기를 더해줌. 결과값 : 캐릭터와 필드의 거리차 + 크기.
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
            // 양수로만 받아온다.
            result = Overlap(characterPos, characterSize, otherPos, otherSize);
            if(SquareDistance(characterPos, otherPos).x < 0)
            {
                result.x = -result.x; // 물체와의 위치에서 캐릭터가 오른쪽에 있으면 - 값으로 반환
            }
            if(SquareDistance(characterPos, otherPos).y < 0)
            {
                result.y = -result.y;
            }
        }
        return result;
    }

    public static bool IsOverlapSquare(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)
    { // 사각형이랑 겹쳤니?
        Vector3 result = characterPos.Overlap(characterSize, otherPos, otherSize);
        return result.x > 0 && result.y > 0;
    }

    public static bool IsOverlapField(this Vector3 characterPos, Vector3 characterSize, Vector3 otherPos, Vector3 otherSize)
    { // 필드 밖이랑 겹쳤니?
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
    {// 두 캐릭터 사각형이 얼마나 겹쳐있는지 구한다.
        // 겹쳐져있지 않음 : 음수값이 나옴, 겹쳐있음 : 양수값이 나옴.
        Vector3 distance = SquareDistance(characterPos, otherPos); // 거리는 무조건 양수.
        distance.y = Mathf.Abs(distance.y);
        distance.x = Mathf.Abs(distance.x);
        Vector3 minDistance = (characterSize + otherSize) * 0.5f; // 캐릭터 사이즈 와 부딛힐 것의 사이즈의 차. / 2

        return minDistance - distance;
    }

    public static Vector3 OtherPlayerVector(this Vector3 player, Vector3 other) // 다른플레이어로 향하는 벡터.
    {
        return SquareDistance(other, player);
    }
}
