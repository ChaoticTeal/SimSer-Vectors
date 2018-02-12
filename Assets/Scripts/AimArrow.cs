using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimArrow : MonoBehaviour
{
    // Serialize fields - for assignment in the editor
    // Target position
    [SerializeField] Transform target;

    // Private fields
    // Angle between mouse position and default position
    float angleDifference;
    // Vertical vector
    Vector2 defaultPosition = new Vector2(0f, 1f);
	
	// Update is called once per frame
	void Update () 
	{
        // Rotate the arrow to face the mouse
        Rotate();
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
}
