using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
    public GameObject[] spawnQueue;
    //public GameObject[] powerupsToSpawn;
    float minSpawnTime = 1f;
    float maxSpawnTime = 3f;
    bool canSpawn = false;
    float powerupChance = 0.20f;
    float spawnZome = 7f;

    IEnumerator delaySpawn(float delay) {
        yield return new WaitForSeconds(delay + 1f);
        canSpawn = true;
        updateSpawner();
    }

	// Use this for initialization
	void Start () {
        updateSpawner();
        StartCoroutine(delaySpawn(Random.Range(minSpawnTime, maxSpawnTime)));
	}
	
	// Update is called once per frame
	void Update () {
	    // pick object to spawn
        if (canSpawn) {
            GameObject block = (GameObject)Instantiate(spawnQueue[0]);
            Vector2 spawn = GameController.control.open[(int)Random.Range(0,GameController.control.open.Count)];
            block.GetComponent<Block>().x = (int)spawn.x;
            block.GetComponent<Block>().y = (int)spawn.y;
            GameController.control.blockGrid[(int)spawn.x, (int)spawn.y] = block;
            //GameController.control.blocks.Add(thisB);
            canSpawn = false;
            StartCoroutine(delaySpawn(Random.Range(minSpawnTime, maxSpawnTime)));

            /*int spawnLayer = (int)Random.Range(8, 9 + GameController.control.level);
            if (Random.value < powerupChance) {
                GameObject powerup = (GameObject)Instantiate(powerupsToSpawn[(int)Mathf.Round(Random.Range(0, enemiesToSpawn.Length - 1))],
                    (this.transform.position + (new Vector3(Random.Range(-spawnZome, spawnZome), Random.Range(-spawnZome, spawnZome), 0f * (spawnLayer - 8) * 100f))),
                    Quaternion.AngleAxis(0f, Vector3.forward));
                powerup.layer = spawnLayer;
                //spawn power up
            } else {
                GameObject enemy = (GameObject)Instantiate(enemiesToSpawn[(int)Mathf.Round(Random.Range(0, enemiesToSpawn.Length - 1))],
                    (this.transform.position + (new Vector3(Random.Range(-spawnZome, spawnZome), Random.Range(-spawnZome, spawnZome), 0f*(spawnLayer - 8) * 100f))), 
                    Quaternion.AngleAxis(0f, Vector3.forward));
                enemy.layer = spawnLayer;

                // spawn enemy
            }*/
        }
	}

   void updateSpawner() {
        minSpawnTime = GameController.control.minSpawnTime;
        maxSpawnTime = GameController.control.maxSpawnTime;
        //powerupChance = GameController.control.powerupChance;
    }
}
