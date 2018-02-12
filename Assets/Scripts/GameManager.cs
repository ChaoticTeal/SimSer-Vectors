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
    // Player aim reticle
    [SerializeField] Transform reticle;

    // Private fields
    // Did the player fire?
    bool checkFire;
    // Is there currently a target?
    bool isTarget;
    // Did the shot hit?
    bool hit;
    // Reference to active target
    GameObject activeTarget;
    // Score
    int score = 0;

	// Use this for initialization
	void Start () 
	{
        UpdateText();
	}
	
	// Update is called once per frame
	void Update () 
	{
        HandleInput();
        if (!isTarget)
            StartCoroutine(ManageTarget());
        if (activeTarget != null)
            hit = activeTarget.transform.position == reticle.position && !arrow.Cooldown;
        else
            hit = false;
        if (checkFire && hit)
            HandleHit();
	}

    void HandleHit()
    {
        score++;
        UpdateText();
        if(activeTarget != null)
            Destroy(activeTarget);
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
            checkFire = true;
        else
            checkFire = false;
    }

    void UpdateText()
    {
        scoreText.text = "Score: " + score;
    }

    // Generate a random point on the grid
    Vector2 RandomPoint()
    {
        int x = Random.Range(-4, 5);
        int y = Random.Range(0, 5);
        if (x == 0 && y == 0)
            y = Random.Range(1, 5);
        return new Vector2(x * 2, y * 2);
    }

    IEnumerator ManageTarget()
    {
        activeTarget = Instantiate(target);
        Vector2 newPosition = RandomPoint();
        activeTarget.transform.position = new Vector3(RandomPoint().x, RandomPoint().y, 0f);
        isTarget = true;
        yield return new WaitForSeconds(targetTime);
        if (activeTarget != null)
            Destroy(activeTarget);
        isTarget = false;
    }
}
