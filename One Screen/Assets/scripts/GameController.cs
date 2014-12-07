using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public static GameController control = null;
    public List<GameObject> blocks = new List<GameObject>(); // keep list to broadcast to
    public int[,] grid;
    public GameObject[,] blockGrid;
    public Canvas canvas;
    
    public Texture2D[] guiLives;
    public Texture2D[] guiLevel;

    public Texture2D bombTex;
    public GameObject floatingText;
    public Camera gameCamera;
    public List<Vector2> open = new List<Vector2>();
    GUIText myGUIText;
    GUIStyle myGUIStyle;
    //public Texture2D[] guiLives;

    bool paused = false;
    public bool canPause = true;

    float fadeSpeed = 3f;          // Speed that the screen fades to and from black.
    private bool sceneStarting = true;      // Whether or not the scene is still fading in.
    public bool sceneEnding = false;
    public bool gameOver = false;
    public bool reversed = false;

	//public AudioClip newWorldAvailableSound;
    //public AudioClip gameoverSound;

    public long score = 0;
    public float multiplyer = 1.0f;
    public int gridWidth = 10;
    public int gridHeight = 10;
    int blockScore = 250;

    //long threshold = 7500;

    //public bool[] underAttack;

    public float minSpawnTime = 1.5f;
    public float maxSpawnTime = 4f;
    //public float powerupChance = .20f;

    public int gravityDirection = 0; //0 is up clockwise to 3, 2 is down

    IEnumerator IncreaseMultiplyer(float delay) {
        yield return new WaitForSeconds(delay);
        multiplyer += 0.5f + 0.05f * delay;
        minSpawnTime = Mathf.Clamp(minSpawnTime -0.1f, 0.1f, 1f);
        maxSpawnTime = Mathf.Clamp(maxSpawnTime - 0.2f, 1.5f, 3f);
        //powerupChance = Mathf.Clamp(powerupChance + 0.01f, 0.20f, 0.33f);
        StartCoroutine(IncreaseMultiplyer(0.04f * delay * delay + 1f + delay));
    }

    IEnumerator MoveBlocks(float delay) {      
        UpdateBlocks();
        yield return new WaitForSeconds(delay);
        StartCoroutine(MoveBlocks(delay));
    }

    IEnumerator rotateScreen(float time, float direction) {
        int steps = 30;
        if (direction == 90f) {
            gravityDirection++;
        } else if (direction == -90f) {
            gravityDirection--;
        }
        if (gravityDirection > 3)
            gravityDirection = 0;
        if (gravityDirection < 0)
            gravityDirection = 3;

        for (int i = 0; i < steps; i++) {
            gameCamera.transform.Rotate(gameCamera.transform.forward, direction / steps);
            yield return new WaitForSeconds(time / steps);
        }
    }

    /*IEnumerator flipScreen(float time, float direction) {
        reversed = !reversed;
        int steps = 60;
        if (direction == 180f) {
            gravityDirection += 2;
        } else if (direction == -180f) {
            gravityDirection += 2;
        }

        if (gravityDirection > 3)
            gravityDirection -= 3;
            //gravityDirection = 0;
        if (gravityDirection < 0)
            gravityDirection += 3;
            //gravityDirection = 3;

        for (int i = 0; i < steps; i++) {
            print("fucking  translating");
            //gameCamera.transform.LookAt(new Vector3(10, 10, 0), gameCamera.transform.up);
            print(steps / direction);
            gameCamera.transform.position = Vector3.Slerp(new Vector3(10, 10, 10), new Vector3(10, 10, -10), (direction / steps));
            //gameCamera.transform.Translate(gameCamera.transform.up * ((direction / steps)/100));
            //gameCamera.transform.Rotate(gameCamera.transform.right, direction / steps);
            yield return new WaitForSeconds(time / steps);
        }
    } TODO */

    public void UpdateBlocks() {
        if( gravityDirection == 0 || gravityDirection == 3) {
            for (int i = 0; i < gridWidth; i++) {
                for (int k = 0; k < gridHeight; k++) {
                    if (blockGrid[i, k] != null) {
                        List<GameObject> b = new List<GameObject>();
                        b = blockGrid[i, k].GetComponent<Block>().checkColors(b);
                        if (b.Count >= 3) {
                            destroyBlocks(b);
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
                            if (b.Count >= 3) {
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

    void destroyBlocks(List<GameObject> b) {
        for (int j = 0; j < b.Count; j++) {
            Destroy(blockGrid[b[j].GetComponent<Block>().x, b[j].GetComponent<Block>().y]);
            //Destroy(b[j]);
            score += (int)Mathf.Pow(j, 2f) / 2 * blockScore;
            //blockGrid[b[j].GetComponent<Block>().x, b[j].GetComponent<Block>().y] = null;
            //Destroy(b[j]);
            //destory and add to points
        }
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
        }
        else if (control != this) {
            Destroy(gameObject);
            return;
        }
        Text text = canvas.GetComponent<Text>();
        //text.fontStyle = myGUIText.fontStyle;
        //text.fontSize = myGUIText.fontSize;

        //fill screen with gui texture
        guiTexture.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
        myGUIText = this.GetComponent<GUIText>();
        myGUIText.pixelOffset = new Vector2(Screen.width - 30f, Screen.height - 15f);
        //myGUIStyle = new GUIStyle();*/
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
        StartCoroutine(MoveBlocks(1f));
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

        if (Input.GetButtonDown("Right")){
            StartCoroutine(rotateScreen(0.1f, 90));
        }
        if (Input.GetButtonDown("Left")) {
            StartCoroutine(rotateScreen(0.1f, -90));
        }

        if (Input.GetButtonDown("Push")) {
            Push();
        }

        /*
        if (Input.GetButtonDown("FlipUp")) {
            StartCoroutine(flipScreen(0.1f, 180));
        }
        if (Input.GetButtonDown("FlipDown")) {
            StartCoroutine(flipScreen(0.1f, -180));
        }*/
	}

    void FixedUpdate() {
        canvas.GetComponent<Text>().text = "SCORE:\n" + score.ToString();
        open.Clear();
        //CheckScore();
        //playerLayer = player.layer;

        // Check for endgame condition
        for (int i = 0; i < gridWidth; i++) {
            for (int k = 0; k < gridHeight; k++) {
                if (blockGrid[i, k] == null) {
                    open.Add(new Vector2(i, k));
                }
            }
        }
        if (open.Count == 0) {
            gameOver = true;
            print("GAME OVER");
            //game over
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
        } else if (!gameOver) {
            // lives - top left
            //GUI.DrawTexture(new Rect(30f, 15f, 110f, 22f), guiLives[lives]);

            //score - top right
            myGUIText.text = "SCORE: " + score.ToString();

            /*// layer - bottom left
            float boxHeight = 26f;
            float boxDelim = 3f;
            float boxWidth = 14f;
            for (int i = 0; i <= level; i++) {
                //if level alerted draw different Texture
                if (playerLayer - 8 == i) {
                    GUI.DrawTexture(new Rect(30f + i * (boxWidth + boxDelim), Screen.height - 30f - boxHeight, boxWidth, boxHeight), guiLevel[0]);
                }
                else {
                    GUI.DrawTexture(new Rect(30f + i * (boxWidth + boxDelim), Screen.height - 30f - boxHeight, boxWidth, boxHeight), guiLevel[1]);
                }
            }*/

            //bombs
            /*GUI.Label(new Rect(Screen.width - 54f - 33f, Screen.height - 30f - boxHeight + 1f, boxWidth + 10f, boxHeight), "x" + pc.bombs.ToString(), myGUIStyle);
            GUI.DrawTexture(new Rect(Screen.width - 54f, Screen.height - 30f - boxHeight, boxWidth + 10f, boxHeight), bombTex);*/
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
            gameOver = true;
            //play game over sound
            //pc.canMove = false;
            //pc.canShoot = false;
            //player.gameObject.active = false;
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
        //Application.LoadLevel("MainMenu_Mac");
        Destroy(this.gameObject);
    }

    /*void CheckScore() {
        if (score >= threshold) {
            if (threshold < 15000) {
                GameObject text = (GameObject)Instantiate(floatingText, this.transform.position + new Vector3(0,10f), Quaternion.identity);
                text.guiText.fontSize = 30;
                text.GetComponent<floatingPoints>().scroll = 0f;
                text.GetComponent<floatingPoints>().scroll = 4f;
                text.guiText.text = "WARNING: ATTACK FROM A NEW DIMENSION!";
                Invoke("textSwitchWorlds", 2f);
                
                multiplyer += 0.75f;
                level++;
                threshold = 30000;
				audio.PlayOneShot(newWorldAvailableSound);
            } else if (threshold == 30000) {
                GameObject text = (GameObject)Instantiate(floatingText, this.transform.position + new Vector3(0,15f), Quaternion.identity);
                text.guiText.fontSize = 30;
                text.GetComponent<floatingPoints>().scroll = 4f;
                text.guiText.text = "WARNING: ATTACK FROM A NEW DIMENSION!";
                multiplyer += 1f;
                level++;
				audio.PlayOneShot(newWorldAvailableSound);
                threshold = 9999999999999;
            } 
        }
    }*/

    /*void textSwitchWorlds() {
        GameObject text = (GameObject)Instantiate(floatingText, this.transform.position + new Vector3(0, 15f), Quaternion.identity);
        text.guiText.fontSize = 30;
        text.GetComponent<floatingPoints>().scroll = 4f;
        text.guiText.text = "PRESS Q OR E TO SWITCH!";
    }*/
}
