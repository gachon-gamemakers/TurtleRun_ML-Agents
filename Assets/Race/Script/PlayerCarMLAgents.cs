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
        private TopDownCarController topDownCarController;
        Rigidbody2D carRigidbody2D;
        public float cumulativeReward = 0; // ���纸������
        public int clashwall = 0; // ���� �浹�� Ƚ��
        public GameObject StartPos; // ���� ������
        public float StartrotationAngle = 0; // ������Ʈ ���۰���
        public int StartCheckpointindex = 0; // ����üũ����Ʈ
        public int nextCheckPoint = 0;
        public int nextcheckpointmax = 0;
        private void Awake()
        {
            topDownCarController = GetComponent<TopDownCarController>();
            carRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {

            nextCheckPoint = TrackCheckPoints.instance.nextCheckpointSingleIndex;
            nextcheckpointmax = TrackCheckPoints.instance.checkpointSingleList.Count;
        }

        private void Update()
        {
            cumulativeReward = GetCumulativeReward(); // ���纸���Լ� 
            if (cumulativeReward <= -2)
            {
                EndEpisode();
            }
            if (clashwall > 3)
            {
                Debug.Log(cumulativeReward);
                EndEpisode();
            }
        }
        public override void OnEpisodeBegin()
        {
            carRigidbody2D.velocity = Vector2.zero; // �ڵ����� �޴��� �ʱ�ȭ
            transform.localPosition = StartPos.transform.localPosition; // ���ο� ���Ǽҵ尡 �����ϸ� ������Ʈ�� ��ġ�� StartPos�� ��ġ��Ŵ
            topDownCarController.rotationAngle = StartrotationAngle; // ���ο� ���Ǽҵ尡 �����ϸ� ������Ʈ�� ���� �ʱ�ȭ�Ѵ�
            TrackCheckPoints.instance.nextCheckpointSingleIndex = StartCheckpointindex; // ���ο� ���Ǽҵ尡 �����ϸ� üũ����Ʈ �ʱ�ȭ
            nextCheckPoint = StartCheckpointindex;
            if (TrackCheckPoints.instance.nextCheckpointSingleIndex == 0)
            {
                topDownCarController.StartSput(); // ���ο� ���Ǽҵ� �����ϸ� ž�ٿ���Ʈ�ѷ��� StartSput�Լ� ����
            }
            clashwall = 0; // ���Ǽҵ���۽� clashwall �ʱ�ȭ
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            var Discreteforward = actions.DiscreteActions[0];
            var Discreteturn = actions.DiscreteActions[1];
            float forwardAmount = 0f; // ������
            float turnAmount = 0f; // �ڵ鰪

            switch (Discreteforward)
            {
                //case 0: forwardAmount = 0f; break; // 0�� ���� ������ ������Ʈ ����
                case 1: forwardAmount = +1f; break;// 1�� ���� ������ ������Ʈ ����
                case 2: forwardAmount = -0.15f; break;// -1�� ���� ������ ������Ʈ ����
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
            Vector3 checkpointForward = TrackCheckPoints.instance.checkpointSingleList[TrackCheckPoints.instance.nextCheckpointSingleIndex].transform.forward;
            float directionDot = Vector3.Dot(transform.forward, checkpointForward); // ������Ʈ�� ���� üũ����Ʈ ������ �Ÿ�
            sensor.AddObservation(directionDot); // ������Ʈ�� ���� üũ����Ʈ ������ �Ÿ�����
            Vector3 checkpointPosition = TrackCheckPoints.instance.checkpointSingleList[TrackCheckPoints.instance.nextCheckpointSingleIndex].transform.position;
            //sensor.AddObservation(checkpointPosition.x);
            //sensor.AddObservation(checkpointPosition.y);
            sensor.AddObservation(transform.position.x);
            sensor.AddObservation(transform.position.y);
            sensor.AddObservation(transform.localRotation.z);
        }
        public void PlayerThroughCheckpoint(CheckPointSingle checkPointSingle)
        {
            if (TrackCheckPoints.instance.checkpointSingleList.IndexOf(checkPointSingle) == nextCheckPoint)
            {
                AddReward(1f);
                nextCheckPoint++;
            }
            else
            {
                AddReward(-1f);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Wall")
            {
                clashwall += 1;
                AddReward(-1f); // ���� �浹�ϸ� �������� -1
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "EndPoint")
            {
                EndEpisode();
            }
        }

    }
}