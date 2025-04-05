using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 遊戲狀態
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentState { get; private set; }

    // 遊戲配置
    [Header("遊戲配置")]
    public float playerMoveSpeed = 3f;
    public float playerBaseAttack = 34f;
    public float playerAttackInterval = 1f;
    public float bulletSpeed = 10f;
    public float enemySpawnDistance = 30f;
    public float enemyMoveSpeed = 1f;
    public float enemyBaseHealth = 100f;
    public float enemySpawnRateStart = 0.2f;
    public float enemySpawnRateEnd = 1f;
    public float expPickupRange = 5f;
    public float expAttackBonus = 1f;
    public int maxPlayerHits = 10;

    // 遊戲數據
    public int KillCount { get; private set; }
    public float CurrentAttack { get; private set; }
    public int PlayerHits { get; private set; }

    // 事件
    public event Action<GameState> OnGameStateChanged;
    public event Action<int> OnKillCountChanged;
    public event Action<float> OnAttackChanged;
    public event Action<int> OnPlayerHitsChanged;
    public event Action OnGameOver;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        CurrentState = GameState.Playing;
        KillCount = 0;
        CurrentAttack = playerBaseAttack;
        PlayerHits = 0;
        
        OnGameStateChanged?.Invoke(CurrentState);
        OnKillCountChanged?.Invoke(KillCount);
        OnAttackChanged?.Invoke(CurrentAttack);
        OnPlayerHitsChanged?.Invoke(PlayerHits);
    }

    public void AddKill()
    {
        KillCount++;
        OnKillCountChanged?.Invoke(KillCount);
    }

    public void AddAttackBonus()
    {
        CurrentAttack += expAttackBonus;
        OnAttackChanged?.Invoke(CurrentAttack);
    }

    public void AddPlayerHit()
    {
        PlayerHits++;
        OnPlayerHitsChanged?.Invoke(PlayerHits);

        if (PlayerHits >= maxPlayerHits)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        OnGameStateChanged?.Invoke(CurrentState);
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        InitializeGame();
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
            OnGameStateChanged?.Invoke(CurrentState);
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            OnGameStateChanged?.Invoke(CurrentState);
        }
    }
} 