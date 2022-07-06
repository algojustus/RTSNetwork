using UnityEngine;

[CreateAssetMenu]
public class Unit : ScriptableObject
{
    public GameObject unit;
    public string prefabname;
    public int movementSpeed = 30;
    public int unit_hp;
    public int level;
    public int damage;
    public int ranged_resistance;
    public int melee_resistance;
}
