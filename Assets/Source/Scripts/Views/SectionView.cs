using TMPro;
using UnityEngine;

public class SectionView : MonoBehaviour
{
    private TextMeshProUGUI _sectionText;

    private void Awake()
    {
        _sectionText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(float value)
    {
        _sectionText.text = value.ToString();
    }
}
