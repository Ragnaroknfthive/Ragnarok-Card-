////////////////////////////////////////////////
///DisplayHiveAccountDetails.cs
///
///This script is used to display the user's Hive account details such as username and profile picture.
///It fetches the user's profile data from the Hive blockchain using the HiveProfileFetcher script.
///The user's profile picture is fetched using the FetchProfilePicture coroutine.
///The user's profile picture is displayed in the RawImage component profileImage.
///The user's username is displayed in the Text component usernameText.

using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class DisplayHiveAccountDetails : MonoBehaviour
{
    public Text usernameText;
    public RawImage profileImage;
    public Image profileImageRegular;
    private HiveProfileFetcher profileFetcher;
    public List<Sprite> dummyProfileimages = new List<Sprite>();


    void Start()
    {
        profileFetcher = GetComponent<HiveProfileFetcher>();//This is a HiveProfileFetcher variable that stores the HiveProfileFetcher component.
        string account = "";//This is a string variable that stores the user's Hive account name.
        if (PlayerPrefs.HasKey("account"))//This is a condition that checks if the PlayerPrefs contains the key "account".
        {
            account = PlayerPrefs.GetString("account");//This is a string variable that stores the value of the key "account".
        }

        //Debug.LogError("Account " + account);//This is an error message that is displayed in the console when the account is fetched from the PlayerPrefs, in other words, when the account is not empty.
        StartCoroutine(profileFetcher.FetchProfile((error, profileData) =>
        {
            if (string.IsNullOrEmpty(error))//This is a condition that checks if the error message is empty.
            {
                if (profileData.ContainsKey("posting_json_metadata"))//This is a condition that checks if the profileData contains the key "posting_json_metadata".
                {
                    string postingJsonMetadataStr = (string)profileData["posting_json_metadata"];//This is a string variable that stores the value of the key "posting_json_metadata".
                    if (!string.IsNullOrEmpty(postingJsonMetadataStr))//This is a condition that checks if the postingJsonMetadataStr is not empty.
                    {
                        try
                        {
                            JObject postingJsonMetadata = JObject.Parse(postingJsonMetadataStr);//This is a JObject variable that stores the parsed postingJsonMetadataStr.
                            usernameText.text = (string)profileData["name"];//This sets the username in the Text component usernameText.
                            string profileImageUrl = (string)postingJsonMetadata["profile"]["profile_image"];//This is a string variable that stores the URL of the profile picture.
                            GameData.hasProfileImage = true;//This is a boolean variable that is set to true when the profile picture is successfully downloaded.
                            StartCoroutine(FetchProfilePicture(profileImageUrl));//This is a coroutine that fetches the profile picture using the FetchProfilePicture coroutine.
                        }
                        catch (System.Exception ex)//This is a catch statement that catches any exception that occurs during the parsing of the posting_json_metadata.
                        {
                            if (string.IsNullOrEmpty(account))//This is a condition that checks if the account is empty.
                            {
                                GameData.hasProfileImage = false;//This is a boolean variable that is set to false when the profile picture is not successfully downloaded.
                                int randomprofileImageIndex = Random.Range(0, GameData.Get().DummyProfile.Count);//This is an integer variable that stores a random number between 0 and the number of dummy profile images.
                                GameData.dummyProfileIndex = randomprofileImageIndex;//This sets the dummy profile index to the randomprofileImageIndex.

                                if (dummyProfileimages[randomprofileImageIndex])//This is a condition that checks if the dummyProfileimages contains the randomprofileImageIndex.
                                    profileImageRegular.sprite = dummyProfileimages[randomprofileImageIndex];//This sets the profile picture in the Image component profileImageRegular.

                                usernameText.text = Photon.Pun.PhotonNetwork.LocalPlayer.NickName;//This sets the username in the Text component usernameText.
                            }
                            Debug.LogError("Error parsing posting_json_metadata: " + ex.Message);//This is an error message that is displayed in the console when there is an error parsing the posting_json_metadata.
                        }
                    }
                    else
                    {

                        GameData.hasProfileImage = false;//This is a boolean variable that is set to false when the profile picture is not successfully downloaded.
                        if (string.IsNullOrEmpty(account))//This is a condition that checks if the account is empty.
                        {
                            GameData.hasProfileImage = false;//This is a boolean variable that is set to false when the profile picture is not successfully downloaded.
                            int randomprofileImageIndex = Random.Range(0, GameData.Get().DummyProfile.Count);//This is an integer variable that stores a random number between 0 and the number of dummy profile images.
                            GameData.dummyProfileIndex = randomprofileImageIndex;//This sets the dummy profile index to the randomprofileImageIndex.

                            if (dummyProfileimages[randomprofileImageIndex])//This is a condition that checks if the dummyProfileimages contains the randomprofileImageIndex.
                                profileImageRegular.sprite = dummyProfileimages[randomprofileImageIndex];//This sets the profile picture in the Image component profileImageRegular.


                            usernameText.text = Photon.Pun.PhotonNetwork.LocalPlayer.NickName;//This sets the username in the Text component usernameText.
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(account))//This is a condition that checks if the account is empty.
                    {
                        GameData.hasProfileImage = false;//This is a boolean variable that is set to false when the profile picture is not successfully downloaded.
                        int randomprofileImageIndex = Random.Range(0, GameData.Get().DummyProfile.Count);//This is an integer variable that stores a random number between 0 and the number of dummy profile images.
                        GameData.dummyProfileIndex = randomprofileImageIndex;//This sets the dummy profile index to the randomprofileImageIndex.

                        if (dummyProfileimages[randomprofileImageIndex])//This is a condition that checks if the dummyProfileimages contains the randomprofileImageIndex.
                            profileImageRegular.sprite = dummyProfileimages[randomprofileImageIndex];//This sets the profile picture in the Image component profileImageRegular.

                        usernameText.text = Photon.Pun.PhotonNetwork.LocalPlayer.NickName;//This sets the username in the Text component usernameText.
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(account)) //This is a condition that checks if the account is empty.
                {
                    GameData.hasProfileImage = false;//This is a boolean variable that is set to false when the profile picture is not successfully downloaded.
                    int randomprofileImageIndex = Random.Range(0, GameData.Get().DummyProfile.Count);//This is an integer variable that stores a random number between 0 and the number of dummy profile images.
                    GameData.dummyProfileIndex = randomprofileImageIndex;//This sets the dummy profile index to the randomprofileImageIndex.

                    if (dummyProfileimages[randomprofileImageIndex])//This is a condition that checks if the dummyProfileimages contains the randomprofileImageIndex.
                        profileImageRegular.sprite = dummyProfileimages[randomprofileImageIndex];//This sets the profile picture in the Image component profileImageRegular.

                    usernameText.text = Photon.Pun.PhotonNetwork.LocalPlayer.NickName;//This sets the username in the Text component usernameText.
                }
            }
        }));
    }

    private IEnumerator FetchProfilePicture(string url)//This is a coroutine that fetches the profile picture using the URL of the profile picture.
    {
        GameData.playerProfileUrl = url;//This is a string variable that stores the URL of the profile picture.
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);//This is a UnityWebRequest variable that gets the texture of the profile picture using the URL.
        yield return www.SendWebRequest();//This is a yield statement that waits for the UnityWebRequest to send the request.
        if (www.result != UnityWebRequest.Result.Success)//This is a condition that checks if the UnityWebRequest result is not successful.
        {
            GameData.hasProfileImage = false;//This is a boolean variable that is set to false when the profile picture is not successfully downloaded.
            int randomprofileImageIndex = Random.Range(0, GameData.Get().DummyProfile.Count);//This is an integer variable that stores a random number between 0 and the number of dummy profile images.
            GameData.dummyProfileIndex = randomprofileImageIndex;//This sets the dummy profile index to the randomprofileImageIndex.
            if (dummyProfileimages[randomprofileImageIndex])//This is a condition that checks if the dummyProfileimages contains the randomprofileImageIndex.
                profileImageRegular.sprite = dummyProfileimages[randomprofileImageIndex];//This sets the profile picture in the Image component profileImageRegular.
        }
        else
        {
            //Debug.Log("Successfully downloaded profile picture.");//This is a log message that is displayed in the console when the profile picture is successfully downloaded.

            GameData.hasProfileImage = true;//This is a boolean variable that is set to true when the profile picture is successfully downloaded.
            GameData.playerProfileUrl = url;//This is a string variable that stores the URL of the profile picture.
            Texture2D texture2D = ((DownloadHandlerTexture)www.downloadHandler).texture;//This is a Texture2D variable that stores the downloaded profile picture.
            GameData.playerProfileTexture = texture2D;//This is a Texture2D variable that stores the downloaded profile picture.
            profileImage.texture = texture2D; //This sets the profile picture in the RawImage component profileImage.
            GameData.playerSprite = GameData.SpriteFromTexture2D(texture2D);//This is a Sprite variable that stores the profile picture as a Sprite.
            profileImageRegular.sprite = GameData.playerSprite;//This sets the profile picture in the Image component profileImageRegular.
        }
    }
}
