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

        isPaused = true;
        pauseMenuUI.SetActive(!isPaused);
        
        pauseButton.onClick.AddListener(PauseButtonClicked);
        resumeButton.onClick.AddListener(ResumeButtonClicked);
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
    
    void ResumeButtonClicked()
    {
        // disable Pause Menu
        if (isPaused)
        {
            isPaused = false;
            pauseMenuUI.SetActive(isPaused);
        }
        isPaused = true; //set up for next pause button conditions
    }
    
    void ExitButtonClicked()
    {
        SceneManager.LoadSceneAsync(0);
    }
    void OnDestroy()
    {
        pauseButton.onClick.RemoveListener(PauseButtonClicked);
        resumeButton.onClick.RemoveListener(ResumeButtonClicked);
        exitButton.onClick.RemoveListener(ExitButtonClicked);
    }
}
