using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    // Load config
    Config config;

    // Current patient loaded
    public Patient currentPatient;
    
    // Scene manager
    IMixedRealitySceneSystem sceneSystem;

    // Start is called before the first frame update
    void Start()
    {
        sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        sceneSystem.LoadContent("WaitingAuth", LoadSceneMode.Single);

        StartCoroutine(GetAccount());
    }

    IEnumerator GetAccount()
    {
        // Device UUID of Hololens
        var deviceId = SystemInfo.deviceUniqueIdentifier;

        // Spawn HTTP request object
        UnityWebRequest www = UnityWebRequest.Get(config.API_URL + "/patient");

        // Include UUID on request
        www.SetRequestHeader("device", deviceId);
        
        // Fetch
        yield return www.SendWebRequest();

        if(www.responseCode == 200) // Patient data exists
        {
            // String -> JSON conversion
            currentPatient = JsonUtility.FromJson<Patient>(www.downloadHandler.text);
            sceneSystem.LoadContent("HandMenu", LoadSceneMode.Single);
        }
        else // Unassociated device
        {
            sceneSystem.LoadContent("Auth", LoadSceneMode.Single);
        }
    }
}
