using UnityEngine;
using System.Collections;

public class Terminal : MonoBehaviour {

	private PlayerControl playerScript;
	public Sprinkler[] sprinklers;

	void Awake()
	{
		GameObject player = GameObject.Find("player");
		playerScript = (PlayerControl) player.GetComponent(typeof(PlayerControl));
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.name == "player")
		{
	        playerScript.NearTerminal(this);
	    }
    }

    void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.name == "player")
		{
	        playerScript.LeftTerminal();
	    }
    }

    public void Use()
    {
    	Debug.Log("DID THA THING");
    	for(int i=0; i < sprinklers.Length; i++)
    	{
    		sprinklers[i].Use();
    	}
    }
}
