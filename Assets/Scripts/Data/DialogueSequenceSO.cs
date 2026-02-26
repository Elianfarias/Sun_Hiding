using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 4)] public string text;
}

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "ScriptableObjects/DialogueSequence")]
public class DialogueSequenceSO : ScriptableObject
{
    public DialogueLine[] lines;
}