using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamandol.Race
{
    public class WheelTrailerRendererHandler : MonoBehaviour
    {
        TopDownCarController2 topDownCarController;
        TrailRenderer trailRenderer;

         void Awake()
        {
            topDownCarController = GetComponentInParent<TopDownCarController2>();

            trailRenderer = GetComponent<TrailRenderer>();

            trailRenderer.emitting = false; // trailRenderer�� ó���� ��Ȱ��ȭ
        }

         void Update()
        {
            //
            if (topDownCarController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
                trailRenderer.emitting = true;
            else trailRenderer.emitting = false;
        }
    }
}
