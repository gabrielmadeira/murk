using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScrip : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject playerPreFab;
    public GameObject blindMonsterPrefab;
    public GameObject goalPrefab;

    public int numberOfBlindMonsters;

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
        
        GameObject wall1 = Instantiate(wallPrefab, new Vector3(scale_x, 1.98f, 0), Quaternion.Euler(0, 90, 0));
        wall1.name = "East Wall";

        GameObject wall2 = Instantiate(wallPrefab, new Vector3(-scale_x, 1.98f, 0), Quaternion.Euler(0, 90, 0));
        wall2.name = "West Wall";


        wallPrefab.transform.localScale = new Vector3(2*scale_x, 4, 2);

        GameObject wall3 = Instantiate(wallPrefab, new Vector3(0, 1.98f, scale_z), Quaternion.Euler(0, 0, 0));
        wall3.name = "North Wall";

        GameObject wall4 = Instantiate(wallPrefab, new Vector3(0, 1.98f, -scale_z), Quaternion.Euler(0, 0, 0));
        wall4.name = "South Wall";
    }

    void SpawnMonster() {
        
        int monstersPlaced = 0;
        while (monstersPlaced < numberOfBlindMonsters)
        {
            do { // Looks for a place to spawn the monster far from the player
                randomPosition = new Vector3(UnityEngine.Random.Range(-scale_x+5,scale_x-5+1),1,UnityEngine.Random.Range(-scale_z+5,scale_z-5+1));
            } while (Vector3.Magnitude(randomPosition) < (scale_x+scale_z)/2);

            GameObject monster = Instantiate(blindMonsterPrefab, randomPosition, blindMonsterPrefab.transform.localRotation);

            monstersPlaced ++; // Counts the monster just instantiated

            monster.name = "Monster " + monstersPlaced; // Gives the monster a number
        }
    }

    void PlaceGoal() {
        do { // Looks for a place to spawn the goal far from the player
            randomPosition = new Vector3(UnityEngine.Random.Range(-scale_x+5,scale_x-5+1),2,UnityEngine.Random.Range(-scale_z+5,scale_z-5+1));
        } while (Vector3.Magnitude(randomPosition) < (scale_x+scale_z)/2);

        GameObject goal = Instantiate(goalPrefab, randomPosition, goalPrefab.transform.localRotation);
        goal.name = "Goal";
    }
}
