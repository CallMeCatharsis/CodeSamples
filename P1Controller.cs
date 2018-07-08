/*
This code was written in C# with Unity

This is a small part of the source code for Smack N Crack
This sample file is the player controller script that allows
the player to move their paddle and fire projectiles.
It moves the paddle with a physics-based movement system and
contains logic that prevents the player from moving off the screen
and for reacting to the collision of enemy projectiles.

https://github.com/RobAdamsDev
hirobadams@gmail.com
*/

﻿using UnityEngine;
using System.Collections;

public class P1Controller : MonoBehaviour {

	// Vector variable for holding a copy of the paddle velocity
	Vector3 PlayerVelocity;
	// Variables to store the locations of the screen boundaries
	float TopEdge;
	float BottomEdge;
	// Public variables to store the projectiles and their spawn location
	public GameObject Projectile;
	public GameObject Spawner;

	// Array to reference number of active projectiles
	private GameObject[] Projectiles;

	// Variable to store fire rate information
	public float FireRate = 1f;
	private float NextFire = 0.0f;

	// Variable to enable/disable player control
	public bool ControlEnabled;

	// Use this for initialization
	void Start ()
	{
		// Makes sure the object's z value matches the camera's z value, I think?
		// So the object's plane from the camera is the same when computing viewport to world space
		float ObjectDistance = (GetComponent<Transform> ().position - Camera.main.transform.position).z;
		// Get float variables to reference the world space locations of the top and bottom screen edges
		// The x and y of the provided vector are screen co-ordinates, and the z is the distance of the plane
		// Still kinda unsure as to how this whole conversion process works
		TopEdge = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, ObjectDistance)).y;
		BottomEdge = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, ObjectDistance)).y;
	}

	void Update()
	{
		// Can't directly mess with the rigidbody's velocity, so we copy and paste changes using PlayerVelocity
		PlayerVelocity = GetComponent<Rigidbody2D> ().velocity;
		// Update projectile array to accurately represent number of active projectiles
		Projectiles = GameObject.FindGameObjectsWithTag("P1Projectile");
	}

	// Update is called once per frame
	// The movement is done in FixedUpdate due to it being physics based
	void FixedUpdate ()
	{

		// Update the player physics, but only if the game mode has been chosen
		if (ControlEnabled)
		{
			// Use the S and X keys to vertically control the paddle, but make sure it never goes above 30 velocity!
			if (Input.GetKey (KeyCode.S) && GetComponent<Rigidbody2D> ().velocity.y < 30)
			{
				// Add a positive physical force value to the Rigidbody's Y component
				GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0f, 1f), ForceMode2D.Impulse);
			}

			if (Input.GetKey (KeyCode.X) && GetComponent<Rigidbody2D> ().velocity.y > -30)
			{
				// Add a positive physical force value to the Rigidbody's Y component
				GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0f, -1f), ForceMode2D.Impulse);
			}

			// The v key will fire a projectile as long as there aren't 5 active projectiles
			// and as long as the FireRate cooldown timer has elapsed
			if (Input.GetKey (KeyCode.V) && Projectiles.Length < 5 && Time.time > NextFire)
			{
				NextFire = Time.time + FireRate;
				Instantiate (Projectile, Spawner.transform.position, Projectile.transform.rotation);
			}

			// Bounce the paddle back using edge locations rather than screen co-ordinates
			// Check based on the object's position plus its height, so as to reference the very top of the object
			if (GetComponent<Transform> ().position.y + GetComponent<SpriteRenderer> ().bounds.extents.y >= TopEdge)
			{
				PlayerVelocity.y *= -1f;
				GetComponent<Rigidbody2D> ().velocity = PlayerVelocity;
			}

			// Do the same, but for the bottom edge of the screen
			if (GetComponent<Transform> ().position.y - GetComponent<SpriteRenderer> ().bounds.extents.y <= BottomEdge)
			{
				PlayerVelocity.y *= -1f;
				GetComponent<Rigidbody2D> ().velocity = PlayerVelocity;
			}
		}
	}

	// Collision function for checking if player has been hit by a projectile
	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "P2Projectile" && GameObject.Find ("Player 2").activeSelf == true)
		{
			// Deactivate the player's paddle if they've been hit by player 2's projectile
			gameObject.SetActive (false);
		}
	}
}
