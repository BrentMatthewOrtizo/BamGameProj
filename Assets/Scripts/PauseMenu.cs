using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; //assign this to optional panel to set it to visible later
    
    //use this for onClick event to make options panel toggle from not visible to visible
    //private Button pauseButton;


    void Start() {
        //check if this is right or if i need to put it in AWAKE
        //pauseButton = GameObject.FindWithTag("PauseButton").GetComponent<Button>(); 
        
        //pauseMenuUI = GameObject.FindWithTag("OptionsPanel");
        
        //this for sure goes here
        //pauseButton.onClick.AddListener(PauseMenuClicked);
    }
    public void PauseMenuClicked()
    {
        // set options menu to visible
        pauseMenuUI.SetActive(true);
    }
}
