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
        public float cumulativeReward = 0; // 현재보상점수
        public int clashwall = 0; // 벽에 충돌한 횟수
        public GameObject StartPos; // 시작 포지션
        public float StartrotationAngle = 0; // 에이전트 시작각도
        public int StartCheckpointindex = 0; // 시작체크포인트
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
            cumulativeReward = GetCumulativeReward(); // 현재보상함수 
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
            carRigidbody2D.velocity = Vector2.zero; // 자동차의 받는힘 초기화
            transform.localPosition = StartPos.transform.localPosition; // 새로운 에피소드가 시작하면 에이전트의 위치를 StartPos로 위치시킴
            topDownCarController.rotationAngle = StartrotationAngle; // 새로운 에피소드가 시작하면 에이전트의 각을 초기화한다
            TrackCheckPoints.instance.nextCheckpointSingleIndex = StartCheckpointindex; // 새로운 에피소드가 시작하면 체크포인트 초기화
            nextCheckPoint = StartCheckpointindex;
            if (TrackCheckPoints.instance.nextCheckpointSingleIndex == 0)
            {
                topDownCarController.StartSput(); // 새로운 에피소드 시작하면 탑다운컨트롤러의 StartSput함수 실행
            }
            clashwall = 0; // 에피소드시작시 clashwall 초기화
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            var Discreteforward = actions.DiscreteActions[0];
            var Discreteturn = actions.DiscreteActions[1];
            float forwardAmount = 0f; // 엑셀값
            float turnAmount = 0f; // 핸들값

            switch (Discreteforward)
            {
                //case 0: forwardAmount = 0f; break; // 0의 값이 나오면 에이전트 정지
                case 1: forwardAmount = +1f; break;// 1의 값이 나오면 에이전트 전진
                case 2: forwardAmount = -0.15f; break;// -1의 값이 나오면 에이전트 후진
            }

            switch (Discreteturn)
            {
                case 0: turnAmount = 0f; break; // 0의 값이 나오면 에이전트의 핸들값이 0
                case 1: turnAmount = +1f; break; // 1의 값이 나오면 에이전트 우회전
                case 2: turnAmount = -1f; break;// -1의 값이 나오면 에이전트 좌회전
            }

            topDownCarController.SetInputs(forwardAmount, turnAmount); // topDownCarController스크립트의 SetInputs함수에 값을주고 실행

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
            float directionDot = Vector3.Dot(transform.forward, checkpointForward); // 에이전트와 다음 체크포인트 사이의 거리
            sensor.AddObservation(directionDot); // 에이전트와 다음 체크포인트 사이의 거리감지
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
                AddReward(-1f); // 벽에 충돌하면 보상점수 -1
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