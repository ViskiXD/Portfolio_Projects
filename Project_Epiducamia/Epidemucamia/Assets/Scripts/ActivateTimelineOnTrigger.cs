using UnityEngine;
using UnityEngine.Playables;

public class ActivateTimelineOnTrigger : MonoBehaviour
{
    public PlayableDirector timeline;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (timeline != null)
            {
                timeline.Play();
            }
            else
            {
                Debug.LogError("Timeline is not assigned");
            }
        }
    }
}