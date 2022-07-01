using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private Vector3 leftClickPosition;
    private Vector3 rightClickPosition;
    private List<UnitSelected> selectedUnits;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform selectionArea;
    private Vector2 selectionStartPos;
    private float y_axis_offset = 3;
    private float x_axis_offset = 0;
    private float z_axis_offset = 0;
    private int unitCounter = 0;
    private Vector3 mousePos;
    private GameObject currentUnit;
    private bool unitSpawnMode;

    void Awake()
    {
        selectedUnits = new List<UnitSelected>();
        selectionArea.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            leftClickPosition = CheckWhereMouseClicked();
            selectionStartPos = Input.mousePosition;
            selectionArea.gameObject.SetActive(true);
        }

        if (Input.GetMouseButton(0))
        {
            ScaleSelectionBox(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            DeactivateAllUnitsBeforeNewSelect();
            DrawSelectionBox();
            selectionArea.gameObject.SetActive(false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            rightClickPosition = CheckWhereMouseClicked();
            InitMovement(rightClickPosition);
        }
        
        if (currentUnit != null)
        {
            mousePos = CheckWhereMouseClicked();
            mousePos.y += 3f;
            currentUnit.transform.position = mousePos;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InstantiateUnitSpawningMode();
        }

        if (unitSpawnMode && Input.GetMouseButtonDown(0))
        {
            mousePos.y += 3f;
            Instantiate(Resources.Load("Unit"),mousePos,new Quaternion());
            Destroy(currentUnit);
            unitSpawnMode = false;
            
        }
        if (unitSpawnMode && Input.GetMouseButtonDown(1))
        {
            Destroy(currentUnit);
            unitSpawnMode = false;
        }
    }

    private void InstantiateUnitSpawningMode()
    {
        mousePos = CheckWhereMouseClicked();
        mousePos.y += 3f;
        currentUnit = Instantiate(Resources.Load("Unit_prefab"),mousePos,new Quaternion()) as GameObject;
        var materialColor = currentUnit.GetComponent<Renderer>().material.color;
        materialColor.a = 0.1f;
        unitSpawnMode = true;
    }
    
    private void InitMovement(Vector3 moveTo)
    {
        Vector3 shufflePosition = moveTo;
        foreach (var units in selectedUnits)
        {
            shufflePosition = ShufflePosition(moveTo);
            units.MoveToPosition(shufflePosition);
        }

        ResetShufflePosition();
    }

    private Vector3 ShufflePosition(Vector3 oldMoveTo)
    {
        unitCounter += 1;
        oldMoveTo.y += y_axis_offset;

        if (unitCounter % 2 == 0 && unitCounter > 0)
        {
            oldMoveTo.x += x_axis_offset;
        }

        if (unitCounter % 2 == 1 && unitCounter > 0)
        {
            oldMoveTo.x += -x_axis_offset;
            x_axis_offset += 5;
        }

        oldMoveTo.z += z_axis_offset;

        if (unitCounter % 7 == 0 && unitCounter > 0)
        {
            z_axis_offset += 5;
            x_axis_offset = 0;
        }

        return oldMoveTo;
    }

    private void ResetShufflePosition()
    {
        unitCounter = 0;
        x_axis_offset = 0;
        z_axis_offset = 0;
    }

    private void ScaleSelectionBox(Vector2 mousePos)
    {
        if (!selectionArea.gameObject.activeInHierarchy)
            selectionArea.gameObject.SetActive(true);

        float width = mousePos.x - selectionStartPos.x;
        float height = mousePos.y - selectionStartPos.y;
        selectionArea.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionArea.anchoredPosition = selectionStartPos + new Vector2(width / 2, height / 2);
    }

    public Vector3 CheckWhereMouseClicked()
    {
        Vector3 mousePosition = new Vector3();
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            mousePosition = raycastHit.point;
        }

        return mousePosition;
    }

    private void DrawSelectionBox()
    {
        Vector3 mouseReleased = CheckWhereMouseClicked();
        Vector3 scale = leftClickPosition - mouseReleased;
        scale.x = Mathf.Abs(scale.x);
        scale.y = Mathf.Abs(scale.y);
        scale.z = Mathf.Abs(scale.z);
        scale = scale * 0.5f;
        Vector3 center = (leftClickPosition + mouseReleased) * 0.5f;
        RaycastHit[] check = Physics.BoxCastAll(center, scale, Vector3.up);

        foreach (var collider in check)
        {
            UnitSelected unitSelected = collider.collider.GetComponent<UnitSelected>();
            if (unitSelected != null)
            {
                unitSelected.SetSelectedVisible(true);
                selectedUnits.Add(unitSelected);
            }
        }
    }

    private void DeactivateAllUnitsBeforeNewSelect()
    {
        foreach (var units in selectedUnits)
        {
            units.SetSelectedVisible(false);
        }

        selectedUnits.Clear();
    }
}