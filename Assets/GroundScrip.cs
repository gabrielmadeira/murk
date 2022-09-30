using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScrip : MonoBehaviour
{
    // All prefabs for instantiation at the start of the game
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    public GameObject blindMonsterPrefab;
    public GameObject goalPrefab;
    public GameObject envObjectPrefab;
    public List<AudioClip> envObjectsClips;
    public GameObject envSoundPrefab;
    public List<AudioClip> envSoundsClips;
    public GameObject cricketPrefab;
    public List<AudioClip> cricketClips;

    public Camera darkCamera;

    public int numberOfBlindMonsters;
    private float numberObjElements;
    private float numberCrickets;

    private float scale_x;
    private float scale_z;

    private Vector3 playerStartingPosition;
    private Vector3 randomPosition;

    public List<GameObject> envObjectsAndSounds;
    public List<int> envObjectsAndSoundsDelay;

    private int minEnvObjectAndSoundDelay = 10; // in seconds
    private int maxEnvObjectAndSoundDelay = 100;

    private float timer;
    private int lastSecond;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(OptionsMenu.mapSizeX, 1, OptionsMenu.mapSizeZ);

        scale_x = 10*transform.localScale.x/2;
        scale_z = 10*transform.localScale.z/2;

        PlacePlayer();

        BuildWalls();
        SpawnMonsters();
        PlaceGoal();

        PlaceEnvObjects();

        PlaceEnvSounds();

        PlaceCrickets();
    }

    // Update is called once per frame
    void Update()
    {
        // Places dark camera above or below the others
        if (OptionsMenu.isDebugMode)
            darkCamera.depth = 0;
        else
            darkCamera.depth = 10;
    }

    void FixedUpdate() 
    {
        timer += Time.deltaTime;
        int second = (int)(timer % 60);
        if(second>lastSecond) {
            for(int i=0; i<envObjectsAndSounds.Count; i++) {
                if(envObjectsAndSoundsDelay[i] <= 0) {
                    AudioSource source = envObjectsAndSounds[i].GetComponent<AudioSource>();
                    if (i+1 < numberObjElements) // Toca sons de objetos normalmente
                    {
                        //Debug.Log("playing object " + i + " sound with: " + source.volume + " volume");
                        source.Play();
                    }
                    else // Toca sons de ambiente
                    {
                        float original_vol = source.volume;
                        source.volume = Random.Range(0.1f,1f); // Varia o volume dos sons de ambiente
                        //Debug.Log("playing enviroment " + i + " sound with: " + source.volume + " volume");
                        source.Play();
                        source.volume = original_vol;
                    }
                    envObjectsAndSoundsDelay[i] = Random.Range(minEnvObjectAndSoundDelay, maxEnvObjectAndSoundDelay);
                }
                envObjectsAndSoundsDelay[i]--;
            }
        }
        lastSecond = second;
    }

    void PlacePlayer()
    {
        // Looks for a random place to spawn player
        playerStartingPosition = new Vector3(UnityEngine.Random.Range(-scale_x+5,scale_x-5+1),1,UnityEngine.Random.Range(-scale_z+5,scale_z-5+1));

        // Spawns it
        GameObject player = Instantiate(playerPrefab, playerStartingPosition, Quaternion.Euler(0, Random.Range(0,360), 0));
        player.name = "Player";
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

    void SpawnMonsters() {
        
        int monstersPlaced = 0;
        while (monstersPlaced < numberOfBlindMonsters)
        {
            do { // Looks for a place to spawn the monster far from the player
                randomPosition = new Vector3(UnityEngine.Random.Range(-scale_x+5,scale_x-5+1),1,UnityEngine.Random.Range(-scale_z+5,scale_z-5+1));
            } while (Vector3.Distance(randomPosition, playerStartingPosition) < (scale_x+scale_z)/2);

            GameObject monster = Instantiate(blindMonsterPrefab, randomPosition, blindMonsterPrefab.transform.localRotation);

            monstersPlaced ++; // Counts the monster just instantiated

            monster.name = "Monster " + monstersPlaced; // Gives the monster a number
        }
    }

    void PlaceGoal() {
        do { // Looks for a place to spawn the goal far from the player
            randomPosition = new Vector3(UnityEngine.Random.Range(-scale_x+5,scale_x-5+1),2,UnityEngine.Random.Range(-scale_z+5,scale_z-5+1));
        } while (Vector3.Distance(randomPosition, playerStartingPosition) < (scale_x+scale_z)/2);

        GameObject goal = Instantiate(goalPrefab, randomPosition, goalPrefab.transform.localRotation);
        goal.name = "Goal";
    }

    void PlaceEnvObjects() {
        numberObjElements = getAmmountFromDensity(60); // Marks 1 object element for creation per 60 square units of area

        for(int i=0; i < (int)numberObjElements; i++){
            // Looks for a random place to spawn env object
            Vector3 envObjectPosition = new Vector3(UnityEngine.Random.Range(-scale_x+5,scale_x-5+1),1,UnityEngine.Random.Range(-scale_z+5,scale_z-5+1));

            // Spawns it
            GameObject envObject = Instantiate(envObjectPrefab, envObjectPosition, Quaternion.Euler(0, Random.Range(0,360), 0));
            envObject.name = "EnvObject " + (i+1);

            AudioSource source = envObject.GetComponent<AudioSource>();
            int pickedSound = Random.Range(0, envObjectsClips.Count);
            source.clip = envObjectsClips[pickedSound];

            envObjectsAndSounds.Add(envObject);
            envObjectsAndSoundsDelay.Add(Random.Range(minEnvObjectAndSoundDelay, maxEnvObjectAndSoundDelay)+10);
        }
    }

    void PlaceEnvSounds() {
        Vector3 envSoundPosition = new Vector3(1,3,1);

        for(int i=0; i<envSoundsClips.Count; i++) {
            GameObject envSound = Instantiate(envSoundPrefab, envSoundPosition, Quaternion.Euler(0, 0, 0));
            envSound.name = "EnvSound " + (i+1);
            envSound.GetComponent<AudioSource>().clip = envSoundsClips[i];

            envObjectsAndSounds.Add(envSound);
            envObjectsAndSoundsDelay.Add(Random.Range(minEnvObjectAndSoundDelay, maxEnvObjectAndSoundDelay)+10);
        }
    }

    void PlaceCrickets() {
        
        numberCrickets = getAmmountFromDensity(60); // Marks 1 object element for creation per 100 square units of area

        for(int i=0; i<numberCrickets; i++)
        {
            // Looks for a random place to spawn cricket object
            Vector3 envObjectPosition = new Vector3(UnityEngine.Random.Range(-scale_x+5,scale_x-5+1),cricketPrefab.transform.position.y,UnityEngine.Random.Range(-scale_z+5,scale_z-5+1));

            // Spawns it
            GameObject cricket = Instantiate(cricketPrefab, envObjectPosition, Quaternion.Euler(0, Random.Range(0,360), 0));
            cricket.name = "Cricket " + (i+1);

            AudioSource source = cricket.GetComponent<AudioSource>();
            source.clip = cricketClips[Random.Range(0, cricketClips.Count)];

            source.Play();
        }
    }

    private float getAmmountFromDensity(float density) {
        float ammount = (OptionsMenu.mapSizeX*OptionsMenu.mapSizeX)/density; // Marks 1 object element for creation per 70 units
        ammount = Random.Range(ammount - Mathf.Sqrt(ammount),ammount + Mathf.Sqrt(ammount)); // Adds variation to the number of object elements
        return Mathf.Min(Mathf.Max(ammount,1),100); // Keeps the number of elements to a minimum of 1 and a maximum of 100
    }
}
