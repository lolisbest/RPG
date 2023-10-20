using RPG.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.UI;


public class DamageTextDrawer : Singleton<DamageTextDrawer>
{
    public List<DamageText> PresentDamageTexts;
    public Queue<DamageText> IdleDamageTexts;

    public GameObject DamageTextPrefab;

    private readonly int _dafultNumber = 10;

    public override void Initialize()
    {
        IdleDamageTexts = new();
        PresentDamageTexts = new();

        for (int i = 0; i < _dafultNumber; i++)
        {
            DamageText damageText = CreateDamageText();
            JointIdleQue(damageText);
        }
    }

    private DamageText CreateDamageText()
    {
        GameObject obj = Instantiate(DamageTextPrefab);
        obj.transform.SetParent(transform);
        DamageText damageText = obj.GetComponent<DamageText>();
        damageText.SetDrawer();
        return damageText;
    }

    public void ShowDamageText(int damage, Vector3 worldPosition)
    {
        Vector2 canvasPosition = Camera.main.WorldToScreenPoint(worldPosition);
        Debug.Log($"{worldPosition} -> {canvasPosition}");
        DamageText damageText;
        if (IdleDamageTexts.Count == 0)
        {
            damageText = CreateDamageText();
        }
        else
        {
            damageText = IdleDamageTexts.Dequeue();
        }

        damageText.AddTextSize(GetRandomFontNoiseSize());
        damageText.Show(damage, canvasPosition + GetNoiseOffset());

        PresentDamageTexts.Add(damageText);
    }

    private Vector2 GetNoiseOffset()
    {
        float offsetX = Random.Range(0f, 10f) * 10f;
        float offsetY = Random.Range(0f, 10f) * 10f;
        return new Vector2(offsetX, offsetY);
    }

    private float GetRandomFontNoiseSize()
    {
        return Random.Range(0f, 4f);
    }

    public void JointIdleQue(DamageText damageText)
    {
        IdleDamageTexts.Enqueue(damageText);
    }

    public void RemoveFromPresents(DamageText damageText)
    {
        PresentDamageTexts.Remove(damageText);
    }
}
