using UnityEngine;

[CreateAssetMenu(fileName = "NewIntroScene", menuName = "Dialogue/IntroScene")]
public class IntroSceneData : ScriptableObject
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string characterName;
        [TextArea(3, 5)]
        public string sentence;
        public Sprite expressionSprite;
        public AudioClip voiceClip; // Add this line for the voice over
    }

    public DialogueLine[] lines;
}