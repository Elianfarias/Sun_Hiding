using UnityEngine;

public enum GameState
{
    MAIN_MENU,
    PLAYING,
    PAUSED,
    GAME_OVER,
    TUTORIAL
}

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private KeyCode keyCodeInmortalMode = KeyCode.M;
    [SerializeField] private AudioClip clipGameOver;
    public static GameStateManager Instance { get; private set; }
    public GameState CurrentGameState { get; private set; } = GameState.PLAYING;
    public bool inmortalMode;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        SetGameState(CurrentGameState);
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyCodeInmortalMode))
            inmortalMode = !inmortalMode;
    }

    public void SetGameState(GameState newState)
    {
        CurrentGameState = newState;
        switch (newState)
        {
            case GameState.TUTORIAL:
                AudioController.Instance.PlayBackgroundMusic();
                break;
            case GameState.PLAYING:
                AudioController.Instance.PlayBackgroundMusic();
                break;
            case GameState.PAUSED:
                break;
            case GameState.GAME_OVER:
                AudioController.Instance.StopBackgroundMusic();
                HUDManager.Instance.ShowPanelPlayerLose();
                Time.timeScale = 0;
                break;
        }
    }
}
