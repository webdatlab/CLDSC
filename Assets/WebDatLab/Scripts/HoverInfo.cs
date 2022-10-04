using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverInfo : MonoBehaviour
{
    public Vector3 noHoverPosition = Vector3.zero;
    public Vector3 stateHoverPosition = Vector3.zero;
    public Vector3 countyHoverPosition = Vector3.zero;
    public Vector3 targetPosition = Vector3.zero;
    public RectTransform panelRectTransform;
    public Vector3 originalPosition = Vector3.zero;
    public Rect panelRect;
    public Vector3 lastTargetPosition = Vector3.zero;
    public Vector3 offset = Vector3.zero;
    public bool tweening = false;
    public float tweenTimer = 0.0f;
    public float tweenDelay = 0.4f;
    public float tweenSpeed = 1.0f;

    public void TweenTo(string target)
    {
        lastTargetPosition = targetPosition;
        if (target.ToLower() == "none")
        {
            targetPosition = noHoverPosition + offset;
        }
        if (target.ToLower() == "state")
        {
            targetPosition = stateHoverPosition + offset;
        }
        if (target.ToLower() == "county")
        {
            targetPosition = countyHoverPosition + offset;
        }
        tweening = true;
        tweenTimer = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(panelRect == null)
        {
            panelRectTransform = GetComponent<RectTransform>();
            panelRect = panelRectTransform.rect;
            originalPosition = panelRectTransform.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(tweening)
        {
            tweenTimer += Time.deltaTime;
            panelRectTransform.localPosition = Vector3.Lerp(panelRectTransform.localPosition, targetPosition, (tweenTimer / tweenDelay)); 
            if(tweenTimer > tweenDelay)
            {
                panelRectTransform.localPosition = targetPosition;
                tweening = false;
            }
        }        
    }
}
