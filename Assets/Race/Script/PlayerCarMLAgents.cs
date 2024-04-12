using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Gamandol.Race;

namespace Gamandol.Race
{
    public class PlayerCarMLAgents : Agent
    {
        [SerializeField] private TrackCheckPoints trackCheckpoints;
        private TopDownCarController topDownCarController;
        Rigidbody2D carRigidbody2D;

        private void Awake()
        {
            topDownCarController = GetComponent<TopDownCarController>();
            carRigidbody2D = GetComponent<Rigidbody2D>();
        }
        public override void OnEpisodeBegin()
        {
            carRigidbody2D.velocity = Vector2.zero; // �ڵ����� �޴��� �ʱ�ȭ
            transform.localPosition = new Vector3(0, 0, 0); // ���ο� ���Ǽҵ尡 �����ϸ� ������Ʈ�� ��ġ�� (0,0,0)���� ��ġ��Ŵ
            topDownCarController.StartSput(); // ���ο� ���Ǽҵ� �����ϸ� ž�ٿ���Ʈ�ѷ��� StartSput�Լ� ����
            topDownCarController.rotationAngle = 0; // ���ο� ���Ǽҵ尡 �����ϸ� ������Ʈ�� ���� (0,0,0)���� �Ѵ� 
            trackCheckpoints.nextCheckpointSingleIndex = 0; // ���ο� ���Ǽҵ尡 �����ϸ� üũ����Ʈ �ʱ�ȭ
            
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            var Discreteforward = actions.DiscreteActions[0];
            var Discreteturn = actions.DiscreteActions[1];
            float forwardAmount = 0f; // ������
            float turnAmount = 0f; // �ڵ鰪

            switch (Discreteforward)
            {
                case 0: forwardAmount = 0f; break; // 0�� ���� ������ ������Ʈ ����
                case 1: forwardAmount = +1f; break;// 1�� ���� ������ ������Ʈ ����
                case 2: forwardAmount = -1f; break;// -1�� ���� ������ ������Ʈ ����
            }

            switch (Discreteturn)
            {
                case 0: turnAmount = 0f; break; // 0�� ���� ������ ������Ʈ�� �ڵ鰪�� 0
                case 1: turnAmount = +1f; break; // 1�� ���� ������ ������Ʈ ��ȸ��
                case 2: turnAmount = -1f; break;// -1�� ���� ������ ������Ʈ ��ȸ��
            }

            topDownCarController.SetInputs(forwardAmount, turnAmount); // topDownCarController��ũ��Ʈ�� SetInputs�Լ��� �����ְ� ����

        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            int forwardAction = 0;
            if (Input.GetKey(KeyCode.UpArrow)) forwardAction = 1;
            if (Input.GetKey(KeyCode.DownArrow)) forwardAction = 2;

            int turnAction = 0;
            if (Input.GetKey(KeyCode.RightArrow)) turnAction = 1;
            if (Input.GetKey(KeyCode.LeftArrow)) turnAction = 2;

            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            discreteActions[0] = forwardAction;
            discreteActions[1] = turnAction;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            Vector3 checkpointForward = trackCheckpoints.checkpointSingleList[trackCheckpoints.nextCheckpointSingleIndex].transform.forward;
            float directionDot = Vector3.Dot(transform.forward, checkpointForward); // ������Ʈ�� ���� üũ����Ʈ ������ �Ÿ�
            sensor.AddObservation(directionDot); // ������Ʈ�� ���� üũ����Ʈ ������ �Ÿ�����
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Wall")
            {
                SetReward(-1f); // ���� ó�� �浹�ϸ� �������� -1
            }

            if(collision.tag == "EndPoint")
            {
                EndEpisode(); // EndPoint ���ӿ�����Ʈ �±׸����� ������Ʈ�� �浹�ϸ� ���Ǽҵ� ����
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == "Wall")
            {
                SetReward(-0.1f); // ���� ���������� �پ������� ����ؼ� �������� -0.1��
            }
        }


    }
}