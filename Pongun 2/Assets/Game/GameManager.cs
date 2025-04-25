using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    const float paddleInitialDist = 8;

	// Spawnpoints
	readonly Vector3 playerStartPosition = new(-paddleInitialDist, 0f, 0f);
	readonly Vector3 enemyStartPosition = new(paddleInitialDist, 0f, 0f);

	const float envInitialZoom = 5;
    float currentZoomMult;
    
    [Header("Objects")]
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    [Header("UI")]
    [SerializeField] private TMP_Text playerScoreText;
    [SerializeField] private TMP_Text enemyScoreText;
    [SerializeField] private TMP_Text ballSpeedText;
    [SerializeField] private TMP_Text speedIncreaseText;

    [Header("Environment")]
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject topBorder;
	[SerializeField] private GameObject bottomBorder;

	private int playerScore;
    private int enemyScore;

    private bool gameActionable = false;

    protected void Awake()
    {
        Instance = Instance != null ? Instance : this;

        // Subscribe to the ball speed increase event.
        Ball.OnBallSpeedIncrease.AddListener((newSpeed, speedInfo) => {
            SpeedStateIncrease(newSpeed, speedInfo.color);
        });
    }

    // Start is called before the first frame update
    protected void Start()
    {
        // Reset the scores.
        playerScore = 0;
        enemyScore = 0;

        // Start the first round.
        StartCoroutine(StartRound());
    }

    protected void Update()
    {
        // TODO remove; update the ball speed text.  
        ballSpeedText.text = Ball.Instance.GetMaxSpeed().ToString("F2");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Exit the game and return to the main menu
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        if (gameActionable)
        {
            currentZoomMult += 0.08f * Time.deltaTime;
            float environmentZoom = envInitialZoom * currentZoomMult;
            float paddleZoom = paddleInitialDist * currentZoomMult;

			cam.orthographicSize = environmentZoom;
            
			topBorder.transform.position = envInitialZoom * Vector2.up;
            bottomBorder.transform.position = envInitialZoom * Vector2.down;

            // Zoom for players
            player.SetDistFromCenter(paddleInitialDist * currentZoomMult);
            enemy.SetDistFromCenter(paddleInitialDist * currentZoomMult);
        }
    }

    IEnumerator StartRound()
    {
        // Destroy all bullets still in the scene.
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            Destroy(bullet);
        }
        
        // Disable the player and enemy from moving/shooting.
        player.Reset();
        enemy.Reset();

        // Reset the ball's velocity.
        Ball.Instance.Reset();

        // Reset the player and enemy positions.
        player.transform.position = playerStartPosition;
        enemy.transform.position = enemyStartPosition;
        Ball.Instance.transform.position = Vector3.zero;

        // Reset size
        currentZoomMult = 1;

        // Wait a second before starting the round.
        yield return new WaitForSeconds(1f);
        StartActionable();
    }

    void StartActionable()
    {
        gameActionable = true;
        
        // Enable the player and enemy moving/shoting.
        player.Actionable();
        enemy.Actionable();

        // Shoot the ball in a random direction.
        Ball.Instance.Shoot();
    }

    public void SpeedStateIncrease(Ball.Speed newSpeed, Color textColor)
    {
        // Don't show the speed state increase text if the speed is too slow.
        if (newSpeed < Ball.Speed.Fast) { return; }
        
        // Show the speed state increase text.
        // Make it fade in while moving up, then make it fade out.
        speedIncreaseText.SetText(newSpeed.ToString().ToUpper());
        speedIncreaseText.color = textColor;

        StartCoroutine(SpeedTextAnim());
    }

    IEnumerator SpeedTextAnim()
    {
        speedIncreaseText.alpha = 0f;
        speedIncreaseText.rectTransform.anchoredPosition = new Vector2(0f, -275f);

        while (speedIncreaseText.alpha < 1f)
        {
            speedIncreaseText.alpha += Time.deltaTime * 1.5f;
            speedIncreaseText.rectTransform.anchoredPosition += new Vector2(0f, 50f) * Time.deltaTime;
            yield return null;
        }
        speedIncreaseText.alpha = 1f;

        yield return new WaitForSeconds(1f);

        while (speedIncreaseText.alpha > 0f)
        {
            speedIncreaseText.alpha -= Time.deltaTime * 2f;
            yield return null;
        }
        speedIncreaseText.alpha = 0f;
        speedIncreaseText.rectTransform.anchoredPosition = new Vector2(0f, -275f);
    }

    public void GoalScored(Paddle paddle)
    {   
        // Increment the score of the paddle that scored.
        if (paddle.Equals(player))
        {
            playerScore++;
            playerScoreText.text = playerScore.ToString();
        }
        else if (paddle.Equals(enemy))
        {
            enemyScore++;
            enemyScoreText.text = enemyScore.ToString();
        }

        // Start a new round.
        StartCoroutine(StartRound());
    }
}
