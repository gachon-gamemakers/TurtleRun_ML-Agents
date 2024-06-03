using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using System.Threading;

namespace Gamandol.Race
{
    public class LearningCarMLAgents : Agent
    {
        public LearningCarController topDownCarController;
        public TrackCheckpoints2 trackCheckpoints;
        Vector3 firstPosition;
        Rigidbody2D carRigidbody2D;
        public int nextCheckPoint = 0; // ���� üũ����Ʈ ��ȣ
        int checkPointMax = 0; // üũ����Ʈ ����
        List<Transform> checkPointList;
        Vector2 nextPointDirection, nextPointDirection2 , nextPointDirection3, nextPointDirection4, nextPointDirection5;
        int directionPoint = -1;
        int actionCount = 0; // �ൿ ī��Ʈ
        float turnAmount = 0;
        private Vector3 originalVelocity; // ���� �ӵ��� ������ ����
        private bool isSlowedDown = false;
        public bool Sensorcheck1 = true;
        public bool Sensorcheck2 = true;
        public bool Sensorspeed = true;
        public bool Sensorcheck = true;
        public int slowcount = 0, wallcount = 0, wrongcount = 0;

        void Awake()
        {
            // topDownCarController = GetComponent<LearningCarController>();
            // trackCheckpoints = GetComponent<TrackCheckpoints2>();
            firstPosition = transform.localPosition;
            carRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            checkPointMax = trackCheckpoints.checkpointSingleList.Count - 1; // TrackCheckpoints2���� üũ����Ʈ����-1�� �������� checkpointmax�� ���� 
            checkPointList = trackCheckpoints.checkpointSingleList; // TrackCheckpoint2���� üũ����Ʈ ����Ʈ������ checkPointList�� ����
        }

        private void Update()
        {
            AddReward(-Time.deltaTime*0.5f);
        }

        public override void OnEpisodeBegin()
        {
            slowcount = 0;
            wallcount = 0;
            topDownCarController.CarReset(); // ������, ȸ���� �ʱ�ȭ
            wrongcount = 0;
            topDownCarController.PlayerStar();
            transform.localPosition = firstPosition; // ��ġ ó���ڸ��� �ʱ�ȭ
            topDownCarController.CarStart(); // �ڵ��� �����Լ�
            nextCheckPoint = 0; // üũ����Ʈ �ʱ�ȭ
        }


