using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingHazard : MonoBehaviour
{
    public enum LoopMode { PingPong, Restart }

    public LoopMode loopMode = LoopMode.PingPong;
    [Tooltip("시작점을 EmptyObject로 지정")]
    public Transform pointA; // 시작점(비워두면 현재 위치)
    [Tooltip("끝점을 EmptyObject로 지정")]
    public Transform pointB; // 끝점
    public float speed = 5f;
    public float waitAtNode = 0f; // 끝점에서 잠시 대기(초)

    Vector3 startPos, endPos;
    float moveT; // 0: startPos ~ 1: endPos
    int moveDir = 1; // 1: A->B, -1: B->A
    float waitTimer;

    Rigidbody2D _rb;
    TimeAffected timeAffected;
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        timeAffected = GetComponent<TimeAffected>();
        RecalcEndpoints();
        moveT = 0f;
    }

    void RecalcEndpoints()
    {
        var here = transform.position;
        startPos = pointA ? pointA.position : here;
        endPos = pointB ? pointB.position : (here + Vector3.right * 3f);
    }

    void FixedUpdate()
    {
        float scaledDeltaTime = Time.fixedDeltaTime * timeAffected.currentTimeScale;
        TickMove(scaledDeltaTime);
    }

    void TickMove(float dt)
    {
        if (waitTimer > 0f)
        {
            waitTimer -= dt;
            return;
        }
        
        float dist = Vector3.Distance(startPos, endPos);
        if (dist < 1e-4f) return;

        moveT += (moveDir * speed * dt) / dist;
        moveT = Mathf.Clamp01(moveT);

        Vector3 next = Vector3.Lerp(startPos, endPos, moveT);
        _rb.MovePosition(next);
        
        if (loopMode == LoopMode.PingPong)
        {
            if (moveT >= 1f || moveT <= 0f)
            {
                moveDir *= -1;
                // 오브젝트 회전 추가하기   
                if (waitAtNode > 0f) waitTimer = waitAtNode;
            }
        }
        else if (loopMode == LoopMode.Restart)
        {
            if (next == endPos)
            {
                if (waitAtNode > 0f) waitTimer = waitAtNode;
                // 시작점으로 즉시 워프 후 다음 프레임부터 다시 B로 향함
                if (_rb != null)
                {
                    _rb.position = startPos;
                    moveT = 0f;
                }
                else transform.position = startPos;
            }
        }
    }
}
