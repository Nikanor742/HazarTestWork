using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/WhellConfig")]
sealed public class WhellConfig : ScriptableObject
{
    public int minRewardValue;
    public int maxRewardValue;
    public int rewardMultiplicity;
    public float timeToStart;
}