using UnityEngine;
using UnityEngine.SceneManagement;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class AudioManager : MonoBehaviour
{
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    
    public static AudioManager Instance;

    [Header("Music Tracks")]
    public AudioClip startScreenMusic;
    public AudioClip desertMusic;
    public AudioClip forestMusic;

    [Header("Sound Effects")]
    public AudioClip jumpSFX;
    public AudioClip itemPickupSFX;
    public AudioClip obeliskInteractSFX;
    public AudioClip startScreenButton;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private string currentBiome = "Desert"; // Default biome when Game scene starts

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        Log.Info($"Scene changed to {newScene.name}");
        PlayMusicForScene(newScene.name);

        // ✅ Restore biome after returning from battle
        if (newScene.name.Contains("Game") && !string.IsNullOrEmpty(WarpManager.previousBiome))
        {
            SwitchBiome(WarpManager.previousBiome);
            RestoreConfiner(WarpManager.previousBiome);
        }
    }

    private void RestoreConfiner(string biome)
    {
        var confiner = FindFirstObjectByType<Unity.Cinemachine.CinemachineConfiner2D>();
        if (confiner == null) return;

        // Tag your PolygonColliders appropriately
        if (biome == "Forest")
        {
            var forestCollider = GameObject.FindWithTag("ForestConfiner");
            if (forestCollider != null)
                confiner.BoundingShape2D = forestCollider.GetComponent<PolygonCollider2D>();
        }
        else if (biome == "Desert")
        {
            var desertCollider = GameObject.FindWithTag("DesertConfiner");
            if (desertCollider != null)
                confiner.BoundingShape2D = desertCollider.GetComponent<PolygonCollider2D>();
        }

        confiner.InvalidateBoundingShapeCache();
    }


    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        if (sceneName.Contains("Start"))
        {
            clipToPlay = startScreenMusic;
            currentBiome = "None";
        }
        else if (sceneName.Contains("Game"))
        {
            // Default to Desert music when entering the Game scene
            clipToPlay = desertMusic;
            currentBiome = "Desert";
        }
        else if (sceneName.Contains("AutobattlePrototype"))
        {
            clipToPlay = null; // No music in battle scene
            currentBiome = "None";
        }

        if (clipToPlay != null)
        {
            if (musicSource.clip != clipToPlay)
            {
                musicSource.clip = clipToPlay;
                musicSource.Play();
            }
        }
        else
        {
            musicSource.Stop();
        }
    }
    
    public string GetCurrentBiome()
    {
        return currentBiome;
    }

    
    public void SwitchBiome(string newBiome)
    {
        if (SceneManager.GetActiveScene().name != "Game") return;

        if (newBiome == currentBiome) return; // No need to restart same track

        AudioClip clipToPlay = null;
        if (newBiome == "Desert")
        {
            clipToPlay = desertMusic;
        }
        else if (newBiome == "Forest")
        {
            clipToPlay = forestMusic;
        }

        if (clipToPlay != null)
        {
            musicSource.clip = clipToPlay;
            musicSource.Play();
            currentBiome = newBiome;
        }
    }
    
    public void PlayJumpSFX()
    {
        sfxSource.PlayOneShot(jumpSFX);
    }

    public void PlayItemPickupSFX()
    {
        sfxSource.PlayOneShot(itemPickupSFX);
    }

    public void PlayObeliskInteractSFX()
    {
        sfxSource.PlayOneShot(obeliskInteractSFX);
    }
    
    public void PlayStartScreenButton()
    {
        sfxSource.PlayOneShot(startScreenButton);
    }
}