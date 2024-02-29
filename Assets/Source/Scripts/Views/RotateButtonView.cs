using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RotateButtonView : MonoBehaviour
{
    [HideInInspector] public Button button;

    private TextMeshProUGUI _buttonText;

    private void Awake()
    {
        button = GetComponent<Button>();
        _buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetSprite(Sprite sprite)
    {
        button.image.sprite = sprite;
    }

    public void SetText(string text)
    {
        _buttonText.text = text;
    }

    public void SetTextColor(Color color)
    {
        _buttonText.color = color;
    }
}
