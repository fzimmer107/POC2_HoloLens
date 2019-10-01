using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class CustomPhotonLauncher : MonoBehaviourPunCallbacks, IOnEventCallback
{

    string gameVersion = "1";
    public const byte InstantiateVrAvatarEventCode = 1;

    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.LocalPlayer.NickName = "HoloLens";
        }
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }


    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }



    public override void OnConnectedToMaster()
    {
        Debug.Log(" OnConnectedToMaster() was called by PUN");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("POnDisconnected() was called by PUN with reason {0}", cause);
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
       Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }


    /// <summary>
    /// this Method was written with help of this tutorial: https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/oculusavatarsdk
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
        PhotonView playSpacePhotonView = GameObject.Find("MixedRealityPlayspace").AddComponent<PhotonView>();

        //we need to add PhotonView to existing GameObject MixedRealityPlaySpace, so the cameras position can be observed
        playSpacePhotonView.ObservedComponents = new List<Component>();
        playSpacePhotonView.ObservedComponents.Add(GameObject.Find("MixedRealityPlayspace").GetComponent<HoloLensLocal>());
        playSpacePhotonView.Synchronization = ViewSynchronization.UnreliableOnChange;

        if (PhotonNetwork.AllocateViewID(playSpacePhotonView))
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
        };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(InstantiateVrAvatarEventCode, playSpacePhotonView.ViewID, raiseEventOptions, sendOptions);
        }

        else
        {
            Debug.Log("Failed to allocate a ViewId");
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == InstantiateVrAvatarEventCode)
        {    
            //this client only instantiates VRAvatars
            GameObject remotAvatar = Instantiate(Resources.Load("VRRemoteAvatar"), new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            PhotonView photonView = remotAvatar.GetComponent<PhotonView>();
            photonView.ViewID = (int)photonEvent.CustomData;
        }
    }


}
