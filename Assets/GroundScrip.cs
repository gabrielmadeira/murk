using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScrip : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject blindMonsterPrefab;
    public GameObject goalPrefab;

    private float scale_x;
    private float scale_z;

    private Vector3 randomPosition;

    // Start is called before the first frame update
    void Start()
    {
        scale_x = 10*transform.localScale.x/2;
        scale_z = 10*transform.localScale.z/2;

        BuildWalls();
        SpawnMonster();
        PlaceGoal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Builds walls of the right size for the plane
    void BuildWalls()
    {
        wallPrefab.transform.localScale = new Vector3(2*scale_z, 4, 2);
        wallPrefab.name = "East Wall";
        Instantiate(wallPrefab, new Vector3(scale_x, 1.98f, 0), Quaternion.Euler(0, 90, 0));
        wallPrefab.name = "West Wall";
        Instantiate(wallPrefab, new Vector3(-scale_x, 1.98f, 0), Quaternion.Euler(0, 90, 0));

        wallPrefab.transform.localScale = new Vector3(2*scale_x, 4, 2);
        wallPrefab.name = "North Wall";
        Instantiate(wallPrefab, new Vector3(0, 1.98f, scale_z), Quaternion.Euler(0, 0, 0));
        wallPrefab.name = "South Wall";
        Instantiate(wallPrefab, new Vector3(0, 1.98f, -scale_z), Quaternion.Euler(0, 0, 0));
    }

    void SpawnMonster() {
        do { // Looks for a place to spawn the monster far from the player
            randomPosition = new Vector3(UnityEngine.Random.Range(-scale_x+5,scale_x-5+1),1,UnityEngine.Random.Range(-scale_z+5,scale_z-5+1));
            Debug.Log(randomPosition);
        } while (Vector3.Magnitude(randomPosition) < (scale_x+scale_z)/2);

        Instantiate(blindMonsterPrefab, randomPosition, blindMonsterPrefab.transform.localRotation);
    }

    void PlaceGoal() {
        do { // Looks for a place to spawn the monster far from the player
            randomPosition = new Vector3(UnityEngine.Random.Range(-scale_x+5,scale_x-5+1),2,UnityEngine.Random.Range(-scale_z+5,scale_z-5+1));
            Debug.Log(randomPosition);
        } while (Vector3.Magnitude(randomPosition) < (scale_x+scale_z)/2);

        Instantiate(goalPrefab, randomPosition, goalPrefab.transform.localRotation);
    }
}
