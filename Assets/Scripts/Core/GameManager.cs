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
    [Tooltip("玩家移動速度（公尺/秒）")]
    public float playerMoveSpeed = 3f;
    
    [Tooltip("玩家基礎攻擊力")]
    public float playerBaseAttack = 34f;
    
    [Tooltip("玩家攻擊間隔（秒）")]
    public float playerAttackInterval = 1f;
    
    [Tooltip("子彈速度（公尺/秒）")]
    public float bulletSpeed = 10f;
    
    [Tooltip("玩家攻擊範圍（公尺）")]
    public float playerAttackRange = 10f;
    
    [Tooltip("敵人生成距離（公尺）")]
    public float enemySpawnDistance = 30f;
    
    [Tooltip("敵人移動速度（公尺/秒）")]
    public float enemyMoveSpeed = 1f;
    
    [Tooltip("敵人基礎生命值")]
    public float enemyBaseHealth = 100f;
    
    [Tooltip("敵人生成初始頻率（個/秒）")]
    public float enemySpawnRateStart = 0.2f;
    
    [Tooltip("敵人生成最大頻率（個/秒）")]
    public float enemySpawnRateEnd = 1f;
    
    [Tooltip("經驗道具拾取範圍（公尺）")]
    public float expPickupRange = 5f;
    
    [Tooltip("每次拾取經驗增加的攻擊力")]
    public float expAttackBonus = 1f;
    
    [Tooltip("玩家最大受傷次數")]
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