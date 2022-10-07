using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartBar : MonoBehaviour
{
    public Transform displayText;
    public Vector3 displayTextOriginalScale = Vector3.one;
    public float scaleMod = 2.0f;

    public void OnMouseEnter()
    {
        displayText.localScale = displayTextOriginalScale * scaleMod;
    }

    public void OnMouseExit()
    {
        displayText.localScale = displayTextOriginalScale;
    }

    public void Start()
    {
        displayTextOriginalScale = displayText.localScale;    
    }
}
