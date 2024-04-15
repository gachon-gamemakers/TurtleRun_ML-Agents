using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamandol.Race
{
    public class TopDownCarController2 : SerializedMonoBehaviour
    {
        // �� ����
        [Header("Car settings")]
        public float driftFactor = 0.95f; // ���� �ٲܶ� ������ ���� ��
        public float accelerationFactor = 30.0f; // ������
        public float turnFactor = 3.5f; // ȸ����
        public float maxSpeed = 20; // �ְ�ӷ�
        public float spurt = 50; // �ʹݼӷ�
        public float Mass = 1; // ���� ����

        // ��������
        float maxSpeedValue = 0; // �ְ�ӷ°� ���庯��
        float accelerationInput = 0; // �����ΰ��� ���ǰ�
        float steeringInput = 0; // �¿���Ⱚ

        float rotationAngle = 0; // ȸ�� ����

        float velocityVsUp = 0;

        // ������Ʈ
        Rigidbody2D carRigidbody2D;

        private void Awake()
        {
            carRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            carRigidbody2D.mass = Mass; // ���Դ���
            maxSpeedValue = maxSpeed; // �ְ�ӷ°� ����
            maxSpeed = 99999; // �ְ�ӵ� ��������
            accelerationFactor += spurt; // �ʹ� ������ ����Ʈ��ŭ �߰�
            Invoke("ReturnNormalSpeed", 0.5f); // 0.5���� �����ӵ��� �ʱ�ȭ 
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
            rotationAngle = 0; // ȸ�� ����

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

            // �ڵ����� 
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
            if (Mathf.Abs(GetLateralVelocity()) > 4.0f)
                return true;

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
    }
}
