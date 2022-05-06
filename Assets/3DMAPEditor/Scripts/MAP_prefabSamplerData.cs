using UnityEngine;

[CreateAssetMenu(fileName = "YuME_prefabSamplerSettings", menuName = "Tools/MAP/Utils", order = 1)]
public class MAP_prefabSamplerData : ScriptableObject
{
    public string destinationFOLDER = "Assets/";
    public string appendName = "";
    public int yPivotType;
    public string[] yPivotTypes = new string[3];
    public Texture2D configButton;
}