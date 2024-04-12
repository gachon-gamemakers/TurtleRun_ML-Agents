using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamandol.Race;

namespace Gamandol.Race
{
    public class CheckPointSingle : MonoBehaviour
    {
        private TrackCheckPoints2 trackCheckpoints;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                trackCheckpoints.PlayerThroughCheckpoint(this);
            }
        }

        public void SetTrackCheckpoints(TrackCheckPoints2 trackCheckpoints)
        {
            this.trackCheckpoints = trackCheckpoints;
        }
    }
}