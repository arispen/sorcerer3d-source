using UnityEngine;

public class RandomMatchmaker : Photon.MonoBehaviour
{
    //private PhotonView myPhotonView;
	bool joined = false;			
    // Use this for initialization
    void Start()
	{	
        //PhotonNetwork.ConnectUsingSettings("0.1");
    }

    void OnJoinedLobby()
    {
       
		PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
		RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 2 };
		PhotonNetwork.CreateRoom("",roomOptions,TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        GameObject monster = PhotonNetwork.Instantiate("skeleton", Vector3.zero, Quaternion.identity, 0);

        
		monster.GetComponent<myThirdPersonController>().isControllable = true;
        



    }

    void OnGUI()
    {

		if ( !joined && GUI.Button(new Rect((Screen.width-200)/2, (Screen.height-50)/2, 200, 50), "Join random 1v1 match")){
		PhotonNetwork.ConnectUsingSettings("0.1");
			joined=true;
		}
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        
    }

		

}
