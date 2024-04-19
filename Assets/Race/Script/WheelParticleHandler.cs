using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamandol.Race
{
    public class WheelParticleHandler : MonoBehaviour
    {
        float particleEmissionRate = 0; //��ƼŬ ����(rateovertime)

        TopDownCarController2 topDownCarController;

        ParticleSystem particleSystemSmoke;
        ParticleSystem.EmissionModule particleSystemEmissionModule;

         void Awake()
        {
            topDownCarController = GetComponentInParent<TopDownCarController2>();

            particleSystemSmoke = GetComponent<ParticleSystem>();

            particleSystemEmissionModule = particleSystemSmoke.emission;

            particleSystemEmissionModule.rateOverTime = 0; // ��ƼŬ ����
        }

         void Update()
        {
            particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
            particleSystemEmissionModule.rateOverTime = particleEmissionRate;

            if(topDownCarController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
            {
                if (isBraking)
                    particleEmissionRate = 15; // ���� �����ϸ� ��ƼŬ ���� 15

                else particleEmissionRate = Mathf.Abs(lateralVelocity) ; // �����϶� ���� Ŀ�갪��2�踸ŭ ��ƼŬ ������ ����
            }
        }
    }
}
