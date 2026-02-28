using UnityEngine;

[CreateAssetMenu(fileName = "MinigameDefinition", menuName = "WarioWare/Minigame Definition")]
public class MinigameDefinition : ScriptableObject
{
    public GameObject Prefab;
    public string Instruction;
    public float TimerDuration = 5f;
}
