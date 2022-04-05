using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinCondition : MonoBehaviour
{
    [SerializeField] public GameObject leftPlayer;
    [SerializeField] public Image leftImage;
    [SerializeField] public GameObject rightPlayer;
    [SerializeField] public Image rightImage;
    private int leftGrade;
    private int rightGrade;
    void Start()
    {
        DetermineWinner();
    }

    void Update(){
        if (leftGrade == 0 && rightGrade == 0){
            DetermineWinner();
        }
    }

    void DetermineWinner(){
        leftGrade = leftPlayer.GetComponent<StatDisplay>().letterGradeIndex;
        rightGrade = rightPlayer.GetComponent<StatDisplay>().letterGradeIndex;
        Debug.Log("left grade: " + leftGrade);
        Debug.Log("right grade: " + rightGrade);

        // Left wins
        if (leftGrade > rightGrade) {
            leftImage.GetComponent<Image>().enabled = true;
            rightImage.GetComponent<Image>().enabled = false;
        }
        // Right wins
        else if (rightGrade > leftGrade){
            leftImage.GetComponent<Image>().enabled = false;
            rightImage.GetComponent<Image>().enabled = true;
        }
    }

}
