using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //Opening Pause Menu
    private GameObject pauseMenuUI;
    private Button pauseButton;
    private bool isPaused;
    
    //Closing Pause Menu
    private Button exitButton;
    

    void Start() {
        pauseMenuUI = GameObject.FindWithTag("PauseMenu");
        
        pauseButton = GameObject.FindWithTag("PauseButton").GetComponent<Button>();
        exitButton = GameObject.FindWithTag("ExitButton").GetComponent<Button>();

        isPaused = true;
        pauseMenuUI.SetActive(!isPaused);
        
        pauseButton.onClick.AddListener(PauseButtonClicked);
        exitButton.onClick.AddListener(ExitButtonClicked);
    }
    void PauseButtonClicked()
    {
        // set Pause Menu to visible
        if (isPaused)
        {
            pauseMenuUI.SetActive(true);
        }
    }
    
    void ExitButtonClicked()
    {
        // disable Pause Menu
        if (isPaused)
        {
            isPaused = false;
            pauseMenuUI.SetActive(isPaused);
        }
        isPaused = true; //set up for next pause button conditions
    }
    
    void OnDestroy()
    {
        // when does OnDestroy get called?
        //pauseButton.onClick.RemoveListener(PauseButtonClicked);
    }
}
