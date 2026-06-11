using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public int currentStageNum = 1;
    public int skillGrade = 0;
    public int lastStage = 40;

    [SerializeField] private AudioClip bgmClip1;
    [SerializeField] private AudioClip bgmClip2;

    private List<GameObject> stages = new List<GameObject>();
    private GameObject currentStage;
    private SkillController skillController;

    public GameObject CurrentStage => currentStage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        skillController = GameObject.Find("Player")?.GetComponent<SkillController>();
        InitStage();
        UpdateBgmForStage(currentStageNum);

        if (skillController != null)
        {
            AdjustSkillGradeForStage();
            ApplySkillForGrade();
        }
    }

    private void InitStage()
    {
        Transform normalWorld = GameObject.Find("Objects")?.GetComponent<Transform>();
        if (normalWorld == null)
        {
            Debug.Log("Can't find Objects");
            return;
        }

        stages.Clear();
        foreach (Transform stage in normalWorld)
        {
            if (!stage.name.StartsWith("Stage")) continue;

            stages.Add(stage.gameObject);
            Tilemap stageTilemap = stage.GetComponentInChildren<Tilemap>();
            if (stageTilemap != null)
                stageTilemap.color = Color.white;

            if (stage.name == "Stage" + currentStageNum)
            {
                stage.gameObject.SetActive(true);
                currentStage = stage.gameObject;
            }
            else
            {
                stage.gameObject.SetActive(false);
            }
        }
    }

    public void IncreaseStage()
    {
        if (currentStageNum >= lastStage) return;

        GameObject prevStage = stages[currentStageNum - 1];
        currentStageNum++;
        currentStage = stages[currentStageNum - 1];
        currentStage.SetActive(true);
        prevStage.SetActive(false);

        CheckForSkillDegradation(currentStageNum);
        UpdateBgmForStage(currentStageNum);
    }

    private void AdjustSkillGradeForStage()
    {
        if (currentStageNum == 5) skillGrade = 0;
        else if (currentStageNum == 10) skillGrade = 1;
        else if (currentStageNum == 15) skillGrade = 2;
    }

    private void ApplySkillForGrade()
    {
        switch (skillGrade)
        {
            case 1:
                skillController.enabled = true;
                UIManager.Instance.UnlockSkill();
                break;
            case 2:
                skillController.enabled = true;
                skillController.releasePointMoveSpeed = 10f;
                skillController.circleShrinkSpeed = 1f;
                skillController.circleGrowSpeed = 0.7f;
                skillController.finalDashForce = 15f;
                skillController.UpdateCircleSize(new Vector3(3f, 3f, 2f));
                break;
            case 3:
                skillController.enabled = true;
                skillController.releasePointMoveSpeed = 15f;
                skillController.circleShrinkSpeed = 2f;
                skillController.circleGrowSpeed = 0.5f;
                skillController.finalDashForce = 20f;
                skillController.UpdateCircleSize(new Vector3(5f, 5f, 2f));
                break;
        }
    }

    private void CheckForSkillDegradation(int stageNum)
    {
        if (skillController == null || !skillController.isActiveAndEnabled) return;

        switch (stageNum)
        {
            case 35:
                skillController.circleShrinkSpeed = 0.7f;
                skillController.circleGrowSpeed = 0.3f;
                skillController.releasePointMoveSpeed = 5f;
                skillController.finalDashForce = 10f;
                skillController.UpdateCircleSize(new Vector3(2f, 2f, 1f));
                break;
            case 36:
                skillController.UpdateCircleSize(new Vector3(1.5f, 1.5f, 1f));
                break;
            case 37:
                skillController.UpdateCircleSize(new Vector3(1.3f, 1.3f, 1f));
                break;
            case 38:
                skillController.UpdateCircleSize(new Vector3(1.2f, 1.2f, 1f));
                break;
            case 39:
                skillController.enabled = false;
                break;
        }
    }

    private void UpdateBgmForStage(int stageNum)
    {
        AudioClip targetBgm;
        float fadeDuration;

        if (stageNum >= 16 && stageNum <= 34)
        {
            targetBgm = bgmClip2;
            fadeDuration = 50.0f;
        }
        else
        {
            targetBgm = bgmClip1;
            fadeDuration = 1.5f;
        }

        if (targetBgm != null)
            SoundManager.Instance.PlayBgm(targetBgm, fadeDuration);
    }

    public void ResetToDefaults()
    {
        currentStageNum = 1;
        skillGrade = 0;
    }

    public void ResetSkillToDefault()
    {
        skillController = GameObject.Find("Player")?.GetComponent<SkillController>();
        if (skillController == null) return;

        skillController.enabled = false;
        skillController.releasePointMoveSpeed = 5f;
        skillController.circleShrinkSpeed = 0.7f;
        skillController.circleGrowSpeed = 0.3f;
        skillController.finalDashForce = 10f;
        skillController.UpdateCircleSize(new Vector3(2.5f, 2.5f, 1f));
    }
}
