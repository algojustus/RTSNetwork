using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask resourceMask;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform selectionArea;
    [SerializeField] private Canvas unitCanvas;
    private Vector3 leftClickPosition;
    private Vector3 rightClickPosition;
    private Vector3 mousePos;
    private Vector2 selectionStartPos;
    private List<UnitSelected> selectedUnits;
    private List<BuildingSelected> buildingUnits;
    private GameObject gameMananger;
    private GameObject clickedResource;
    private GameObject attackTarget;
    private ResourcesUI _resources;
    public Slider progressbar;
    private string currentUnitPrefabname;
    private float y_axis_offset = 3;
    private float x_axis_offset = 0;
    private float z_axis_offset = 0;
    private int unitCounter = 0;

    void Awake()
    {
        gameMananger = GameObject.Find("GameManager");
        selectedUnits = new List<UnitSelected>();
        buildingUnits = new List<BuildingSelected>();
        selectionArea.gameObject.SetActive(false);
        _resources = gameMananger.GetComponent<ResourcesUI>();
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
    }

    IEnumerator SummonCooldown(float cooldown)
    {
        progressbar.gameObject.SetActive(true);
        for (float alpha = 0; alpha <= cooldown; alpha += 0.1f)
        {
            progressbar.value += 0.1f / cooldown;
            yield return new WaitForSeconds(.1f);
        }

        progressbar.gameObject.SetActive(false);
        progressbar.value = 0;
    }

    public void InitVillager(BuildingSelected buildingSelected)
    {
        StartCoroutine(InstantiateVillager(buildingSelected));
    }

    public void InitSpear(BuildingSelected buildingSelected)
    {
        StartCoroutine(InstantiateSpear(buildingSelected));
    }

    public void InitSword(BuildingSelected buildingSelected)
    {
        StartCoroutine(InstantiateSword(buildingSelected));
    }

    public void InitBow(BuildingSelected buildingSelected)
    {
        StartCoroutine(InstantiateBow(buildingSelected));
    }

    public IEnumerator InstantiateVillager(BuildingSelected buildingSelected)
    {
        if (_resources.villager_max <= _resources.villager_count)
            yield break;
        StartCoroutine(SummonCooldown(5));
        yield return new WaitForSeconds(5);
        bool allowedToCreate = _resources.HasEnoughResources(50, 0, 0, 0);
        if (allowedToCreate)
        {
            int unit_id;
            Vector3 spawnPos = new Vector3(
                buildingSelected._selectedGameObject.transform.position.x,
                3f,
                buildingSelected._selectedGameObject.transform.position.z);
            currentUnitPrefabname = "villager" + Client.myGameColor;
            GameObject currentUnit =
                Instantiate(Resources.Load(currentUnitPrefabname), spawnPos, new Quaternion()) as GameObject;
            UnitSelected data = currentUnit.GetComponent<UnitSelected>();
            unit_id = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .unitcounter;
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .AddUnit(
                    unit_id,
                    currentUnitPrefabname,
                    spawnPos,
                    new Quaternion(),
                    data.unitData.unit_hp,
                    data.unitData.damage,
                    data.unitData.ranged_resistance,
                    data.unitData.melee_resistance);
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .UnitDictionary[unit_id].unit = currentUnit;
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .UnitCounter();
            _resources.BuildWithResources(currentUnitPrefabname);
        }

        _resources.villager_count++;
        _resources.villager_ui.text = _resources.villager_count + "|" + _resources.villager_max;
    }

    public IEnumerator InstantiateSpear(BuildingSelected buildingSelected)
    {
        if (_resources.villager_max <= _resources.villager_count)
            yield break;
        StartCoroutine(SummonCooldown(10));
        yield return new WaitForSeconds(10);
        bool allowedToCreate = _resources.HasEnoughResources(30, 25, 0, 0);
        if (allowedToCreate)
        {
            int unit_id;
            Vector3 spawnPos = new Vector3(
                buildingSelected._selectedGameObject.transform.position.x,
                3f,
                buildingSelected._selectedGameObject.transform.position.z);
            currentUnitPrefabname = "spear" + Client.myGameColor;
            GameObject currentUnit =
                Instantiate(Resources.Load(currentUnitPrefabname), spawnPos, new Quaternion()) as GameObject;
            UnitSelected data = currentUnit.GetComponent<UnitSelected>();
            unit_id = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .unitcounter;
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .UnitCounter();
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .AddUnit(
                    unit_id,
                    currentUnitPrefabname,
                    spawnPos,
                    new Quaternion(),
                    data.unitData.unit_hp,
                    data.unitData.damage,
                    data.unitData.ranged_resistance,
                    data.unitData.melee_resistance);
            _resources.BuildWithResources(currentUnitPrefabname);
        }

        _resources.villager_count++;
        _resources.villager_ui.text = _resources.villager_count + "|" + _resources.villager_max;
    }

    public IEnumerator InstantiateSword(BuildingSelected buildingSelected)
    {
        if (_resources.villager_max <= _resources.villager_count)
            yield break;
        StartCoroutine(SummonCooldown(20));
        yield return new WaitForSeconds(20);
        bool allowedToCreate = _resources.HasEnoughResources(60, 0, 35, 0);
        if (allowedToCreate)
        {
            int unit_id;
            Vector3 spawnPos = new Vector3(
                buildingSelected._selectedGameObject.transform.position.x,
                3f,
                buildingSelected._selectedGameObject.transform.position.z);
            currentUnitPrefabname = "sword" + Client.myGameColor;
            GameObject currentUnit =
                Instantiate(Resources.Load(currentUnitPrefabname), spawnPos, new Quaternion()) as GameObject;
            UnitSelected data = currentUnit.GetComponent<UnitSelected>();
            unit_id = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .unitcounter;
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .UnitCounter();
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .AddUnit(
                    unit_id,
                    currentUnitPrefabname,
                    spawnPos,
                    new Quaternion(),
                    data.unitData.unit_hp,
                    data.unitData.damage,
                    data.unitData.ranged_resistance,
                    data.unitData.melee_resistance);
            _resources.BuildWithResources(currentUnitPrefabname);
        }

        _resources.villager_count++;
        _resources.villager_ui.text = _resources.villager_count + "|" + _resources.villager_max;
    }

    public IEnumerator InstantiateBow(BuildingSelected buildingSelected)
    {
        if (_resources.villager_max <= _resources.villager_count)
            yield break;
        StartCoroutine(SummonCooldown(10));
        yield return new WaitForSeconds(10);
        bool allowedToCreate = _resources.HasEnoughResources(0, 35, 50, 0);
        if (allowedToCreate)
        {
            int unit_id;
            Vector3 spawnPos = new Vector3(
                buildingSelected._selectedGameObject.transform.position.x,
                3f,
                buildingSelected._selectedGameObject.transform.position.z);
            currentUnitPrefabname = "bow" + Client.myGameColor;
            GameObject currentUnit =
                Instantiate(Resources.Load(currentUnitPrefabname), spawnPos, new Quaternion()) as GameObject;
            UnitSelected data = currentUnit.GetComponent<UnitSelected>();
            unit_id = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .unitcounter;
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .AddUnit(
                    unit_id,
                    currentUnitPrefabname,
                    spawnPos,
                    new Quaternion(),
                    data.unitData.unit_hp,
                    data.unitData.damage,
                    data.unitData.ranged_resistance,
                    data.unitData.melee_resistance);
            _resources.BuildWithResources(currentUnitPrefabname);
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .UnitDictionary[unit_id].unit = currentUnit;
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .UnitCounter();
        }

        _resources.villager_count++;
        _resources.villager_ui.text = _resources.villager_count + "|" + _resources.villager_max;
    }

    private void InitMovement(Vector3 moveTo)
    {
        Vector3 shufflePosition = moveTo;
        foreach (var units in selectedUnits)
        {
            if (units.CompareTag("player1_villager"))
            {
                shufflePosition = ShufflePosition(moveTo);
                units.MoveToPosition(shufflePosition);
                if (clickedResource != null)
                {
                    units.GetComponent<GatheringUnit>().resourceNode =
                        clickedResource.GetComponent<ResourceClicked>().ResourceNode;
                    units.GetComponent<GatheringUnit>().movement = GatheringUnit.State.Idle;
                    clickedResource = null;
                    return;
                }

                units.GetComponent<GatheringUnit>().movement = GatheringUnit.State.Movement;
                units.GetComponent<GatheringUnit>().state = GatheringUnit.State.Idle;
            }

            if (units.CompareTag("Player1"))
            {
                shufflePosition = ShufflePosition(moveTo);
                units.MoveToPosition(shufflePosition);
                if (attackTarget != null)
                {
                    units.GetComponent<AttackUnit>().target = attackTarget;
                    units.GetComponent<AttackUnit>().movement = AttackUnit.Attack.Idle;
                    attackTarget = null;
                    return;
                }

                units.GetComponent<AttackUnit>().movement = AttackUnit.Attack.Movement;
                units.GetComponent<AttackUnit>().state = AttackUnit.Attack.Idle;
            }
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

        if (Physics.Raycast(ray, out RaycastHit raycastHit2, float.MaxValue, resourceMask))
        {
            clickedResource = raycastHit2.collider.gameObject;
        }

        if (Physics.Raycast(ray, out RaycastHit raycastHit3, float.MaxValue, enemyMask))
        {
            attackTarget = raycastHit3.collider.gameObject;
        }

        return mousePosition;
    }

    private void DrawSelectionBox()
    {
        //bool selectBuildings = true;
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
            if (collider.transform.CompareTag("Player1") || collider.transform.CompareTag("player1_villager"))
            {
                UnitSelected unitSelected = collider.collider.GetComponent<UnitSelected>();
                if (unitSelected != null)
                {
                    unitSelected.SetSelectedVisible(true);
                    selectedUnits.Add(unitSelected);
                    if (unitSelected.transform.CompareTag("player1_villager"))
                    {
                        gameMananger.GetComponent<BuildingController>().villagerClicked.gameObject.SetActive(true);
                    }
                }
            }

            if (collider.transform.CompareTag("Player1") && selectedUnits.Count == 1)
            {
                unitCanvas.transform.Find("Slider/Attack/Damage").GetComponent<Text>().text =
                    "" + selectedUnits[0].unitData.damage;
                unitCanvas.transform.Find("Slider/UnitIcon/Hitpoints").GetComponent<Text>().text =
                    Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                        .UnitDictionary[selectedUnits[0].GetComponent<RTSView>().unit_id].current_hp + "|" +
                    selectedUnits[0].unitData.unit_hp;
                unitCanvas.transform.Find("Slider/Armor/Shield").GetComponent<Text>().text =
                    selectedUnits[0].unitData.melee_resistance + "|" + selectedUnits[0].unitData.ranged_resistance;
                unitCanvas.transform.Find("Slider/UnitName").GetComponent<Text>().text =
                    "" + selectedUnits[0].unitData.prefabname;
                unitCanvas.gameObject.SetActive(true);
            }

            if (collider.transform.CompareTag("player1_kaserne"))
            {
                BuildingSelected building = collider.collider.GetComponent<BuildingSelected>();
                buildingUnits.Add(building);
                building.SetSelectedVisible(true);
                gameMananger.GetComponent<BuildingController>().SetUiOnBuilding(
                    collider.transform.GetComponent<BuildingSelected>().buildingData,
                    building);
                gameMananger.GetComponent<BuildingController>().buildings.gameObject.SetActive(true);
            }

            if (collider.transform.CompareTag("player1_towncenter"))
            {
                BuildingSelected building = collider.collider.GetComponent<BuildingSelected>();
                buildingUnits.Add(building);
                building.SetSelectedVisible(true);
                gameMananger.GetComponent<BuildingController>().SetUiOnBuilding(
                    collider.transform.GetComponent<BuildingSelected>().buildingData,
                    building);
                gameMananger.GetComponent<BuildingController>().buildings.gameObject.SetActive(true);
            }
        }

        if (selectedUnits.Count > 0 && buildingUnits.Count > 0)
        {
            foreach (var building in buildingUnits)
            {
                building.SetSelectedVisible(false);
            }

            gameMananger.GetComponent<BuildingController>().buildings.gameObject.SetActive(false);
            gameMananger.GetComponent<BuildingController>().ResetBuildingUI(
                buildingUnits[0].buildingData,
                buildingUnits[0]);
            buildingUnits.Clear();
        }
    }

    public void DeactivateAllUnitsBeforeNewSelect()
    {
        foreach (var units in selectedUnits)
        {
            if (units.transform.CompareTag("player1_villager"))
            {
                gameMananger.GetComponent<BuildingController>().villagerClicked.gameObject.SetActive(false);
            }

            if (units.transform.CompareTag("Player1"))
            {
                unitCanvas.gameObject.SetActive(false);
            }

            units.SetSelectedVisible(false);
        }

        foreach (var buildings in buildingUnits)
        {
            if (buildings.transform.CompareTag("player1_kaserne"))
            {
                gameMananger.GetComponent<BuildingController>().buildings.gameObject.SetActive(false);
                gameMananger.GetComponent<BuildingController>().ResetBuildingUI(buildings.buildingData, buildings);
            }

            if (buildings.transform.CompareTag("player1_towncenter"))
            {
                gameMananger.GetComponent<BuildingController>().buildings.gameObject.SetActive(false);
                gameMananger.GetComponent<BuildingController>().ResetBuildingUI(buildings.buildingData, buildings);
            }

            buildings.SetSelectedVisible(false);
        }

        buildingUnits.Clear();
        selectedUnits.Clear();
    }
}
