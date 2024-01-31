using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MatrixBtn : MonoBehaviour, IPointerEnterHandler, IScrollHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{


    private MatrixController _mc;
    private Image _img;
    public RelationshipSquare _relationshipSquare;

    // Animation
    private float _animationTime = 0.05f;
    private float _animationScale = 1.05f;

    public void Init(MatrixController mc, RelationshipSquare relationshipSquare)
    {
        _mc = mc;
        _img = GetComponent<Image>();
        _relationshipSquare = relationshipSquare;

        // Set color
        _img.color = GetColorByValue(_mc._repelColor, _mc._attractionColor, _relationshipSquare._weight);
    }

    // Scroll events
    public void OnScroll(PointerEventData eventData)
    {
        if (eventData.scrollDelta.y > 0)
        {
            ChangeColor(0);
            ClickAnimation();
        }
        else if (eventData.scrollDelta.y < 0)
        {
            ChangeColor(1);
            ClickAnimation();
        }
    }

    // Hold down event
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ClickDownAnimation();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClickDownAnimation();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            ClickDownAnimation();
        }
    }

    // Hold release event
    public void OnPointerUp(PointerEventData eventData)
    {
        // LeftClick - Add attraction
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ChangeColor(0);
            ClickReleaseAnimation();
        }

        // RightClick - Add repulsion
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ChangeColor(1);
            ClickReleaseAnimation();
        }

        // MiddleScrollClick - Reset
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            ChangeColor(2);
            ClickReleaseAnimation();
        }
    }

    private void ChangeColor(int clickType)
    {
        switch (clickType)
        {
            case 0:
                // LeftClick/ScrollUp - Add attraction
                _relationshipSquare.ChangeWeight(ClampWeight(_relationshipSquare._weight + _mc._editValue));
                break;
            case 1:
                // RightClick/ScrollDown - Add repulsion
                _relationshipSquare.ChangeWeight(ClampWeight(_relationshipSquare._weight - _mc._editValue));
                break;
            case 2:
                // MiddleScrollClick - Reset
                _relationshipSquare.ChangeWeight(0);
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

    private void ClickAnimation()
    {
        LeanTween
          .scale(gameObject, new Vector3(1f, 1f, 1f), _animationTime / 2)
          .setEase(LeanTweenType.easeOutSine)
          .setOnComplete(ClickReleaseAnimation);
    }

    private void ClickDownAnimation()
    {
        LeanTween
            .scale(gameObject, new Vector3(1f, 1f, 1f), _animationTime / 2)
            .setEase(LeanTweenType.easeOutSine);
    }

    private void ClickReleaseAnimation()
    {
        LeanTween
            .scale(gameObject, new Vector3(_animationScale, _animationScale, _animationScale), _animationTime / 2)
            .setEase(LeanTweenType.easeOutSine);
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
        LeanTween.scale(gameObject, new Vector3(_animationScale, _animationScale, _animationScale), _animationTime);
    }

    // Hover: Exit
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        LeanTween.scale(gameObject, Vector3.one, _animationTime);
    }
}
