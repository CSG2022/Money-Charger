using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private float distance;
    public Vector3 _startingPosition;
    [SerializeField] public SwitchButton _clickableButton;

    // for mobile
    private Vector3 screenPoint;
    private Vector3 offset;
    private float zCoord;

    private void Start()
    {
        _startingPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    isDragging = true;
                    distance = hit.distance;
                    _clickableButton.IsDraggingToMergeOrMove = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            if(_clickableButton.ReadyToMerge)
            {
                _clickableButton.IsDraggingToMergeOrMove = false;
            }
            else
            {
                _clickableButton.IsDraggingToMergeOrMove = false;
                transform.position = _startingPosition;
            }
        }

        if (isDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Grabbing the object by its z axis
/*
            if (GameManager.draggingNewSwitch)
            {
                transform.position = new Vector3(ray.GetPoint(distance).x, transform.position.y, transform.position.z);
            }
            else*/
                transform.position = new Vector3(transform.position.x, transform.position.y, ray.GetPoint(distance).z);
        }
    }


    // For mobile

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        zCoord = gameObject.transform.position.z;
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
        transform.position = new Vector3(transform.position.x, transform.position.y, zCoord);
    }
}
