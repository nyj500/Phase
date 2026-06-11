using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Ready, Playing, Paused, GameOver, Clear }
    private bool isPaused = false;
    public GameState State { get; private set; }
    public int clearCnt = 0;

    [SerializeField] private GameObject[] playerDeathFragments;
    [SerializeField] private float respawnDelay = 2f;
    private Transform respawnPoint;

    public AudioClip dieClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        State = GameState.Ready;
    }

    public void Init()
    {
        StageManager.Instance.Init();
        ChangeState(GameState.Playing);

        GameObject player = GameObject.Find("Player");
        if (player == null) return;

        respawnPoint = StageManager.Instance.CurrentStage?.transform.Find("RespawnPoint");
        if (respawnPoint != null)
            player.transform.position = respawnPoint.position;

        if (StageManager.Instance.currentStageNum == 1)
            player.transform.position = new Vector3(-17f, 37f, 0);
    }

    private void Update()
    {
        if (State != GameState.Ready && Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                ChangeState(GameState.Paused);
                isPaused = true;
            }
            else
            {
                ChangeState(GameState.Playing);
                isPaused = false;
            }
        }
    }

    public void PlayerDie(PlayerController player)
    {
        SoundManager.Instance.PlaySfx(dieClip);
        SkillController skillController = player.GetComponent<SkillController>();
        if (skillController != null)
        {
            skillController.RevertInversion();
        }
        if (player.gameObject.activeSelf)
        {
            StartCoroutine(PlayerDieCoroutine(player));
        }
    }

    private IEnumerator PlayerDieCoroutine(PlayerController player)
    {
        ChangeState(GameState.GameOver);

        Vector3 deathPosition = player.transform.position;
        Transform circleEffects = player.transform.parent.Find("CircleEffects");
        player.gameObject.SetActive(false);
        if (circleEffects != null)
        {
            circleEffects.gameObject.SetActive(false);
        }
        if (playerDeathFragments.Length > 0)
        {
            foreach (GameObject fragmentPrefab in playerDeathFragments)
            {
                GameObject fragment = Instantiate(fragmentPrefab, deathPosition, Quaternion.identity);
                Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                    rb.AddForce(direction * 200f);
                    float randomTorque = Random.Range(-30f, 30f);
                    rb.AddTorque(randomTorque);
                }
            }
        }

        yield return new WaitForSeconds(respawnDelay);

        GameObject currentStage = StageManager.Instance.CurrentStage;
        ResetObjects(currentStage.transform);

        crackedTile[] crackedTilesInStage = currentStage.GetComponentsInChildren<crackedTile>(true);
        foreach (crackedTile tile in crackedTilesInStage)
        {
            tile.gameObject.SetActive(true);
        }

        Key[] keysInStage = currentStage.GetComponentsInChildren<Key>(true);
        foreach (Key key in keysInStage)
        {
            key.gameObject.SetActive(true);
        }

        respawnPoint = currentStage.transform.Find("RespawnPoint");
        if (respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
        }

        player.gameObject.SetActive(true);
        if (circleEffects != null)
        {
            circleEffects.gameObject.SetActive(true);
        }

        SkillController skillController = player.GetComponent<SkillController>();
        if (skillController != null && skillController.isActiveAndEnabled)
        {
            skillController.ResetSkillState();
        }

        ChangeState(GameState.Playing);
    }

    public void ChangeState(GameState newState)
    {
        State = newState;
        switch (State)
        {
            case GameState.Ready:
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                UIManager.Instance.HidePausePopup();
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                UIManager.Instance.ShowPausePopup();
                break;
            case GameState.GameOver:
                break;
            case GameState.Clear:
                clearCnt++;
                SaveManager.Instance.ResetSave();
                UIManager.Instance.Init();
                break;
        }
    }

    public void RestartGame()
    {
        Debug.Log("게임 재시작");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return null;

        StageManager.Instance.ResetToDefaults();
        isPaused = false;

        Init();

        if (Camera.main != null)
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController != null)
                cameraController.Init();
        }

        if (UIManager.Instance != null)
            UIManager.Instance.ShowMainMenu();

        StageManager.Instance.ResetSkillToDefault();
    }

    private void ResetObjects(Transform stageTransform)
    {
        Dictionary<GameObject, bool> originalStates = new Dictionary<GameObject, bool>();
        foreach (Transform child in stageTransform)
        {
            originalStates.Add(child.gameObject, child.gameObject.activeSelf);
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in stageTransform)
        {
            if (originalStates[child.gameObject])
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
