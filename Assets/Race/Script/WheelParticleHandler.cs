using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamandol.Race
{
    public class WheelParticleHandler : MonoBehaviour
    {
        float particleEmissionRate = 0; //파티클 지속시간(rateovertime)

        TopDownCarController2 topDownCarController;

        ParticleSystem particleSystemSmoke;
        ParticleSystem.EmissionModule particleSystemEmissionModule;

         void Awake()
        {
            topDownCarController = GetComponentInParent<TopDownCarController2>();

            particleSystemSmoke = GetComponent<ParticleSystem>();

            particleSystemEmissionModule = particleSystemSmoke.emission;

            particleSystemEmissionModule.rateOverTime = 0; // 파티클 지속시간 0으로 초기화
        }

         void Update()
        {
            particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
            particleSystemEmissionModule.rateOverTime = particleEmissionRate;

        }
    }
}
