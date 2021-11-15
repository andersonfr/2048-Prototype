using UnityEngine;

[CreateAssetMenu(fileName = "BoardTilePreset", menuName = "Presets/BoardTilePreset", order = 2)]
public class BoardTilePreset : ScriptableObject
{
    public int tileValue;
    public Color color;
}
