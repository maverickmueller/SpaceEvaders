using UnityEngine;
using System.Collections;

public class Sprinkler : MonoBehaviour {
	
	public void Use()
	{
		RaycastHit2D raycastGround = Physics2D.Linecast(transform.position, new Vector2(transform.position.x, transform.position.y - 100), 1 << LayerMask.NameToLayer("Ground"));
		Vector2 ground = raycastGround.point;
		RaycastHit2D raycastFire = Physics2D.Linecast(transform.position, ground, 1 << LayerMask.NameToLayer("Fire"));
		if(raycastFire)
		{
			Destroy(raycastFire.transform.gameObject);
		}
		Debug.Log("spraying ground at " + ground.x + ", " + ground.y);
	}
}
