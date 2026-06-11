using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EscapeTriggerController : MonoBehaviour
{
    static bool isSfxPlaying = false;

    public GameObject groundCheckObject;
    public GameObject[] fragmentPrefabs;
    public float explosionForce = 300f;
    public float torqueAmount = 100f;
    private bool hasShattered = false;
    public AudioClip escapeClip;

    private Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private LockCore[] lockCores;
    private Key[] keys;
    private PushLockCore[] pushLockCores;

    void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        lockCores = FindObjectsByType<LockCore>(FindObjectsSortMode.None);
        keys = FindObjectsByType<Key>(FindObjectsSortMode.None);
        pushLockCores = FindObjectsByType<PushLockCore>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (CanEnableEscapeTrigger())
        {
            if (!isSfxPlaying)
            {
                SoundManager.Instance.PlaySfx(escapeClip, 0.3f);
                isSfxPlaying = true;
            }
            // 해금 조건을 만족했고, 아직 파편 효과가 실행되지 않았다면
            if (!hasShattered)
            {
                Shatter();
                hasShattered = true;
            }
            if (collider2D != null) collider2D.isTrigger = true;
            if (spriteRenderer != null) spriteRenderer.enabled = false;

            // GroungCheckObject ��Ȱ��ȭ�� ��
            if (groundCheckObject != null)
            {
                groundCheckObject.SetActive(false);
            }
        }
        else
        {
            if (collider2D != null) collider2D.isTrigger = false;
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (groundCheckObject != null)
            {
                groundCheckObject.SetActive(true);
            }
            hasShattered = false;
            isSfxPlaying = false;
        }
    }

    void Shatter()
    {
        if (fragmentPrefabs == null || fragmentPrefabs.Length == 0)
        {
            return;
        }

        foreach (GameObject fragmentPrefab in fragmentPrefabs)
        {
            GameObject frament = Instantiate(fragmentPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = frament.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                rb.AddForce(direction * explosionForce);
                float randomTorque = Random.Range(-torqueAmount, torqueAmount);
                rb.AddTorque(randomTorque);
            }
        }
    }
    
    bool CanEnableEscapeTrigger()
    {
        foreach (LockCore lockCore in lockCores)
            if (!lockCore.isBroken) return false;

        foreach (Key key in keys)
            if (!key.isAcquired) return false;

        foreach (PushLockCore pushLockCore in pushLockCores)
            if (!pushLockCore.isPushed) return false;

        return true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            StageManager.Instance.IncreaseStage();
        }
    }
}
