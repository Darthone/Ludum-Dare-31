using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public static GameController control = null;
    public GameObject[,] blockGrid;
    public Canvas canvas;
    
    public Texture2D bombTex;
    public GameObject floatingText;
    public GameObject explosionPrefab;
    public Camera gameCamera;
    public List<Vector2> open = new List<Vector2>();
    GUIText myGUIText;
    GUIStyle myGUIStyle;

    bool paused = false;
    public bool canPause = true;

    float fadeSpeed = 5f;          // Speed that the screen fades to and from black.
    private bool sceneStarting = true;      // Whether or not the scene is still fading in.
    public bool sceneEnding = false;
    public bool gameOver = false;
    public bool reversed = false;
    bool rotating = false;
    string Rules = "\n-------Rules------- \nMatch 4\n\n Left  \n\nRight  \n\n Space\n\n\n\nLD48: 31\nOne Screen\n\nDarioMarasco\n\nPolybox\n Games\n2014";
    bool playing = false;

    //public string Rules = "\n0x21B5";
	//public AudioClip newWorldAvailableSound;
    //public AudioClip gameoverSound;

    public long score = 0;
    public float multiplyer = 1.0f;
    public float refresh = 1.0f;
    public int gridWidth = 4;
    public int gridHeight = 4;
    int blockScore = 50;
    int blockChainSize = 16; // min size to explode
    public int level = 3; // 3 is base level spawn new color blocks
    public int gridsize = 4;

    //long threshold = 7500;

    public float minSpawnTime = 1.5f;
    public float maxSpawnTime = 4f;
    //public float powerupChance = .20f;

    public int gravityDirection = 0; //0 is up clockwise to 3, 2 is down

    IEnumerator IncreaseMultiplyer(float delay) {
        yield return new WaitForSeconds(delay);
        multiplyer += 0.2f + 0.01f * delay;
        minSpawnTime = Mathf.Clamp(minSpawnTime -0.1f, 0.1f, 1f);
        maxSpawnTime = Mathf.Clamp(maxSpawnTime - 0.2f, 1.5f, 3f);
        //powerupChance = Mathf.Clamp(powerupChance + 0.01f, 0.20f, 0.33f);
        StartCoroutine(IncreaseMultiplyer(0.04f * delay * delay + 1f + delay));
    }

    IEnumerator MoveBlocks(float delay) {      
        UpdateBlocks();
        yield return new WaitForSeconds(delay);
        refresh = Mathf.Clamp(refresh - .003f, 0.15f, 1f);
        StartCoroutine(MoveBlocks(refresh));
    }
    void setupCamera() { 
        gridHeight = gridsize;
        gridWidth = gridsize;
        gameCamera.orthographicSize = gridWidth / 2;
        gameCamera.transform.position = new Vector3(gridWidth / 2, gridHeight / 2, -100f);

    }

    IEnumerator IncrementLevel(float delay) {
        yield return new WaitForSeconds(delay);
        if (level < 10) {
            //gridsize += 2;
            //setupCamera();
            level = Mathf.Clamp(level + 1, 3, 10);
            StartCoroutine(IncrementLevel(delay + 5f)); // increse time between new blocks.  currently about 3 minutes to reach all blocks
        }
    }

    IEnumerator rotateScreen(float time, int direction) {
        int steps = 15;
        if (direction == 90) {
            gravityDirection++;
        } else if (direction == -90) {
            gravityDirection--;
        }
        if (gravityDirection > 3)
            gravityDirection = 0;
        if (gravityDirection < 0)
            gravityDirection = 3;

        

        for (int i = 0; i < steps; i++) {
            gameCamera.transform.Rotate(gameCamera.transform.forward, direction / (float)steps);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }


    public void UpdateBlocks() {
        if( gravityDirection == 0 || gravityDirection == 3) {
            for (int i = 0; i < gridWidth; i++) {
                for (int k = 0; k < gridHeight; k++) {
                    if (blockGrid[i, k] != null) {
                        List<GameObject> b = new List<GameObject>();
                        b = blockGrid[i, k].GetComponent<Block>().checkColors(b);
                        if (b.Count >= blockChainSize) {
                            destroyBlocks(b);
                            //return;
                        }
                        if (blockGrid[i, k] == null)
                            continue;
                        switch (gravityDirection) {
                            case 0:
                                if (k - 1 >= 0 && blockGrid[i, k - 1] == null) {
                                    blockGrid[i, k].GetComponent<Block>().x = i;
                                    blockGrid[i, k].GetComponent<Block>().y = k - 1;
                                    blockGrid[i, k - 1] = blockGrid[i, k];
                                    blockGrid[i, k] = null;
                                }
                                break;
                            case 3:
                                if (i - 1 >= 0 && blockGrid[i - 1, k] == null) {
                                    blockGrid[i, k].GetComponent<Block>().x = i - 1;
                                    blockGrid[i, k].GetComponent<Block>().y = k;
                                    blockGrid[i - 1, k] = blockGrid[i, k];
                                    blockGrid[i, k] = null;
                                }
                                break;
                            }
                        }
                    }
                }
            } else {
                for (int i = gridWidth - 1; i >= 0; i--) {
                    for (int k = gridHeight - 1; k >= 0; k--) {
                        if (blockGrid[i, k] != null) {
                            List<GameObject> b = new List<GameObject>();
                            b = blockGrid[i, k].GetComponent<Block>().checkColors(b);
                            if (b.Count >= blockChainSize) {
                                destroyBlocks(b);
                            }
                            if (blockGrid[i, k] == null)
                                continue;
                            switch (gravityDirection) {
                                case 1:
                                    if (i + 1 < gridWidth && blockGrid[i + 1, k] == null) {
                                        blockGrid[i, k].GetComponent<Block>().x = i + 1;
                                        blockGrid[i, k].GetComponent<Block>().y = k;
                                        blockGrid[i + 1, k] = blockGrid[i, k];
                                        blockGrid[i, k] = null;
                                    }
                                    break;
                                case 2:
                                    if (k + 1 < gridHeight && blockGrid[i, k + 1] == null) {
                                        blockGrid[i, k].GetComponent<Block>().x = i;
                                        blockGrid[i, k].GetComponent<Block>().y = k + 1;
                                        blockGrid[i, k + 1] = blockGrid[i, k];
                                        blockGrid[i, k] = null;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
    }

    void destroyBlocks(List<GameObject> a) {
        long tot = 0;
        Vector3 loc = new Vector3(5,5); // spawn in center by default
        List<GameObject> b = new List<GameObject>();
        for (int i = 0; i < a.Count; i++) {
            if (!b.Contains(a[i]))
                b.Add(a[i]);
        }

        for (int j = 0; j < b.Count; j++) {
            loc = new Vector3(b[j].GetComponent<Block>().x, b[j].GetComponent<Block>().y, -30);
            Destroy(blockGrid[b[j].GetComponent<Block>().x, b[j].GetComponent<Block>().y]);
            Destroy(b[j]);
            GameObject explosion = (GameObject)Instantiate(explosionPrefab, loc, Quaternion.identity);
            tot += (long)(((int)Mathf.Pow(j, 2f) / 2 * blockScore) * multiplyer);
        }
        //draw floating text
        //GameObject text = (GameObject)Instantiate(floatingText, loc, Quaternion.identity);
        //text.GetComponent<floatingPoints>().text = "+" + tot;
        score += tot;
    }

    void Push() {
        if (gravityDirection == 1 || gravityDirection == 2) {
            for (int i = 0; i < gridWidth; i++) {
                for (int k = 0; k < gridHeight; k++) {
                    if (blockGrid[i, k] != null) {
                        switch (gravityDirection) {
                            case 1:
                                if (i + 1 < gridWidth && blockGrid[i + 1, k] == null) {
                                    blockGrid[i, k].GetComponent<Block>().x = i + 1;
                                    blockGrid[i, k].GetComponent<Block>().y = k;
                                    blockGrid[i + 1, k] = blockGrid[i, k];
                                    blockGrid[i, k] = null;
                                }
                                break;
                            case 2:
                                if (k + 1 < gridHeight && blockGrid[i, k + 1] == null) {
                                    blockGrid[i, k].GetComponent<Block>().x = i;
                                    blockGrid[i, k].GetComponent<Block>().y = k + 1;
                                    blockGrid[i, k + 1] = blockGrid[i, k];
                                    blockGrid[i, k] = null;
                                }
                                break;
                            
                        }
                    }
                }
            }
        }
        else {
            for (int i = gridWidth - 1; i >= 0; i--) {
                for (int k = gridHeight - 1; k >= 0; k--) {
                    if (blockGrid[i, k] != null) {
                        switch (gravityDirection) {
                            case 0:
                                if (k - 1 >= 0 && blockGrid[i, k - 1] == null) {
                                    blockGrid[i, k].GetComponent<Block>().x = i;
                                    blockGrid[i, k].GetComponent<Block>().y = k - 1;
                                    blockGrid[i, k - 1] = blockGrid[i, k];
                                    blockGrid[i, k] = null;
                                }
                                break;
                            case 3:
                                if (i - 1 >= 0 && blockGrid[i - 1, k] == null) {
                                    blockGrid[i, k].GetComponent<Block>().x = i - 1;
                                    blockGrid[i, k].GetComponent<Block>().y = k;
                                    blockGrid[i - 1, k] = blockGrid[i, k];
                                    blockGrid[i, k] = null;
                                }
                                break;
                        }
                    }
                }
            }
        }

    }

    public IEnumerator Shake(float duration, float magnitude) {
        // shakes the camera
        float elapsed = 0.0f;
        Vector3 originalCamPos = Camera.main.transform.position;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= magnitude * damper;
            y *= magnitude * damper;

            Camera.main.transform.position = new Vector3(x, y, originalCamPos.z);
            yield return null;
        }

        Camera.main.transform.position = originalCamPos;
    }

	// Use this for initialization
    void Awake() {
        if (control == null) {
            DontDestroyOnLoad(gameObject);
            control = this;
        } else if (control != this) {
            Destroy(gameObject);
            return;
        }
        Text text = canvas.GetComponent<Text>();
        //fill screen with gui texture
        guiTexture.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
        myGUIText = this.GetComponent<GUIText>();
        myGUIText.pixelOffset = new Vector2(Screen.width - 30f, Screen.height - 15f);
        myGUIStyle = new GUIStyle();
        myGUIStyle.fontStyle = myGUIText.fontStyle;
        myGUIStyle.fontSize = myGUIText.fontSize;
        myGUIStyle.font = myGUIText.font;
        myGUIStyle.normal.textColor = Color.white;
        text.fontStyle = myGUIText.fontStyle;
        text.fontSize = myGUIText.fontSize;
        text.font = myGUIText.font;
        text.color = Color.white;

        //init grid
        blockGrid = new GameObject[gridWidth, gridHeight];
        for (int i = 0; i < gridWidth; i++) {
            for (int k = 0; k < gridHeight; k++) {
                blockGrid[i, k] = null;
            }
        }
        StartCoroutine(IncreaseMultiplyer(10f));
        StartCoroutine(MoveBlocks(1f));
        StartCoroutine(IncrementLevel(35f));
        setupCamera();
        Time.timeScale = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Menu")) {
            if (canPause) {
                paused = togglePause();
                canPause = false;
                Invoke("resetPause", 0.05f);
            }
        }

        if (Input.GetButtonUp("Menu")) {
            canPause = true;
        }

        if (Input.GetButtonDown("Mute")){
            AudioListener.pause = !AudioListener.pause;
        }

        if (Input.GetButtonDown("Right")) {
            StartCoroutine(rotateScreen(0.1f, 90));
        }
        if (Input.GetButtonDown("Left")) {
            StartCoroutine(rotateScreen(0.1f, -90));
        }

        if (Input.GetButtonDown("Push")) {
            Push();
        }
	}

    void FixedUpdate() {
        if(!gameOver)
            canvas.GetComponent<Text>().text = "SCORE:\n" + score.ToString() + "\n\n" + Rules;
        
        // Check for endgame condition
        open.Clear();
        for (int i = 0; i < gridWidth; i++) {
            for (int k = 0; k < gridHeight; k++) {
                if (blockGrid[i, k] == null) {
                    open.Add(new Vector2(i, k));
                }
            }
        }
        if (open.Count == 0) {
            GameOver();
        }

        if (sceneStarting)
            StartScene();
        if (sceneEnding) {
            EndScene();
        }

    }

    void OnGUI() {
        if (paused) {
            GUILayout.Label("Game is paused!");
            if (GUILayout.Button("Unpause"))
                paused = togglePause();
        } else if (!playing) {
            GUILayout.Label("\n\n\n\n\n\n\n\n\n\n\n\tRead the Rules and then Press Space!", myGUIStyle);
            if (Input.GetButtonDown("Select") || Input.GetButtonDown("Push")) {
                playing = true;
                Time.timeScale = 1f;
            }
        } else {
            myGUIText.text = "SCORE: " + score.ToString();
        }
    }

    bool togglePause() {
        if (Time.timeScale == 0f) {
            Time.timeScale = 1f;
            return (false);
        }
        else {
            Time.timeScale = 0f;
            return (true);
        }
    }

    //Fading Functions
    void OnLevelWasLoaded() {
        sceneStarting = true;
        StartScene();
    }

    void FadeToClear() {
        // Lerp the colour of the texture between itself and transparent.
        guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, fadeSpeed * Time.deltaTime);
    }

    void FadeToBlack() {
        // Lerp the colour of the texture between itself and black.
        guiTexture.color = Color.Lerp(guiTexture.color, Color.black, fadeSpeed * Time.deltaTime);
    }

    void StartScene() {
        // Fade the texture to clear.
        FadeToClear();
        // If the texture is almost clear...
        if (guiTexture.color.a <= 0.02f) {
            // ... set the colour to clear and disable the GUITexture.
            guiTexture.color = Color.clear;
            guiTexture.enabled = false;

            // The scene is no longer starting.
            sceneStarting = false;
        }
    }

    public void EndScene() {
        // Make sure the texture is enabled.
        guiTexture.enabled = true;

        // Start fading towards black.
        FadeToBlack();
        if (guiTexture.color.a >= 0.95f) {
            // ... set the colour to clear and disable the GUITexture.
            guiTexture.color = Color.black;
            sceneEnding = false;
        }
    }

    public void GameOver() {
        if (!gameOver) {
            StartCoroutine(Shake(3f, 1f));
            gameOver = true;
            //AudioSource.PlayClipAtPoint(gameoverSound, this.transform.position); TODO
            sceneEnding = true;
            FadeToBlack();
            myGUIText.anchor = TextAnchor.MiddleCenter;
            myGUIText.fontSize = 60;
            myGUIText.pixelOffset = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Invoke("RestartGame", 5f);
        }
    }

    void RestartGame() {
		Application.LoadLevel ("Main");
        Destroy(this.gameObject);
    }
}
