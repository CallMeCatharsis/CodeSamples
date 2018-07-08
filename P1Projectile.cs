/*
This code was written in C# with Unity

This is a small part of the source code for Smack N Crack
This sample file is the script behind the game's projectiles.
It moves the projectiles based on their rigidbody physics,
bounces them off the top and bottom edges of the screen and deletes
them when they leave the screenspace from either the left or right edges.
With the inverse tangent and quaternion spherical linear interpolation
functions, the projectile smoothly turns to face the direction it is travelling.

https://github.com/RobAdamsDev
hirobadams@gmail.com
*/

﻿using UnityEngine;
using System.Collections;

public class P1Projectile : MonoBehaviour {

	// Vector variable for holding a copy of the projectile velocity
	Vector3 ProjectileVelocity;
	// Variables to store the locations of the screen boundaries
	float TopEdge;
	float BottomEdge;
	float RightEdge;
	float LeftEdge;

	// Use this for initialization
	void Start () {
		// Set the object's plane at equal depth to the camera
		float ObjectDistance = (GetComponent<Transform> ().position - Camera.main.transform.position).z;
		// Transform the viewport space of the screen edges into world space and stores it in the float variables
		TopEdge = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, ObjectDistance)).y;
		BottomEdge = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, ObjectDistance)).y;
		RightEdge = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, ObjectDistance)).x;
		LeftEdge = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, ObjectDistance)).x;

		// Assign the player paddle's velocity to new vector
		Vector2 VerticalForce = GameObject.Find("Player 1").GetComponent<Rigidbody2D> ().velocity;
		// Divide the inherited vertical force by 2 because it's too extreme
		VerticalForce.y /= 2f;
		// Add a force to the projectile as soon as it spawns
		GetComponent<Rigidbody2D> ().AddForce (new Vector2 (10f, 0f), ForceMode2D.Impulse);
		// and Add a vertical force equal to the paddle's velocity
		GetComponent<Rigidbody2D> ().AddForce (VerticalForce, ForceMode2D.Impulse);
	}

	// Update is called once per frame
	void Update () {
		// Update the velocity vector with the rigidbody's velocity
		ProjectileVelocity = GetComponent<Rigidbody2D> ().velocity;

		// Check to see if the projectile has left the screen, if so, destroy it
		// Projectiles are designed to collide and bounce off the top and bottom edges, but
		// they are still destroyed past these edges as a failsafe
		if (GetComponent<Transform> ().position.x - GetComponent<SpriteRenderer> ().bounds.extents.x >= RightEdge
		   || GetComponent<Transform> ().position.x + GetComponent<SpriteRenderer> ().bounds.extents.x <= LeftEdge
		   || GetComponent<Transform> ().position.y - GetComponent<SpriteRenderer> ().bounds.extents.y >= TopEdge
		   || GetComponent<Transform> ().position.y + GetComponent<SpriteRenderer> ().bounds.extents.y <= BottomEdge)
		{
			Destroy (gameObject);
		}
	}

	// Update for physics
	void FixedUpdate()
	{
		// Bounce using edge locations rather than screen co-ordinates
		// Check based on the object's position plus its height, so as to reference the very top of the object
		if (GetComponent<Transform> ().position.y + GetComponent<SpriteRenderer> ().bounds.extents.y >= TopEdge)
		{
			ProjectileVelocity.y *= -1f;
			GetComponent<Rigidbody2D> ().velocity = ProjectileVelocity;
		}

		// Do the same for the bottom edge of the object
		if (GetComponent<Transform> ().position.y - GetComponent<SpriteRenderer> ().bounds.extents.y <= BottomEdge)
		{
			ProjectileVelocity.y *= -1f;
			GetComponent<Rigidbody2D> ().velocity = ProjectileVelocity;
		}

		// Converted from radians to degrees, the variable targetAngle reprents an angle
		// created from the inverse tangent of the rigidbody's velocity in the y and x components
		// The projectile's transform rotation component can then be slerped to the targetAngle
		// to cause the projectile to always face the direction it is travelling in a smooth and fluid animation
		float targetAngle = Mathf.Atan2(GetComponent<Rigidbody2D> ().velocity.y, GetComponent<Rigidbody2D> ().velocity.x) * Mathf.Rad2Deg;
		GetComponent<Transform> ().rotation = Quaternion.Slerp (GetComponent<Transform> ().rotation, Quaternion.Euler (0, 0, targetAngle), 0.5f);
	}

}
