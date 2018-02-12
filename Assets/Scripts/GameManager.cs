using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    // Serialize Fields for assignment in-editor
    // Reference to aim arrow (to check cooldown)
    [SerializeField] AimArrow arrow;
    // Time a target is available
    [SerializeField] float targetTime = 2f;
    // Target
    [SerializeField] GameObject target;
    // Target score (required to continue)
    [SerializeField] int requiredScore = 10;
    // Score text
    [SerializeField] Text scoreText;
    // Assessment text
    [SerializeField] Text assessmentText;
    // Player aim reticle
    [SerializeField] Transform reticle;

    // Private fields
    // Did the player fire?
    bool checkFire;
    // Is there currently a target?
    bool isTarget;
    // Did the shot hit?
    bool hit;
    // Has the win requirement been met?
    bool hasWon;
    // Reference to active target
    GameObject activeTarget;
    // Score
    int score = 0;

	// Use this for initialization
	void Start () 
	{
        UpdateScoreText();
	}
	
	// Update is called once per frame
	void Update () 
	{
        // Check input
        HandleInput();
        // If there isn't a target, make a new one
        if (!isTarget)
            StartCoroutine(ManageTarget());
        // If there is still a target, check whether or not it was hit
        if (activeTarget != null)
            // The target was hit if the reticle and target positions overlap and the player didn't just fire
            hit = activeTarget.transform.position == reticle.position && !arrow.Cooldown;
        // Failing any of the above, the target was not hit
        else
            hit = false;
        // If it actually hit, process the hit
        if (checkFire && hit)
            HandleHit();
	}

    // If the win condition has been met, move to phase 2
    void CheckWin()
    {
        if (score >= requiredScore)
            hasWon = true;
    }

    // Respond to a hit target
    void HandleHit()
    {
        // Increment score
        score++;
        // Update score text
        UpdateScoreText();
        // If the target exists (which it should, but check), destroy it
        if(activeTarget != null)
            Destroy(activeTarget);
        // Check if the game has been completed
        CheckWin();
    }

    // Respond to input
    void HandleInput()
    {
        // Fire with the left mouse button
        if (Input.GetMouseButtonDown(0))
            checkFire = true;
        else
            checkFire = false;
    }

    // Update score text
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    // Generate a random point on the grid
    Vector2 RandomPoint()
    {
        // Randomly select an X-coordinate between -4 and 4 (upper range is exclusive)
        int x = Random.Range(-4, 5);
        // Randomly select a Y-coordinate between 0 and 5
        int y = Random.Range(0, 5);
        // If both coordinates are 0, move it up so it's not on the origin
        if (x == 0 && y == 0)
            y = Random.Range(1, 5);
        return new Vector2(x * 2, y * 2);
    }

    // Create and destroy a target
    IEnumerator ManageTarget()
    {
        // Instantiate the target prefab
        activeTarget = Instantiate(target);
        // Make a new 2D vector for the X and Y position of the target
        Vector2 newPosition = RandomPoint();
        // Set the target's position
        activeTarget.transform.position = new Vector3(newPosition.x, newPosition.y, 0f);
        // Prevent multiple targets from being created
        isTarget = true;
        // Wait for a defined amount of time
        yield return new WaitForSeconds(targetTime);
        // If the target still exists, destroy it
        if (activeTarget != null)
            Destroy(activeTarget);
        // Allow another target to exist
        isTarget = false;
    }
}
