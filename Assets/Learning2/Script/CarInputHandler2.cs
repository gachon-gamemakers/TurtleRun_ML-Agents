using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

namespace Gamandol.Race
{
    public class CarInputHandler2 : Agent
    {
        TopDownCarController2 topDownCarController;
        Vector3 firstPosition;
        Rigidbody2D carRigidbody2D;
        public int nextCheckPoint = 0;
        int checkPointMax = 0;
        List<Transform> checkPointList;
        Vector2 nextPointDirection, nextPointDirection2;
        int directionPoint = -1;


        void Awake()
        {
            topDownCarController = GetComponent<TopDownCarController2>();
            firstPosition = transform.localPosition;
            carRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            checkPointMax = TrackCheckpoints2.instance.checkpointSingleList.Count - 1;
            checkPointList = TrackCheckpoints2.instance.checkpointSingleList;
        }

        public override void OnEpisodeBegin()
        {
            transform.localPosition = firstPosition;
            topDownCarController.CarReset();
            nextCheckPoint = 0;
        }

        
        public override void OnActionReceived(ActionBuffers actions)
        {
            AddReward(-0.002f);

            float forwardAmount = 0f;
            float turnAmount = 0f;

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
                nextPointDirection = checkPointList[nextCheckPoint].transform.forward;
                int doubleNextCeechPoint = nextCheckPoint + 1 >= checkPointList.Count ? 0 : nextCheckPoint + 1;
                nextPointDirection2 = checkPointList[doubleNextCeechPoint].transform.forward;

                directionPoint = nextCheckPoint;
            }
            
            sensor.AddObservation(Vector2.Dot(transform.forward, nextPointDirection));
            //sensor.AddObservation(Vector2.Dot(transform.forward, nextPointDirection2));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("triger");
            if (collision.tag == "CheckPoint") 
            {
                if (TrackCheckpoints2.instance.GetCheckPointIndex(collision.transform) == nextCheckPoint)
                {
                    Debug.Log("+1");
                    AddReward(1f);
                    nextCheckPoint++;
                    if(nextCheckPoint >= checkPointMax) 
                    {
                        nextCheckPoint = 0;
                        EndEpisode();
                    }
                }
                else
                {
                    Debug.Log("-1");
                    AddReward(-1f);
                }
            }
        }
    }
}
