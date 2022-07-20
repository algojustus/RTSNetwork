using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelected : MonoBehaviour
{
    public Building buildingData;
    public GameObject _selectedGameObject;
    private Dictionary<int, Vector3> unitsEntered;
    
    void Awake()
    {
        _selectedGameObject = transform.Find("Selected").gameObject;
        SetSelectedVisible(false);
    }
    public void SetSelectedVisible(bool visible)
    {
        _selectedGameObject.SetActive(visible);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("player1_villager"))
        {
            foreach (var kvp in unitsEntered)
            {
                if (collision.transform.position == kvp.Value)
                {
                    //dobuild
                } else
                {
                    //doupdatepos until not moving
                }
            }
        }
        //aktuallisiere solange die pos, bis stillstand
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("player1_villager"))
        {
             unitsEntered.Add(collision.transform.GetComponent<UnitData>().id,collision.transform.position);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("player1_villager"))
        {
            unitsEntered.Remove(collision.transform.GetComponent<UnitData>().id);
        }
    }

    private void DoBuild()
    {
        //More Units faster build
        //load build time from buildingdata
        //add new progressbar
    }
}
