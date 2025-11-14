using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscene Type")]
    [SerializeField] private CutsceneType cutsceneType = CutsceneType.Timeline;

    [Header("Timeline Cutscene Settings")]
    [SerializeField] private PlayableDirector timelineDirector;

    [Header("Black Screen Settings")]
    [SerializeField] private FadeScreen fadeScreen;
    [SerializeField] private string[] textLines = new string[] { "To be continued...", "The path continues..." };
    [SerializeField] private float textDisplayDuration = 3f;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Player Control")]
    [SerializeField] private bool disablePlayerControl = true;
    [SerializeField] private GameObject playerController; // Assign your player object
    [SerializeField] private bool resumeGameplayAfter = true;
    [SerializeField] private float resumeDelay = 2f;

    private bool isPlaying = false;

    public enum CutsceneType
    {
        Timeline,
        BlackScreenWithText,
        Both // Timeline first, then black screen with text
    }

    public void StartCutscene()
    {
        if (isPlaying)
        {
            Debug.Log("Cutscene is already playing.");
            return;
        }

        isPlaying = true;
        StartCoroutine(PlayCutsceneSequence());
    }

    private IEnumerator PlayCutsceneSequence()
    {
        // Disable player control if needed
        if (disablePlayerControl)
        {
            DisablePlayerControl();
        }

        // Play the appropriate cutscene type
        switch (cutsceneType)
        {
            case CutsceneType.Timeline:
                yield return StartCoroutine(PlayTimelineCutscene());
                break;

            case CutsceneType.BlackScreenWithText:
                yield return StartCoroutine(PlayBlackScreenCutscene());
                break;

            case CutsceneType.Both:
                yield return StartCoroutine(PlayTimelineCutscene());
                yield return StartCoroutine(PlayBlackScreenCutscene());
                break;
        }

        // Wait before resuming
        if (resumeGameplayAfter)
        {
            yield return new WaitForSeconds(resumeDelay);
            ResumeGameplay();
        }

        isPlaying = false;
    }

    private IEnumerator PlayTimelineCutscene()
    {
        if (timelineDirector == null)
        {
            Debug.LogError("Timeline Director is not assigned!");
            yield break;
        }

        Debug.Log("Playing Timeline cutscene...");
        timelineDirector.Play();

        // Wait for timeline to finish
        while (timelineDirector.state == PlayState.Playing)
        {
            yield return null;
        }

        Debug.Log("Timeline cutscene finished.");
    }

    private IEnumerator PlayBlackScreenCutscene()
    {
        if (fadeScreen == null)
        {
            Debug.LogError("FadeScreen is not assigned!");
            yield break;
        }

        Debug.Log("Playing Black Screen cutscene...");

        // Fade to black
        yield return StartCoroutine(fadeScreen.FadeToBlack(fadeDuration));

        // Display text lines
        foreach (string text in textLines)
        {
            fadeScreen.ShowText(text);
            yield return new WaitForSeconds(textDisplayDuration);
        }

        // Fade back to clear
        fadeScreen.HideText();
        yield return StartCoroutine(fadeScreen.FadeToClear(fadeDuration));

        Debug.Log("Black Screen cutscene finished.");
    }

    private void DisablePlayerControl()
    {
        if (playerController != null)
        {
            // Disable all MonoBehaviour scripts on player (generic approach)
            MonoBehaviour[] scripts = playerController.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null && script.enabled)
                {
                    script.enabled = false;
                }
            }

            // Lock cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            Debug.Log("Player control disabled.");
        }
    }

    private void ResumeGameplay()
    {
        // Re-enable player control
        if (disablePlayerControl && playerController != null)
        {
            // Re-enable all MonoBehaviour scripts on player
            MonoBehaviour[] scripts = playerController.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null)
                {
                    script.enabled = true;
                }
            }

            // Lock cursor back
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            Debug.Log("Player control re-enabled.");
        }

        Debug.Log("Gameplay resumed.");
    }

    // Public method to manually start cutscene (for testing)
    [ContextMenu("Test Cutscene")]
    public void TestCutscene()
    {
        StartCutscene();
    }
}

