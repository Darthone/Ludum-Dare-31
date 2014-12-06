using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

    public int color = 0;  // used to find matching blocks
    public int x = 0;
    public int y = 0;
    public int dir = 0;

    public float width = 25;
    public float height = 10;
    //gridWidth
    // this moves the block to the correct location if applicable
    public void refresh(int [,] grid) {
        grid[x, y] = -1;
        switch(dir){
            case 0:
                if (y - 1 >= 0 && grid[x, y - 1] == -1){
                    y--;
                }
                break;
            case 1:
                if (x + 1 < GameController.control.gridWidth && grid[x + 1, y] == -1){
                    x++;
                }
                break;
            case 2:
                if (y + 1 < GameController.control.gridHeight && grid[x, y + 1] == -1) {
                    y++;
                }
                break;
            case 3:
                if (x - 1 >= 0 && grid[x - 1, y] == -1){
                    x--;
                }
                break;
        }
        grid[x, y] = color;
    }


	// Use this for initialization
	void Start () {
        color = (int)Random.Range(0,5);

        switch (color){
            case 0:
                this.renderer.material.color = Color.green;
                break;
            case 1:
                this.renderer.material.color = Color.blue;
                break;
            case 2:
                this.renderer.material.color = Color.yellow;
                break;
            case 3:
                this.renderer.material.color = Color.red;
                break;
            case 4:
                this.renderer.material.color = Color.white;
                break;
            case 5:
                this.renderer.material.color = Color.white;
                break;


        }
	}
	
	// Update is called once per frame
	void Update () {
        print((float)Screen.width / GameController.control.gridWidth);
        print(Screen.width);
        this.transform.position = new Vector2(x + .5f , y + .5f);
	}
}
