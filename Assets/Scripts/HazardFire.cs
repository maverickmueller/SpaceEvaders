using UnityEngine;
using System.Collections;

public class HazardFire : MonoBehaviour {

	private PlayerControl playerScript;

	void Awake()
	{
		GameObject player = GameObject.Find("player");
		playerScript = (PlayerControl) player.GetComponent(typeof(PlayerControl));
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.name == "player")
		{
	        playerScript.Death();
	    }
    }

    public void Destroy()
    {
    	Destroy(gameObject);
    }
}
