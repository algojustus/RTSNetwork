using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelected : MonoBehaviour
{
    public Building buildingData;
    public GameObject _selectedGameObject;
    
    
    void Awake()
    {
        _selectedGameObject = transform.Find("Selected").gameObject;
        SetSelectedVisible(false);
    }
    public void SetSelectedVisible(bool visible)
    {
        _selectedGameObject.SetActive(visible);
    }
}
