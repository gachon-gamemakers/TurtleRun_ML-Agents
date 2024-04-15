using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamandol.Race;

namespace Gamandol.Race
{

    public class TrackCheckPoints : MonoBehaviour 
    {
        static public TrackCheckPoints instance;
    public List<CheckPointSingle> checkpointSingleList;
    public int nextCheckpointSingleIndex;
    
        [SerializeField] private PlayerCarMLAgents playerCarMLAgents;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
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

        public int GetCheckPointIndex(CheckPointSingle checkPoint)
        {
            return checkpointSingleList.IndexOf(checkPoint);
        }

        public void PlayerThroughCheckpoint(CheckPointSingle checkPointSingle)
        {
            if (checkpointSingleList.IndexOf(checkPointSingle) == nextCheckpointSingleIndex)
            {
                Debug.Log("OK");
                nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count; // �ѹ��� ���� üũ����Ʈ �ʱ�ȭ
                
            }
            else
            {
                Debug.Log("Nope");
                
            }
        }
    }
}
