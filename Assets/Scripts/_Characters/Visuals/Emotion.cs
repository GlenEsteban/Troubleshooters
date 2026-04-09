using System;
using UnityEngine;

/// <summary>
/// Tracks the current emotion and notifies when emotion changes.
/// </summary>
public class Emotion : MonoBehaviour {
    public event Action<EmotionType> EmotionChanged;
    public EmotionType CurrentEmotion => currentEmotion;

    [SerializeField] private EmotionType currentEmotion;

    public void SetEmotion(EmotionType emotion) {
        if (currentEmotion == emotion) { return; }

        currentEmotion = emotion;

        EmotionChanged?.Invoke(currentEmotion);
    }
}