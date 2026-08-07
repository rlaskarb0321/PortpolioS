# PortpolioS

> 게임 클라이언트 개발자 공고에서 자주 요구되거나 차별점이 되는 기능을 도입한, 기술 중심 ARPG 포트폴리오

---

## 목차

- [1. 개요](#1-개요)
  - [1.1. 프로젝트 목표](#11-프로젝트-목표)
  - [1.2. 기술 스택](#12-기술-스택)
- [2. 프로젝트 구조](#2-프로젝트-구조)
  - [2.1. 어셈블리 구성](#21-어셈블리-구성)
  - [2.2. 씬 구성](#22-씬-구성)
- [3. 앱 부트스트랩](#3-앱-부트스트랩)
  - [3.1. AppLaunchManager 와 실행 모듈](#31-applaunchmanager-와-실행-모듈)
  - [3.2. 로딩 씬과 씬 로드 전략](#32-로딩-씬과-씬-로드-전략)
  - [3.3. 게임 모드와 서브 매니저](#33-게임-모드와-서브-매니저)
- [4. 네트워크 멀티플레이](#4-네트워크-멀티플레이)
  - [4.1. NetworkRunnerController](#41-networkrunnercontroller)
  - [4.2. 매치메이킹과 로비](#42-매치메이킹과-로비)
  - [4.3. 인게임 씬 구성 흐름](#43-인게임-씬-구성-흐름)
- [5. 플레이어 컨트롤](#5-플레이어-컨트롤)
  - [5.1. 입력과 가상 조이스틱](#51-입력과-가상-조이스틱)
  - [5.2. 플레이어 FSM](#52-플레이어-fsm)
  - [5.3. 애니메이션 전략](#53-애니메이션-전략)
  - [5.4. 애니메이션 타임라인 마커](#54-애니메이션-타임라인-마커)
- [6. 데이터 파이프라인](#6-데이터-파이프라인)
  - [6.1. 뒤끝 차트 데이터](#61-뒤끝-차트-데이터)
  - [6.2. 캐릭터 Config](#62-캐릭터-config)
  - [6.3. Addressables 프리로드](#63-addressables-프리로드)
- [7. 에디터 툴](#7-에디터-툴)
  - [7.1. Go Test Scene](#71-go-test-scene)
  - [7.2. Character Combat Config Editor](#72-character-combat-config-editor)
  - [7.3. Chart Update](#73-chart-update)

---

## 1. 개요

### 1.1. 프로젝트 목표

클라이언트 개발 직군에서 실제로 검증하는 영역 — 결정론, 네트워크 동기화, 상태 기반 전투, 데이터 주도 설계, 에셋 로딩 파이프라인 — 을 한 프로젝트 안에서 동작하는 형태로 구현하는 것을 목표로 합니다.

[↑ 목차](#목차)

### 1.2. 기술 스택

| 분류 | 사용 기술 |
| --- | --- |
| 엔진 | Unity |
| 네트워크 | Photon Fusion 2 (Shared / AutoHostOrClient), Fusion KCC Addon, Fusion Physics Addon |
| 백엔드 | 뒤끝(TheBackend) — 로그인, 유저 데이터, CDN 차트 |
| 비동기 | UniTask |
| 에셋 로딩 | Addressables, AWS S3 |
| 입력 | Unity Input System + 커스텀 가상 조이스틱 |

[↑ 목차](#목차)

---

## 2. 프로젝트 구조

### 2.1. 어셈블리 구성

프로젝트 규모 확장에 따른 의존성 관리와 구조 파악의 어려움을 줄이기 위해 `asmdef`를 적용했습니다. 
기능 단위로 Assembly를 분리하여 의존 방향과 컴파일 범위를 명확하게 통제합니다.

| 어셈블리 | 폴더 | 역할 |
| --- | --- | --- |
| `Core` | `02. Scripts/Core` | 공통 유틸. 참조 목록이 비어 있는 최하위 |
| `BootstrapScene`(#3-앱-부트스트랩) | `02. Scripts/Bootstrap Scene` | 앱 진입, 실행 모듈, 게임 모드, 네트워크 러너 |
| `Multiplay Definitions` | `02. Scripts/Multiplay Definitions` | 멀티플레이 열거형 · 스테이지 정의. 데이터 계층이 공유하는 어휘만 담음 |
| `PlayerController` | `02. Scripts/Player` | 플레이어 입력 · FSM · 애니메이션 |
| `InGameMultiplay` | `02. Scripts/Other Scenes/InGame Multiplay` | 인게임 씬 조립 (맵/스폰/UI/Config 프리로드) |
| `BackendChart` | `02. Scripts/Scriptable Obejct/Chart Data` | 뒤끝 차트 SO. `Multiplay Definitions` 하나만 참조 |
| `PlayerController.Editor` | `02. Scripts/Player/Editor` | 에디터 전용. `includePlatforms: Editor` 라 빌드에 아예 포함되지 않음 |

[↑ 목차](#목차)

### 2.2. 씬 구성

| 씬 | 설명 |
| --- | --- |
| `Bootstrap - Instance Scene` | 앱 진입점. 전역 인스턴스 보유 |
| `Bootstrap - Loading Scene` | 로딩 연출 및 씬 전환 담당 |
| `Game Mode - Menu Scene` | 메인 메뉴 |
| `Game Mode - Multiplay Lobby Scene` | 멀티플레이 로비 |
| `Game Mode - InGame Multiplay` | 멀티플레이 인게임 |
| `Game Mode - Stage Mode Scene` | 스테이지 모드 |
| `UI Scene - Player Input` | 플레이어 입력 UI (Additive) |
| `For Test/Develop Scene` | 로그인·초기화를 건너뛰는 개발 전용 씬 |

[↑ 목차](#목차)

---

## 3. 앱 부트스트랩

### 3.1. AppLaunchManager 와 실행 모듈

앱 초기화를 단일 함수가 아니라 **순서를 가진 모듈 배열**로 표현합니다. `AppLaunchManager` 는 인스펙터에 등록된 `AppLaunchModuleBase` 를 순차적으로 `await` 하기만 합니다.

```
AppLaunchManager
├── firstLaunchModule        → ExecuteSync()  : 로딩 씬 먼저 띄움
└── appLaunchModules[]       → ExecuteAsync() : 순차 실행
    ├── TheBackendInitManager
    ├── TheBackendLoginManager
    ├── TheBackendUserDataLoader
    └── ...
```

초기화 단계가 늘어나도 매니저 코드를 건드리지 않고 모듈만 추가합니다.

[↑ 목차](#목차)

### 3.2. 로딩 씬과 씬 로드 전략

씬 로드는 로컬 로드와 네트워크 로드가 필요하지만 호출부는 같아야 하므로, `SceneLoadStrategyBase` 를 두고 구현을 갈랐습니다.

| 전략 | 용도 |
| --- | --- |
| `LocalSceneLoader` | `SceneManager` 기반 단독 씬 로드 |
| `NetworkSceneLoader` | Fusion `NetworkSceneManager` 를 통한 동기화 로드 |

`LoadingSceneManager` 는 `OnSceneActivated` / `OnCompleteLoad` 이벤트를 발행해, 게임 모드가 "에셋 활성화 시점"과 "모든 준비 완료 시점"을 구분해 훅을 걸 수 있게 합니다.

[↑ 목차](#목차)

### 3.3. 게임 모드와 서브 매니저

씬마다 `GameModeBase` 파생 클래스가 하나 존재하고, 그 아래 `SubManagerBase` 배열이 초기화 순서를 표현합니다.

```
GameModeBase (EGameModeType)
├── OnSceneActivated()          로딩 중 — SubManager 들의 DoInit() 을 순서대로 await
└── OnSceneCompletelyLoaded()   로딩 완료 후 진입 처리
```

`MultiplayerInGameMode` 에서는 이 순서가 곧 의존 순서입니다. 예를 들어 `CharacterConfigPreloader` 는 반드시 `PlayerCharacterSpawner` 보다 앞에 있어야 합니다.

[↑ 목차](#목차)

---

## 4. 네트워크 멀티플레이

### 4.1. NetworkRunnerController

`INetworkRunnerCallbacks` 를 구현한 단일 지점으로, Fusion 콜백을 C# 이벤트로 재발행해 다른 시스템이 Fusion 인터페이스를 직접 구현하지 않아도 되게 합니다.

| 이벤트 | 발행 시점 |
| --- | --- |
| `SceneLoadStart` / `SceneLoadDone` | 네트워크 씬 로드 전후 |
| `InputPolling` | `OnInput` — 입력 수집 |
| `PlayerJoined` | 플레이어 참가 |

[↑ 목차](#목차)

### 4.2. 매치메이킹과 로비

`MatchMakingManager` 가 `GameMode.AutoHostOrClient` 로 세션 참가/생성을 일원화합니다. 첫 참가자가 호스트가 되고 이후 참가자는 클라이언트로 붙습니다.

로비는 MVP 로 분리되어 있습니다.

```
MultiplayerScenePresenter
├── LobbyStateManager            로비 상태
├── MatchMakingManager           세션 참가/생성
└── View
    ├── CreateSessionView
    ├── LobbyPlayerManagerView
    ├── LobbyCharacterSlotView
    ├── LobbyCharacterReadyView
    └── MultiplayStageElementView
```

[↑ 목차](#목차)

### 4.3. 인게임 씬 구성 흐름

```
MultiplayerInGameMode
└── SubManagers (순서 = 의존 순서)
    ├── MapLoader                  스테이지 정의에 따른 맵 로드
    ├── CharacterConfigPreloader   참가자 선택 캐릭터의 Config 프리로드
    ├── PlayerCharacterSpawner     플레이어 스폰 (onBeforeSpawned 로 캐릭터명 주입)
    └── UISceneLoader              입력 UI 씬 Additive 로드
```

스폰 시점에 Config 가 이미 메모리에 있어야 하므로 프리로드가 스폰보다 앞섭니다.

[↑ 목차](#목차)

---

## 5. 플레이어 컨트롤

### 5.1. 입력과 가상 조이스틱

Fusion 의 `INetworkInput` 구조체로 이동 방향과 버튼을 함께 전송합니다.

```csharp
public struct PlayerInput : INetworkInput
{
    public Vector3 direction;
    public NetworkButtons buttons;
}

public enum EPlayerButton
{
    NormalAttack, Dodge, Expert, Ultimate, Interact, Count
}
```

가상 조이스틱은 Presenter / View 로 분리되어 있습니다 (`VirtualJoystickPresenter`, `VirtualJoystickBackgroundView`, `VirtualJoystickHandleView`).

[↑ 목차](#목차)

### 5.2. 플레이어 FSM

`PlayerFSMController` 는 `NetworkBehaviour` 로, 현재 상태를 `[Networked]` 프로퍼티로 동기화합니다. 상태 전이는 `FixedUpdateNetwork` 안에서만 일어나므로 Fusion 의 재시뮬레이션에 안전합니다.

| 상태 | 클래스 |
| --- | --- |
| Idle | `IdleState` |
| Move | `MoveState` |
| NormalAttack | `NormalAttackState` |
| Dodge | `DodgeState` |
| Expert | `ExpertState` |

```
FixedUpdateNetwork
├── TryGetInputForPlayer
├── buttons.GetPressed(PreviousButtons)   이번 틱의 신규 입력 산출
├── ResolveDesiredState(input, pressed)   원하는 상태 결정
├── ConvertState(desired)                 전이 가능하면 상태 교체
└── stateDict[CurrentState].OnUpdateState
```

이동은 Fusion KCC 를 통해 처리하며, `MaxMoveSpeed` 는 캐릭터 Config 에서 주입해 `EnvironmentProcessor.KinematicSpeed` 에 반영합니다.

[↑ 목차](#목차)

### 5.3. 애니메이션 전략

상태별 애니메이션 재생 로직은 FSM 상태와 분리해 `PlayerAnimationStrategyBase` 파생 클래스로 두었습니다.

| 전략 | 대응 |
| --- | --- |
| `LocomotionStrategy` | Idle / Move |
| `NormalAttackStrategy` | 일반 공격 콤보 |
| `DodgeStrategy` | 회피 |
| `ExpertStrategy` | 전문 스킬 |

재생 자체는 `PlayerNetworkedAnimatorController` 가 네트워크 동기화된 형태로 담당합니다.

[↑ 목차](#목차)

### 5.4. 애니메이션 타임라인 마커

애니메이션 클립의 이벤트를 베이크해 `AnimationTimeline` 으로 만들고, FSM 이 프레임이 아니라 **마커 구간**을 기준으로 판정합니다.

| 마커 | 종류 | 의미 |
| --- | --- | --- |
| `ComboInput` | Range | 다음 콤보 입력을 받는 구간 |
| `ComboDecision` | Point | 콤보 이행 여부를 확정하는 시점 |
| `Invincible` | Range | 무적 구간 |
| `Hitbox` | Range | 히트박스 활성 구간 |
| `Trigger` | Point | 임의 트리거 |

`AnimMarkerInfo.KindOf` 가 태그별 종류를 한 곳에서 정의하며, 베이크 시 검증 규칙도 여기에 맞춰 동작합니다. 마커 수신은 `AnimationMarkerReceiver` 가 담당합니다.

[↑ 목차](#목차)

---

## 6. 데이터 파이프라인

### 6.1. 뒤끝 차트 데이터

`ChartDataSOBase` 는 에디터에서 뒤끝 CDN 의 차트를 받아 ScriptableObject 로 역직렬화합니다. 파생 클래스는 `DeserializeFlattenRows` 만 구현하면 됩니다.

| SO | 내용 |
| --- | --- |
| `MultiplayStageDataSO` | 멀티플레이 스테이지 정보 |
| `PlayableCharacterInfoSO` | 플레이 가능 캐릭터 목록 |

런타임에는 네트워크 호출 없이 SO 만 읽으므로, 차트 갱신 비용이 플레이 중으로 새지 않습니다.

[↑ 목차](#목차)

### 6.2. 캐릭터 Config

캐릭터의 수치와 애니메이션 데이터는 두 개의 SO 로 나뉩니다.

| SO | 내용 |
| --- | --- |
| `CharacterCombatConfig` | 이동 속도, 상태별 애니메이션 스텝(`EAnimStepKey` → `AnimationTimeline[]`), 회피 속도 커브 |
| `CharacterStatConfig` | 캐릭터 스탯 |

`CharacterConfigRegistry` 는 캐릭터 이름으로 이 둘을 찾는 정적 보관소입니다. **로딩은 `CharacterConfigPreloader`(InGame Multiplay 어셈블리), 보관/조회는 Registry(Player Controller 어셈블리)** 로 나뉘어 있는데, 어셈블리 의존 방향이 단방향이라 플레이어 쪽에서 프리로더를 참조할 수 없기 때문입니다. 애셋 핸들 소유권은 프리로더에 있고, Registry 는 참조만 들고 있다가 씬 종료 시 비워집니다.

[↑ 목차](#목차)

### 6.3. Addressables 프리로드

`CharacterConfigPreloader` 는 세션 참가자들이 고른 캐릭터만 골라 Config 를 미리 로드합니다.

```
DoInit()
├── PlayableCharacterInfoSO 로드
├── SessionContext.UserData 에서 각 플레이어의 mainCharacterIndex 조회
├── 캐릭터별로 Data/Config/Combat/{name}, Data/Config/Stat/{name} 로드
└── CharacterConfigRegistry.Register
```

중복 선택은 걸러내고, 핸들은 리스트로 모아 `OnDestroy` 에서 일괄 해제합니다. 덕분에 `PlayerFSMController.Spawned` 는 비동기 대기 없이 동기 조회만 하면 됩니다.

> **주의**: 씬 인스펙터 설정이 사라진 것처럼 보이면 오래된 번들이 로드된 경우일 수 있습니다. Addressables 의 Play Mode Script 를 `Use Asset Database` 로 두고 확인하세요.

[↑ 목차](#목차)

---

## 7. 에디터 툴

### 7.1. Go Test Scene

로그인과 앱 초기화를 건너뛰고 곧장 개발 씬으로 진입하는 에디터 전용 스위치입니다. `GoTestSceneSettings.Enabled` 가 켜져 있으면 `AppLaunchManager` 가 초기화 파이프라인 대신 `Develop Scene` 을 로드합니다. 씬 뷰 오버레이(`GoTestSceneSceneViewOverlay`)와 전용 윈도우(`GoTestSceneWindow`)로 토글합니다.

[↑ 목차](#목차)

### 7.2. Character Combat Config Editor

`CharacterCombatConfigEditor` 는 애니메이션 스텝 그룹 편집과 클립 이벤트 → 마커 베이크를 담당합니다. 베이크 시 `AnimMarkerInfo.KindOf` 규칙에 따라 Range 마커의 시작/끝 쌍을 검증합니다.

[↑ 목차](#목차)

### 7.3. Chart Update

`Assets/Editor/Chart Update` 의 툴로 뒤끝 CDN 차트를 내려받아 대응하는 ScriptableObject 를 갱신합니다.

[↑ 목차](#목차)
