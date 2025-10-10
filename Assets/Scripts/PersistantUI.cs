using UnityEngine;
using UnityEngine.SceneManagement;
public class PersistantUI : MonoBehaviour
{
    private static PersistantUI instance;
    [SerializeField] private FillStatusBar healthBar;
    GameObject character;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep this Canvas across scenes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates if you revisit a scene
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to find the Player in the new scene
        character = GameObject.FindGameObjectWithTag("Player");
        if (character != null)
        {
            healthBar.SetCharacter(character);
        }
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
}


