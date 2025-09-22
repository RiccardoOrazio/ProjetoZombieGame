using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerTemporario : MonoBehaviour
{
    public static GameManagerTemporario instance;

    [SerializeField] private GameObject winScreenPanel;
    [SerializeField] private int enemiesToWin = 3;

    private int enemiesKilled = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;
        if (enemiesKilled >= enemiesToWin)
        {
            ShowWinScreen();
        }
    }

    private void ShowWinScreen()
    {
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}