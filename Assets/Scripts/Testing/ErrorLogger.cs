using UnityEngine;
using System.IO;

public class ErrorLogger : MonoBehaviour {

    private static string path = "/logs/";
    private static string currentPath;
    private static string filename = "null";

    private static string tempText;

    private void Start()
    {
        currentPath = $"{path}{System.DateTime.Now.ToString("MM-dd-yyyy")}/";
    }

    private static void Initialize()
    {
        if (filename == "null")
            filename = $"Log-{System.DateTime.Now.ToString("HH-mm-ss")}.txt";
        else
            tempText = $"/n{File.ReadAllText(Application.dataPath + currentPath + filename)}";

        Directory.CreateDirectory(Application.dataPath + currentPath);
    }

    public static void Log(string report, bool closeGame = false)
    {
        Initialize();

        string formReport = $"{report} - {System.DateTime.Now.ToString("HH-mm-ss")}";
        File.WriteAllText(Application.dataPath + currentPath + filename, formReport + tempText);
        Debug.LogWarning(report);

        if (closeGame) Application.Quit();
    }

    public static void Log(string report, MonoBehaviour script, bool closeGame = false)
    {
        Initialize();

        string formReport = $"{report} ({script.name}) - {System.DateTime.Now.ToString("HH-mm-ss")}";
        File.WriteAllText(Application.dataPath + currentPath + filename, formReport + tempText);
        Debug.LogWarning(report);

        script.enabled = false;

        if (closeGame) Application.Quit();
    }
}
