using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ExplosionAnimator : MonoBehaviour
{
    [Header("Sprite Sheet Settings")]
    [SerializeField] private int tilesX = 8;
    [SerializeField] private int tilesY = 8;
    [SerializeField] private int cycles = 1;
    [SerializeField] private bool randomStartFrame = false;
    [SerializeField] private float animationSpeed = 1f;
    
    void Awake()
    {
        SetupTextureSheetAnimation();
    }
    
    void SetupTextureSheetAnimation()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps == null) return;
        
        // Configure main module
        var main = ps.main;
        main.startLifetime = 1.5f; // Match explosion duration
        main.startSpeed = 0f;
        main.startSize = 1f;
        main.maxParticles = 1;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = true;
        
        // Configure emission - single particle
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0.0f, 1)
        });
        
        // Configure renderer for billboard
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.alignment = ParticleSystemRenderSpace.Facing;
            renderer.sortMode = ParticleSystemSortMode.None;
        }
        
        // Enable and configure texture sheet animation module
        var textureSheet = ps.textureSheetAnimation;
        textureSheet.enabled = true;
        textureSheet.mode = ParticleSystemAnimationMode.Grid;
        
        // Set tile configuration
        textureSheet.numTilesX = tilesX;
        textureSheet.numTilesY = tilesY;
        
        // Animation type - plays through all frames over particle lifetime
        textureSheet.animation = ParticleSystemAnimationType.WholeSheet;
        textureSheet.timeMode = ParticleSystemAnimationTimeMode.Lifetime;
        
        // Cycles - how many times to play the animation
        textureSheet.cycleCount = cycles;
        
        // Random start frame for variation
        if (randomStartFrame)
        {
            textureSheet.startFrame = new ParticleSystem.MinMaxCurve(0, tilesX * tilesY - 1);
            textureSheet.frameOverTime = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(1, 1)
            ));
        }
        else
        {
            textureSheet.startFrame = new ParticleSystem.MinMaxCurve(0);
            // Animate through all frames over lifetime
            int totalFrames = tilesX * tilesY;
            textureSheet.frameOverTime = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(1, totalFrames - 1)
            ));
        }
        
        // Apply speed multiplier
        textureSheet.fps = 0; // Use curve instead
        textureSheet.speedRange = new Vector2(animationSpeed, animationSpeed);
        
        // Play the particle system
        ps.Play();
    }
}

