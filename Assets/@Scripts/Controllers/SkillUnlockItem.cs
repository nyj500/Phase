using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUnlockItem : MonoBehaviour
{
    public enum UpgradeType
    {
        UnLock, 
        Upgrade_1, 
        Upgrade_2 
    }
    public UpgradeType itemType = UpgradeType.UnLock;
    private AudioSource audioSource;
    public AudioClip launchClip;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void OnEnable()
    {
        audioSource.loop = true;
        audioSource.Play();
    }
    void OnDisable()
    {
        audioSource.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController != null)
            {
                cameraController.RestoreCameraPosz();
            }

            if (collision.GetComponent<SkillController>() == null) return;

            switch (itemType)
            {
                case UpgradeType.UnLock:    StageManager.Instance.skillGrade = 1; break;
                case UpgradeType.Upgrade_1: StageManager.Instance.skillGrade = 2; break;
                case UpgradeType.Upgrade_2: StageManager.Instance.skillGrade = 3; break;
            }
            StageManager.Instance.ApplySkillForGrade();
            SoundManager.Instance.PlaySfx(launchClip);
            gameObject.SetActive(false);
        }
    }
}
