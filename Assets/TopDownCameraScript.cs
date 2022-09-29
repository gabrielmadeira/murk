using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCameraScript : MonoBehaviour
{
    // Ground
    public Camera topDownCamera;

    private float scale_x;
    private float scale_z;

    // Start is called before the first frame update
    void Start()
    {
        scale_x = 10*(OptionsMenu.mapSizeX)/2;
        scale_z = 10*(OptionsMenu.mapSizeZ)/2;
        topDownCamera.orthographicSize = Mathf.Max(scale_x,scale_z);
    }
}
