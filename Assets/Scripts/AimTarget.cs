using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTarget : MonoBehaviour 
{
    // Private fields
    // Mouse position in world coordinates
    Vector3 mouseRelativePosition;
	
	// Update is called once per frame
	void Update () 
	{
        Move();
	}

    // Move to the nearest grid point to the mouse cursor
    private void Move()
    {
        // The following code adapted from https://answers.unity.com/questions/540888/converting-mouse-position-to-world-stationary-came.html
        mouseRelativePosition = Input.mousePosition;
        mouseRelativePosition.z = 10.0f; // Camera position
        mouseRelativePosition = Camera.main.ScreenToWorldPoint(mouseRelativePosition);
        // End adapted code

        Vector2 newPosition = new Vector2(mouseRelativePosition.x, mouseRelativePosition.y);

        // Round position to multiples of 2
        // The grid is in Unity units of 2
        if (newPosition.x % 2 != 0)
            newPosition.x = RoundToTwo(newPosition.x);
        if (newPosition.y % 2 != 0)
            newPosition.y = RoundToTwo(newPosition.y);

        transform.position = newPosition;
    }

    // Round the given value to the nearest multiple of 2
    private float RoundToTwo(float f)
    {
        int nearest = (int)(f / 2);
        float grid = f / 2;
        if (grid >= 0 && (grid % 1f) * 10f >= 5f)
            return (nearest + 1) * 2f;
        else if (grid < 0 && (grid % 1f) * 10f <= -5f)
            return (nearest - 1) * 2f;
        else
            return nearest * 2f;
    }
}
