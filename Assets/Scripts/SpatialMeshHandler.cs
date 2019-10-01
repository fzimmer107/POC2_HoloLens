using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class SpatialMeshHandler : MonoBehaviourPun, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>
{
    private PhotonView RPCPhotonView;
    private Dictionary<int, int> meshDataUpdates;
    private Camera holoCamera;

    void Start()
    {
        RPCPhotonView = GetComponent<PhotonView>();
        meshDataUpdates = new Dictionary<int, int>();
        holoCamera = Camera.main;

    }

    //for accessing SpatialAwarenessSystem
    private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;

    //for accessing DataProvider
    private IMixedRealityDataProviderAccess dataProviderAccess = null;

    //for accessing MeshObserver
    public IMixedRealitySpatialAwarenessMeshObserver meshObserver = null;

    public IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem
    {
        get
        {
            if (spatialAwarenessSystem == null)
            {
                MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out spatialAwarenessSystem);
            }
            return spatialAwarenessSystem;
        }
    }

    public IMixedRealityDataProviderAccess DataProviderAccess
    {
        get
        {
            if (dataProviderAccess == null)
            {
                dataProviderAccess = spatialAwarenessSystem as IMixedRealityDataProviderAccess;
            }
            return dataProviderAccess;
        }
    }

    private IMixedRealitySpatialAwarenessMeshObserver MeshObserver
    {
        get
        {
            if (dataProviderAccess != null)
                meshObserver = dataProviderAccess.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
            else
                Debug.Log("DataAccessProvider is null");
            return meshObserver;
        }
       
    }

    private async void OnEnable()
    {
        await new WaitUntil(() => SpatialAwarenessSystem != null);
        SpatialAwarenessSystem.Register(gameObject);
        await new WaitUntil(() => DataProviderAccess != null);
        await new WaitUntil(() => MeshObserver != null);     
    }

    private void OnDisable()
    {
        SpatialAwarenessSystem?.Unregister(gameObject);

    }

  
    public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData)
    {
        if (!meshDataUpdates.ContainsKey(eventData.Id))
        {

            meshDataUpdates.Add(eventData.Id, 0);

        } 

    }

    public void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData)
    {
        
        if(meshDataUpdates.ContainsKey(eventData.Id))
            {

            RPCPhotonView.RPC("RemoveMesh", RpcTarget.Others, eventData.Id);          
            meshDataUpdates.Remove(eventData.Id);
            }
            
    }

    public void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData)
    {
        // RPCPhotonView.RPC("RPCTest", RpcTarget.Others, meshObserver.Meshes[eventData.Id].GameObject.transform.position, eventData.Id);
        // RPCPhotonView.RPC("SendError", RpcTarget.Others, eventData.SpatialObject.GameObject.transform.position, eventData.Id);




        int updateCount = 0;

           if(meshDataUpdates.TryGetValue(eventData.Id, out updateCount))
           {
               meshDataUpdates[eventData.Id] = ++updateCount;
                
               if ((updateCount % 2) == 0)
               {
                GameObject gameObject = meshObserver.Meshes[eventData.Id].GameObject;
                RPCPhotonView.RPC("RecieveMeshData", RpcTarget.Others, gameObject.GetComponent<MeshFilter>().mesh.vertices, gameObject.GetComponent<MeshFilter>().mesh.triangles,
                  gameObject.GetComponent<MeshFilter>().mesh.uv, gameObject.transform.position, gameObject.transform.eulerAngles, eventData.SpatialObject.Id, holoCamera.transform.position, holoCamera.transform.eulerAngles);
            }
        }  
    }



    [PunRPC]
    public void SendMeshData()
    {

    }

}
