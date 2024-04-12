using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamandol.Race
{
    public class CameraController : MonoBehaviour
    {
        public Transform playerTransform; // �÷��̾��� Transform ������Ʈ
        private Vector3 offset; // �÷��̾�� ī�޶� ������ ����� �Ÿ�

        void Start()
        {
            // ������ �� �÷��̾�� ī�޶� ������ ����� �Ÿ��� ���
            offset = transform.position - playerTransform.position;
        }

        void Update()
        {
            //��� �÷��̾��� ��ġ�� offset��ŭ ���ؼ� ī�޶��� ��ġ�� �����Ѵ�.
            transform.position = playerTransform.position + offset;
        }
    }
}