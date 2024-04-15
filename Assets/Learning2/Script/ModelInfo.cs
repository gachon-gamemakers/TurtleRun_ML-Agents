using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ModelInfo : ScriptableObject
{
    [Header("Car settings")]
    public float driftFactor = 0.95f; // 방향 바꿀때 앞으로 가던 힘
    public float accelerationFactor = 3f; // 가속힘
    public float turnFactor = 3f; // 회전힘
    public float maxSpeed = 15; // 최고속력
    public float spurt = 1; // 초반속력
    public float Mass = 1; // 차의 무게

    [TextArea(7, 10)]
    public string Infomation;
}
