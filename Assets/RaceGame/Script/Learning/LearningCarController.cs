using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamandol.Race
{
    public class LearningCarController : SerializedMonoBehaviour
    {
        // �� ����
        [Header("�� �ɷ�ġ")]
        public float driftFactor = 0.95f; // ���� �ٲܶ� ������ ���� ��
        public float accelerationFactor = 30.0f; // ���ӵ�
        public float turnFactor = 3.5f; // �ڵ鸵
        public float maxSpeed = 20; // �ְ�ӷ�
        public float spurt = 50; // ��ŸƮ ���ǵ�
        public float Mass = 1; // ������
        [Header("�� ����")]
        public int CarType = 0;
        [Header("�� ����")]
        public int maxspeedStar = 1; // �ִ�ӷ� ������
        public int massStar = 1; // ������ ������
        public int spurtStar = 1; // ����Ʈ ������
        public int accelStar = 1; // ���ӵ� ������
        public int ModelId = 0;
        public float anc = 0;

        // ��������
        float maxSpeedValue = 0; // �ְ�ӷ°� ���庯��
        float accelerationFactorValue = 0; // ���ӵ� ���庯��
        float accelerationInput = 0; // ����������
        float steeringInput = 0; // �¿���Ⱚ

        float rotationAngle = 0; // ȸ�� ����

        float velocityVsUp = 0; // ���� �ӵ�

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
            CarStart(); // �� �ʹݼ���
        }
        private void Update()
        {
            ChangeSpeed(); // �ð��� ���� �ӵ�����(Ư�� �𵨸� ����)
        }

        private void FixedUpdate()
        {
            ApplyEngineForce(); // �ӵ� ����

            KillOrthogonalvelocity();

            ApplySteering(); // �ڵ鸵 ����
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
            carRigidbody2D.mass = Mass + ((massStar - 1) * 1f); // ���Դ���
            maxSpeedValue = maxSpeed + ((maxspeedStar - 1) * 3f); // �ְ�ӷ°� ����
            accelerationFactorValue = accelerationFactor + ((accelStar - 1) * 2f); // �ӵ��� ����
            animator.SetInteger("Cartype", CarType); // �ڵ��� �ִϸ��̼� ����

            CarStop(); // ī��Ʈ �ٿ�� ����
            Invoke("Spurt", 0.0f); // ī��Ʈ �ٿ��� StartBoost ����
        }

        //��ŸƮ �ν�Ʈ
        public void Spurt()
        {
            accelerationFactor = accelerationFactorValue; // �����ϱ����� ���ӵ��� 0�̾����� ����ȭ
            maxSpeed = maxSpeedValue;
            Boost(spurt + ((spurtStar - 1) * 5f), 1.5f);
        }


        //�ν�Ʈ
        public void Boost(float power, float time)
        {
            maxSpeed += power; // �ְ�ӵ� ��������
            accelerationFactor += power; // power��ŭ ������
            Invoke("EndBoost", time); // time���� �����ӵ��� �ʱ�ȭ 
            switch (CarType) // cartype�� ���� �ν�Ʈ �ִϸ��̼� Ȱ��ȭ
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
            maxSpeed = maxSpeedValue; // �ְ�ӵ� ���󺹱�
            accelerationFactor = accelerationFactorValue; // ����Ʈ �ӵ� ����
            switch (CarType) // �ν�Ʈ �ִϸ��̼� ����
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
            rotationAngle = anc; // ȸ�� ����

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
                maxSpeed -= Time.deltaTime * 0.3f; // �ְ�ӷ� ������ ����
            }
            if (CarType == 7 && accelerationFactor > 1f)
            {
                accelerationFactor -= Time.deltaTime * 0.15f; // ���ӵ� ������ ����
            }
            if (CarType == 8 && maxSpeed < 40)
            {
                maxSpeed += Time.deltaTime * 0.6f; // �ְ�ӵ� ������ ����
            }
            if (CarType == 8 && accelerationFactor < 20)
            {
                accelerationFactor += Time.deltaTime * 0.3f; // ���ӵ� ������ ��
            }

        }



        void ApplyEngineForce()
        {
            // �ڵ����� �ӵ��� �������� �󸶳� �����ΰ��� �ִ��� ���
            velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);

            // �չ������� �ִ�ӵ����� ���� �� �� ������ ����
            if (velocityVsUp > maxSpeed && accelerationInput > 0)
                return;

            // �ݴ�������� �ִ� �ӵ��� ���ݺ��� ������ �� �� ������ ����
            if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
                return;

            // �ڵ����� �����ϴ� ���� � �������ε� �� ���� �� �� ������ ����
            if (carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
                return;

            // �������� ������ ���� ���ߵ��� ����
            if (accelerationInput == 0)
                carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 3.0f, Time.fixedDeltaTime * 3);
            else carRigidbody2D.drag = 0;

            // ���� ��
            Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

            // ���� ������ �̴� ��
            carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
        }

        void ApplySteering()
        {
            // õõ�� ������ �� ���� ȸ�� �ɷ��� ����
            float minSpeedBeforeAllowTurningFactor = (carRigidbody2D.velocity.magnitude * 0.4f);
            minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

            // ȸ�� ����
            rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

            // �ڵ����� rotationAngle��ŭ ȸ����Ų�� 
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
            //�ڵ����� ������ �󸶳� ������ �̵��ϴ��� ��ȯ
            return Vector2.Dot(transform.right, carRigidbody2D.velocity);
        }

        public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
        {
            lateralVelocity = GetLateralVelocity();
            isBraking = false;

            //���� ������ ���ư��� �ִ��� ������� Ȯ��
            if (accelerationInput < 0 && velocityVsUp > 0)
            {
                isBraking = true;
                return true;
            }

            //Ŀ�긦 ũ�� ���� �ڱ��� ����
            if (Mathf.Abs(GetLateralVelocity()) > 3.5f)
            {
                return true;
            }

            return false;
        }

        public void SetInputVector(Vector2 inputVector)
        {
            steeringInput = inputVector.x; // steeringInput�� �¿���Ⱚ ����
            accelerationInput = inputVector.y; // accelerationInput�� ���Ϲ��Ⱚ ����
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
