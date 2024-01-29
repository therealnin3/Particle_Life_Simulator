using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatrixController : MonoBehaviour
{
    [Header("---Prefabs---")]
    [SerializeField] private GameObject _matrixBtnPrefab;

    [Header("---References---")]
    [SerializeField] GameManager _gm;

    [Header("---Colors---")]
    [SerializeField] public Color _attractionColor = Color.green;
    [SerializeField] public Color _repelColor = Color.red;

    private GridLayoutGroup _glg;

    private void Start()
    {
        _glg = GetComponent<GridLayoutGroup>();
        _glg.constraintCount = _gm._colorPalette.Length;
        SpawnMatrixBtns();
    }

    private void SpawnMatrixBtns()
    {
        int amount = _gm._colorPalette.Length;

        for (int i = 0; i < amount; i++)
        {
            for (int j = 0; j < amount; j++)
            {
                GameObject btn = Instantiate(_matrixBtnPrefab, Vector3.zero, Quaternion.identity);

                Debug.Log(_gm);
                Debug.Log(_gm._weights[i][j]);

                btn.GetComponent<MatrixBtn>().Init(this, _gm._weights[i][j]);
                btn.transform.SetParent(transform, false);
            }
        }
    }
}
