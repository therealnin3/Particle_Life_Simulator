using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MatrixBtn : MonoBehaviour, IPointerClickHandler//, IPointerEnterHandler, IPointerExitHandler
{
    private MatrixController _mc;
    private Image _img;
    public RelationshipSquare _relationshipSquare;

    public void Init(MatrixController mc, RelationshipSquare relationshipSquare)
    {
        _mc = mc;
        _img = GetComponent<Image>();
        _relationshipSquare = relationshipSquare;

        // Set color
        _img.color = GetColorByValue(_mc._repelColor, _mc._attractionColor, _relationshipSquare._weight);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // LeftClick - Add attraction
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Left click");
            ChangeColor(0);
        }

        // RightClick - Add repulsion
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right click");
            ChangeColor(1);
        }
    }

    private void ChangeColor(int clickType)
    {
        switch (clickType)
        {
            case 0:
                // LeftClick - Add attraction
                _relationshipSquare.ChangeWeight(ClampWeight(_relationshipSquare._weight + _mc._editValue));
                break;
            case 1:
                // RightClick - Add repulsion
                _relationshipSquare.ChangeWeight(ClampWeight(_relationshipSquare._weight - _mc._editValue));
                break;
            default:
                Debug.Log("Invalid clickType");
                break;
        }

        // Change color after changing weight
        _img.color = GetColorByValue(_mc._repelColor, _mc._attractionColor, _relationshipSquare._weight);

        // Testing
        _relationshipSquare.Test();
    }

    private float ClampWeight(float weight)
    {
        if (weight > 1)
        {
            return 1;
        }
        else if (weight < -1)
        {
            return -1;
        }
        else
        {
            return weight;
        }
    }

    private Color GetColorByValue(Color c1, Color c2, float value)
    {
        // Map _value from [-1, 1] to [0, 1]
        float normalizedValue = (value + 1f) / 2f;

        // Interpolate between _repelColor and _attractionColor
        return Color.Lerp(c1, c2, normalizedValue);
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
