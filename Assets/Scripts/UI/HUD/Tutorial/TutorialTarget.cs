using UnityEngine;

public class TutorialTarget : MonoBehaviour
{
    [SerializeField] private string targetId;

    private void Start()
    {
        TutorialManager.Instance.RegisterTarget(targetId, GetComponent<RectTransform>());
    }
}