        public override void OnActionReceived(ActionBuffers actions)
        {

            actionCount++;

            if (actionCount == 50)
            {
                actionCount = 0;
                AddReward((Vector2.Dot(transform.up, nextPointDirection) - 0.9f) * 5);

            }
            float forwardAmount = 0f;
            turnAmount = 0f;

            switch (actions.DiscreteActions[0])
            {
                case 0:
                    forwardAmount = 0f;
                    AddReward(-0.01f);
                    break;
                case 1:
                    forwardAmount = -1f;
                    AddReward(-0.01f);
                    break;
                case 2:
                    forwardAmount = 1f;
                    AddReward(0.005f);
                    break;
            }

            switch (actions.DiscreteActions[1])
            {
                case 0:
                    turnAmount = 0f;
                    AddReward(0.005f);
                    break;
                case 1:
                    turnAmount = -1f;
                    AddReward(-0.005f);
                    break;
                case 2:
                    turnAmount = 1f;
                    AddReward(-0.005f);
                    break;
            }

            topDownCarController.SetInputs(forwardAmount, turnAmount);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteAction = actionsOut.DiscreteActions;

            switch ((int)Input.GetAxisRaw("Vertical"))
            {
                case 0: discreteAction[0] = 0; break;
                case -1: discreteAction[0] = 1; break;
                case 1: discreteAction[0] = 2; break;
            }

            switch ((int)Input.GetAxisRaw("Horizontal"))
            {
                case 0: discreteAction[1] = 0; break;
                case -1: discreteAction[1] = 1; break;
                case 1: discreteAction[1] = 2; break;
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (directionPoint != nextCheckPoint)
            {
                nextPointDirection = checkPointList[nextCheckPoint].transform.up; // ���� üũ����Ʈ�� ���κ���ġ�� nextPointDirection�� ����
                int doubleNextCeechPoint = nextCheckPoint + 1 >= checkPointList.Count ? 0 : nextCheckPoint + 1;
                int NextCeechPoint3 = nextCheckPoint + 2 >= checkPointList.Count ? 0 : nextCheckPoint + 2;
                int NextCeechPoint4 = nextCheckPoint + 3 >= checkPointList.Count ? 0 : nextCheckPoint + 3;
                int NextCeechPoint5 = nextCheckPoint + 4 >= checkPointList.Count ? 0 : nextCheckPoint + 4;
                nextPointDirection2 = checkPointList[doubleNextCeechPoint].transform.up;
                nextPointDirection3 = checkPointList[NextCeechPoint3].transform.up;
                nextPointDirection4 = checkPointList[NextCeechPoint4].transform.up;
                nextPointDirection5 = checkPointList[NextCeechPoint5].transform.up;

                directionPoint = nextCheckPoint;
            }
            if (Sensorcheck1)
                sensor.AddObservation(Vector2.Dot(transform.up, nextPointDirection)); // ����üũ����Ʈ�� �Ÿ�����
            if (Sensorcheck2)
                sensor.AddObservation(Vector2.Dot(transform.up, nextPointDirection2)); // �ٴ��� üũ����Ʈ�� �Ÿ�����
            if (Sensorcheck)
            {
                sensor.AddObservation(Vector2.Dot(transform.up, nextPointDirection3)); // �ٴ��� üũ����Ʈ�� �Ÿ�����
                sensor.AddObservation(Vector2.Dot(transform.up, nextPointDirection4)); // �ٴ��� üũ����Ʈ�� �Ÿ�����
                sensor.AddObservation(Vector2.Dot(transform.up, nextPointDirection5)); // �ٴ��� üũ����Ʈ�� �Ÿ�����
            }
            if (Sensorspeed)
                sensor.AddObservation(carRigidbody2D.velocity.magnitude); // ������Ʈ�� ����ӷ�

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "CheckPoint")
            {
                // Debug.Log(Vector2.Dot(transform.up, nextPointDirection));
                if (trackCheckpoints.GetCheckPointIndex(collision.transform) == nextCheckPoint) // �ùٸ� üũ����Ʈ �浹��
                {
                    AddReward(3f);
                    AddReward(carRigidbody2D.velocity.magnitude * 0.1f);
                    nextCheckPoint++;
                    if (nextCheckPoint >= checkPointMax)
                    {
                        nextCheckPoint = 0;
                        AddReward(50f);
                        EndEpisode();
                    }
                }
                else // �ٸ� üũ����Ʈ �浹��
                {
                    if (Vector2.Dot(transform.up, nextPointDirection) < 0) // ������Ʈ�� �ݴ� �������� �浹
                    {
                        wrongcount++;
                        Debug.Log("back");
                        AddReward(-10f);
                        if (wrongcount > 3)
                        {
                            AddReward(-10f);
                            EndEpisode();
                        }
                    }
                    else // ������Ʈ�� �����ؼ� �浹
                    {
                        AddReward(-3.0f);
                    }
                }
            }
            if (collision.tag == "Player")
            {
               //  AddReward(6f);
            }
            if (collision.tag == "Boost")
            {
                topDownCarController.Boost(5, 2.0f);
              //  AddReward(5f);
            }
            if(collision.tag == "Slow" && !isSlowedDown)
            {
                /*slowcount++;
                AddReward(-100f);
                if (slowcount > 1)
                {
                    EndEpisode();
                    AddReward(-100);
                }*/
                originalVelocity = carRigidbody2D.velocity; // ���� �ӵ��� ����
                carRigidbody2D.velocity = carRigidbody2D.velocity * 0.1f; // �ӵ� ����
                isSlowedDown = true;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Slow" && isSlowedDown)
            {
                carRigidbody2D.velocity = originalVelocity * 0.7f; // ����� �ӵ��� ����
                isSlowedDown = false;
            }
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.tag == "Wall")
            {
                wallcount++;
                AddReward(-2.0f); // ���� �浹�� �������� ����
              /*  if (wallcount > 20)
                {
                    AddReward(-50);
                    EndEpisode();
                }*/
            }
        }
    }
}
