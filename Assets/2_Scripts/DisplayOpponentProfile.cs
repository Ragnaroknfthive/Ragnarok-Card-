////////////////////////////////////////////////////////
///DisplayOpponentProfile.cs
///
///This script is responsible for displaying the profile of the opponent in the game.
///It fetches the profile data of the opponent from the Hive blockchain and displays the name and profile image.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Debug = UnityEngine.Debug;

public class DisplayOpponentProfile : MonoBehaviour
{
    public Text opponentProfileName;//Text field to display the name of the opponent
    public RawImage opponentProfileImage;//RawImage field to display the profile image of the opponent

    private HiveProfileFetcher hiveProfileFetcher;//Reference to the HiveProfileFetcher script

    public void DisplayOpponent(string opponentAccount)//Method to display the profile of the opponent
    {
        Debug.Log("DisplayOpponent called with account: " + opponentAccount);//Log the account of the opponent
        hiveProfileFetcher = FindObjectOfType<HiveProfileFetcher>();//Find the HiveProfileFetcher script in the scene
        StartCoroutine(hiveProfileFetcher.FetchProfile(opponentAccount, HandleOpponentProfileData));//Fetch the profile data of the opponent
    }

    private void HandleOpponentProfileData(string error, JObject profileData)//Method to handle the profile data of the opponent
    {
        if (error != null)//If there is an error
        {
            Debug.LogError("Failed to fetch profile data: " + error);//Log the error
            return;
        }

        if (profileData == null)//If the profile data is null
        {
            Debug.LogError("Profile data is null.");//Log that the profile data is null
            return;
        }

        JObject jsonMetadata = null;//JSON object to store the parsed profile data
        JObject parsedProfileData = null;//JSON object to store the parsed profile data

        if (profileData["json_metadata"] != null)//If the profile data contains 'json_metadata' property
        {
            jsonMetadata = JObject.Parse((string)profileData["json_metadata"]);//Parse the 'json_metadata' property
            parsedProfileData = jsonMetadata["profile"] as JObject;//Get the 'profile' property from the parsed JSON object
        }

        if (parsedProfileData == null && profileData["posting_json_metadata"] != null)//If the parsed profile data is null and the profile data contains 'posting_json_metadata' property
        {
            jsonMetadata = JObject.Parse((string)profileData["posting_json_metadata"]);//Parse the 'posting_json_metadata' property
            parsedProfileData = jsonMetadata["profile"] as JObject;//Get the 'profile' property from the parsed JSON object
        }

        if (parsedProfileData == null)
        {
            Debug.LogError("JSON metadata is missing 'profile' property.");//Log that the JSON metadata is missing 'profile' property
            return;
        }

        string name = (string)parsedProfileData["name"];//Get the name of the opponent from the parsed profile data
        opponentProfileName.text = name;//Display the name of the opponent

        StartCoroutine(LoadProfileImage((string)parsedProfileData["profile_image"]));//Load the profile image of the opponent
    }

    private IEnumerator LoadProfileImage(string url)
    {
        yield return LoadImage(url, (error, texture) =>//Load the profile image from the URL
        {
            if (error == null)//If there is no error
            {
                opponentProfileImage.texture = texture;//Display the profile image
            }
            /*else
            {
                Debug.LogError("Failed to load profile image: " + error);//Log that the profile image failed to load
            }*/
        });
    }

    private IEnumerator LoadImage(string url, Action<string, Texture2D> onCompleted)//Method to load the image from the URL
    {
        if (string.IsNullOrEmpty(url))//If the URL is empty
        {
            yield break;
        }

        //Debug.Log("Loading image from URL: " + url);//Log that the image is being loaded from the URL

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))//Create a UnityWebRequest to load the image
        {
            yield return www.SendWebRequest();//Send the request

            if (www.result != UnityWebRequest.Result.Success)//If the request is not successful
            {
                //Debug.LogError("Failed to load image: " + www.error);//Log that the image failed to load
                onCompleted?.Invoke(www.error, null);//Invoke the onCompleted callback with the error
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);//Get the texture from the request
                onCompleted?.Invoke(null, texture);//Invoke the onCompleted callback with the texture
            }
        }
    }
}
