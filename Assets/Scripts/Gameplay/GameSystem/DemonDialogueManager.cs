using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DemonDialogueManager : MonoBehaviour
{
    public static DemonDialogueManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private GameObject playerObject;

    [Header("Demon")]
    [SerializeField] private GameObject demonObject;
    [SerializeField] private ParticleSystem appearVFX;
    [SerializeField] private AudioClip appearSound;
    [SerializeField] private float appearDuration = 0.4f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button nextButton;

    [Header("Demon Float")]
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float Scale = 0.5f;
    [SerializeField] private float floatSpeed = 1.2f;

    private DialogueSequenceSO _current;
    private int _lineIndex;
    private Vector3 _demonOriginalPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        demonObject.SetActive(false);
        dialoguePanel.SetActive(false);
        nextButton.onClick.AddListener(NextLine);
    }

    public void StartDialogue(DialogueSequenceSO dialogue)
    {
        _current = dialogue;

        if (PlayerPrefs.GetInt(_current.name, 0) != 1)
        {
            _lineIndex = 0;
            GameStateManager.Instance.SetGameState(GameState.TUTORIAL);
            AppearDemon(() => ShowLine(_lineIndex));
        }
    }

    private void AppearDemon(System.Action onComplete)
    {
        demonObject.SetActive(true);
        demonObject.transform.position = playerObject.transform.position + new Vector3(1f, 0.5f, 0);

        // Sonido
        if (appearSound != null)
            AudioController.Instance.PlaySoundEffect(appearSound);

        // VFX polvo
        if (appearVFX != null)
            appearVFX.Play();

        // Escala de 0 a 1
        demonObject.transform.localScale = Vector3.zero;
        LeanTween.scale(demonObject, new Vector3(Scale, Scale, Scale), appearDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() =>
            {
                StartFloat();
                dialoguePanel.SetActive(true);
                onComplete?.Invoke();
            });
    }

    private void StartFloat()
    {
        LeanTween.cancel(demonObject);
        LeanTween.moveY(demonObject, playerObject.transform.position.y + floatAmplitude, floatSpeed)
            .setEase(LeanTweenType.easeInOutSine)
            .setLoopPingPong();
    }

    private void ShowLine(int index)
    {
        dialogueText.text = _current.lines[index].text;
    }

    private void NextLine()
    {
        _lineIndex++;

        if (_lineIndex >= _current.lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowLine(_lineIndex);
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        // Desaparecer demon
        LeanTween.cancel(demonObject);
        LeanTween.scale(demonObject, Vector3.zero, appearDuration)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                demonObject.SetActive(false);
            });

        PlayerPrefs.SetInt(_current.name, 1);
        PlayerPrefs.Save();
        GameStateManager.Instance.SetGameState(GameState.PLAYING);
    }
}