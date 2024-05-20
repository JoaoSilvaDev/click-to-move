using UnityEngine;

[CreateAssetMenu(menuName = "DEMO/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] private uint id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;

    public uint ID { get { return id; } }
    public string DisplayName { get { return displayName; } }
    public Sprite Icon { get { return icon; } }
}