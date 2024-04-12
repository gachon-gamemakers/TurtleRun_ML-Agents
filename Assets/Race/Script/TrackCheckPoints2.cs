using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamandol.Race;

namespace Gamandol.Race
{

    public class TrackCheckPoints2 : MonoBehaviour 
    { 
    public List<CheckPointSingle> checkpointSingleList;
    public int nextCheckpointSingleIndex;
    
        [SerializeField] private PlayerCarMLAgents playerCarMLAgents;
        private void Awake()
        {
            Transform checkpointsTransform = transform.Find("CheckPoints"); // checkpointsTransform에 CheckPoints라는 이름을 가진 오브젝트를 찾아서 대입

            checkpointSingleList = new List<CheckPointSingle>();
            foreach (Transform checkpointSingleTransform in checkpointsTransform)
            {
                CheckPointSingle checkPointSingle = checkpointSingleTransform.GetComponent<CheckPointSingle>();
                checkPointSingle.SetTrackCheckpoints(this);
                checkpointSingleList.Add(checkPointSingle);
            }

            nextCheckpointSingleIndex = 0;
        }

        public void PlayerThroughCheckpoint(CheckPointSingle checkPointSingle)
        {
            if (checkpointSingleList.IndexOf(checkPointSingle) == nextCheckpointSingleIndex)
            {
                // 올바른 체크포인트
                Debug.Log("OK");
                nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count; // 한바퀴 돌면 체크포인트 초기화
                playerCarMLAgents.SetReward(1f); // 올바른 체크포인트 통과시 보상점수 +1
            }
            else
            {
                Debug.Log("Nope");
                playerCarMLAgents.SetReward(-1f); // 올바르지 않은 체크포인트 통과시 보상점수 -1
            }
        }
    }
}
