/*
This code was written in C# with Unity

This is a small part of the source code for my Gravity Manipulation Platformer demo
This sample file is the GravityManipulation script that allows
the player to reverse gravity at will with a click of the mouse.
As well as reversing global gravity the script also spherically interpolates
the player controller to smoothly animate a 180 flip motion along the z axis,
and uses dot product to identify if the controller is perfectly upright or upside-down.

https://github.com/CallMeCatharsis
hirobadams@gmail.com
*/

using UnityEngine;
using System.Collections;

public class GravityManipulation : MonoBehaviour {
	// GameObject-centric gravity manipulation can be achieved with this psuedo-gravity variable
	// Using this variable means that the gravity effect will only affect the game object that the
	// script is attached to. The only problem is that this method of gravity manipulation causes
	// different bugs with the Collision detection functions for walking that do not happen with
	// global gravity manipulation, so it is currently not used.
	//float gravity = -9.8f;

	// Creating a vector variable to hold values for a velocity variable that can be used to instantly
	// modify the player's velocity. This can allow for gradual or instant animations/transitions
	// between the player falling/rising. It is currently not used as not resetting the velocity
	// with this temporary variable makes for a smooth height transition, rather than a kind of
	// gravity-snapping effect when instantly resetting the player's velocty to 0
	// Vector3 playerVelocity;

	// Vector for modifying global physics -9.81
	Vector3 newGravity = new Vector3(0.0f, -9.81f, 0.0f);

	// Variables for character rotation
	Quaternion startAngle;
	Quaternion targetAngle;
	Vector3 targetEuler;
	float currentInterpolationTime;
	float maxInterpolationTime;
	float interpolationTime;
	public bool flip;

	// Use this for initialization
	void Start () {
		// Variables for constant Slerp time: 2.5 achieves a nice slow Slerp
		// These are used to rotate the player controller to face the correct orientation when
		// switching gravity, as the player controller is slerped into the new angle
		// i.e upside-down or down-side up
		// The start and target angle variables represent the desired rotation based on the player's
		// current orientation within the level
		currentInterpolationTime = 2.5f;
		maxInterpolationTime = 2.5f;
		startAngle = new Quaternion (0.0f, 0.0f, 0.0f, 1.0f);
		targetAngle = new Quaternion (0.0f, 0.0f, 0.0f, 1.0f);
		startAngle = Quaternion.Euler (0.0f, 0.0f, 0.0f);
		targetAngle = Quaternion.Euler (0.0f, 0.0f, 180f);
	}

	// Update is called once per frame
	void Update ()
	{
		// Since all other updates are physics-based, the only logic in this function is for the
		// gravity reversing ability that is activated when the player presses left mouse click
		if (Input.GetMouseButtonDown (0))
		{
			ReverseGravity ();
		}
	}

	// Called once per frame, but fixed with deltatime for physics updates
	void FixedUpdate ()
	{
		/* Currently unused code for the GameObject-centric gravity
		playerVelocity.y += gravity * Time.fixedDeltaTime;
		GetComponent<Rigidbody>().velocity = playerVelocity;
		*/

		// Only increment the interpolation ratio if it has not fully interpolated
		if (currentInterpolationTime != maxInterpolationTime)
		{
			currentInterpolationTime += 0.01f;
		}

		// Character Rotation Interpolation at a constant rate of 0.7
		// flip defines whether the player is upside-down or down-side up
		// It was made when initially trying to achieve a single flip to the upside down orientation
		// but since the player can now flip back and forth in this version, this variable needs to be renamed
		// as it is very ambiguous just being called "flip". "Upsidedown" would be a far better name
		if (flip == true && currentInterpolationTime < 0.7f)
		{
			gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targetAngle, currentInterpolationTime / maxInterpolationTime);
		}
		if (flip == false && currentInterpolationTime < 0.7f)
		{
			gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, startAngle, currentInterpolationTime / maxInterpolationTime);
		}

		// Shiny new dot product code to identify if player has finished interpolating to a fully upright position
		// Only re-enables camera rotation when game object is either fully right-side up or upside down
		// When using dot product to compare an object's transform up component to the absolute Vector3.up value
		// -1 will mean it's upside down, 0 will mean it's horizontal and 1 will mean that it's right-side up
		// This allows the following code to re-enable the controller's rotation once it has finished interpolating
		if ((Vector3.Dot(gameObject.transform.up, Vector3.up) == -1)
			|| Vector3.Dot(gameObject.transform.up, Vector3.up) == 1)
			{
				gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController> ().rotation = true;
			}
	}

	void ReverseGravity ()
	{
		/* Currently unused transition-snapping and gameObject-centric gravity code
		// Smooth or instant Transition?
		// Setting to 0 makes it instant
		//playerVelocity.y = 0;
		//GetComponent<Rigidbody>().velocity = playerVelocity;
		// Personal gravity
		//gravity = -gravity;
		*/

		// Universal gravity
		newGravity.y *= -1.0f;
		Physics.gravity = newGravity;

		// Flip Character upside down or back up based on dot product of up vs down vectors
		// When compared against Vector3.Up:
		// if the dot product is 1, the character is right-side up, if it's 0 then they're horizontal and if it's -1 they're upside down
		// Object is currently upside down, so flip it back to normal
		if (Vector3.Dot(gameObject.transform.up, Vector3.down) > 0)
		{
			flip = false;
			Invoke ("saveRotation", 1.2f);
		}
		// Object is currently normalside up so flip it upside down: 0.5
		if (Vector3.Dot(gameObject.transform.up, Vector3.down) < 0.5)
		{
			flip = true;
			Invoke ("saveRotation", 1.2f);
		}

		// Reset interpolation time back to 0
		currentInterpolationTime = 0.0f;
		// Temporarily disable camera rotation during character interpolation
		gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController> ().rotation = false;
	}

	void saveRotation ()
	{
		gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController> ().mouseLook.m_CharacterTargetRot = gameObject.transform.localRotation;
	}

	/// NOTES/THINGS TO DO
	///
	///
	/// Character and any objects with this script currently interpolate upside-down to normal-side up pretty much perfectly
	/// This script also contains code for gameobject specific gravity, which is much better, but for some reason
	/// it stops the character controller from being able to walk whereas modifying global gravity does not
	///
	/// - Controller is always rotated to face the same direction, y axis should always remain on same rotation
	///   the only axis that needs to be rotated is z. Needs to be modified to not change the y axis
	/// - Would be better if camera rotation got enabled earlier or if the camera rotation didn't actually need to
	///   get turned off at all. Disabling the camera movement definitely harms game-feel
	/// - rename/refactor boolean flip variable
	/// - Fix gameObject-centric gravity. Manipulating global gravity every time is very extreme.
	///
}
