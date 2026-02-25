using UnityEngine;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "ScriptableObjects/TutorialStep")]
public class TutorialStepSO : ScriptableObject
{
    [TextArea] public string description;
    public string highlightTargetName; // nombre del RectTransform a destacar
}