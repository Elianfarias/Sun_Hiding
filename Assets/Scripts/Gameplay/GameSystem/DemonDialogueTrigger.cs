using UnityEngine;

public class DemonDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSequenceSO dialogue;
    [SerializeField] private bool triggerOnce = true;

    private bool _triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered && triggerOnce) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;
        DemonDialogueManager.Instance.StartDialogue(dialogue);
    }
}