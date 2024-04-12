using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gamandol.Race
{
    public class CarInputHandler2 : Agent
    {
        TopDownCarController2 topDownCarController;
        Vector3 firstPosition;
        public Quaternion firstQuaternion;
        Rigidbody2D carRigidbody2D;
        public int nextCheckPoint = 0;
        int checkPointMax = 0;

        void Awake()
        {
            topDownCarController = GetComponent<TopDownCarController2>();
            firstPosition = transform.localPosition;
            firstQuaternion = transform.localRotation;
            carRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            checkPointMax = TrackCheckpoints2.instance.checkpointSingleList.Count - 1;
        }

        public override void OnEpisodeBegin()
        {
            transform.localPosition = firstPosition;
            carRigidbody2D.SetRotation(Quaternion.Euler(0, 0, 0));
            carRigidbody2D.velocity = Vector3.zero;
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

            Debug.Log("ver : " + (int)Input.GetAxisRaw("Vertical"));
            Debug.Log("hori : " + (int)Input.GetAxisRaw("Horizontal"));

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

            Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
            Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

            sensor.AddObservation(forwardVelocity);
            sensor.AddObservation(rightVelocity);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("triger");
            if (collision.tag == "CheckPoint") 
            {
                if (TrackCheckpoints2.instance.GetCheckPointIndex(collision.transform) == nextCheckPoint)
                {
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
                    AddReward(-1f);
                }
            }
        }
    }
}
