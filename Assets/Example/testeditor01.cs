using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class testeditor01 : MonoBehaviour
{
    [ContextMenuItem("Reset", "ResetNum")] [ContextMenuItem("Random", "RandomNum")]
    public int testValue;

    [ColorUsage(true)] public Color colr1;

    [ColorUsage(true, true)] public Color color2;

    [Tooltip("TOOLTOPSSSS")] public int tooltips;


    [SerializeField] private string m_strtemp002;

    public string strtemo002
    {
        get => m_strtemp002;
        set => m_strtemp002 = value;
    }

    private void Start()
    {
        Debug.LogError("ds");
    }

    public void ResetNum()
    {
        testValue = 100;
    }

    public void RandomNum()
    {
        testValue = Random.Range(0, 100);
    }
}