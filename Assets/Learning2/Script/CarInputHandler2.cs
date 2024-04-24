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
    public class CarInputHandler2 : Agent
    {
        TopDownCarController2 topDownCarController;
        Vector3 firstPosition;
        Rigidbody2D carRigidbody2D;
        public int nextCheckPoint = 0; // ���� üũ����Ʈ ��ȣ
        int checkPointMax = 0; // üũ����Ʈ ����
        List<Transform> checkPointList;
        Vector2 nextPointDirection, nextPointDirection2;
        int directionPoint = -1;
        int actionCount = 0; // �ൿ ī��Ʈ
        int wallCount = 0;
        float turnAmount = 0;
        public bool Sensorcheck1 = true;
        public bool Sensorcheck2 = true;
        public bool Sensorspeed = true;
        public bool Sensorrot = true;
        void Awake()
        {
            topDownCarController = GetComponent<TopDownCarController2>();
            firstPosition = transform.localPosition;
            carRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            checkPointMax = TrackCheckpoints2.instance.checkpointSingleList.Count - 1; // TrackCheckpoints2���� üũ����Ʈ����-1�� �������� checkpointmax�� ���� 
            checkPointList = TrackCheckpoints2.instance.checkpointSingleList; // TrackCheckpoint2���� üũ����Ʈ ����Ʈ������ checkPointList�� ����
        }

        public override void OnEpisodeBegin()
        {
            transform.localPosition = firstPosition; // ��ġ ó���ڸ��� �ʱ�ȭ
            topDownCarController.CarSpurt(); // �ڵ��� ����Ʈ
            topDownCarController.CarReset(); // ������, ȸ���� �ʱ�ȭ
            nextCheckPoint = 0; // üũ����Ʈ �ʱ�ȭ
            wallCount = 0; // ���� �ε���Ƚ�� �ʱ�ȭ
        }

        
        public override void OnActionReceived(ActionBuffers actions)
        {
            AddReward(-0.004f);
            actionCount++;

            if (actionCount == 50)
            {
                actionCount = 0;
                AddReward((Vector2.Dot(transform.up, nextPointDirection) - 0.9f) * 5);

            }

/*            if (actionCount == 50)
            {
                actionCount = 0;
                AddReward((Vector2.Dot(transform.up, nextPointDirection) - 0.9f) * 5);

            }*/

            /*            if (timeCount > 200)
                        {
                            AddReward(-10f);
                            timeCount = 0;
                            EndEpisode();
                        }*/

            float forwardAmount = 0f;
            turnAmount = 0f;

            switch (actions.DiscreteActions[0])
            {
                case 0: forwardAmount = 0f; break;
                case 1: forwardAmount = -1f; break;
                case 2: forwardAmount = 1f; break;
            }

            switch (actions.DiscreteActions[1])
            {
                case 0: turnAmount = 0f; break;
                case 1: turnAmount = -1f; break;
                case 2: turnAmount = 1f; break;
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
            /*
            Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
            Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

            Debug.Log(forwardVelocity);
            Debug.Log(rightVelocity);

            sensor.AddObservation(forwardVelocity);
            sensor.AddObservation(rightVelocity);
            */
            if(directionPoint != nextCheckPoint)
            {
                nextPointDirection = checkPointList[nextCheckPoint].transform.up; // ���� üũ����Ʈ�� ���κ���ġ�� nextPointDirection�� ����
                int doubleNextCeechPoint = nextCheckPoint + 1 >= checkPointList.Count ? 0 : nextCheckPoint + 1;
                nextPointDirection2 = checkPointList[doubleNextCeechPoint].transform.up;

                directionPoint = nextCheckPoint;
            }

            //Debug.Log("a: " + Vector2.Dot(transform.up, nextPointDirection));
            if(Sensorcheck1)
            sensor.AddObservation(Vector2.Dot(transform.up, nextPointDirection)); // ����üũ����Ʈ�� �Ÿ�����
            if(Sensorcheck2)
            sensor.AddObservation(Vector2.Dot(transform.up, nextPointDirection2)); // �ٴ��� üũ����Ʈ�� �Ÿ�����
            if(Sensorspeed)
            sensor.AddObservation(carRigidbody2D.velocity.magnitude); // ������Ʈ�� ����ӷ�
            if(Sensorrot)
            sensor.AddObservation(turnAmount);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "CheckPoint") 
            {
                if (TrackCheckpoints2.instance.GetCheckPointIndex(collision.transform) == nextCheckPoint) // �ùٸ� üũ����Ʈ �浹��
                {
                    AddReward(1f);
                    AddReward(carRigidbody2D.velocity.magnitude * 0.1f);
                    //AddReward(carRigidbody2D.velocity.magnitude * (Vector2.Dot(carRigidbody2D.velocity.normalized, nextPointDirection) - 0.8f) * 0.5f);
                    //Debug.Log("dd" + carRigidbody2D.velocity.magnitude * (Vector2.Dot(carRigidbody2D.velocity.normalized, nextPointDirection) - 0.8f) * 0.2f);
                    nextCheckPoint++;

                    if(nextCheckPoint >= checkPointMax) 
                    {
                        nextCheckPoint = 0;
                        EndEpisode();
                    }
                }
                else // �ٸ� üũ����Ʈ �浹��
                {
                    if (Vector2.Dot(transform.up, nextPointDirection) < 0) // ������Ʈ�� �ݴ� �������� �浹
                    {
                        Debug.Log("back");
                        AddReward(-10f);
                        EndEpisode();
                    }
                    else // ������Ʈ�� �����ؼ� �浹
                    {
                        Debug.Log("-1");
                        AddReward(-1f);
                    }
                }
            }
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.tag == "Wall")
            {
                
                AddReward(-2f); // ���� �浹�� �������� ����
                wallCount++;
                // if (wallCount > 2)
                //     EndEpisode();
                /*                contacts = new ContactPoint2D[collision.contactCount];
                                collision.GetContacts(contacts);
                                totalImpulse = 0;
                                foreach (ContactPoint2D contact in contacts)
                                {
                                    totalImpulse += contact.normalImpulse;
                                }

                                if (totalImpulse > 6)
                                {
                                    AddReward(-totalImpulse * 2);
                                    Debug.Log("fast2");
                                    EndEpisode();
                                }*/
            }
            /*if (collision.collider.tag == "Player")
            {
                Debug.Log("�浹");
                AddReward(7f);
            }*/
        }

/*
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.collider.tag == "Wall")
            {
                AddReward(-0.005f);
            }
        }*/

        /*        private void OnCollisionExit2D(Collision2D collision)
                {
                    collisionCount = 0;
                }*/
    }
}
