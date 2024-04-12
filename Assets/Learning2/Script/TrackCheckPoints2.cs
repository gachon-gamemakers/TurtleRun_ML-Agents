using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamandol.Race;

namespace Gamandol.Race
{

    public class TrackCheckpoints2 : MonoBehaviour 
    { 
        static public TrackCheckpoints2 instance;

        public List<Transform> checkpointSingleList;
        public int nextCheckpointSingleIndex;
    
        [SerializeField] private PlayerCarMLAgents playerCarMLAgents;
        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
                
            Transform checkpointsTransform = transform.Find("CheckPoints"); // checkpointsTransform�� CheckPoints��� �̸��� ���� ������Ʈ�� ã�Ƽ� ����

            foreach (Transform checkpointSingleTransform in checkpointsTransform)
            {
                checkpointSingleList.Add(checkpointSingleTransform);
            }

            nextCheckpointSingleIndex = 0;
        }

        public int GetCheckPointIndex(Transform checkPoint)
        {
            return checkpointSingleList.IndexOf(checkPoint);
        }

        public void PlayerThroughCheckpoint(Transform checkPointSingle)
        {
            if (checkpointSingleList.IndexOf(checkPointSingle) == nextCheckpointSingleIndex)
            {
                // �ùٸ� üũ����Ʈ
                Debug.Log("OK");
                nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count; // �ѹ��� ���� üũ����Ʈ �ʱ�ȭ
                playerCarMLAgents.SetReward(1f); // �ùٸ� üũ����Ʈ ����� �������� +1
            }
            else
            {
                Debug.Log("Nope");
                playerCarMLAgents.SetReward(-1f); // �ùٸ��� ���� üũ����Ʈ ����� �������� -1
            }
        }
    }
}
