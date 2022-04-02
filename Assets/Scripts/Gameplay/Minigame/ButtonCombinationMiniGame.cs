using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class ButtonCombinationMiniGame : MonoBehaviour
{
    //Getting reference to all our buttons (and imported unity engine ui)
    public List<Button> Buttons;
    public List<Button> shuffleButtons;
    int count = 0; //tracking the button presses
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        RestartGame(); //Starts the minigame
    }

    public void RestartGame(){
        count = 0;
        shuffleButtons=Buttons.OrderBy(a => Random.RandomRange(0,100)).ToList(); //Randomly orders the buttons and saves to shuffled buttons
        //Run the loop for the number of buttons in the minigame so in our case 10 loops
        for (int i=1; i<11;i++){
            shuffleButtons[i-1].GetComponentInChildren<Text>().text = i.ToString(); //set text on buttons to their corresponding number
            shuffleButtons[i-1].interactable = true; //just making sure you can press the button
            shuffleButtons[i-1].image.color = new Color32(170, 220, 230, 250); //initial colours of the buttons
        }

    }

    public void pressButton(Button button){
        if((int.Parse(button.GetComponentInChildren<Text>().text)-1) == count){ //Checking if the button is already pressed
        count ++;
        button.interactable = false; //Now you can't click the button
        button.image.color = Color.green; //Make the button green
        if (count == 10){
            StartCoroutine(PresentResult(true));
        } else{
            StartCoroutine(PresentResult(false));
        }
        }
    }

    public IEnumerator PresentResult(bool win){
        if (!win){
            foreach(var button in shuffleButtons){
                button.image.color = Color.red;
                button.interactable = false;
            }
        }
        yield return new WaitForSeconds(2f);
        RestartGame();
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("You won!");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
