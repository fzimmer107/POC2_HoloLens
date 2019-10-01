using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedCube : MonoBehaviourPun, IPunObservable
{

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(this.transform.position);
            stream.SendNext(this.transform.rotation);
        }
        else
        {
            this.transform.position = (Vector3)stream.ReceiveNext();
            this.transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

}
