using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatrixController : MonoBehaviour
{
    [Header("---Prefabs---")]
    [SerializeField] GameObject _matrixBtnPrefab;

    [Header("---References---")]
    [SerializeField] GameManager _gm;

    [Header("---Colors---")]
    [SerializeField] public Color _attractionColor = Color.green;
    [SerializeField] public Color _repelColor = Color.red;

    private void Start()
    {
        int amount = _gm._colorPalette.Length;
        GetComponent<GridLayoutGroup>().constraintCount = amount + 1;

        for (int i = 0; i < amount + 1; i++)
        {
            for (int j = 0; j < amount + 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    CreateImageGameObject("Transparent", new Color(0, 0, 0, 0), transform);
                }
                else if (i == 0)
                {
                    CreateImageGameObject("ColTitle_" + j, _gm._colorPalette[j - 1], transform);
                }
                else if (j == 0)
                {
                    CreateImageGameObject("RowTitle_" + i, _gm._colorPalette[i - 1], transform);
                }
                else
                {
                    GameObject obj = Instantiate(_matrixBtnPrefab, transform);
                    obj.GetComponent<MatrixBtn>().Init(this.GetComponent<MatrixController>(), _gm._weights[i - 1][j - 1]);
                }
            }
        }
    }

    private void CreateImageGameObject(String name, Color color, Transform parent)
    {
        GameObject obj = new GameObject(name);
        Image img = obj.AddComponent<Image>();
        img.color = color;
        obj.transform.SetParent(parent, false);
    }
}
