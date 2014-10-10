using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

	// Use this for initialization
	public ParticleSystem ps;
	public static float cd = 5f;
	public static float last = 0;
	// Use this for initialization
	void Start () {
		ps = this.GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(ps)
		{
			if(ps.time >= 0.1f){
				gameObject.GetComponent<Light>().enabled = false;
			}
			
			if(!ps.IsAlive())
			{
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}
}
