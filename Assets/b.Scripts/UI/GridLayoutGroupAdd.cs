using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridLayoutGroupAdd : MonoBehaviour
{
    private GridLayoutGroup _gridLayoutGroup;
    private RectTransform _rect;

    public int Column;
    //public bool IsColumnFixed;

    void Awake()
    {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        _rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        //Vector2 size = _rect.anchoredPosition;
        //Debug.Log("size: " + _rect.anchoredPosition);
        //Debug.Log("size: " + _rect.rect.width);
        //Debug.Log("size: " + _rect.rect.height);
        ////Debug.Log("size: " + _rect.);
        //Debug.Log("size: " + _rect.anchoredPosition);
        //Debug.Log("size: " + _rect.anchoredPosition);

        //RectOffset padding = _gridLayoutGroup.padding;
        //Debug.Log("padding: " + padding);

        //Vector2 spacing = _gridLayoutGroup.spacing;
        //Debug.Log("spacing: " + spacing);

        //float spacingRow = spacing.x * (Column - 1);
        //Debug.Log("spacingRow: " + spacingRow);

        //float inner = _rect.rect.width - padding.left - padding.right - spacingRow;
        //Debug.Log("inner: " + inner);

        //float cellSize = inner / Column;
        //Debug.Log("cellSize: " + cellSize);

        //_gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }


    private void FixedUpdate()
    {
        RectOffset padding = _gridLayoutGroup.padding;

        Vector2 spacing = _gridLayoutGroup.spacing;

        float spacingRow = spacing.x * (Column - 1);

        float inner = _rect.rect.width - padding.left - padding.right - spacingRow;

        float cellSize = inner / Column;

        _gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }
}
