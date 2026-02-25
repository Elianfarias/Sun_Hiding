using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Steps")]
    [SerializeField] private TutorialStepSO[] steps;

    [Header("UI References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button nextButton;
    [SerializeField] private RectTransform highlightFrame;

    [Header("Highlight Settings")]
    [SerializeField] private float highlightPulseScale = 1.08f;
    [SerializeField] private float highlightPulseDuration = 0.6f;

    // Todos los posibles targets registrados por nombre
    private readonly Dictionary<string, RectTransform> _targets = new();
    private int _currentStep = 0;
    private const string PlayerPrefKey = "TutorialCompleted";

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        nextButton.onClick.AddListener(NextStep);

        // Esperar un frame para que todos los TutorialTarget se registren
        LeanTween.delayedCall(0.1f, () =>
        {
            if (PlayerPrefs.GetInt(PlayerPrefKey, 0) == 1)
            {
                tutorialPanel.SetActive(false);
                GameStateManager.Instance.SetGameState(GameState.PLAYING);
                return;
            }

            StartTutorial();
        });
    }

    // Cada elemento de HUD se registra a sí mismo
    public void RegisterTarget(string id, RectTransform target)
    {
        if (!_targets.ContainsKey(id))
            _targets.Add(id, target);
    }

    public void StartTutorial()
    {
        _currentStep = 0;
        tutorialPanel.SetActive(true);
        GameStateManager.Instance.SetGameState(GameState.TUTORIAL);
        ShowStep(_currentStep);
    }

    private void ShowStep(int index)
    {
        if (index >= steps.Length)
        {
            CompleteTutorial();
            return;
        }

        TutorialStepSO step = steps[index];
        descriptionText.text = step.description;

        // Mover highlight al target
        if (_targets.TryGetValue(step.highlightTargetName, out RectTransform target))
        {
            highlightFrame.gameObject.SetActive(true);
            highlightFrame.position = target.position;
            highlightFrame.sizeDelta = target.sizeDelta + new Vector2(10f, 10f);
            PlayHighlightPulse();
        }
        else
        {
            highlightFrame.gameObject.SetActive(false);
        }
    }

    private void NextStep()
    {
        _currentStep++;
        ShowStep(_currentStep);
    }

    private void CompleteTutorial()
    {
        LeanTween.cancel(highlightFrame.gameObject);
        tutorialPanel.SetActive(false);
        PlayerPrefs.SetInt(PlayerPrefKey, 1);
        PlayerPrefs.Save();
        GameStateManager.Instance.SetGameState(GameState.PLAYING);
    }

    private void PlayHighlightPulse()
    {
        LeanTween.cancel(highlightFrame.gameObject);
        highlightFrame.localScale = Vector3.one;

        LeanTween.scale(highlightFrame.gameObject, Vector3.one * highlightPulseScale, highlightPulseDuration)
            .setEase(LeanTweenType.easeInOutSine)
            .setLoopPingPong();
    }
}