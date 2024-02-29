using UnityEngine;

[CreateAssetMenu(menuName = "Config/WhellConfig")]
sealed public class WhellConfig : ScriptableObject
{
    public float timeToStart;
    public int   minRewardValue;
    public int   maxRewardValue;
    public int   rewardMultiplicity;
}