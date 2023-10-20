using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioKeeper : MonoBehaviour
{
    [SerializeField] private RectTransform _other;

    [SerializeField] private RectTransform _target;

    public float WidthRatio;
    public float HeightRatio;

    [Tooltip("Width 비율을 사용")]
    public bool Width;
    [Tooltip("Height 비율을 사용")]
    public bool Height;

    [Tooltip("Width의 Min, Max를 범위로 제한할지")]
    public bool LimitedWidth;

    [Tooltip("Height의 Min, Max를 범위로 제한할지")]
    public bool LimitedHeight;

    [Tooltip("비율에 따라 결정되는 Width의 최솟값")]
    public float MinWidth = 5f;

    [Tooltip("비율에 따라 결정되는 Height의 최솟값")]
    public float MinHeight = 5f;
    
    [Tooltip("비율에 따라 결정되는 Width의 최댓값")]
    public float MaxWidth = 200f;

    [Tooltip("비율에 따라 결정되는 Height의 최댓값")] 
    public float MaxHeight = 200f;

    public bool CopyWidthToHeight;
    public bool CopyHeightToWidth;

    private void Reset()
    {
        LimitedWidth = true;
        LimitedHeight = true;
        MinWidth = 5f;
        MinHeight = 5f;
        MaxWidth = 200f;
        MaxHeight = 200f;
    }

    void Start()
    {
        _target = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Adjust();
    }

    private void Adjust()
    {
        Vector2 targetSize = _target.rect.size;
        Vector2 otherSize = _other.rect.size;

        float newWidth;
        float newHeight;

        if (Width)
        {
            newWidth = otherSize.x * WidthRatio;
        }
        else
        {
            newWidth = targetSize.x;
        }

        if (Height)
        {
            newHeight = otherSize.y * HeightRatio;
        }
        else
        {
            newHeight = targetSize.y;
        }

        if (LimitedWidth)
        {
            newWidth = Mathf.Clamp(newWidth, MinWidth, MaxWidth);
        }

        if (LimitedHeight)
        {
            newHeight = Mathf.Clamp(newHeight, MinHeight, MaxHeight);
        }

        if (CopyHeightToWidth) { newWidth = newHeight; }
        
        if (CopyWidthToHeight) { newHeight = newWidth; }

        _target.sizeDelta = new Vector2(newWidth, newHeight);
    }

    public void SetOtherReference(RectTransform rect)
    {
        _other = rect;
    }
}
