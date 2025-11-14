using System.Collections.Generic;
using UnityEngine;

// Attach to any AudioSource you want processed by the RNBO SpaceEcho patch.
[RequireComponent(typeof(AudioSource))]
public class RNBOAudioFilter : MonoBehaviour
{
    public static readonly List<RNBOAudioFilter> ActiveFilters = new List<RNBOAudioFilter>();

    [HideInInspector]
    public SpaceEchoHandle handle;

    void OnEnable()
    {
        if (!ActiveFilters.Contains(this)) ActiveFilters.Add(this);
    }

    void OnDisable()
    {
        ActiveFilters.Remove(this);
    }

    void Start()
    {
        if (handle == null)
        {
            try
            {
                handle = new SpaceEchoHandle();
            }
            catch (System.DllNotFoundException)
            {
                Debug.LogWarning("SpaceEcho native plugin not found - audio filtering disabled. This is normal if the plugin wasn't included.");
                handle = null;
                this.enabled = false; // Disable this component since it can't function without the plugin
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to initialize SpaceEcho handle: {e.Message}");
                handle = null;
                this.enabled = false;
            }
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (handle == null) return;
        handle.Process(data, channels);
    }
}


