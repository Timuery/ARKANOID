using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public List<EffectData> allEffects; // ����� ����� ��������� ��� ��������� �������

    public EffectData GetEffectById(int id)
    {
        return allEffects.Find(effect => effect.id == id);
    }
}

[System.Serializable]
public class EffectData
{
    public int id; // ���������� ������������� �������
    public string effectName;
    public float duration; // ������������ �������
    public float sizeMultiplier = 1; // ��������� ������� ����
    public float speedMultiplier = 1; // ��������� �������� ����
    public int damage = 1; // ����, ��������� ������
    public Sprite newTexture; // �������� ��� ������� (��������, ��� ���������)
    public bool isTemporary; // ��������� ������ ��� ����������
}
