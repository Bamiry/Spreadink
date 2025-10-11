using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu]
public class StagesScriptableObject : ScriptableObject
{
    /// <summary>
    /// ステージID数字 -> ステージ情報
    /// </summary>
    public Dictionary<int, StageInfo> StageDict => _stages.ToDictionary(x => x.Id, x => x);

    [SerializeField] private StageInfo[] _stages;

#if UNITY_EDITOR
    private void OnValidate()
    {
        var set = new HashSet<int>();

        foreach (var item in _stages)
        {
            if (set.Contains(item.Id))
            {
                Debug.LogWarning($"There is data with duplicate IDs: {item.Id}");
            }
            else
            {
                set.Add(item.Id);
            }
        }
    }
#endif
}