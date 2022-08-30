using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public static bool isDebugMode;

    public static float mapSizeX;
    public static float mapSizeZ;
    public TMP_InputField SizeSetter;

    public Toggle DebugToggler;

    void Start() {
        mapSizeX = Mathf.Max(Mathf.Min(mapSizeX,50),10);
        mapSizeZ = mapSizeX;

        SizeSetter.text = (mapSizeX/5).ToString();

        DebugToggler.isOn = isDebugMode;
    }

    public void MapSize(string sizeText) {
        if (!int.TryParse(sizeText, out int size))
        {
            size = 5;
            SizeSetter.text = "25";
        }
        else {
            size = Mathf.Max(Mathf.Min(size,10),1); // Adjusts the map size from the minumum to the maximum
            SizeSetter.text = size.ToString();
        }

        mapSizeX = size*5;
        mapSizeZ = size*5;
    }

    public void DebugMode(bool tog) {
        isDebugMode = tog;
        print(isDebugMode);
    }

    public void QuitGame() {
        SceneManager.LoadScene(0);
    }
}