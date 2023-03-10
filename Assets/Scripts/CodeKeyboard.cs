using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CodeKeyboard : MonoBehaviour
{
    string API_URL = "https://spokeslens.azurewebsites.net";

    IMixedRealitySceneSystem sceneSystem;

    TouchScreenKeyboard keyboard;
    TMP_InputField inputField;

    void Start()
    {
        // Open keyboard
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
        keyboard.characterLimit = 6;

        // Get scene system
        sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

        // Physical input field
        inputField = GetComponent<TMP_InputField>();
    }

    void Update()
    {
        if (keyboard != null)
        {
            // Get text from keyboard
            var code = keyboard.text;

            // Copy it into field
            inputField.text = code;

            if (code.Length == 6)
            {
                // Attempt registeration
                StartCoroutine(RegisterDevice(code));
            }
        }
    }

    IEnumerator RegisterDevice(string code)
    {
        // Device UUID of Hololens
        var deviceId = SystemInfo.deviceUniqueIdentifier;

        // Spawn HTTP request object
        var www = new UnityWebRequest(API_URL + "/patient", "POST");

        // Encode body
        var rawBody = Encoding.UTF8.GetBytes($"{{\"code\":\"{code}\"}}");

        // Embed body
        www.uploadHandler = new UploadHandlerRaw(rawBody);

        // JSON request
        www.SetRequestHeader("content-type", "application/json");

        // Include UUID on request
        www.SetRequestHeader("device", deviceId);

        // Fetch
        yield return www.SendWebRequest();

        Debug.Log(www.responseCode);

        if (www.responseCode == 200) // Patient data exists
        {
            // String -> JSON conversion
            PlayerPrefs.SetString("patient", www.downloadHandler.text);
            PlayerPrefs.Save();

            sceneSystem.LoadContent("HandMenu", LoadSceneMode.Single);
        }
        else // Unassociated device
        {
            keyboard.text = "";
        }
    }
}
