using System;
using UnityEngine;

/// <summary>
/// Tracks the current emotion and notifies when emotion changes.
/// </summary>
public class Emotion : MonoBehaviour {
    public event Action<DisplayAnimation> EmotionChanged;
    public DisplayAnimation CurrentEmotion => currentEmotion;

    [SerializeField] private DisplayAnimation currentEmotion;

    public void SetEmotion(DisplayAnimation emotion) {
        if (currentEmotion == emotion) { return; }

        currentEmotion = emotion;

        EmotionChanged?.Invoke(currentEmotion);
    }
}