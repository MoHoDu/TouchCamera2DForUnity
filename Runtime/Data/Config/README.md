# Camera Behavior Config System

카메라 동작을 유연하게 설정할 수 있는 ScriptableObject 기반의 설정 시스템입니다.

## 목차

- [개요](#개요)
- [주요 특징](#주요-특징)
- [아키텍처](#아키텍처)
- [사용 방법](#사용-방법)
- [에디터 사용 가이드](#에디터-사용-가이드)
- [런타임 사용 가이드](#런타임-사용-가이드)
- [수정 내역](#수정-내역)

---

## 개요

이 시스템은 카메라 입력, 동작, 영역 등을 **폴리모픽하게** 설정할 수 있도록 설계되었습니다.
Unity 인스펙터에서 드롭다운으로 타입을 선택하고, 각 타입별 설정값을 인라인으로 저장합니다.

### 핵심 구조

```
CameraBehaviorProfile (ScriptableObject)
  └── List<CameraActionUnit>
      ├── name, order, enabled
      ├── InputConfigBase (SerializeReference) - 입력 방식
      ├── AreaConfigBase (SerializeReference) - 영역 설정
      ├── SectionConfigBase (SerializeReference) - 섹션 분할
      ├── ActionConfigBase (SerializeReference) - 메인 동작
      └── PostActionConfigBase (SerializeReference) - 후처리 동작
```

---

## 주요 특징

### 1. 폴리모픽 설정 (SerializeReference)

각 Config는 `[SerializeReference]`를 사용하여 **인라인으로 저장**됩니다.
- ✅ 별도의 ScriptableObject 에셋 파일 생성 불필요
- ✅ 모든 설정이 하나의 Profile 안에 저장
- ✅ 인스펙터에서 드롭다운으로 타입 선택 가능

### 2. 중첩된 폴리모픽 필드 지원

`ZoomActionConfig` 같은 경우, 내부에 또 다른 `[SerializeReference]` 필드(`zoomSettings`)를 가질 수 있습니다.
- ✅ 재귀적으로 중첩된 설정 지원
- ✅ 에디터가 자동으로 감지하여 드롭다운 렌더링

### 3. 커스텀 에디터

`CameraBehaviorProfileEditor`를 통해:
- ✅ 드롭다운으로 타입 선택
- ✅ 타입별 설정 필드 자동 표시
- ✅ Add/Remove Action Unit 버튼
- ✅ Validate All 버튼으로 유효성 검사

### 4. 성능 최적화

- ✅ 타입 스캔 결과 캐싱 (어셈블리 스캔은 최초 1회만)
- ✅ 인스펙터 렌더링 최적화

---

## 아키텍처

### 계층 구조

```
ConfigBase (Serializable, IValidatable)
  ├── InputConfigBase (IInputConfig)
  │   ├── MouseDragConfig
  │   └── MouseWheelConfig
  │
  ├── AreaConfigBase (IAreaConfig)
  │   └── (구현 클래스들)
  │
  ├── SectionConfigBase (ISectionConfig)
  │   └── (구현 클래스들)
  │
  ├── ActionConfigBase (IActionConfig)
  │   ├── MoveActionConfig
  │   └── ZoomActionConfig
  │       └── [SerializeReference] ZoomActionConfigBase (IZoomActionConfig)
  │           ├── ContinuousZoomActionConfig
  │           ├── StepZoomActionConfig
  │           └── FixedStepZoomActionConfig
  │
  └── PostActionConfigBase (IPostActionConfig)
      └── (구현 클래스들)
```

### ConfigBase 설계

```csharp
[Serializable]
public abstract class ConfigBase : IValidatable
{
    [SerializeField] private string displayName;
    [SerializeField] private bool enabled = true;

    public string DisplayName => string.IsNullOrEmpty(displayName) ? displayName : displayName;
    public bool Enabled => enabled;

    public virtual void Validate(List<string> warnings) { }
}
```

**중요**: ConfigBase는 **일반 Serializable 클래스**입니다.
- ❌ ~~ScriptableObject 상속 안 함~~
- ✅ `[SerializeReference]`와 함께 사용 가능
- ✅ 인라인 직렬화

---

## 사용 방법

### 1. Profile 생성

1. Project 창에서 우클릭
2. `Create > Camera > Behavior Profile`
3. 생성된 Profile을 선택

### 2. Action Unit 추가

1. Inspector에서 `+ Add New Action Unit` 버튼 클릭
2. 자동으로 기본값이 설정됨:
   - **Input Config**: 첫 번째 구현 타입으로 자동 초기화 (필수)
   - **Main Action**: 첫 번째 구현 타입으로 자동 초기화 (필수)
   - **Area Config, Section Config, Post Action**: (None) 으로 초기화 (선택)

### 3. 설정값 입력

각 필드의 드롭다운에서 원하는 타입을 선택하면:
- 해당 타입의 설정 필드들이 자동으로 표시됩니다
- 값을 입력하면 인라인으로 저장됩니다

### 4. 중첩된 설정 (예: Zoom)

1. Main Action에서 `ZoomActionConfig` 선택
2. `zoomMultiplier`, `type` 등의 일반 필드 설정
3. **Zoom Settings** 드롭다운이 자동으로 나타남
4. 원하는 Zoom Settings 타입 선택 (ContinuousZoom, StepZoom 등)
5. 해당 타입의 세부 설정 입력

---

## 에디터 사용 가이드

### CameraBehaviorProfileEditor 주요 기능

#### 1. Action Unit 관리

```
+ Add New Action Unit    // 새 유닛 추가 (기본값 자동 설정)
🗑 Remove Action Unit    // 유닛 삭제
```

#### 2. 필드 타입

| 필드 | 필수 여부 | 설명 |
|------|----------|------|
| Input Config | ✅ 필수 | 입력 방식 (None 선택 불가) |
| Area Config | ⬜ 선택 | 영역 설정 (None 선택 가능) |
| Section Config | ⬜ 선택 | 섹션 분할 (None 선택 가능) |
| Main Action | ✅ 필수 | 메인 동작 (None 선택 불가) |
| Post Action | ⬜ 선택 | 후처리 동작 (None 선택 가능) |

#### 3. 유효성 검사

```
✅ Validate All    // 모든 설정 유효성 검사
```

검사 항목:
- 필수 필드 null 체크
- 값 범위 검사 (예: minDelta >= 0)
- 글로벌 정책 검사 (예: globalMinDelta 비교)

#### 4. 폴리모픽 필드 렌더링

에디터는 자동으로:
1. `[SerializeReference]` 필드를 감지
2. 리플렉션으로 인터페이스 타입 추론
3. 해당 인터페이스를 구현하는 모든 타입을 드롭다운에 표시
4. 타입 선택 시 해당 타입의 필드들을 자동 렌더링

---

## 런타임 사용 가이드

### 기본 사용법

```csharp
using UnityEngine;
using CameraBehavior.Configs;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CameraBehaviorProfile profile;

    void Start()
    {
        // 모든 활성화된 액션 유닛 순회
        foreach (var unit in profile.actions)
        {
            if (!unit.enabled) continue;

            Debug.Log($"Unit: {unit.name}, Order: {unit.order}");

            // 입력 설정 확인
            if (unit.input != null)
            {
                Debug.Log($"Input: {unit.input.GetType().Name}");
            }

            // 메인 액션 확인
            if (unit.action != null)
            {
                Debug.Log($"Action: {unit.action.GetType().Name}");
            }
        }
    }
}
```

### 타입별 처리

```csharp
using CameraBehavior.Configs.Input;
using CameraBehavior.Configs.Action;

void ProcessActions()
{
    foreach (var unit in profile.actions)
    {
        if (!unit.enabled) continue;

        // 입력 타입별 처리
        if (unit.input is MouseDragConfig dragConfig)
        {
            ProcessMouseDrag(dragConfig);
        }
        else if (unit.input is MouseWheelConfig wheelConfig)
        {
            ProcessMouseWheel(wheelConfig);
        }

        // 액션 타입별 처리
        if (unit.action is ZoomActionConfig zoomConfig)
        {
            ProcessZoom(zoomConfig);
        }
        else if (unit.action is MoveActionConfig moveConfig)
        {
            ProcessMove(moveConfig);
        }
    }
}
```

### 중첩된 설정 접근

```csharp
void ProcessZoom(ZoomActionConfig zoomConfig)
{
    float multiplier = zoomConfig.zoomMultiplier;
    ZoomType type = zoomConfig.type;

    // 중첩된 zoomSettings 접근
    if (zoomConfig.zoomSettings is ContinuousZoomActionConfig continuous)
    {
        // ContinuousZoom 전용 설정 사용
    }
    else if (zoomConfig.zoomSettings is StepZoomActionConfig step)
    {
        // StepZoom 전용 설정 사용
    }
}
```

### Order 기반 정렬

```csharp
using System.Linq;

void Awake()
{
    var sortedActions = profile.actions
        .Where(unit => unit.enabled)
        .OrderBy(unit => unit.order)
        .ToList();

    // Order 순서대로 실행
    foreach (var unit in sortedActions)
    {
        ProcessActionUnit(unit);
    }
}
```

### 리소스 로드

```csharp
void LoadProfile()
{
    // Resources 폴더에서 로드
    var profile = Resources.Load<CameraBehaviorProfile>("CameraConfig/DefaultProfile");

    if (profile == null)
    {
        Debug.LogError("Profile not found!");
        return;
    }

    // 유효성 검사
    var warnings = profile.ValidateAll();
    if (warnings.Count > 0)
    {
        foreach (var warning in warnings)
            Debug.LogWarning(warning);
    }
}
```

---

## 수정 내역

### 주요 수정 사항

#### 1. ConfigBase를 일반 클래스로 변경

**문제**: `ConfigBase`가 `ScriptableObject`를 상속하여 `[SerializeReference]`와 충돌

**해결**:
```csharp
// Before
public abstract class ConfigBase : ScriptableObject, IValidatable

// After
[Serializable]
public abstract class ConfigBase : IValidatable
```

- `ScriptableObject` 상속 제거
- `[Serializable]` 속성 추가
- 인라인 직렬화 가능하도록 변경

#### 2. CameraBehaviorProfileEditor 성능 최적화

**문제**: `GetImplementationsOf()` 메서드가 매 프레임마다 전체 어셈블리를 스캔하여 심각한 렌더링 렉 발생

**해결**:
```csharp
// 타입 스캔 결과 캐싱
private static Dictionary<Type, List<Type>> implementationCache = new();

private List<Type> GetImplementationsOf(Type interfaceType)
{
    // 캐시에서 먼저 확인
    if (implementationCache.TryGetValue(interfaceType, out var cachedTypes))
        return cachedTypes;

    // 캐시에 없으면 스캔 후 저장
    var types = /* 어셈블리 스캔 */;
    implementationCache[interfaceType] = types;
    return types;
}
```

- 최초 1회만 어셈블리 스캔
- 이후 캐시에서 즉시 반환
- 성능 크게 개선

#### 3. 중첩된 폴리모픽 필드 자동 렌더링

**문제**: `ZoomActionConfig`의 `zoomSettings` 필드가 드롭다운으로 표시되지 않음

**해결**:
```csharp
// 일반 필드와 SerializeReference 필드를 분리하여 렌더링
if (fieldProp.managedReferenceValue != null)
{
    // 일반 필드들 먼저 표시
    DrawSerializedFields(fieldProp);

    // 중첩된 SerializeReference 필드들을 재귀적으로 처리
    DrawNestedPolymorphicFields(fieldProp);
}
```

- `DrawSerializedFields()`: 일반 필드만 표시
- `DrawNestedPolymorphicFields()`: SerializeReference 필드를 재귀적으로 드롭다운 렌더링
- `GetFieldInterfaceType()`: 리플렉션으로 인터페이스 타입 자동 추론

#### 4. Add Unit 시 필수 필드 자동 초기화

**문제**: Add New Action Unit 버튼 클릭 시 모든 필드가 null로 초기화되어 설정값이 보이지 않음

**해결**:
```csharp
// 필수 필드는 기본 타입으로 자동 초기화
var inputTypes = GetImplementationsOf(typeof(IInputConfig));
if (inputTypes.Count > 0)
    newElement.FindPropertyRelative("input").managedReferenceValue =
        Activator.CreateInstance(inputTypes[0]);

var actionTypes = GetImplementationsOf(typeof(IActionConfig));
if (actionTypes.Count > 0)
    newElement.FindPropertyRelative("action").managedReferenceValue =
        Activator.CreateInstance(actionTypes[0]);
```

- 필수 필드(Input Config, Main Action): 첫 번째 구현 타입으로 자동 생성
- 선택 필드(Area, Section, Post Action): null로 초기화

#### 5. allowNull 파라미터 적용

**문제**: 모든 필드에서 (None)을 선택할 수 있어 필수 필드를 비울 수 있음

**해결**:
```csharp
DrawPolymorphicField(element, "input", "Input Config", typeof(IInputConfig), allowNull: false);
DrawPolymorphicField(element, "area", "Area Config", typeof(IAreaConfig), allowNull: true);
DrawPolymorphicField(element, "section", "Section Config", typeof(ISectionConfig), allowNull: true);
DrawPolymorphicField(element, "action", "Main Action", typeof(IActionConfig), allowNull: false);
DrawPolymorphicField(element, "postAction", "Post Action", typeof(IPostActionConfig), allowNull: true);
```

- `allowNull: false`: 필수 필드, (None) 옵션 없음
- `allowNull: true`: 선택 필드, (None) 옵션 표시

#### 6. 인덱스 범위 에러 수정

**문제**: 드롭다운 사용 시 `IndexOutOfRangeException` 발생

**해결**:
```csharp
// 안전한 인덱스 계산
int foundIndex = displayNames.IndexOf(currentTypeName);
currentIndex = foundIndex >= 0 ? foundIndex : 0;

// 안전한 타입 선택
int typeIndex = allowNull ? newIndex - 1 : newIndex;
if (typeIndex >= 0 && typeIndex < allTypes.Count)
{
    var selectedType = allTypes[typeIndex];
    fieldProp.managedReferenceValue = Activator.CreateInstance(selectedType);
}
```

- `IndexOf`가 -1을 반환할 경우 대비
- 배열 접근 전 범위 체크

---

## 확장 가이드

### 새로운 Config 타입 추가

1. **인터페이스 정의** (예: `INewConfig.cs`)
```csharp
public interface INewConfig
{
    // 공통 속성 정의
}
```

2. **베이스 클래스 생성** (예: `NewConfigBase.cs`)
```csharp
[Serializable]
public abstract class NewConfigBase : ConfigBase, INewConfig
{
    public override void Validate(List<string> warnings)
    {
        base.Validate(warnings);
        // 추가 검증 로직
    }
}
```

3. **구체 클래스 구현** (예: `ConcreteNewConfig.cs`)
```csharp
[Serializable]
public class ConcreteNewConfig : NewConfigBase
{
    [SerializeField] private float someValue;

    public override void Validate(List<string> warnings)
    {
        base.Validate(warnings);
        if (someValue < 0)
            warnings.Add($"{DisplayName}: someValue must be positive");
    }
}
```

4. **CameraActionUnit에 필드 추가**
```csharp
[SerializeReference] public NewConfigBase newConfig;
```

5. **에디터에서 렌더링 추가**
```csharp
DrawPolymorphicField(element, "newConfig", "New Config", typeof(INewConfig), allowNull: true);
```

에디터가 자동으로:
- 인터페이스 타입 감지
- 구현 타입들을 드롭다운에 표시
- 선택된 타입의 필드들을 렌더링

### 중첩된 폴리모픽 필드 추가

기존 Config 내부에 `[SerializeReference]` 필드를 추가하면 자동으로 처리됩니다:

```csharp
[Serializable]
public class MyActionConfig : ActionConfigBase
{
    [SerializeField] private float baseValue;

    // 중첩된 폴리모픽 필드
    [SerializeReference] public ISubConfig subConfig;
}
```

에디터가 자동으로:
- `subConfig` 필드를 감지
- 리플렉션으로 `ISubConfig` 타입 추론
- 드롭다운 렌더링

---

## 트러블슈팅

### Q: 드롭다운에 타입이 나타나지 않아요

**A**: 다음을 확인하세요:
1. 해당 타입이 추상 클래스가 아닌 구체 클래스인가?
2. 올바른 인터페이스를 구현하고 있나?
3. `[Serializable]` 속성이 있나?

### Q: 설정값이 저장되지 않아요

**A**: `serializedObject.ApplyModifiedProperties()`가 호출되는지 확인하세요.

### Q: 인스펙터가 느려요

**A**: 타입 캐싱이 제대로 작동하는지 확인하세요. 에디터를 재시작하면 캐시가 초기화됩니다.

### Q: 중첩된 필드가 표시되지 않아요

**A**: 부모 Config가 null이 아닌지, `[SerializeReference]`가 제대로 선언되었는지 확인하세요.

---

## 라이센스

이 프로젝트는 Camera Input System의 일부입니다.

## 작성자

- 최초 작성: 2025
- 마지막 수정: 2025-01-XX
