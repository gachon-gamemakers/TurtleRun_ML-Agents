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
            Transform checkpointsTransform = transform.Find("CheckPoints"); // checkpointsTransform�� CheckPoints��� �̸��� ���� ������Ʈ�� ã�Ƽ� ����

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
