using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// This class handles incoming remotePlayer data to update the position and rotation
/// of a remote users head and hands
/// </summary>

public class NetworkedRemotePlayer : MonoBehaviourPun, IPunObservable
{

    //necessary, for changing position and rotation
    public Transform playerGlobal;
    public Transform playerLocal;
    public GameObject leftHand;
    public GameObject rightHand;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player instantiated");

        if (photonView.IsMine)
        {
            Debug.Log("Player is mine");
        }
    }

    /// <summary>
    /// Recieves position and rotation data of remote users
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsReading)
        {
            Vector3 globalPosition = (Vector3)stream.ReceiveNext();

            Vector3 localPosition = (Vector3)stream.ReceiveNext();
            Quaternion localRotation = (Quaternion)stream.ReceiveNext();

            Vector3 leftHandLocalPos = (Vector3)stream.ReceiveNext();
            Quaternion leftHandRot = (Quaternion)stream.ReceiveNext();

            Vector3 rightHandLocalPos = (Vector3)stream.ReceiveNext();
            Quaternion rightHandRot = (Quaternion)stream.ReceiveNext();

            calculatePosition(globalPosition, localPosition, localRotation, leftHandLocalPos, leftHandRot, rightHandLocalPos, rightHandRot);
        }

    }

    /// <summary>
    /// Calculates and sets the position of remote user head and left and right controllers
    /// </summary>
    /// <param name="globalPos">global position of user</param>
    /// <param name="localPos">local position of the head</param>
    /// <param name="localRot">local rotation of the head</param>
    /// <param name="leftHandLocalPos">local position of left controller </param>
    /// <param name="leftHandRot">rotation of left controller</param>
    /// <param name="rightHandLocalPos">local position of right controller</param>
    /// <param name="rightHandRot">roation of right controller</param>
    private void calculatePosition(Vector3 globalPos, Vector3 localPos, Quaternion localRot, Vector3 leftHandLocalPos, Quaternion leftHandRot, Vector3 rightHandLocalPos, Quaternion rightHandRot)
    {

        //to display the users position correctly we need to consider both global (movement by controller) and local (movement of head) position
        playerGlobal.position = globalPos;

        playerLocal.position = globalPos + localPos;
        playerLocal.rotation = localRot;

        //controller position is dependend on global position of the user
        //don't add the local position of the user, otherswise the controllers will follow his head movment/rotation
        leftHand.transform.position = leftHandLocalPos + globalPos;
        leftHand.transform.rotation = leftHandRot;

        rightHand.transform.position = rightHandLocalPos + globalPos;
        rightHand.transform.rotation = rightHandRot;
    }

}
