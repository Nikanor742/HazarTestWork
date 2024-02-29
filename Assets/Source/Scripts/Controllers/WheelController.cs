using Cysharp.Threading.Tasks;
using Supyrb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public GameConfig gameConfig;
    public WhellConfig wheelConfig;

    private WheelView _wheel;
    private RotateButtonView _rotateButton;
    private SectionView[] _sections;

    private float _currentTime;

    private List<SectionModel> _sectionModels;
    private RewardModel _rewardModel;

    private int _currentRewardIndex;

    private Signal<RewardModel, SectionView> _getRewardSignal;


    private void Start()
    {
        _getRewardSignal = Signals.Get<GetRewardSignal>();
        _rewardModel = new RewardModel();
        _wheel = FindObjectOfType<WheelView>();
        _wheel.OnWheelStopped += OnFortuneWheelStopped;
        _sections = _wheel.GetComponentsInChildren<SectionView>();
        CreateModels();
        _rotateButton = _wheel.GetComponentInChildren<RotateButtonView>();
        _rotateButton.button.onClick.AddListener(OnRotateButtonClick);
        StartGame(0);
    }

    private async void StartGame(int delay)
    {
        await UniTask.Delay(delay);
        _rotateButton.SetTextColor(gameConfig.timerTextColor);
        GenerateRandomWheelData();
        _wheel.SetRewardImageState(true);
        _wheel.SetRewardTextState(false);
        StartCoroutine(Timer());
    }

    private void CreateModels()
    {
        _sectionModels = new List<SectionModel>();
        foreach (var item in _sections)
        {
            _sectionModels.Add(new SectionModel());
        }
    }

    private void FortuneWhellReady()
    {
        _rotateButton.SetTextColor(gameConfig.readyTextColor);
        _rotateButton.SetSprite(gameConfig.activeButton);
        _rotateButton.SetText("Испытать удачу");
    }

    private void OnFortuneWheelStopped()
    {
        _wheel.SetRewardImageState(false);
        _wheel.SetRewardText(0);
        _getRewardSignal?.Dispatch(_rewardModel, _sections[_currentRewardIndex]);
        StartGame(5000);
    }

    private void OnRotateButtonClick()
    {
        if (_currentTime >= wheelConfig.timeToStart)
        {
            _currentTime = 0;
            _rotateButton.SetSprite(gameConfig.inactiveButton);
            var randomIndex = UnityEngine.Random.Range(0, _sectionModels.Count);
            _currentRewardIndex = randomIndex;
            _rewardModel.count = _sectionModels[_currentRewardIndex].value;
            Debug.Log("Result:" + _sectionModels[randomIndex].value);

            var rotationZ = 15f + (30f * randomIndex);
            _wheel.RotateWheel(rotationZ, 5);
        }
    }

    public static List<int> GenerateUniqueRandomNumbers(int n, int a, int b, int Z)
    {
        var numbers = new List<int>();

        var candidates = new List<int>();
        for (int i = a; i <= b; i++)
        {
            if (i % Z == 0)
            {
                candidates.Add(i);
            }
        }
        for (int i = 0; i < n; i++)
        {
            var index = UnityEngine.Random.Range(0, candidates.Count);
            numbers.Add(candidates[index]);
            candidates.RemoveAt(index);
        }

        return numbers;
    }

    private void GenerateRandomWheelData()
    {
        var randomReward = _rewardModel.rewardType;
        var values = (ERewardType[])Enum.GetValues(typeof(ERewardType));
        while (randomReward == _rewardModel.rewardType)
        {
            randomReward = values[UnityEngine.Random.Range(0, values.Length)];
        }
        _rewardModel.rewardType = randomReward;
        _wheel.SetRewardImage(gameConfig.rewardConfig.
            Where(r => r.rewardType == _rewardModel.rewardType).First().sprite);

        
        var randomValues = GenerateUniqueRandomNumbers(
            _sectionModels.Count, wheelConfig.minRewardValue, wheelConfig.maxRewardValue, wheelConfig.rewardMultiplicity);
        
        for (int i = 0; i < randomValues.Count; i++)
        {
            _sectionModels[i].value = randomValues[i];
            _sections[i].SetText(randomValues[i]);
        }

    }

    private IEnumerator Timer()
    {
        _rotateButton.SetText((wheelConfig.timeToStart - _currentTime).ToString());
        yield return new WaitForSeconds(1f);
        _currentTime += 1f;
        if (_currentTime >= wheelConfig.timeToStart)
        {
            FortuneWhellReady();
        }
        else
        {
            StartCoroutine(Timer());
        }
        GenerateRandomWheelData();
    }
    private void OnDestroy()
    {
        _rotateButton.button.onClick.RemoveAllListeners();
        _wheel.OnWheelStopped -= OnFortuneWheelStopped;
    }
}
