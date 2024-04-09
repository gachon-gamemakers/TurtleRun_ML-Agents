using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    private List<CheckPointSingle> checkpointSingleList;
    private int nextCheckpointSingleIndex;
    private void Awake()
    {
        Transform checkpointsTransform = transform.Find("CheckPoints");

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
        if(checkpointSingleList.IndexOf(checkPointSingle)== nextCheckpointSingleIndex)
        {
            // 올바른 체크포인트
            Debug.Log("OK");
            nextCheckpointSingleIndex++;
        } else
        {
            Debug.Log("Nope");
        }
    }
}
