using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    // Load config
    Config config;

    // Current patient loaded
    public Patient currentPatient;

    // Start is called before the first frame update
    void Start()
    {
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
        }
        else // Unassociated device
        {
            // TODO - Pop up
        }
    }
}
