using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScrip : MonoBehaviour
{
    public GameObject wallPrefab;

    private float scale_x;
    private float scale_y;

    // Start is called before the first frame update
    void Start()
    {
        BuildWalls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Builds walls of the right size for the plane
    void BuildWalls()
    {
        scale_x = 10*transform.localScale.x;
        scale_y = 10*transform.localScale.y;

        wallPrefab.transform.localScale = new Vector3(scale_x, 4, 2);

        Instantiate(wallPrefab, new Vector3(scale_x/2, 1.98f, 0), Quaternion.Euler(0, 90, 0));
        Instantiate(wallPrefab, new Vector3(-scale_x/2, 1.98f, 0), Quaternion.Euler(0, 90, 0));

        Instantiate(wallPrefab, new Vector3(0, 1.98f, scale_x/2), Quaternion.Euler(0, 0, 0));
        Instantiate(wallPrefab, new Vector3(0, 1.98f, -scale_x/2), Quaternion.Euler(0, 0, 0));


    }
}
