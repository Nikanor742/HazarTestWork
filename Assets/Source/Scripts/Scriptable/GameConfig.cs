using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/GameConfig")]
sealed public class GameConfig : ScriptableObject
{
    public int minRewardValue;
    public int maxRewardValue;
    public int rewardMultiplicity;
    public float timeToStart;
    public RewardData[] rewardConfig;
    public Sprite inactiveButton;
    public Sprite activeButton;
    public Color timerTextColor;
    public Color readyTextColor;
}

[Serializable]
sealed public class RewardData
{
    public ERewardType rewardType;
    public Sprite sprite;
    public GameObject currencyPrefab;
}