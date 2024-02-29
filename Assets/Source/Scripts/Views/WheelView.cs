using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private Transform       _wheel;
    [SerializeField] private Transform       _arrow;
    [SerializeField] private Image           _rewardImage;
    

    public event Action OnWheelStopped;

    private void Awake()
    {
        SetRewardTextState(false);
    }

    public void SetRewardImage(Sprite sprite)
    {
        _rewardImage.sprite = sprite;
    }

    public void RotateWheel(float finalAngle, float totalTime)
    {
        float endAngle = 360 * 2;

        float myFloat = 0;
        DOTween.To(() => myFloat, x => myFloat = x, endAngle + finalAngle, totalTime)
            .OnUpdate(()=> 
        {
            _wheel.localEulerAngles = new Vector3(0, 0, myFloat);
        })
            .OnComplete(()=> 
        {
            OnWheelStopped?.Invoke();
            SetRewardTextState(true);
        });
    }

    public void SetRewardImageState(bool state)
    {
        _rewardImage.gameObject.SetActive(state);
    }
    public void SetRewardTextState(bool state)
    {
        _rewardText.gameObject.SetActive(state);
    }

    public void SetRewardText(int reward)
    {
        _rewardText.text = reward.ToString();
    }
}
