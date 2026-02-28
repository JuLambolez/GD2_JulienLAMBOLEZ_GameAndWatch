using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MinigameSequence", menuName = "WarioWare/Minigame Sequence")]
public class MinigameSequence : ScriptableObject
{
    public List<MinigameDefinition> Minigames = new();
}
