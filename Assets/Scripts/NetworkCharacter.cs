using UnityEngine;

public class NetworkCharacter : Photon.MonoBehaviour
{
    private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
	public int Hp = 100;
	//private GameObject hpBar = null;
	public bool dead = false;
	public GameObject camera;
	public GameObject hpBar;
	public static int MyId = 1;
    // Update is called once per frame
	void Start()
	{
		camera = GameObject.FindWithTag("MainCamera");

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players)
		{
			if(this.gameObject.collider!= player.collider){
				Physics.IgnoreCollision(this.gameObject.collider, player.collider);
			}

			
		}



		if (photonView.isMine) {
			MyId = photonView.viewID;

		}

		hpBar = PhotonNetwork.Instantiate("health_bar", transform.position + new Vector3(0,1,0), Quaternion.Euler(45,0,0), 0);
	}
    void Update()
    {
		Plane hPlane = new Plane(Vector3.up, new Vector3(0,0.5f,0));
		Vector3 playerCenter = transform.position + new Vector3(0,0.5f,0) + transform.forward*0.5f;
		float distance = 0; 

        if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
        }
		else
		{//nasz obiekt


			camera.transform.position = transform.position 
				+ new Vector3(0,
				              camera.GetComponent<MainCamera>().yOffset,
								camera.GetComponent<MainCamera>().zOffset);
			//####fireball
			if(!dead){
				//Debug.Log(Time.time - Fireball.lastfb);
				if(Time.time - Fireball.last > Fireball.cd){


					Ray ray = camera.camera.ScreenPointToRay(Input.mousePosition);
					if(Input.GetMouseButtonDown(0)){
						if (hPlane.Raycast(ray, out distance)){
							// get the hit point:
							Vector3 shootPoint = ray.GetPoint(distance)-playerCenter;
							shootPoint = Vector3.Normalize(shootPoint);
							
							Quaternion fireballDir = Quaternion.LookRotation(Quaternion.Euler(0, 180, 0) * shootPoint); //obrót fireballa

							//Attack ();
							GameObject fb = PhotonNetwork.Instantiate("fireball",playerCenter,fireballDir,0) ;
							fb.GetComponent<Rigidbody>().velocity = shootPoint * Fireball.speed; //szybkosc fireballa
						}
						Fireball.last = Time.time;
					}


				}
				//###tele
				if(Time.time - Portal.last > Portal.cd){
					if(Input.GetMouseButtonDown(1)){
						Ray ray = camera.camera.ScreenPointToRay(Input.mousePosition);

						if (hPlane.Raycast(ray, out distance)){
							// get the hit point:
							Vector3 shootPoint = ray.GetPoint(distance);
							PhotonNetwork.Instantiate("portal",shootPoint,Quaternion.identity,0) ;
							gameObject.transform.position = shootPoint + new Vector3(0,-0.5f,0);
						}
						Portal.last = Time.time;
					}
				}
				//####tele
			}
			else{ //nie żyjemy

				this.SendMessage("Disable");

			}
			//####fireball

		

		}

		Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position + new Vector3(0,1.2f,0));
		hpBar.transform.position = screenPos;
		float hp2 = Hp / 2;
		hpBar.guiTexture.pixelInset = new Rect (hpBar.guiTexture.pixelInset.x,
		                                        hpBar.guiTexture.pixelInset.y,
		                                        hp2,
		                                        hpBar.guiTexture.pixelInset.height);//działą?
		
		
	}
	void Attack(){


		//animation["attack"].wrapMode = WrapMode.ClampForever;
		animation.Play("attack");

	}
	void LateUpdate(){

		//if (photonView.isMine){
			if(Hp <= 0){
				animation["die"].wrapMode = WrapMode.ClampForever;
				animation.Play("die");
				dead = true;
				//Game.Die(PhotonNetwork.player.ID);
			}
		//}
		if (Hp <= 0) {
			Hp = 0;
		}





	}
	void OnGUI() {
		/*if (photonView.isMine) {
			GUI.Label (new Rect (50, 50, 100, 20), "HP: " + Hp);
		}*/

		if(photonView.isMine){
			if ( dead && GUI.Button(new Rect((Screen.width-200)/2, (Screen.height-50)/2, 200, 50), "YOU LOSE. Play again?")){
				PhotonNetwork.Disconnect();
				Application.LoadLevel(0);
			}
		}
		else{
			if(dead && GUI.Button(new Rect((Screen.width-200)/2, (Screen.height-50)/2, 200, 50), "YOU WIN. Play again?")){
				PhotonNetwork.Disconnect();
				Application.LoadLevel(0);
			}
		}


	}
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

            myThirdPersonController myC = GetComponent<myThirdPersonController>();
            stream.SendNext((int)myC._characterState);

			stream.SendNext(Hp);
		}
        else
        {
            // Network player, receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();

            myThirdPersonController myC = GetComponent<myThirdPersonController>();
            myC._characterState = (CharacterState)stream.ReceiveNext();

			Hp = (int)stream.ReceiveNext();
        }
    }
	/*void NewGame(){
		Hp = 100;
		dead = false;
		this.SendMessage("Enable");
	}*/
}
