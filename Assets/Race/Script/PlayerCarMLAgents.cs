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
            carRigidbody2D.velocity = Vector2.zero; // 자동차의 받는힘 초기화
            transform.localPosition = new Vector3(0, 0, 0); // 새로운 에피소드가 시작하면 에이전트의 위치를 (0,0,0)으로 위치시킴
            topDownCarController.StartSput(); // 새로운 에피소드 시작하면 탑다운컨트롤러의 StartSput함수 실행
            topDownCarController.rotationAngle = 0; // 새로운 에피소드가 시작하면 에이전트의 각을 (0,0,0)으로 한다 
            trackCheckpoints.nextCheckpointSingleIndex = 0; // 새로운 에피소드가 시작하면 체크포인트 초기화
            
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            var Discreteforward = actions.DiscreteActions[0];
            var Discreteturn = actions.DiscreteActions[1];
            float forwardAmount = 0f; // 엑셀값
            float turnAmount = 0f; // 핸들값

            switch (Discreteforward)
            {
                case 0: forwardAmount = 0f; break; // 0의 값이 나오면 에이전트 정지
                case 1: forwardAmount = +1f; break;// 1의 값이 나오면 에이전트 전진
                case 2: forwardAmount = -1f; break;// -1의 값이 나오면 에이전트 후진
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
            Vector3 checkpointForward = trackCheckpoints.checkpointSingleList[trackCheckpoints.nextCheckpointSingleIndex].transform.forward;
            float directionDot = Vector3.Dot(transform.forward, checkpointForward); // 에이전트와 다음 체크포인트 사이의 거리
            sensor.AddObservation(directionDot); // 에이전트와 다음 체크포인트 사이의 거리감지
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Wall")
            {
                SetReward(-1f); // 벽에 처음 충돌하면 보상점수 -1
            }

            if(collision.tag == "EndPoint")
            {
                EndEpisode(); // EndPoint 게임오브젝트 태그를가진 오브젝트에 충돌하면 에피소드 종료
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == "Wall")
            {
                SetReward(-0.1f); // 벽에 지속적으로 붙어있으면 계속해서 보상점수 -0.1점
            }
        }


    }
}