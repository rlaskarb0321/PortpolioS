using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터 이름으로 Combat/Stat Config 를 찾는 보관소.
///
/// 채우는 쪽은 씬 로딩 단계의 CharacterConfigPreloader(InGameMultiplay 어셈블리),
/// 읽는 쪽은 스폰된 플레이어 컴포넌트다. 어셈블리 의존 방향이
/// InGameMultiplay → PlayerController 이므로 플레이어 쪽이 프리로더를 직접
/// 참조할 수 없다. 그래서 "보관"만 이쪽에 두고 "로딩"은 저쪽에 남긴다.
///
/// 수명은 인게임 씬 스코프다 — 프리로더가 OnDestroy 에서 Clear 한다.
/// 여기서는 애셋 핸들을 소유하지 않는다(참조만 들고 있다). 해제 책임은 프리로더에 있다.
/// </summary>
public static class CharacterConfigRegistry
{
    private static readonly Dictionary<string, CharacterCombatConfig> combatConfigs = new Dictionary<string, CharacterCombatConfig>();
    private static readonly Dictionary<string, CharacterStatConfig> statConfigs = new Dictionary<string, CharacterStatConfig>();

    public static int Count => combatConfigs.Count;

    public static void Register(string characterName, CharacterCombatConfig combatConfig, CharacterStatConfig statConfig)
    {
        if (combatConfig != null)
            combatConfigs[characterName] = combatConfig;

        if (statConfig != null)
            statConfigs[characterName] = statConfig;
    }

    public static bool Contains(string characterName)
    {
        return combatConfigs.ContainsKey(characterName);
    }

    /// <summary>스폰 이후 동기 조회. 프리로드가 끝난 뒤에만 호출된다.</summary>
    public static CharacterCombatConfig GetCombatConfig(string characterName)
    {
        if (combatConfigs.TryGetValue(characterName, out CharacterCombatConfig config) == true)
            return config;

        Debug.LogError($"[CharacterConfigRegistry] CombatConfig 미등록: {characterName} " +
                       $"(SubManager 순서에서 CharacterConfigPreloader 가 PlayerCharacterSpawner 보다 앞에 있는지 확인할 것)");
        return null;
    }

    /// <summary>스폰 이후 동기 조회. 프리로드가 끝난 뒤에만 호출된다.</summary>
    public static CharacterStatConfig GetStatConfig(string characterName)
    {
        if (statConfigs.TryGetValue(characterName, out CharacterStatConfig config) == true)
            return config;

        Debug.LogError($"[CharacterConfigRegistry] StatConfig 미등록: {characterName} " +
                       $"(SubManager 순서에서 CharacterConfigPreloader 가 PlayerCharacterSpawner 보다 앞에 있는지 확인할 것)");
        return null;
    }

    public static void Clear()
    {
        combatConfigs.Clear();
        statConfigs.Clear();
    }
}
