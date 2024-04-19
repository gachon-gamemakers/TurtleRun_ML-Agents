using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamandol.Race
{
    public class WheelParticleHandler : MonoBehaviour
    {
        float particleEmissionRate = 0; //파티클 갯수(rateovertime)

        TopDownCarController2 topDownCarController;

        ParticleSystem particleSystemSmoke;
        ParticleSystem.EmissionModule particleSystemEmissionModule;

         void Awake()
        {
            topDownCarController = GetComponentInParent<TopDownCarController2>();

            particleSystemSmoke = GetComponent<ParticleSystem>();

            particleSystemEmissionModule = particleSystemSmoke.emission;

            particleSystemEmissionModule.rateOverTime = 0; // 파티클 갯수
        }

         void Update()
        {
            particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
            particleSystemEmissionModule.rateOverTime = particleEmissionRate;

            if(topDownCarController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
            {
                if (isBraking)
                    particleEmissionRate = 15; // 차가 정지하면 파티클 갯수 15

                else particleEmissionRate = Mathf.Abs(lateralVelocity) ; // 움직일땐 차의 커브값에2배만큼 파티클 갯수에 대입
            }
        }
    }
}
