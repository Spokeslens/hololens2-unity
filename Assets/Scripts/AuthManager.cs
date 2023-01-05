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
    string API_URL = "https://spokeslens.azurewebsites.net";
    
    // Scene manager
    IMixedRealitySceneSystem sceneSystem;

    // Start is called before the first frame update
    async void Start()
    {
        sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        await sceneSystem.LoadContent("WaitingAuth", LoadSceneMode.Single);

        StartCoroutine(GetAccount());
    }

    IEnumerator GetAccount()
    {
        // Device UUID of Hololens
        var deviceId = SystemInfo.deviceUniqueIdentifier;

        // Spawn HTTP request object
        var www = UnityWebRequest.Get(API_URL + "/patient");

        // Include UUID on request
        www.SetRequestHeader("device", deviceId);

        // Fetch
        yield return www.SendWebRequest();

        if(www.responseCode == 200) // Patient data exists
        {
            // String -> JSON conversion
            PlayerPrefs.SetString("patient", www.downloadHandler.text);
            PlayerPrefs.Save();

            sceneSystem.LoadContent("HandMenu", LoadSceneMode.Single);
        }
        else // Unassociated device
        {
            sceneSystem.LoadContent("Auth", LoadSceneMode.Single);
        }
    }
}
