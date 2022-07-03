using UnityEngine;

public class UnitData
{
    public GameObject unit;
    public int id;
    public string prefabname;
    public Vector3 position;
    public Quaternion rotation;
    public int unit_hp;
    public int level;
    public int damage;
    public int ranged_resistance;
    public int melee_resistance;

    public UnitData(int _id, string _prefabname, Vector3 _spawnposition, Quaternion _spawnrota, int hitpoints,
        int _damage, int rangedResistance, int meleeResistance)
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
    
    public void SpawnIngameUnit()
    {
        unit = ObjectSpawner.SpawnObject(prefabname, position, rotation);
    }
    
    public void SetIngameUnit(string prefab, Vector3 _position, Quaternion _rotation)
    {
        prefabname = prefab + "_prefab";
        position = _position;
        rotation = _rotation;
        SpawnIngameUnit();
    }

    public GameObject ReturnIngameUnit()
    {
        return unit;
    }
}
