using DG.Tweening;
using Supyrb;
using System.Linq;
using UnityEngine;

public class RewardController : MonoBehaviour
{
    private WheelView _wheel;
    private Signal<RewardModel, SectionView> _getRewardSignal;

    public GameConfig gameConfig;

    private int _reward;

    private void Start()
    {
        _getRewardSignal = Signals.Get<GetRewardSignal>();
        _getRewardSignal.AddListener(GetReward);
        _wheel = FindObjectOfType<WheelView>();
    }

    private void AnimateCurrency(int value, RectTransform startPoint,RectTransform endPoint,GameObject currencyPrefab)
    {
        var currencySpawnTransform = startPoint.position;

        var currency = Instantiate(currencyPrefab, currencySpawnTransform, Quaternion.identity);
        currency.transform.SetParent(_wheel.transform.parent);
        var currencyPosition = currency.transform.position;

        currency.transform.localScale = Vector3.zero;
        var currencyAnim = DOTween.Sequence();
        currencyAnim.Append(currency.transform.DOMove(new Vector3(currencyPosition.x + Random.Range(-100f, 100f),
            currencyPosition.y + Random.Range(-100f, 100f), 0), 0f));
        currencyAnim.Join(currency.transform.DOScale(1f, 0.4f));
        currencyAnim.AppendInterval(Random.Range(1,2.5f));
        currencyAnim.Append(currency.transform.DOMove(endPoint.position, 0.5f));
        currencyAnim.Join(currency.transform.DOScale(0, 0.5f));
        currencyAnim.OnComplete(() =>
        {
            _reward += value;
            _wheel.SetRewardText(_reward);
            Destroy(currency.gameObject);
        });
    }

    private void GetReward(RewardModel rewardModel, SectionView section)
    {
        _reward = 0;
        var rewardData = gameConfig.rewardConfig.Where(r => r.rewardType == rewardModel.rewardType).First();

        if (rewardModel.count <= 20)
        {
            for (int i = 0; i < rewardModel.count; i++)
            {
                AnimateCurrency(1, section.GetComponent<RectTransform>(), _wheel.GetComponent<RectTransform>(), rewardData.currencyPrefab);
            }
        }
        else
        {
            int baseValue = rewardModel.count / 20;
            int remainder = rewardModel.count % 20;

            for (int i = 0; i < 20; i++)
            {
                AnimateCurrency(baseValue + (i < remainder ? 1 : 0), 
                    section.GetComponent<RectTransform>(), 
                    _wheel.GetComponent<RectTransform>(), 
                    rewardData.currencyPrefab);
            }
        }
    }

    private void OnDestroy()
    {
        _getRewardSignal?.RemoveListener(GetReward);
    }
}
