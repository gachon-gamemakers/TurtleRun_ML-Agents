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
            Vector2 inputVector = Vector2.zero; // inputVector 생성 및 초기화

            inputVector.x = Input.GetAxis("Horizontal"); // 좌우방향값받고 inputVecotor.x에 삽입
            inputVector.y = Input.GetAxis("Vertical"); // 상하방향값받고 inputVector.y에 삽입

            topDownCarController.SetInputVector(inputVector); //  스크립트topDownCarController의 SetInputVector함수 실행
        }
    }
}
