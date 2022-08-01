using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Building : ScriptableObject
{
    public GameObject building;
    public int buildingtime;
    public string prefabname;
    public int building_hp;
    public int damage;
    public int ranged_resistance;
    public int melee_resistance;
    public List<string> spawnables;
    public List<Sprite> sprites;
    public List<string> technologies;
}
