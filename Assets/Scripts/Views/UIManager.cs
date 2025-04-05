using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("遊戲狀態UI")]
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI playerHitsText;

    [Header("遊戲結束UI")]
    public GameObject gameOverPanel;
    public Button restartButton;

    private void Start()
    {
        // 註冊事件
        GameManager.Instance.OnKillCountChanged += UpdateKillCount;
        GameManager.Instance.OnAttackChanged += UpdateAttack;
        GameManager.Instance.OnPlayerHitsChanged += UpdatePlayerHits;
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        GameManager.Instance.OnGameOver += ShowGameOver;

        // 設置重啟按鈕
        restartButton.onClick.AddListener(() => {
            gameOverPanel.SetActive(false);
            GameManager.Instance.RestartGame();
        });

        // 初始化UI
        UpdateKillCount(GameManager.Instance.KillCount);
        UpdateAttack(GameManager.Instance.CurrentAttack);
        UpdatePlayerHits(GameManager.Instance.PlayerHits);
        gameOverPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // 取消註冊事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnKillCountChanged -= UpdateKillCount;
            GameManager.Instance.OnAttackChanged -= UpdateAttack;
            GameManager.Instance.OnPlayerHitsChanged -= UpdatePlayerHits;
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            GameManager.Instance.OnGameOver -= ShowGameOver;
        }
    }

    private void UpdateKillCount(int count)
    {
        killCountText.text = $"擊殺數: {count}";
    }

    private void UpdateAttack(float attack)
    {
        attackText.text = $"攻擊力: {attack:F1}";
    }

    private void UpdatePlayerHits(int hits)
    {
        playerHitsText.text = $"受傷次數: {hits}/10";
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Playing:
                gameOverPanel.SetActive(false);
                break;
            case GameManager.GameState.Paused:
                // 可以添加暫停菜單
                break;
        }
    }

    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
} 