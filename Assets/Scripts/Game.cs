using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	private static PhotonView GamePhotonView;
	// Use this for initialization
	void Start () {
		GamePhotonView = this.GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();

		}
		/*if(Input.GetKeyDown(KeyCode.Return)){
			PhotonView.Find (NetworkCharacter.MyId).gameObject.SendMessage("NewGame");
		}*/
	}
	public static void DealDamage(int taker, int value)
	{

		GamePhotonView.RPC("DealtDamage", PhotonTargets.All, taker, value);
	}
	[RPC]
	public void DealtDamage(int taker, int value){
		if (taker == PhotonNetwork.player.ID) {
			PhotonView.Find (NetworkCharacter.MyId).gameObject.GetComponent<NetworkCharacter> ().Hp -= value;
		}
	}


}
