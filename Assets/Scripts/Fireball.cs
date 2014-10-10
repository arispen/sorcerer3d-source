using UnityEngine;
using System.Collections;

public class Fireball : Photon.MonoBehaviour {

	private Vector3 correctPos = Vector3.zero; // We lerp towards this
	private Quaternion correctRot = Quaternion.identity; // We lerp towards this

	public static int damage = 7;
	public static int speed = 5;
	public static float cd = 0.6f;
	public static float last = 0;


	// Use this for initialization
	void Start () {
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in players) {
			if(player.GetPhotonView().owner == gameObject.GetPhotonView().owner){
				player.GetComponent<NetworkCharacter> ().SendMessage ("Attack");
			
			}
		}


	
	}
	
	// Update is called once per frame
	void Update () {

		if (!photonView.isMine)
		{
			transform.position = Vector3.Lerp(transform.position, this.correctPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, this.correctRot, Time.deltaTime * 5);
		}


	}
	void OnTriggerEnter(Collider collider) {

		if (collider.tag == "Terrain") {
				
				PhotonNetwork.Instantiate ("flame", transform.position, transform.rotation, 0);
				PhotonNetwork.Destroy (this.gameObject);
		}
		if (collider.tag == "Player") {
				PhotonView colliderPhotonView = PhotonView.Get (collider.gameObject);
				PhotonView fireballPhotonView = PhotonView.Get (this.gameObject);

				if (!colliderPhotonView.isMine && fireballPhotonView.isMine) { //player nie ja, fireball mój
						Game.DealDamage (colliderPhotonView.ownerId, Fireball.damage);
						PhotonNetwork.Instantiate ("flame", transform.position, transform.rotation, 0);
						PhotonNetwork.Destroy (this.gameObject);

				}
		}
		if (collider.tag == "Fireball") {
			PhotonNetwork.Instantiate ("flame", transform.position, transform.rotation, 0);
			PhotonNetwork.Destroy (this.gameObject);
		}
				
	}



	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			

		}
		else
		{
			// Network player, receive data
			this.correctPos = (Vector3)stream.ReceiveNext();
			this.correctRot = (Quaternion)stream.ReceiveNext();
			

		}
	}

}
