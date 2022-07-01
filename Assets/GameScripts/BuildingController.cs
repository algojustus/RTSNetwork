using UnityEngine;

public class BuildingController : MonoBehaviour
{
    private UnitController _unitController;
    private Vector3 mousePos;
    private GameObject currentBuilding;
    private bool buildingMode;
    private void Start()
    {
         _unitController = transform.GetComponent<UnitController>();
    }

    private void Update()
    {
        if (currentBuilding != null)
        {
            mousePos = _unitController.CheckWhereMouseClicked();
            currentBuilding.transform.position = mousePos;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            InstantiateBuildingMode();
        }

        if (buildingMode && Input.GetMouseButtonDown(0))
        {
            Instantiate(Resources.Load("House"),mousePos,new Quaternion());
            Destroy(currentBuilding);
            buildingMode = false;
        }
        if (buildingMode && Input.GetMouseButtonDown(1))
        {
            Destroy(currentBuilding);
            buildingMode = false;
        }
    }

    private void InstantiateBuildingMode()
    {
        currentBuilding = Instantiate(Resources.Load("House"),mousePos,new Quaternion()) as GameObject;
        var materialColor = currentBuilding.GetComponent<Renderer>().material.color;
        materialColor.a = 0.1f;
        buildingMode = true;
    }
}
