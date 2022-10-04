using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapState : MonoBehaviour
{
    public Manager manager;
    public Vector3 originalScale = Vector3.one;
    public float hoverHeightMod = 1.5f;
    public float cameraDistanceCheckTimer = 0.0f;
    public float cameraDistanceCheckDelay = 0.4f;
    public float cameraDistanceCheckRandom = 0.15f;
    public float cameraDistanceThreshold = 30.0f;
    public Material originalMAT;
    public bool disabled = false;
    

    void OnMouseEnter()
    {
        if(disabled) { return; }
        //manager.mapTooltip.position = manager.GetMeshCenterPosition(GetComponent<MeshFilter>().mesh) + manager.mapTooltipOffset;
        //manager.mapTooltip.GetComponentInChildren<TMPro.TextMeshPro>().text = name;
        transform.localScale = new Vector3(originalScale.x, originalScale.y * hoverHeightMod, originalScale.z);
        manager.stateHoverInfo.gameObject.SetActive(true);
        manager.SetHoverFlag(name);
        manager.countyNameTMPro.text = "";
    }

    void OnMouseExit()
    {
        if (disabled) { return; }
        transform.localScale = originalScale;
        manager.stateHoverInfo.gameObject.SetActive(false);
    }

    void OnMouseUpAsButton()
    {
        if (disabled) { return; }
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            manager.AddStateToCompareList(name.ToLower());
        }
        //UnityEngine.Debug.Log("Clicked:"+name);
    }

    /* // DETECT DOUBLE CLICKS
     public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            Debug.Log("double click");
        }
    }*/

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "MapSwitcher")
        {
            GetComponent<Renderer>().material = manager.stateWireFrameMAT;
            disabled = true;
            transform.localScale = originalScale;
            //GetComponent<Renderer>().enabled = false;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.name == "MapSwitcher")
        {
            GetComponent<Renderer>().material = originalMAT;
            disabled = false;
            //GetComponent<Renderer>().enabled = true;
        }
    }

    void Start()
    {
        manager = Manager.GetCurrentManager();
        originalScale = transform.localScale;
        GetComponent<Renderer>().material.SetColor("_Color", (GetComponent<Renderer>().material.GetColor("_Color") *0.75f) + (Random.ColorHSV() * 0.25f));
        originalMAT = GetComponent<Renderer>().material;
    }

    void Update()
    {
        /*cameraDistanceCheckTimer += Time.deltaTime;
        if(cameraDistanceCheckTimer > cameraDistanceCheckDelay)
        {
            float dist = Vector3.Distance(transform.position, manager.mapSwitcher.position);
            UnityEngine.Debug.Log("STATE DIST: " + dist.ToString());
            if (dist > cameraDistanceThreshold)
            {
                GetComponent<Renderer>().enabled = true;
            } 
            else
            {
                GetComponent<Renderer>().enabled = false;
            }
            cameraDistanceCheckTimer = 0.0f - Random.Range(0.0f, cameraDistanceCheckRandom);
        }*/
    }
}
