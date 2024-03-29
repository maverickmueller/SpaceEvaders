﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
//HI Caleb
public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.


	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	//public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	//public AudioClip[] taunts;				// Array of clips for when the player taunts.
	public float tauntProbability = 50f;	// Chance of a taunt happening.
	public float tauntDelay = 1f;			// Delay for when the taunt should happen.

	private float prevGravity;


	//private int tauntIndex;					// The index of the taunts array indicating the most recent taunt.
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private Transform ladderCheckRight;
	private Transform ladderCheckLeft;
	private Transform ladder;
	private Terminal terminal;
	private bool grounded = false;			// Whether or not the player is grounded.
	private bool byLadder = false;			// Whether or not the player is standing by a ladder
	private bool climbing = false;			// Whether or not the player is actively climbing
	//private Animator anim;					// Reference to the player's animator component.


	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		ladderCheckRight = transform.Find("ladderCheckRight");
		ladderCheckLeft = transform.Find("ladderCheckLeft");
		prevGravity = GetComponent<Rigidbody2D>().gravityScale;
		//anim = GetComponent<Animator>();
	}

	void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

		if(terminal)
		{
			if(Input.GetButtonDown("Fire2"))
			{
				terminal.Use();
			}
		}

		if(!climbing)
		{
			RaycastHit2D hitLadder = Physics2D.Linecast(transform.position, ladderCheckLeft.position, 1 << LayerMask.NameToLayer("Ladder"));
			if(hitLadder)
			{
				ladder = hitLadder.transform;
				byLadder = true;
			}
			else 
			{
				hitLadder = Physics2D.Linecast(transform.position, ladderCheckRight.position, 1 << LayerMask.NameToLayer("Ladder"));
				if(hitLadder)
				{
					ladder = hitLadder.transform;
					byLadder = true;
				}
				else
				{
					byLadder = false;
				}
			}
		}

		if (!climbing && byLadder && Input.GetButtonDown("Vertical"))
		{
			climbing = true;
			GetComponent<Rigidbody2D>().gravityScale = 0;
			GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
			GetComponent<Rigidbody2D>().position = new Vector2(ladder.position.x, GetComponent<Rigidbody2D>().position.y);
		}

		// If the jump button is pressed and the player is grounded then the player should jump.
		if(Input.GetButtonDown("Jump") && (grounded || climbing))
		{
			jump = true;
			climbing = false;
			GetComponent<Rigidbody2D>().gravityScale = prevGravity;
		}
	}


	void FixedUpdate ()
	{
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal");

		float v = Input.GetAxis("Vertical");

		// The Speed animator parameter is set to the absolute value of the horizontal input.
		//anim.SetFloat("Speed", Mathf.Abs(h));
		if (!climbing)
		{
			// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
			if(h * GetComponent<Rigidbody2D>().velocity.x < maxSpeed)
				// ... add a force to the player.
				GetComponent<Rigidbody2D>().AddForce(Vector2.right * h * moveForce);

			// If the player's horizontal velocity is greater than the maxSpeed...
			if(Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > maxSpeed)
				// ... set the player's velocity to the maxSpeed in the x axis.
				GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

			// If the input is moving the player right and the player is facing left...
			if(h > 0 && !facingRight)
				// ... flip the player.
				Flip();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if(h < 0 && facingRight)
				// ... flip the player.
				Flip();

			if(GetComponent<Rigidbody2D>().position.y < -100)
				Death();
		}
		else
		{
			//climbing mechanics

			if(GetComponent<Rigidbody2D>().position.y > ladder.up.y && v > 0)// + ladder.position.y)
				GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
			else
			{

				if(v * GetComponent<Rigidbody2D>().velocity.y < maxSpeed)
					// ... add a force to the player.
					GetComponent<Rigidbody2D>().position = new Vector2(GetComponent<Rigidbody2D>().position.x, GetComponent<Rigidbody2D>().position.y + v/7);//AddForce(Vector2.up * v * moveForce);

				// If the player's vertical velocity is greater than the maxSpeed...
				if(Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y) > maxSpeed)
					// ... set the player's velocity to the maxSpeed in the x axis.
					GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, Mathf.Sign(GetComponent<Rigidbody2D>().velocity.y) * maxSpeed);
			}
			//if player's y is >= ladder's top y, then velocity = 0
		}

		// If the player should jump...
		if(jump)
		{
			// Set the Jump animator trigger parameter.
			//anim.SetTrigger("Jump");

			// Play a random jump audio clip.
			//int i = Random.Range(0, jumpClips.Length);
			//AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

			// Add a vertical force to the player.
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}
	}
	
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void NearTerminal(Terminal newTerminal)
	{
		terminal = newTerminal;
		Debug.Log("Near terminal");
	}

	public void LeftTerminal()
	{
		terminal = null;
		Debug.Log("Left terminal");
	}

	public void Death()
	{
		Debug.Log("ded :(");
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

/*
	public IEnumerator Taunt()
	{
		// Check the random chance of taunting.
		float tauntChance = Random.Range(0f, 100f);
		if(tauntChance > tauntProbability)
		{
			// Wait for tauntDelay number of seconds.
			yield return new WaitForSeconds(tauntDelay);

			// If there is no clip currently playing.
			if(!GetComponent<AudioSource>().isPlaying)
			{
				// Choose a random, but different taunt.
				tauntIndex = TauntRandom();

				// Play the new taunt.
				GetComponent<AudioSource>().clip = taunts[tauntIndex];
				GetComponent<AudioSource>().Play();
			}
		}
	}


	int TauntRandom()
	{
		// Choose a random index of the taunts array.
		int i = Random.Range(0, taunts.Length);

		// If it's the same as the previous taunt...
		if(i == tauntIndex)
			// ... try another random taunt.
			return TauntRandom();
		else
			// Otherwise return this index.
			return i;
	}*/
}
