using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fadeImage; // Black overlay image
    [SerializeField] private Text fadeText; // Text to display
    
    [Header("Settings")]
    [SerializeField] private bool createUIAutomatically = true;
    
    [Header("Audio Settings")]
    [SerializeField] private bool stopAudioOnFade = true;
    [SerializeField] private float audioStopDelay = 1f; // Delay before stopping audio (in seconds)
    [SerializeField] private bool resumeAudioAfterFade = false;

    private Canvas fadeCanvas;
    private CanvasGroup canvasGroup;
    private AudioSource[] pausedAudioSources;

    private void Awake()
    {
        if (createUIAutomatically && (fadeImage == null || fadeText == null))
        {
            CreateFadeUI();
        }

        // Make sure the fade starts invisible
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }

        if (fadeText != null)
        {
            fadeText.text = "";
        }
    }

    private void CreateFadeUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform);
        fadeCanvas = canvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999; // Make sure it's on top

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Create Black Image (Fade overlay)
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);
        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0); // Start transparent
        
        RectTransform imageRect = fadeImage.GetComponent<RectTransform>();
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.sizeDelta = Vector2.zero;
        imageRect.anchoredPosition = Vector2.zero;

        // Create Text
        GameObject textObj = new GameObject("FadeText");
        textObj.transform.SetParent(canvasObj.transform);
        
        fadeText = textObj.AddComponent<Text>();
        fadeText.text = "";
        fadeText.alignment = TextAnchor.MiddleCenter;
        fadeText.color = Color.white;
        fadeText.fontSize = 48;
        fadeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform textRect = fadeText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        Debug.Log("Fade UI created automatically.");
    }

    public IEnumerator FadeToBlack(float duration)
    {
        // Start audio stop delay coroutine if enabled
        if (stopAudioOnFade && audioStopDelay > 0f)
        {
            StartCoroutine(StopAudioAfterDelay(audioStopDelay));
        }
        else if (stopAudioOnFade)
        {
            StopAllAudio();
        }

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    // Coroutine to stop audio after a delay
    private IEnumerator StopAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopAllAudio();
    }

    public IEnumerator FadeToClear(float duration)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;

        // Resume audio if enabled
        if (resumeAudioAfterFade)
        {
            ResumeAllAudio();
        }
    }

    public void ShowText(string text)
    {
        if (fadeText != null)
        {
            fadeText.text = text;
        }
    }

    public void HideText()
    {
        if (fadeText != null)
        {
            fadeText.text = "";
        }
    }

    // Optional: Instant fade methods
    public void SetBlackInstant()
    {
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;
        
        if (stopAudioOnFade && audioStopDelay > 0f)
        {
            StartCoroutine(StopAudioAfterDelay(audioStopDelay));
        }
        else if (stopAudioOnFade)
        {
            StopAllAudio();
        }
    }

    public void SetClearInstant()
    {
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;
        
        if (resumeAudioAfterFade)
        {
            ResumeAllAudio();
        }
    }

    // Audio control methods
    private void StopAllAudio()
    {
        // Find all active audio sources in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        
        // Store references to audio sources that were playing
        System.Collections.Generic.List<AudioSource> playingAudioSources = new System.Collections.Generic.List<AudioSource>();
        
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource.isPlaying)
            {
                playingAudioSources.Add(audioSource);
                audioSource.Stop();
            }
        }
        
        pausedAudioSources = playingAudioSources.ToArray();
        
        // Also stop the AudioListener (global volume control)
        AudioListener.pause = false; // Make sure it's not paused
        AudioListener.volume = 0f; // Set volume to 0
        
        Debug.Log($"Stopped {pausedAudioSources.Length} audio sources.");
    }

    private void ResumeAllAudio()
    {
        // Restore AudioListener volume
        AudioListener.volume = 1f;
        
        // Resume previously playing audio sources
        if (pausedAudioSources != null)
        {
            foreach (AudioSource audioSource in pausedAudioSources)
            {
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
            
            Debug.Log($"Resumed {pausedAudioSources.Length} audio sources.");
        }
    }

    // Public methods for manual audio control
    public void ManualStopAllAudio()
    {
        StopAllAudio();
    }

    public void ManualResumeAllAudio()
    {
        ResumeAllAudio();
    }
}

