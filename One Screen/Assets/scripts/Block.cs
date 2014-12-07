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

    public List<GameObject> checkColors(List<GameObject> found, int entry = -1) {
        if(found.Contains(this.gameObject)){
            return new List<GameObject>();
        } else {
            found.Add(this.gameObject);
        }
        Block left = null;
        Block right = null;
        Block top = null;
        Block bottom = null;
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

        /*switch (entry) {
            case -1: //check all sides
                if (left && !found.Contains(left))
                    found.AddRange(left.checkColors(1));
                if (right)
                    found.AddRange(right.checkColors(3));
                if (top)
                    found.AddRange(top.checkColors(2));
                if (bottom)
                    found.AddRange(bottom.checkColors(0));

                break;
            case 0: // check left right and bottom
                if (left)
                    found.AddRange(left.checkColors(1));
                if (right)
                    found.AddRange(right.checkColors(3));
                if (bottom)
                    found.AddRange(bottom.checkColors(0));
                break;
            case 1: //top bottom left
                if (left)
                    found.AddRange(left.checkColors(1));
                if (top)
                    found.AddRange(top.checkColors(2));
                if (bottom)
                    found.AddRange(bottom.checkColors(0));
                break;
            case 2: // left right top
                if (left)
                    found.AddRange(left.checkColors(1));
                if (right)
                    found.AddRange(right.checkColors(3));
                if (top)
                    found.AddRange(top.checkColors(2));
                break;
            case 3: // top bottom right
                if (right)
                    found.AddRange(right.checkColors(3));
                if (top)
                    found.AddRange(top.checkColors(2));
                if (bottom)
                    found.AddRange(bottom.checkColors(0));
                break;
        }*/
        //return found;
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
