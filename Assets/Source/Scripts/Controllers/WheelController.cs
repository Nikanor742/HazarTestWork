using Cysharp.Threading.Tasks;
using Supyrb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class WheelController : MonoBehaviour
{
    [Inject] private GameConfig  _gameConfig;
    [Inject] private WhellConfig _wheelConfig;
    [Inject] private WheelView   _wheel;

    private List<SectionModel> _sectionModels;
    private SectionView[]      _sections;

    private RotateButtonView _rotateButton;
    private RewardModel      _rewardModel;

    private float _currentTime;
    private int   _currentRewardIndex;

    private Signal<RewardModel,SectionView> _getRewardSignal;


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
        _rotateButton.SetTextColor(_gameConfig.timerTextColor);
        _rotateButton.button.interactable = false;

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
        _rotateButton.SetTextColor(_gameConfig.readyTextColor);
        _rotateButton.SetSprite(_gameConfig.activeButton);
        _rotateButton.button.interactable = true;
        _rotateButton.SetText("Испытать удачу");
    }

    private void OnFortuneWheelStopped()
    {
        _wheel.SetRewardImageState(false);
        _wheel.SetRewardText(0);
        _getRewardSignal?.Dispatch(_rewardModel, _sections[_currentRewardIndex]);
        StartGame(4000);
    }

    private void OnRotateButtonClick()
    {
        if (_currentTime >= _wheelConfig.timeToStart)
        {
            _currentTime = 0;
            _rotateButton.SetSprite(_gameConfig.inactiveButton);
            _rotateButton.button.interactable = false;

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
        _wheel.SetRewardImage(_gameConfig.rewardConfig.
            Where(r => r.rewardType == _rewardModel.rewardType).First().sprite);

        
        var randomValues = GenerateUniqueRandomNumbers(
            _sectionModels.Count, _wheelConfig.minRewardValue, _wheelConfig.maxRewardValue, _wheelConfig.rewardMultiplicity);
        
        for (int i = 0; i < randomValues.Count; i++)
        {
            _sectionModels[i].value = randomValues[i];
            _sections[i].SetText(randomValues[i]);
        }

    }

    private IEnumerator Timer()
    {
        _rotateButton.SetText((_wheelConfig.timeToStart - _currentTime).ToString());
        yield return new WaitForSeconds(1f);
        _currentTime += 1f;
        if (_currentTime >= _wheelConfig.timeToStart)
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
