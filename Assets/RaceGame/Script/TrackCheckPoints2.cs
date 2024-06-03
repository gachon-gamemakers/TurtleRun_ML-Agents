using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamandol.Race;

namespace Gamandol.Race
{

    public class TrackCheckpoints2 : MonoBehaviour 
    { 
        
        public List<Transform> checkpointSingleList;
        public int nextCheckpointSingleIndex;
    
        private void Awake()
        {
                
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

    }
}
