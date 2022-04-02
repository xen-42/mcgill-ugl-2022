using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    [SerializeField] int passingGrade;
    [SerializeField] int highestGrade;

    [SerializeField] Image avatar;
    [SerializeField] Text usernameText;
    [SerializeField] Text gradeText;

    [SerializeField] Text assignmentsWritten;
    [SerializeField] Text assignmentsScanned;
    [SerializeField] Text assignmentsSubmitted;
    [SerializeField] Text assignmentsStolen;
    [SerializeField] Text drinksDrank;
    [SerializeField] Text catsPet;
    [SerializeField] Text plantsWatered;
    [SerializeField] Text airConditionersFixed;

    private string[] grades = new string[] { "F", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+"};

    public void SetStats(StatTracker.PlayerStats stats)
    {
        // TODO: write username and display avatar
        usernameText.text = stats.username;

        // Calculate letter grade
        var letterGrade = Mathf.Clamp01((stats.AssignmentsSubmitted - passingGrade) / (float)(highestGrade - passingGrade));
        int letterIndex = Mathf.FloorToInt(letterGrade * (grades.Length-1));

        Debug.Log($"Letter index: {letterGrade} -> {letterIndex} -> {grades[letterIndex]}");

        gradeText.text = $"Grade: {grades[letterIndex]}";

        // Write stats
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
