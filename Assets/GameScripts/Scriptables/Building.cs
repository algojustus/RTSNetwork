using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Building : ScriptableObject
{
    public GameObject building;
    public string prefabname;
    public int building_hp;
    public int damage;
    public int ranged_resistance;
    public int melee_resistance;
    public List<GameObject> spawnables;
    public List<string> technologies;
}
