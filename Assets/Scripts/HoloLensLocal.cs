using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HoloLensLocal : MonoBehaviourPun, IPunObservable
{

    public Transform playerGlobal;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            Debug.Log("Player is mine");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //write position to stream for display in remote client
        if (stream.IsWriting)
        {
            stream.SendNext(playerGlobal.position);
            stream.SendNext(playerGlobal.rotation);
        }
    }


}
