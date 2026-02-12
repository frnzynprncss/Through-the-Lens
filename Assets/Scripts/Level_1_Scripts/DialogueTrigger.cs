using UnityEngine;



public class DialogueTrigger : MonoBehaviour

{

    [TextArea] public string lineToSay;

    [SerializeField] private AudioClip dialogueAudio;



    [Header("Expressions")]

    [SerializeField] private Sprite expressionA;

    [SerializeField] private Sprite expressionB;

    [SerializeField] private bool useExpressionB = false;



    private bool hasTriggered = false;



    private void OnTriggerEnter2D(Collider2D other)

    {

        if (other.CompareTag("Player") && !hasTriggered)

        {

            Time.timeScale = 0f; // Freeze everything



            Sprite selectedSprite = useExpressionB ? expressionB : expressionA;

            NarrativeEngine.Instance.PlayLine(lineToSay, dialogueAudio, selectedSprite);



            hasTriggered = true;

        }

    }

}