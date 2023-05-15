using System.Diagnostics;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

public class DisplayAccount : MonoBehaviour
{
    public Text profileName;
    public RawImage profileImage;

    private string account;

    private HiveProfileFetcher hiveProfileFetcher;

    private void Start()
    {
        hiveProfileFetcher = FindObjectOfType<HiveProfileFetcher>();
        account = PlayerPrefs.GetString("account");
        StartCoroutine(hiveProfileFetcher.FetchProfile(account, HandleProfileData));
    }

    private void HandleProfileData(string error, JObject profileData)
    {
        if (error != null)
        {
            Debug.LogError("Failed to fetch profile data: " + error);
            return;
        }

        if (profileData == null)
        {
            Debug.LogError("Profile data is null.");
            return;
        }

        JObject jsonMetadata = null;
        JObject parsedProfileData = null;

        if (profileData["json_metadata"] != null)
        {
            jsonMetadata = JObject.Parse((string)profileData["json_metadata"]);
            parsedProfileData = jsonMetadata["profile"] as JObject;
        }

        if (parsedProfileData == null && profileData["posting_json_metadata"] != null)
        {
            jsonMetadata = JObject.Parse((string)profileData["posting_json_metadata"]);
            parsedProfileData = jsonMetadata["profile"] as JObject;
        }

        if (parsedProfileData == null)
        {
            Debug.LogError("JSON metadata is missing 'profile' property.");
            return;
        }

        account = (string)parsedProfileData["account"];
        profileName.text = (string)parsedProfileData["name"];

        StartCoroutine(LoadProfileImage((string)parsedProfileData["profile_image"]));
    }

    private IEnumerator LoadProfileImage(string url)
    {
        yield return LoadImage(url, (error, texture) =>
        {
            if (error == null)
            {
                profileImage.texture = texture;
            }
            else
            {
                Debug.LogError("Failed to load profile image: " + error);
            }
        });
    }

    private IEnumerator LoadImage(string url, Action<string, Texture2D> onCompleted)
    {
        if (string.IsNullOrEmpty(url))
        {
            yield break;
        }

        Debug.Log("Loading image from URL: " + url);

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load image: " + www.error);
                onCompleted?.Invoke(www.error, null);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                onCompleted?.Invoke(null, texture);
            }
        }
    }
}
