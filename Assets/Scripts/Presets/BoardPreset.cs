using UnityEngine;

[CreateAssetMenu(fileName = "BoardPreset", menuName = "Presets/BoardPreset", order = 1)]
public class BoardPreset : ScriptableObject 
{
    public GameObject boardBackgroundObj;
    public GameObject boardTileObj;
    public BoardTilePreset[] boardTilePresets;
    public int width;
    public int height;
}
