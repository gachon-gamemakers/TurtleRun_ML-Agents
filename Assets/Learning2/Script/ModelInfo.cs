using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ModelInfo : ScriptableObject
{
    [Header("Car settings")]
    public float driftFactor = 0.95f; // ���� �ٲܶ� ������ ���� ��
    public float accelerationFactor = 3f; // ������
    public float turnFactor = 3f; // ȸ����
    public float maxSpeed = 15; // �ְ�ӷ�
    public float spurt = 1; // �ʹݼӷ�
    public float Mass = 1; // ���� ����

    [TextArea(7, 10)]
    public string Infomation;
}
