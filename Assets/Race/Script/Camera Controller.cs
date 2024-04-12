using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamandol.Race
{
    public class CameraController : MonoBehaviour
    {
        public Transform playerTransform; // 플레이어의 Transform 컴포넌트
        private Vector3 offset; // 플레이어와 카메라 사이의 상대적 거리

        void Start()
        {
            // 시작할 때 플레이어와 카메라 사이의 상대적 거리를 계산
            offset = transform.position - playerTransform.position;
        }

        void Update()
        {
            //계속 플레이어의 위치에 offset만큼 더해서 카메라의 위치를 갱신한다.
            transform.position = playerTransform.position + offset;
        }
    }
}