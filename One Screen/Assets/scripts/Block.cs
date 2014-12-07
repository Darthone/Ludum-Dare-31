using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour {

    public int color = 0;  // used to find matching blocks
    public int x = 0;
    public int y = 0;
    public int dir = 0;

    // this moves the block to the correct location if applicable
    /*public void refresh(int [,] grid) {
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
    }*/


	// Use this for initialization
	void Start () {
        color = (int)Random.Range(0,4);

        switch (color){
            case 0:
                this.renderer.material.color = new Color(.267f, 1f, .278f); // 68 255 71
                //this.renderer.material.color = Color.green;
                break;
            case 1:
                this.renderer.material.color = new Color(.29f, .835f, 1f); // 74 213 254
                //this.renderer.material.color = Color.blue;
                break;
            case 2:
                this.renderer.material.color = new Color(.941f, 1f, .373f); // 240 255 95
                //this.renderer.material.color = Color.yellow;
                break;
            case 3:
                this.renderer.material.color = new Color(.99f, .30f, .251f); //253 77 66
                //this.renderer.material.color = Color.red;
                break;
            case 4:
                //this.renderer.material.color = Color.white;
                break;
            case 5:
                //this.renderer.material.color = Color.white;
                break;


        }
	}

    public List<GameObject> checkColors(List<GameObject> found, int entry = -1) {
        if(found.Contains(this.gameObject)){
            return new List<GameObject>();
        } else {
            found.Add(this.gameObject);
        }
        if (x > 0 && GameController.control.blockGrid[x - 1, y] != null && GameController.control.blockGrid[x - 1, y].GetComponent<Block>().color == this.color) {
            found.Add(GameController.control.blockGrid[x - 1, y]);
            found.AddRange(GameController.control.blockGrid[x - 1, y].GetComponent<Block>().checkColors(found));
        }
        if (x < GameController.control.gridWidth - 1 && GameController.control.blockGrid[x + 1, y] != null && GameController.control.blockGrid[x + 1, y].GetComponent<Block>().color == this.color) {
            found.Add(GameController.control.blockGrid[x + 1, y]);
            found.AddRange(GameController.control.blockGrid[x + 1, y].GetComponent<Block>().checkColors(found));
        }
        if (y > 0 && GameController.control.blockGrid[x, y - 1] != null && GameController.control.blockGrid[x, y - 1].GetComponent<Block>().color == this.color) {
            found.Add(GameController.control.blockGrid[x, y - 1]);
            found.AddRange(GameController.control.blockGrid[x, y - 1].GetComponent<Block>().checkColors(found));
        }
        if (y < GameController.control.gridHeight - 1 && GameController.control.blockGrid[x, y + 1] != null && GameController.control.blockGrid[x, y + 1].GetComponent<Block>().color == this.color) {
            found.Add(GameController.control.blockGrid[x, y + 1]);
            found.AddRange(GameController.control.blockGrid[x, y + 1].GetComponent<Block>().checkColors(found));
        }
        return found;
    }

	// Update is called once per frame
	void Update () {
        this.transform.position = new Vector2(x + .5f , y + .5f);
	}

    /*void FixedUpdate() {
        // check for similar colors
        List<Block> attached = checkColors();
        for( int i = 0; i < attached.Count; i ++){
            //destory and add to points
        }
    }*/
}
