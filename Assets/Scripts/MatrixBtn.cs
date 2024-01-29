using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MatrixBtn : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    private MatrixController _mc;
    private Image _img;
    private float _value = 0f;

    public void Init(MatrixController mc, float value)
    {
        // Get components
        _img = GetComponent<Image>();

        // Set references
        _value = value;
        _mc = mc;

        // Set color
        SetColorByValue(_value);
    }

    private void SetColorByValue(float value)
    {
        // Map _value from [-1, 1] to [0, 1]
        float normalizedValue = (value + 1f) / 2f;

        // Interpolate between _repelColor and _attractionColor
        _img.color = Color.Lerp(_mc._repelColor, _mc._attractionColor, normalizedValue);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // RightClick
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
            Debug.Log(name + " Game Object Right Clicked!");
        }

        // LeftClick
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log(name + " Game Object Left Clicked!");
        }
    }

    // Hover: Enter
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        ChangeOpacity(0.8f);
    }

    // Hover: Exit
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ChangeOpacity(1f);
    }

    private void ChangeOpacity(float opacity)
    {
        Color tempColor = _img.color;
        tempColor.a = opacity;
        _img.color = tempColor;
    }
}
