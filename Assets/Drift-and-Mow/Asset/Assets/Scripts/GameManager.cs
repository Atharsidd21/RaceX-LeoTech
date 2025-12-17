using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject PausePanel;
    public GameObject PauseBtn;
    public GameObject GameOverPanel;
    public GameObject LevelCompletePanel;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coinText;
    public GameObject LeaderboardPanel;
    public List<GameObject> playerListLeaderboard;

    [Header("Gameplay Settings")]
    [SerializeField] private float timer = 300f;
    [SerializeField] private float distanceToScoreFactor = 1f;

    private int currentScore = 0;
    private int coinScore = 0;
    private int currentHealth = 100;
    private float distanceTravelled = 0f;
    private Vector3 lastPosition;

    private int totalTargets = 0;
    private bool isGameOver = false;

    private GameObject player;

    private const string CurrencyKey = "Currency";

    string username = "";

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        coinScore = PlayerPrefs.GetInt(CurrencyKey, 0);
        username = PlayerPrefs.GetString(Menu.nameStr);
    }

    void Update()
    {
        if (player == null || isGameOver) return;

        HandleDistanceScore();
        HandleTimer();
    }

    public void AssignPlayer(GameObject car)
    {
        player = car;
        lastPosition = player.transform.position;
    }

    private void HandleDistanceScore()
    {
        float distance = Vector3.Distance(player.transform.position, lastPosition);
        if (distance > 0.01f)
        {
            distanceTravelled += distance;
            int newScore = Mathf.FloorToInt(distanceTravelled * distanceToScoreFactor);
            if (newScore > currentScore)
            {
                currentScore = newScore;
                UpdateScoreUI();
            }
            lastPosition = player.transform.position;
        }
    }

    private void HandleTimer()
    {
        timer -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();

        if (timer <= 0)
        {
            ShowGameOver();
        }
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $": {currentScore}";
        coinText.text = $": {coinScore}";
        healthText.text = $"Health: {currentHealth}";
    }

    public void AddCoins(int value)
    {
        coinScore += value;
        PlayerPrefs.SetInt(CurrencyKey, coinScore);
        PlayerPrefs.Save();
        UpdateScoreUI();
    }

    public void DamageCar()
    {
        currentHealth -= 10;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
        currentScore = Mathf.Max(0, currentScore - 5);
        UpdateScoreUI();

        if (currentHealth <= 0)
        {
            ShowGameOver();
        }
        else
        {
            Vector3 offset = player.transform.forward * -2f;
            player.transform.position += offset;
        }
    }

    public void ShowGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Stop car sounds
        if (player != null)
        {
            Controller carController = player.GetComponent<Controller>();
            if (carController != null)
            {
                carController.OnGameOver();
            }
        }

        GameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }


    public void ShowLevelComplete()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Stop car sounds
        if (player != null)
        {
            Controller carController = player.GetComponent<Controller>();
            if (carController != null)
            {
                carController.OnGameOver();
            }
        }

        //LevelCompletePanel.SetActive(true);
        ShowLeaderboard(PlayerPrefs.GetInt(Menu.LeaderboardRank));
        LeaderboardPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ShowLeaderboard(int rank)
    {
        List<string> names = new List<string>();
        for (int i = 0; i < Menu.playerName.Count; i++)
        {
            names.Add(Menu.playerName[i]);
        }
        for (int i = 0; i < 5; i++)
        {
            if (i == rank)
            {
                if (string.IsNullOrEmpty(username))
                {
                    username = "username" + UnityEngine.Random.Range(000000, 999999);
                    PlayerPrefs.SetString(Menu.nameStr, username);
                }
                playerListLeaderboard[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = username;
            }
            else
            {
                int ind = UnityEngine.Random.Range(0, names.Count - 1);
                playerListLeaderboard[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = names[ind];
                names.RemoveAt(ind);
            }
        }
    }


    public void OnClickPauseBtn()
    {
        PausePanel.SetActive(true);
        PauseBtn.SetActive(false);

        if (player != null)
        {
            Controller carController = player.GetComponent<Controller>();
            if (carController != null)
            {
                carController.OnPause(); //  only pause audio, not stop control
            }
        }

        Time.timeScale = 0f;
    }


    public void OnClickResumeBtn()
    {
        PausePanel.SetActive(false);
        PauseBtn.SetActive(true);
        Time.timeScale = 1f;

        if (player != null)
        {
            Controller carController = player.GetComponent<Controller>();
            if (carController != null)
            {
                carController.OnResume(); //  resume full control + sound
            }
        }
    }


    public void OnClickRestartBtn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitBtn()
    {
        PlayerPrefs.SetInt(Menu.GotoHome, 1);
        SceneManager.LoadSceneAsync(0);
    }

    public void OnClickNextLevelBtn()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
             //PlayerPrefs.SetInt(Menu.ShowLeaderBoard, 1);
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
    public void TargetDestroyed()
    {
        totalTargets--;
    }

    public void CollectCoin()
    {
        AddCoins(5); // Or however many coins you want per target
    }

    public void HealCar()
    {
        if (currentHealth < 100)
        {
            currentHealth += 10;
            currentHealth = Mathf.Clamp(currentHealth, 0, 100);
            UpdateScoreUI();
        }
    }
}
