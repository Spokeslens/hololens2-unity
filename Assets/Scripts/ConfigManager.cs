using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public Config config;
    [SerializeField] TextAsset jsonFile;

    void Start()
    {
        config = JsonUtility.FromJson<Config>(jsonFile.text);
    }
}

public class Config
{
    public string API_URL;
}
