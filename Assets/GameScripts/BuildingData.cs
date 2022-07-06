using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData
{
    public GameObject building;
    public int id;
    public string prefabname;
    public Vector3 position;
    public Quaternion rotation;
    public int building_dmg;
    public int building_hp;
    public int ranged_resistance;
    public int melee_resistance;
    
    public BuildingData(int _id, string _prefabname, Vector3 _spawnposition, Quaternion _spawnrota, int hitpoints, int rangedResistance, int meleeResistance)
    {
        id = _id;
        prefabname = _prefabname;
        position = _spawnposition;
        rotation = _spawnrota;
        building_hp = hitpoints;
        ranged_resistance = rangedResistance;
        melee_resistance = meleeResistance;
    }
    public BuildingData(int _id, string _prefabname, Vector3 _spawnposition, Quaternion _spawnrota, GameObject _building)
    {
        id = _id;
        prefabname = _prefabname;
        position = _spawnposition;
        rotation = _spawnrota;
        building = _building;
        
        var scriptableObject = building.GetComponent<BuildingSelected>().buildingData;
        building_hp = scriptableObject.building_hp;
        building_dmg = scriptableObject.damage;
        ranged_resistance = scriptableObject.ranged_resistance;
        melee_resistance = scriptableObject.melee_resistance;
    }
    
    public BuildingData(int _id, string _prefabname, Vector3 _spawnposition, Quaternion _spawnrota)
    {
        id = _id;
        prefabname = _prefabname;
        position = _spawnposition;
        rotation = _spawnrota;
    }
    public void SpawnIngameBuilding()
    {
        building = ObjectSpawner.SpawnObject(prefabname, position, rotation);
        var scriptableObject = building.GetComponent<BuildingSelected>().buildingData;
        building_hp = scriptableObject.building_hp;
        building_dmg = scriptableObject.damage;
        ranged_resistance = scriptableObject.ranged_resistance;
        melee_resistance = scriptableObject.melee_resistance;
    }
}
