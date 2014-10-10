using UnityEngine;
using System.Collections;

public class Flame : MonoBehaviour {

	public ParticleSystem ps;
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
