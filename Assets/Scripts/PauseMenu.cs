using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //Opening Pause Menu
    private GameObject pauseMenuUI;
    private Button pauseButton;
    private bool isPaused;
    
    //Closing Pause Menu
    private Button resumeButton;
    private Button exitButton;
    

    void Start() {
        pauseMenuUI = GameObject.FindWithTag("PauseMenu");
        
        pauseButton = GameObject.FindWithTag("PauseButton").GetComponent<Button>();
        resumeButton = GameObject.FindWithTag("ResumeButton").GetComponent<Button>();
        exitButton =  GameObject.FindWithTag("ExitButton").GetComponent<Button>();

        isPaused = false; 
        pauseMenuUI.SetActive(isPaused); 
        
        pauseButton.onClick.AddListener(PauseButtonClicked);
        resumeButton.onClick.AddListener(ResumeButtonClicked);
        exitButton.onClick.AddListener(ExitButtonClicked);
    }
    void PauseButtonClicked()
    {
        // set Pause Menu to visible
        if (!isPaused) 
        {
            pauseMenuUI.SetActive(true);
            isPaused = true; 
        }
    }
    
    void ResumeButtonClicked()
    {
        // disable Pause Menu
        if (isPaused)
        {
            pauseMenuUI.SetActive(false);
            isPaused = false;
        }
    }
    
    void ExitButtonClicked()
    {
        if (isPaused)
        {
            SceneManager.LoadSceneAsync(0);
            pauseMenuUI.SetActive(false);
            isPaused = false;
        }
    }
    
}
