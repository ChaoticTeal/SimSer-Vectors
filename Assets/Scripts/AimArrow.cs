﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimArrow : MonoBehaviour
{
    // Serialize fields - for assignment in the editor
    // Time to restrict movement and display data
    [SerializeField] float delay;
    // Slider for visual representation of vector
    [SerializeField] Slider slider;
    // Text for data display
    [SerializeField] Text dataText;
    // Target position
    [SerializeField] Transform target;

    // Private fields
    // Has the vector just been drawn?
    bool cooldown = false;
    // Should the vector be drawn?
    bool shouldFire;
    // Angle between mouse position and default position
    float angleDifference;
    // Vertical vector
    Vector2 defaultPosition = new Vector2(0f, 1f);

    // Properties
    public bool Cooldown
    {
        get { return cooldown; }
    }
	
	// Update is called once per frame
	void Update () 
	{
        // Handle input
        HandleInput();
        if (!cooldown)
        {
            // Rotate the arrow to face the mouse
            Rotate();
            if(shouldFire)
            // "Fire" the vector
                Fire();
        }
	}

    // Fire the vector
    void Fire()
    {
        slider.value = target.position.magnitude;
        dataText.text = "Vector: <" + target.position.x / 2 + "," + target.position.y / 2 + ">\nMagnitude: " + target.position.magnitude +
            "\nDirection: " + (Vector2.SignedAngle(defaultPosition, target.position) + 90f) + "\u00B0";
        StartCoroutine(VectorCooldown());
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
            shouldFire = true;
        else
            shouldFire = false;
    }

    // Rotates the arrow towards the mouse
    void Rotate()
    {
        // Calculate angle between the target position and a vertical default vector
        angleDifference = Vector2.SignedAngle(defaultPosition, target.position);
        // Constrain angle between -90 and 90 degrees
        angleDifference = Mathf.Min(angleDifference, 90f);
        angleDifference = Mathf.Max(angleDifference, -90f);
        // Rotate to the calculated angle
        if(transform.rotation.z * Mathf.Rad2Deg <= 90f && transform.rotation.z * Mathf.Rad2Deg >= -90f)
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angleDifference));
    }

    IEnumerator VectorCooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(delay);
        cooldown = false;
        dataText.text = "";
        slider.value = 0;
    }
}
