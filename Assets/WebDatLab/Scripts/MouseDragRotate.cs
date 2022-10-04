using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDragRotate : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    public float dragSpeed = 1.0f;
    public Transform dragThis;
    public float reverseY = 1;
    public bool noY = false;
    public float keySpeedMod = 0.1f;
    public Manager manager;

    void OnMouseDrag()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Drag(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
        }
    }

    void OnMouseEnter()
    {
        manager.countyNameTMPro.text = "";
    }

    public void Drag(Vector2 dragVec)
    {
        dragThis.Rotate(Vector3.down, dragVec.x * rotationSpeed);
        dragThis.Rotate(Vector3.right, dragVec.y * rotationSpeed);
        if (noY)
        {
            dragThis.Translate(new Vector3(dragVec.x * dragSpeed, 0.0f, dragVec.y * dragSpeed * reverseY));
        }
        else
        {
            dragThis.Translate(new Vector3(dragVec.x * dragSpeed, dragVec.y * dragSpeed * reverseY, 0.0f));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        manager = Manager.GetCurrentManager();
        if (dragThis == null)
        {
            dragThis = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Drag(new Vector2(0.0f, -dragSpeed * Time.deltaTime) * keySpeedMod);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Drag(new Vector2(dragSpeed * Time.deltaTime, 0.0f) * keySpeedMod);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Drag(new Vector2(0.0f, dragSpeed * Time.deltaTime) * keySpeedMod);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Drag(new Vector2(-dragSpeed * Time.deltaTime, 0.0f) * keySpeedMod);
        }
    }
}
