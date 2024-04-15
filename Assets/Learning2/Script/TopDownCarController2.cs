using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamandol.Race
{
    public class TopDownCarController2 : SerializedMonoBehaviour
    {
        // 차 설정
        [Header("Car settings")]
        public float driftFactor = 0.95f; // 방향 바꿀때 앞으로 가던 힘
        public float accelerationFactor = 30.0f; // 가속힘
        public float turnFactor = 3.5f; // 회전힘
        public float maxSpeed = 20; // 최고속력
        public float spurt = 50; // 초반속력
        public float Mass = 1; // 차의 무게

        // 지역변수
        float maxSpeedValue = 0; // 최고속력값 저장변수
        float accelerationInput = 0; // 앞으로가는 힘의값
        float steeringInput = 0; // 좌우방향값

        float rotationAngle = 0; // 회전 각도

        float velocityVsUp = 0;

        // 컴포넌트
        Rigidbody2D carRigidbody2D;

        private void Awake()
        {
            carRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            carRigidbody2D.mass = Mass; // 무게대입
            maxSpeedValue = maxSpeed; // 최고속력값 저장
            maxSpeed = 99999; // 최고속도 제한해제
            accelerationFactor += spurt; // 초반 가속힘 스퍼트만큼 추가
            Invoke("ReturnNormalSpeed", 0.5f); // 0.5초후 원래속도로 초기화 
        }

        private void FixedUpdate()
        {
            ApplyEngineForce();

            KillOrthogonalvelocity();

            ApplySteering();
        }

        [Button]
        public void CarReset()
        {
            rotationAngle = 0; // 회전 각도

            carRigidbody2D.SetRotation(rotationAngle);
            carRigidbody2D.velocity = Vector3.zero;
        }

        void ReturnNormalSpeed()
        {
            maxSpeed = maxSpeedValue;
            accelerationFactor -= spurt;
        }

        void ApplyEngineForce()
        {
            // 자동차가 속도의 방향으로 얼마나 앞으로가고 있는지 계산
            velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);

            // 앞방향으로 최대속도보다 빨리 갈 수 없도록 제한
            if (velocityVsUp > maxSpeed && accelerationInput > 0)
                return;

            // 반대방향으로 최대 속도의 절반보다 빠르게 갈 수 없도록 제한
            if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
                return;

            // 자동차가 가속하는 동안 어떤 방향으로도 더 빨리 갈 수 없도록 제한
            if (carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
                return;

            // 움직이지 않을때 차가 멈추도록 제어
            if (accelerationInput == 0)
                carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 3.0f, Time.fixedDeltaTime * 3);
            else carRigidbody2D.drag = 0;
            
            // 엔진 힘
            Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

            // 차를 앞으로 미는 힘
            carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
        }

        void ApplySteering()
        {
            // 천천히 움직일 때 차의 회전 능력을 제한
            float minSpeedBeforeAllowTurningFactor = (carRigidbody2D.velocity.magnitude * 0.4f);
            minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

            // 회전 각도
            rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

            // 자동차의 
            carRigidbody2D.MoveRotation(rotationAngle);
        }

        void KillOrthogonalvelocity()
        {
            Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
            Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

            carRigidbody2D.velocity = forwardVelocity + rightVelocity * driftFactor;
        }

        float GetLateralVelocity()
        {
            //자동차가 옆으로 얼마나 빠르게 이동하는지 반환
            return Vector2.Dot(transform.right, carRigidbody2D.velocity);
        }

        public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
        {
            lateralVelocity = GetLateralVelocity();
            isBraking = false;

            //차가 앞으로 나아가고 있는지 멈췄는지 확인
            if (accelerationInput < 0 && velocityVsUp > 0)
            {
                isBraking = true;
                return true;
            }

            //커브를 크게 돌면 자국이 남음
            if (Mathf.Abs(GetLateralVelocity()) > 4.0f)
                return true;

            return false;
        }

        public void SetInputVector(Vector2 inputVector)
        {
            steeringInput = inputVector.x; // steeringInput에 좌우방향값 삽입
            accelerationInput = inputVector.y; // accelerationInput에 상하방향값 삽입
        }

        public void SetInputs(float forwardamount, float turnamount)
        {
            steeringInput = turnamount;
            accelerationInput = forwardamount;
        }
    }
}
