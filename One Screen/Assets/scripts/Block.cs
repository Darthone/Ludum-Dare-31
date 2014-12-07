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
        color = (int)Random.Range(0,GameController.control.level);
        switch (color){ // maybe some day different colors will be have differently
            case 0: //green
                this.renderer.material.color = new Color(.267f, 1f, .278f); // 68 255 71
                break;
            case 1://cyan
                this.renderer.material.color = new Color(.29f, .835f, 1f); // 74 213 254
                break;
            case 2://yellow
                this.renderer.material.color = new Color(.941f, 1f, .373f); // 240 255 95
                break;
            // END BASE COLORS
            case 3://red
                this.renderer.material.color = new Color(.99f, .30f, .251f); //253 77 66
                break;
            case 4://orange
                this.renderer.material.color = new Color(1f, .53f, .235f);  //R: 255 G: 135 B: 60
                break;
            case 5://darkblue
                this.renderer.material.color = new Color(.28f, .5f, 1f); //R: 72 G: 127 B: 254
                break;
            case 6://pink
                this.renderer.material.color = new Color(.918f, .313f, .682f); //R: 234 G: 80 B: 174
                break;
            case 7://white
                this.renderer.material.color = new Color(1f, 1f, 1f);
                break;
            case 8: //black -- can't be destoryed TODO maybe
                this.renderer.material.color = new Color(0f, 0f, 0f);
                break;

        }
	}

    public List<GameObject> checkColors(List<GameObject> found) {
        //List<GameObject> result = new List<GameObject>();
        // remove duplicates

        // look around at other blocks
        if(found.Contains(this.gameObject)){
            return new List<GameObject>();
        } else {
            //result.Add(this.gameObject);
            found.Add(this.gameObject);
        }
        if (x > 0 && GameController.control.blockGrid[x - 1, y] != null && GameController.control.blockGrid[x - 1, y].GetComponent<Block>().color == this.color) {
            //found.Add(GameController.control.blockGrid[x - 1, y]);
            found.AddRange(GameController.control.blockGrid[x - 1, y].GetComponent<Block>().checkColors(found));
        }
        if (x < GameController.control.gridWidth - 1 && GameController.control.blockGrid[x + 1, y] != null && GameController.control.blockGrid[x + 1, y].GetComponent<Block>().color == this.color) {
            //found.Add(GameController.control.blockGrid[x + 1, y]);
            found.AddRange(GameController.control.blockGrid[x + 1, y].GetComponent<Block>().checkColors(found));
        }
        if (y > 0 && GameController.control.blockGrid[x, y - 1] != null && GameController.control.blockGrid[x, y - 1].GetComponent<Block>().color == this.color) {
            //found.Add(GameController.control.blockGrid[x, y - 1]);
            found.AddRange(GameController.control.blockGrid[x, y - 1].GetComponent<Block>().checkColors(found));
        }
        if (y < GameController.control.gridHeight - 1 && GameController.control.blockGrid[x, y + 1] != null && GameController.control.blockGrid[x, y + 1].GetComponent<Block>().color == this.color) {
            //found.Add(GameController.control.blockGrid[x, y + 1]);
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
