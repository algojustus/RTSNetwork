using UnityEngine;

public class UnitData
{
    public GameObject unit;
    public int id;
    public string prefabname;
    public Vector3 position;
    public Vector3 currentlyMovingToPos;
    public Quaternion rotation;
    public int movementSpeed;
    public int unit_hp;
    public int level;
    public int damage;
    public int ranged_resistance;
    public int melee_resistance;

    public UnitData(
        int _id,
        string _prefabname,
        Vector3 _spawnposition,
        Quaternion _spawnrota,
        int hitpoints,
        int _damage,
        int rangedResistance,
        int meleeResistance)
    {
        id = _id;
        prefabname = _prefabname;
        position = _spawnposition;
        rotation = _spawnrota;
        unit_hp = hitpoints;
        level = 1;
        damage = _damage;
        ranged_resistance = rangedResistance;
        melee_resistance = meleeResistance;
    }

    public UnitData(int unitID, string _prefabName, Vector3 spawnposition, Quaternion spawnrota)
    {
        id = unitID;
        prefabname = _prefabName;
        position = spawnposition;
        rotation = spawnrota;
    }

    public void SpawnIngameUnit()
    {
        string teamcolor = GetEnemyTeamColor();
        unit = ObjectSpawner.SpawnObject(prefabname, position, rotation);
        var scriptableObject = unit.GetComponent<UnitSelected>().unitData;
        unit_hp = scriptableObject.unit_hp;
        level = 1;
        damage = scriptableObject.damage;
        ranged_resistance = scriptableObject.ranged_resistance;
        melee_resistance = scriptableObject.melee_resistance;
    }

    public void MoveTo()
    {
        unit.GetComponent<UnitSelected>().MoveToPosition(currentlyMovingToPos);
    }

    private string GetEnemyTeamColor()
    {
        string teamcolor="";
        if (Client.serverlist.ServerlistDictionary[Client.myCurrentServer].player1_id == Client.clientID)
            teamcolor = "_rot";
        if (Client.serverlist.ServerlistDictionary[Client.myCurrentServer].player2_id == Client.clientID)
            teamcolor = "_blau";
        return teamcolor;
    }

    public void SetIngameUnit(string prefab, Vector3 _position, Quaternion _rotation)
    {
        prefabname = prefab + "_enemy";
        position = _position;
        rotation = _rotation;
        SpawnIngameUnit();
    }

    public GameObject ReturnIngameUnit()
    {
        return unit;
    }
}
