using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    // Serialize Fields for assignment in-editor
    // Reference to aim arrow (to check cooldown)
    [SerializeField] AimArrow arrow;
    // Time a target is available
    [SerializeField] float targetTime = 2f;
    // Explosion
    [SerializeField] GameObject explosion;
    // Target
    [SerializeField] GameObject target;
    // Invisible target for assessment
    [SerializeField] GameObject invisibleTarget;
    // Target score (required to continue)
    [SerializeField] int requiredScore = 10;
    // Name of next scene
    [SerializeField] string nextScene;
    // Score text
    [SerializeField] Text scoreText;
    // Assessment text
    [SerializeField] Text assessmentText;
    // Player aim reticle
    [SerializeField] Transform reticle;

    // Private fields
    // Did the player fire?
    bool checkFire;
    // Did the shot hit?
    bool hit;
    // Has the win requirement been met?
    bool hasWon;
    // Is there currently a target?
    bool isTarget;
    // Is the assessment running?
    bool isTesting;
    // Test questions answered
    bool[] testQuestionsAnswered = new bool[3];
    // Is the test finished?
    bool testFinished;
    // Reference to active explosion
    GameObject activeExplosion;
    // Reference to active target
    GameObject activeTarget;
    // Score
    int score = 0;
    // Default position for angle comparison
    Vector2 defaultPosition = new Vector2(0f, 1f);
    // Position of player selection for assessment
    Vector2 testAnswer;

	// Use this for initialization
	void Start () 
	{
        UpdateScoreText();
        // Give basic instructions
        assessmentText.text = "Fire at the targets.";
	}
	
	// Update is called once per frame
	void Update () 
	{
        // Check input
        HandleInput();
        if (!hasWon)
        {
            // If the player has hit a target, remove the instructions
            if (score >= 1)
                assessmentText.text = "";
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
        else
        {
            if (!isTesting)
                StartCoroutine(RunAssessment());
            if (checkFire)
                testAnswer = reticle.position;
            if (testFinished)
                SceneManager.LoadScene(nextScene);
        }
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
        // Create an explosion at the target
        activeExplosion = Instantiate(explosion);
        activeExplosion.transform.position = activeTarget.transform.position;
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
        // If an explosion exists, destroy it
        if (activeExplosion != null)
            Destroy(activeExplosion);
        // Allow another target to exist
        isTarget = false;
    }

    // Run the assessment
    IEnumerator RunAssessment()
    {
        // Prevent the test from being run multiple times
        isTesting = true;
        // Present the first question
        assessmentText.text = "Fire vector <3,4>";
        while(!testQuestionsAnswered[0])
        {
            if (testAnswer != null)
                // Check the proposed position, move on if it's right
                if (testAnswer.x / 2 == 3f && testAnswer.y / 2 == 4f)
                    testQuestionsAnswered[0] = true;
            yield return null;
        }
        // Give feedback
        assessmentText.text = "Correct.\nThe first value denotes horizontal position, the second denotes vertical.";

        // Wait on feedback
        yield return new WaitForSeconds(targetTime);

        // Present the second question
        assessmentText.text = "Fire a vector of magnitude 2";
        while(!testQuestionsAnswered[1])
        {
            if (testAnswer != null)
                // Check the proposed magnitude, move on if correct
                if (testAnswer.magnitude / 2 == 2f)
                    testQuestionsAnswered[1] = true;
            yield return null;
        }
        // Give feedback
        assessmentText.text = "Correct.\nA vector's magnitude is proportional to its length.";

        // Wait on feedback
        yield return new WaitForSeconds(targetTime);

        // Present the final question
        assessmentText.text = "Fire a vector in a direction of 135\u00B0.";
        while(!testQuestionsAnswered[2])
        {
            if (testAnswer != null)
                // Check the proposed angle, move on if correct
                if ((Vector2.SignedAngle(defaultPosition, testAnswer) + 90f) == 135f)
                    testQuestionsAnswered[2] = true;
            yield return null;
        }
        // Give feedback
        assessmentText.text = "Correct.\nA vector's direction can be measured in degrees.";

        // Wait on feedback
        yield return new WaitForSeconds(targetTime);

        // Prepare to move to the end scene
        testFinished = true;
    }
}
