using Steamworks;
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
    [SerializeField] Text socksPickedUp;
    public int letterGradeIndex;

    private string[] grades = new string[] { "F", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+"};

    protected Callback<AvatarImageLoaded_t> _avatarImageLoaded;

    private ulong _steamID;

    // Sounds
    [SerializeField] public AudioSource goodGradeSound;
    [SerializeField] public AudioSource okGradeSound;
    [SerializeField] public AudioSource badGradeSound;

    public void Awake()
    {
        _avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    public void SetStats(StatTracker.PlayerStats stats, string username, ulong steamID)
    {
        _steamID = steamID;

        // TODO: write username and display avatar
        usernameText.text = stats.username;

        // Calculate letter grade
        var letterGrade = Mathf.Clamp01((stats.AssignmentsSubmitted - passingGrade) / (float)(highestGrade - passingGrade));
        int letterIndex = Mathf.FloorToInt(letterGrade * (grades.Length-1));
        letterGradeIndex = letterIndex;

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
        socksPickedUp.text = $"Socks picked up: {stats.SocksPickedUp}";

        usernameText.text = username;

        UpdateAvatar();

        if (goodGradeSound != null && okGradeSound != null && badGradeSound != null){
            PlayEndSound(letterIndex);
        }
    }

    private void UpdateAvatar()
    {
        avatar.sprite = Utils.LoadAvatar(SteamFriends.GetLargeFriendAvatar(new CSteamID(_steamID)));
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        UpdateAvatar();
    }

    private void PlayEndSound(int letterIndex){
        // Good grade
        if (letterIndex >= 7){
            goodGradeSound.Play();
        }
        // Ok grade
        else if (letterIndex >= 1 && letterIndex < 7){
            okGradeSound.Play();
        }
        // Bad grade
        else{
            badGradeSound.Play();
        }
    }
}
