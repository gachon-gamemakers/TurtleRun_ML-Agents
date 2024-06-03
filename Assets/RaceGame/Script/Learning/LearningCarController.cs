using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamandol.Race
{
    public class LearningCarController : SerializedMonoBehaviour
    {
        // 차 설정
        [Header("차 능력치")]
        public float driftFactor = 0.95f; // 방향 바꿀때 앞으로 가던 힘
        public float accelerationFactor = 30.0f; // 가속도
        public float turnFactor = 3.5f; // 핸들링
        public float maxSpeed = 20; // 최고속력
        public float spurt = 50; // 스타트 스피드
        public float Mass = 1; // 강인함
        [Header("차 종류")]
        public int CarType = 0;
        [Header("별 갯수")]
        public int maxspeedStar = 1; // 최대속력 별갯수
        public int massStar = 1; // 강인함 별갯수
        public int spurtStar = 1; // 스퍼트 별갯수
        public int accelStar = 1; // 가속도 별갯수
        public int ModelId = 0;
        public float anc = 0;

        // 지역변수
        float maxSpeedValue = 0; // 최고속력값 저장변수
        float accelerationFactorValue = 0; // 가속도 저장변수
        float accelerationInput = 0; // 전진후진값
        float steeringInput = 0; // 좌우방향값

        float rotationAngle = 0; // 회전 각도

        float velocityVsUp = 0; // 현재 속도

        private Rigidbody2D carRigidbody2D;

        private Animator animator;

        private void Awake()
        {
            carRigidbody2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            PlayerStar();
        }

        private void Start()
        {
            CarStart(); // 차 초반설정
        }
        private void Update()
        {
            ChangeSpeed(); // 시간에 따른 속도변경(특정 모델만 적용)
        }

        private void FixedUpdate()
        {
            ApplyEngineForce(); // 속도 설정

            KillOrthogonalvelocity();

            ApplySteering(); // 핸들링 설정
        }

        public void PlayerStar()
        {
            int maxstar = Random.Range(3, 4);
            int spurtstar = Random.Range(3, 4);
            int massstar = Random.Range(3, 4);
            int accelstar = Random.Range(3, 4);
            maxspeedStar = maxstar;
            spurtStar = spurtstar;
            massStar = massstar;
            accelStar = accelstar;

        }
        public void CarStart()
        {
            if (CarType == 7)
            {
                maxSpeed += 3;
                accelerationFactor += 3f;
            }
            if (CarType == 8)
            {
                maxSpeed -= 3;
                accelerationFactor -= 3f;
            }
            carRigidbody2D.mass = Mass + ((massStar - 1) * 1f); // 무게대입
            maxSpeedValue = maxSpeed + ((maxspeedStar - 1) * 3f); // 최고속력값 저장
            accelerationFactorValue = accelerationFactor + ((accelStar - 1) * 2f); // 속도값 저장
            animator.SetInteger("Cartype", CarType); // 자동차 애니메이션 설정

            CarStop(); // 카운트 다운동안 정지
            Invoke("Spurt", 0.0f); // 카운트 다운후 StartBoost 실행
        }

        //스타트 부스트
        public void Spurt()
        {
            accelerationFactor = accelerationFactorValue; // 정지하기위해 가속도값 0이었던거 정상화
            maxSpeed = maxSpeedValue;
            Boost(spurt + ((spurtStar - 1) * 5f), 1.5f);
        }


        //부스트
        public void Boost(float power, float time)
        {
            maxSpeed += power; // 최고속도 제한해제
            accelerationFactor += power; // power만큼 빨라짐
            Invoke("EndBoost", time); // time초후 원래속도로 초기화 
            switch (CarType) // cartype에 따른 부스트 애니메이션 활성화
            {
                case 0: animator.SetBool("TurtleBoost", true); break;
                case 1: animator.SetBool("CactusBoost", true); break;
                case 2: animator.SetBool("ChickenBoost", true); break;
                case 3: animator.SetBool("DogBoost", true); break;
                case 4: animator.SetBool("DuckBoost", true); break;
                case 5: animator.SetBool("FoxBoost", true); break;
                case 6: animator.SetBool("RabbitBoost", true); break;
                case 7: animator.SetBool("SphinxBoost", true); break;
                case 8: animator.SetBool("CrocodileBoost", true); break;
                case 9: animator.SetBool("FishBoost", true); break;
            }
        }

        void EndBoost()
        {
            maxSpeed = maxSpeedValue; // 최고속도 원상복구
            accelerationFactor = accelerationFactorValue; // 스퍼트 속도 종료
            switch (CarType) // 부스트 애니메이션 종료
            {
                case 0: animator.SetBool("TurtleBoost", false); break;
                case 1: animator.SetBool("CactusBoost", false); break;
                case 2: animator.SetBool("ChickenBoost", false); break;
                case 3: animator.SetBool("DogBoost", false); break;
                case 4: animator.SetBool("DuckBoost", false); break;
                case 5: animator.SetBool("FoxBoost", false); break;
                case 6: animator.SetBool("RabbitBoost", false); break;
                case 7: animator.SetBool("SphinxBoost", false); break;
                case 8: animator.SetBool("CrocodileBoost", false); break;
                case 9: animator.SetBool("FishBoost", false); break;
            }
        }

        public void CarStop()
        {
            accelerationFactor = 0;
            maxSpeed = 0;
        }

        [Button]
        public void CarReset()
        {
            rotationAngle = anc; // 회전 각도

            carRigidbody2D.SetRotation(rotationAngle);
            carRigidbody2D.velocity = Vector3.zero;

            maxSpeed = 5;
            accelerationFactor = 3;
            spurt = 3;
            Mass = 1;

            if (CarType == 7)
            {
                maxSpeed += 4;
                accelerationFactor += 2;
            }
        }
        void ChangeSpeed()
        {

            if (CarType == 7 && maxSpeed > 3)
            {
                maxSpeed -= Time.deltaTime * 0.3f; // 최고속력 서서히 감소
            }
            if (CarType == 7 && accelerationFactor > 1f)
            {
                accelerationFactor -= Time.deltaTime * 0.15f; // 가속도 서서히 감소
            }
            if (CarType == 8 && maxSpeed < 40)
            {
                maxSpeed += Time.deltaTime * 0.6f; // 최고속도 서서히 증가
            }
            if (CarType == 8 && accelerationFactor < 20)
            {
                accelerationFactor += Time.deltaTime * 0.3f; // 가속도 서서히 즌가
            }

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

            // 자동차를 rotationAngle만큼 회전시킨다 
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
            if (Mathf.Abs(GetLateralVelocity()) > 3.5f)
            {
                return true;
            }

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

        public float GetVelocityMagnitude()
        {
            return carRigidbody2D.velocity.magnitude;
        }
    }
}
