using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    // Stuff we don't need/want to see
    private bool paused = false;
    private bool gameInProgress = false;
    private GameObject lander;
    private GameObject landingPad;
    private GameObject laserBase;
    private GameObject terrain;
    private Camera mainCamera;
    private float landerLastSeen = 0;
    private int score = 0;

    // Gamplay variables
    public float elapsedGameTime;
    public int livesCurrent = 0;
    public int livesMax = 3;

    // UI Variables
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject elapsedGameTimeText;
    public GameObject scoreText;
    public GameObject livesText;
    public GameObject healthBar;

    // Prefabs
    public GameObject landerPrefab;
    public GameObject landingPadPrefab;
    public GameObject laserBasePrefab;
    public GameObject terrainPrefab;

    void Start()
    {
        mainCamera = Camera.main;
        Pause();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }


        if (!paused && gameInProgress)
        {
            elapsedGameTime += Time.deltaTime;
            elapsedGameTimeText.GetComponent<Text>().text = "Time: " + Mathf.RoundToInt(elapsedGameTime).ToString();
            scoreText.GetComponent<Text>().text = "Score: " + Mathf.RoundToInt(score).ToString();
            livesText.GetComponent<Text>().text = "Lives: " + Mathf.RoundToInt(livesCurrent).ToString();

            if (lander)
            {
                landerLastSeen = elapsedGameTime;
                if (lander.GetComponent<Rigidbody2D>().IsSleeping())
                {
                    LanderLanded();
                }
            }
            else if (elapsedGameTime > landerLastSeen + 2)
            {
                LanderDestroyed();
            }
        }

        // Keep lander, landing pad, and laser in view
        if (lander && landingPad && laserBase)
        {
            // AdjustCamera(lander, landingPad, laserBase);
        }
    }

    void LanderDestroyed()
    {
        livesCurrent -= 1;
        if (livesCurrent < 0)
        {
            NewGame(true);
        }
        else
        {
            NewGame(false);
        }
    }

    void LanderLanded()
    {
        score += 1;
        NewGame(false);
    }

    void LateUpdate()
    {
        if (lander)
        {
            healthBar.SetActive(true);
            RectTransform healthBarRect = healthBar.GetComponent<RectTransform>();
            float healthCurrent = lander.GetComponent<Health>().healthCurrent;
            float healthMax = lander.GetComponent<Health>().healthMax;
            int healthPercent = Mathf.RoundToInt(healthCurrent / healthMax * 100);
            healthBarRect.sizeDelta = new Vector2(healthPercent, healthBarRect.sizeDelta.y);
            Vector2 landerScreenPosition = RectTransformUtility.WorldToScreenPoint(mainCamera, lander.transform.position);
            Vector2 healthBarPosition = new Vector2(Mathf.Round(landerScreenPosition.x), Mathf.Round(landerScreenPosition.y + 40));
            healthBarRect.position = healthBarPosition;
        }
        else
        {
            healthBar.SetActive(false);
        }
    }

    public void AdjustCamera(GameObject lander, GameObject landingPad, GameObject laserBase)
    {
        // List<Vector3> vectors = new List<Vector3>();
        // vectors.Add(lander.transform.position);
        // vectors.Add(landingPad.transform.position);
        // vectors.Add(laserBase.transform.position);
        // Vector3 cameraPosition = CenterOfVectors(vectors.ToArray());
        float minZoom = 25f;
        float maxZoom = 100f;
        float padding = -10f;
        float cameraZoom = Vector3.Distance(lander.transform.position, laserBase.transform.position);
        Vector3 vectorToLander = laserBase.transform.position - lander.transform.position;
        vectorToLander.Normalize();
        Vector3 cameraPosition = vectorToLander * cameraZoom / 2 * -1;
        float cameraX = cameraPosition.x;
        float cameraY = cameraPosition.y;

        mainCamera.orthographicSize = Mathf.Clamp(cameraZoom + padding, minZoom, maxZoom);
        mainCamera.transform.position = new Vector3(cameraX, cameraY, -10);
    }

    public Vector3 CenterOfVectors(Vector3[] vectors)
    {
        Vector3 sum = Vector3.zero;
        if (vectors == null || vectors.Length == 0)
        {
            return sum;
        }

        foreach (Vector3 vec in vectors)
        {
            sum += vec;
        }
        return sum / vectors.Length;
    }

    public void EndGame()
    {
        // Cleans up the game world
        Debug.Log("End Game");
        if (lander)
        {
            Destroy(lander);
        }
        if (terrain)
        {
            Destroy(terrain);
        }
        if (laserBase)
        {
            Destroy(laserBase);
        }
        if (landingPad)
        {
            Destroy(landingPad);
        }
    }


    public void NewGame(bool reset = true)
    {
        // Reset some stuff
        if (reset)
        {
            score = 0;
            livesCurrent = livesMax;
        }

        // Set these to active (only useful on first start)
        elapsedGameTimeText.SetActive(true);
        scoreText.SetActive(true);
        livesText.SetActive(true);

        // Fake
        float screenLeftEdge = -30;
        float screenRightEdge = 30;
        float screenTopEdge = 20;
        float screenBottomEdge = -10;
        // float screenWidth = Mathf.Abs(screenLeftEdge - screenRightEdge);
        float screenHeight = Mathf.Abs(screenTopEdge - screenBottomEdge);

        Debug.Log("New Game");
        EndGame();  // Deletes everything and sets up our scene again
        elapsedGameTime = 0f;
        gameInProgress = true;
        if (paused)
        {
            Pause();
        }

        // Instantiate Terrain
        // Done first so we know heights for the laser base and landing pad
        Vector3 terrainPosition = new Vector3(0, 0, 0);
        terrain = Instantiate(terrainPrefab, terrainPosition, Quaternion.identity);

        // Adjust terrain settings
        LineGenerator lineGenerator = terrain.GetComponent<LineGenerator>();
        lineGenerator.offset = Random.Range(5, 500);
        // Tuned so that there is 1 segment per unit (makes the landing pad and laser height calculations easer)
        lineGenerator.leftEdge = -1000;
        lineGenerator.rightEdge = 1000;
        lineGenerator.segments = 2000;
        lineGenerator.GenerateLine();
        var vertices = lineGenerator.edgeVertices.ToArray();
        // Heights is reused for setting the landing pad and laser heights
        List<float> heights = new List<float>();
        for (int i = 0; i < vertices.Length; i++)
        {
            heights.Add(vertices[i].y);
        }
        float maxHeight = Mathf.Max(heights.ToArray());
        // float minHeight = Mathf.Min(heights.ToArray());

        terrain.transform.position = new Vector3(0, maxHeight * -1, 0.2f);

        // Instantiate Lander
        float landerX = Random.Range(screenLeftEdge / 2, screenRightEdge / 2);
        float landerY = Random.Range(screenTopEdge - screenHeight / 10, screenTopEdge);
        Vector3 landerPosition = new Vector3(landerX, landerY, 0);
        lander = Instantiate(landerPrefab, landerPosition, Quaternion.identity);

        // Base random position
        float baseX = Random.Range(screenLeftEdge, screenRightEdge);

        // Instantiate Landing Pad
        float landingPadX = baseX;
        int landingPadTerrainVertexIndex = Mathf.RoundToInt(landingPadX - lineGenerator.leftEdge);
        Debug.Log(landingPadTerrainVertexIndex);
        int landingPadWidth = 8;
        int landingPadHeight = 10;
        float landingPadTerrainSum = 0f;
        for (int i = landingPadWidth * -1 / 2; i < landingPadWidth / 2; i++)
        {
            landingPadTerrainSum += heights[landingPadTerrainVertexIndex + i];
        }
        float landingPadTerrainAverage = landingPadTerrainSum / landingPadWidth;
        float landingPadY = landingPadTerrainAverage + maxHeight * -1 + landingPadHeight / 2;
        Vector3 landingPadPosition = new Vector3(landingPadX, landingPadY, 0);
        landingPad = Instantiate(landingPadPrefab, landingPadPosition, Quaternion.identity);
        // Instantiate Laser Base
        float laserOffset = Random.Range(15f, 50f);
        // Randomize side laser is on
        if (Random.Range(0, 1) < 0.5f)
        {
            laserOffset *= -1;
        }
        // Make sure laser doesn't go off screen
        if (laserOffset + baseX < screenLeftEdge || laserOffset + baseX > screenRightEdge)
        {
            laserOffset *= -1;
        }
        float laserBaseX = baseX + laserOffset;
        int laserBaseTerrainVertexIndex = Mathf.RoundToInt(laserBaseX - lineGenerator.leftEdge);
        Debug.Log(laserBaseTerrainVertexIndex);
        int laserBaseWidth = 14;
        int laserBaseHeight = 10;
        float laserBaseTerrainSum = 0f;
        for (int i = laserBaseWidth * -1 / 2; i < laserBaseWidth / 2; i++)
        {
            laserBaseTerrainSum += heights[laserBaseTerrainVertexIndex + i];
        }
        float laserBaseTerrainAverage = laserBaseTerrainSum / laserBaseWidth;
        float laserBaseY = laserBaseTerrainAverage + maxHeight * -1 + laserBaseHeight / 2;
        Vector3 laserBasePosition = new Vector3(laserBaseX, laserBaseY, 0);
        laserBase = Instantiate(laserBasePrefab, laserBasePosition, Quaternion.identity);
    }

    void Pause()
    {
        if (paused && gameInProgress)
        {
            Debug.Log("Game Unpaused");
            Time.timeScale = 1;
            mainMenu.SetActive(false);
            optionsMenu.SetActive(false);
        }
        else
        {
            Debug.Log("Game Paused");
            Time.timeScale = 0;
            mainMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
        paused = !paused;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
