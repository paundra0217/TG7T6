using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public enum GameStatus
{
    DEFAULT,
    IMMORTAL,
    LOBBY,
    SETTINGS,
    INGAME,
    DIALOGUE,
    TRANSITION, //jika lg dalam animasi anything (ga cuma transition)
    SELECTION,
    PAUSE,
    DEATH,
    ENDING
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private float gravityScale;
    [SerializeField] private float defaultPlayerHealth = 100f;
    [SerializeField] private float defaultPlayerSpeed = 100f;
    [SerializeField] private float defaultPlayerStamina = 100f;
    [SerializeField] private int defaultPlayerLives = 3;
    [SerializeField] private GameStatus status;
    [SerializeField] private bool testMode = false;

    private CinemachineVirtualCamera CinemachineCamera;
    private GameObject spawnedPlayer;
    private bool IsPaused = false;
    private Vector2 spawnLocation = new Vector2(0, 0);
    private int currentLives;
    private Vector2 cameraReposition = new Vector2(0, 0);

    private float currentPlayerHealth;
    private float currentPlayerSpeed;
    private float currentPlayerStamina;

    private string InteractableAreaName;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Game Manager is Null");
            }

            return _instance;
        }
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void Start()
    {
        if (testMode)
        {
            currentLives = defaultPlayerLives;
            spawnLocation = new Vector2(0, 0);
            Physics2D.gravity = new Vector2(0, -gravityScale);
        }
        else
        {
            AudioController.Instance.PlayBGM("Main");
        }

        print(status);
    }

    public GameStatus GetStatus()
    {
        return status;
    }

    public void ChangeStatus(GameStatus newStatus)
    {
        status = newStatus;
    }

    public bool CompareStatus(GameStatus status)
    {
        return this.status == status;
    }

    public void TestFunction()
    {
        Debug.Log("Received called in GameManager");
    }

    public void StartGame()
    {
        if (testMode)
        {
            Debug.LogWarning("Game manager is set in test mode, disable test mode to start real game");
            return;
        }

        Physics2D.gravity = new Vector2(0, -gravityScale);
        Transition.Instance.SwitchScene("Level");
    }

    public void RetryGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void PauseGame()
    {
        if (!IsPaused)
        {
            ChangeStatus(GameStatus.PAUSE);
        }
        else
        {
            ChangeStatus(GameStatus.DEFAULT);
        }
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayerSpawn()
    {
        print("SpawnPlayer");
        ChangeStatus(GameStatus.DEFAULT);
        spawnedPlayer = Instantiate(player, new Vector3(spawnLocation.x, spawnLocation.y, 0), gameObject.transform.rotation);
        spawnedPlayer.GetComponent<PlayerHealth>().SetDefaultHP(defaultPlayerHealth);
        spawnedPlayer.GetComponent<PlayerHealth>().ResetHealth();
        spawnedPlayer.GetComponent<PlayerStamina>().SetStamina(defaultPlayerStamina);
        spawnedPlayer.GetComponent<PlayerSpeed>().SetSpeed(defaultPlayerSpeed);
        CinemachineCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        CinemachineCamera.Follow = spawnedPlayer.transform;
        print(GetStatus());
    }

    public void PlayerDeath()
    {
        CinemachineCamera.Follow = null;
        print("Player dead");
        Destroy(spawnedPlayer);
        print(spawnedPlayer);
        InteractableManager.Instance.TriggerGameOver();
    }

    public void KillPlayer()
    {
        spawnedPlayer.GetComponent<PlayerHealth>().KillPlayer();
    }

    public void RespawnPlayer()
    {
        print(currentLives);
        Destroy(spawnedPlayer);
        PlayerSpawn();
    }

    public void CameraStopFollow()
    {
        CinemachineCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        CinemachineCamera.Follow = null;
    }

    public void CameraStartFollow(GameObject target)
    {
        CinemachineCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        CinemachineCamera.Follow = target.transform;
    }

    public void CameraStartFollowPlayer()
    {
        CinemachineCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        CinemachineCamera.Follow = spawnedPlayer.transform;
    }

    public void SetInteractableArea(string name)
    {
        InteractableAreaName = name;
    }

    public void InteractItem()
    {
        if (InteractableAreaName == null) return;
    }

    public int GetLives()
    {
        return currentLives;
    }

    public void TakeLive()
    {
        print("Lives taken");
        currentLives--;
    }

    public void SetSpawnPoint(float x, float y)
    {
        spawnLocation = new Vector2(x, y);
    }

    public void TriggerCredits()
    {
        CinemachineCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        CinemachineCamera.Follow = GameObject.Find("CreditTarget").transform;
        ChangeStatus(GameStatus.ENDING);
        AudioController.Instance.PlayBGM("Credit");
    }

    private void SceneChanged(Scene current, Scene next)
    {
        switch (next.name)
        {
            case "Level":
                Cursor.visible = false;
                AudioController.Instance.PlayBGM("Climbing");
                currentLives = defaultPlayerLives;
                spawnLocation = new Vector2(0, 0);
                PlayerSpawn();
                break;

            case "MainMenu":
                Cursor.visible = true;
                ChangeStatus(GameStatus.LOBBY);
                AudioController.Instance.PlayBGM("Main");
                break;

            case "GameOver":
                Cursor.visible = true;
                break;
        }
    }
}
