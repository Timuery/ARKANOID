using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public List<EffectData> allEffects; // Здесь будут храниться все возможные эффекты

    public EffectData GetEffectById(int id)
    {
        return allEffects.Find(effect => effect.id == id);
    }
}

[System.Serializable]
public class EffectData
{
    public int id; // Уникальный идентификатор эффекта
    public string effectName;
    public float duration; // Длительность эффекта
    public float sizeMultiplier = 1; // Изменение размера шара
    public float speedMultiplier = 1; // Изменение скорости шара
    public int damage = 1; // Урон, наносимый шарами
    public Sprite newTexture; // Текстура для эффекта (например, для огненного)
    public bool isTemporary; // Временный эффект или постоянный
}
