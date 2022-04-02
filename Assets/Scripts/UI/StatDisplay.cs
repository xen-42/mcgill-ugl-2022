using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    [SerializeField] Text assignmentsWritten;
    [SerializeField] Text assignmentsScanned;
    [SerializeField] Text assignmentsSubmitted;
    [SerializeField] Text assignmentsStolen;
    [SerializeField] Text drinksDrank;
    [SerializeField] Text catsPet;
    [SerializeField] Text plantsWatered;
    [SerializeField] Text airConditionersFixed;

    public void SetStats(StatTracker.PlayerStats stats)
    {
        assignmentsWritten.text = $"Assignments written: {stats.AssignmentsWritten}";
        assignmentsScanned.text = $"Assignments scanned: {stats.AssignmentsScanned}";
        assignmentsSubmitted.text = $"Assignments submitted: {stats.AssignmentsSubmitted}";
        assignmentsStolen.text = $"Assignments stolen: {stats.AssignmentsStolen}";
        drinksDrank.text = $"Drinks had: {stats.DrinksDrank}";
        catsPet.text = $"Cat pets: {stats.CatsPet}";
        plantsWatered.text = $"Plants watered: {stats.PlantsWatered}";
        airConditionersFixed.text = $"Air conditioners fixed: {stats.AirConditionersFixed}";
    }
}
