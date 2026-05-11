# Phase

> Unity 2D 플랫포머 | 팀 프로젝트 | Steam 출시작

이동·점프·시간 정지+대시 스킬로 장애물을 피하고 탈출 조건을 달성해 맵을 탈출하는 게임.
플래시 게임 *Focus*에서 영감을 받아 제작.

🎮 [Steam 페이지](https://store.steampowered.com/app/4097670/Phase/)

## 담당 역할

**게임 시스템 로직 설계 및 UI 구현**  
GameManager · UIManager · SaveManager · EscapeTriggerController · 장애물 스크립트 (Enemy · MissileHazard · Spike · MovingSpike · Key)

## 주요 구현

### 매니저 기반 게임 시스템 설계

싱글톤 패턴으로 `GameManager` · `UIManager` · `SaveManager`를 분리해 각 시스템이 독립적으로 동작하도록 설계.

`GameManager`는 게임 상태(`Ready / Playing / Paused / GameOver / Clear`)를 열거형으로 관리하고, 상태 전환 시 `Time.timeScale`을 제어해 일시정지를 구현한다.

```csharp
// GameManager.cs
void ChangeState(GameState newState)
{
    currentState = newState;
    Time.timeScale = newState == GameState.Paused ? 0f : 1f;
}
```

### JSON 기반 저장/로드 시스템

에디터와 빌드 환경의 저장 경로를 전처리기 지시문으로 분기해 플랫폼별 호환성을 확보.

```csharp
// SaveManager.cs
#if UNITY_EDITOR
    savePath = "Assets/Resources/save.json";
#else
    savePath = Application.persistentDataPath + "/save.json";
#endif
```

`SaveData`에 현재 스테이지·스킬 등급·클리어 횟수를 JSON 직렬화하여 저장. `UIManager`가 `SaveManager`와 연동해 LoadButton 활성화 여부를 결정한다.

### 스테이지 탈출 조건 시스템

`EscapeTriggerController`는 씬에 존재하는 `LockCore`(파괴 여부) · `Key`(수집 여부) · `PushLockCore`(이동 여부) 조건을 순회해 탈출 가능 여부를 판별한다. 조건 오브젝트가 없으면 자동 통과.

조건 충족 시 Collider를 trigger로 전환하고 파편 이펙트를 생성. 플레이어 진입 시 `GameManager.IncreaseStage()`를 호출해 다음 스테이지로 전환한다.

### 미사일 패턴 — 이중 모드 설계

발사 시점에 `GuidanceMode`와 `ExplosionMode`를 주입받아 동작을 결정하는 구조로 설계.

```csharp
// MissileHazard.cs
public enum GuidanceMode { Straight, Guided }
public enum ExplosionMode { None, Explosive }

// Guided 모드: 회전각 제한으로 완전 추적 방지
float cross = Vector3.Cross(toTarget, transform.right).z;
angularVelocity = cross * rotateSpeed;  // 회전 속도 상한으로 회피 가능
```

- **Straight**: 발사 시점의 방향 벡터 고정 후 일정 속도로 직진
- **Guided**: 플레이어를 실시간 추적하되 회전 속도에 상한을 두어 회피 가능

시간 정지 스킬 활성화 시 `currentTimeScale`을 반영해 미사일 속도·회전이 함께 감소한다.

## 기술 스택

Unity · C# · 싱글톤 패턴 · JSON 직렬화 · Rigidbody2D · Coroutine
