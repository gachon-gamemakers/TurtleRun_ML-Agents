using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Gamandol.Race;

namespace Gamandol.Race
{
    public class PlayerCarMLAgents2 : Agent
    {
        [SerializeField] private TrackCheckpoints trackCheckpoints;
        private TopDownCarController2 topDownCarController;

        private void Awake()
        {
            topDownCarController = GetComponent<TopDownCarController2>();
        }
        public override void OnEpisodeBegin()
        {
            transform.localPosition = new Vector3(-12, -8, 0);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            float forwardAmount = 0f;
            float turnAmount = 0f;

            switch (actions.DiscreteActions[0])
            {
                case 0: forwardAmount = 0f; break;
                case 1: forwardAmount = +1f; break;
                case 2: forwardAmount = -1f; break;
            }

            switch (actions.DiscreteActions[1])
            {
                case 0: turnAmount = 0f; break;
                case 1: turnAmount = +1f; break;
                case 2: turnAmount = -1f; break;
            }

            topDownCarController.SetInputs(forwardAmount, turnAmount);

        }

        public override void CollectObservations(VectorSensor sensor)
        {
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Wall")
            {
                SetReward(-1f);
            }

            if(collision.tag == "EndPoint")
            {
                EndEpisode();
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == "Wall")
            {
                SetReward(-0.1f);
            }
        }


    }
}