using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poster : MonoBehaviour
{
    [SerializeField] Image Xmark;
    // Start is called before the first frame update
    void Awake()
    {
        Xmark.gameObject.SetActive(false);
    }

    public void Cleared()
    {
        Xmark.gameObject.SetActive(true);
    }
}
