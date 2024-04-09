using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamandol.Race
{
    public class CarInputHandler : MonoBehaviour
    {
        TopDownCarController topDownCarController;

        void Awake()
        {
            topDownCarController = GetComponent<TopDownCarController>();  
        }
        
        void Update()
        {
            Vector2 inputVector = Vector2.zero; // inputVector ���� �� �ʱ�ȭ

            inputVector.x = Input.GetAxis("Horizontal"); // �¿���Ⱚ�ް� inputVecotor.x�� ����
            inputVector.y = Input.GetAxis("Vertical"); // ���Ϲ��Ⱚ�ް� inputVector.y�� ����

            topDownCarController.SetInputVector(inputVector); //  ��ũ��ƮtopDownCarController�� SetInputVector�Լ� ����
        }
    }
}
