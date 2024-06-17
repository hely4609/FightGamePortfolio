using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    Character parentCharacter;
    [SerializeField]DamageBox damageBox;
    private void Awake()
    {
        parentCharacter= GetComponentInParent<Character>();
    }

    public void GenerateDamageBox(string positionScaleString)
    {
        float[] boxTransformFloat = new float[4];
        boxTransformFloat = ParseTransform(positionScaleString);
        damageBox = parentCharacter.InstantiateDamageBox();
        Debug.Log("»ý¼º");
        GameManager.Instance.PhysicsManager.InsertDamageBox(damageBox);
        damageBox.transform.localPosition = new Vector2(boxTransformFloat[0], boxTransformFloat[1]);
        damageBox.transform.localScale = new Vector2(boxTransformFloat[2], boxTransformFloat[3]);
    }
    public void TransformDamageBox(string positionScaleString)
    {
        float[] boxTransformFloat = new float[4];
        boxTransformFloat =  ParseTransform(positionScaleString);
        
        damageBox.transform.localPosition = new Vector2(boxTransformFloat[0], boxTransformFloat[1]);
        damageBox.transform.localScale = new Vector2(boxTransformFloat[2], boxTransformFloat[3]);
    }
    public void DestroyDamageBox()
    {
        if(damageBox!= null)
        {

        Destroy(damageBox.gameObject);
        }
    }

    private float[] ParseTransform(string positionScaleString)
    {
        string[] boxTransformString = positionScaleString.Split(',');
        float[] boxTransformFloat = new float[boxTransformString.Length];
        for (int i = 0; i < boxTransformString.Length; i++)
        {
            boxTransformFloat[i] = float.Parse(boxTransformString[i]);
            Debug.Log(boxTransformFloat[i]);
        }
        return boxTransformFloat;
    }
        
}
