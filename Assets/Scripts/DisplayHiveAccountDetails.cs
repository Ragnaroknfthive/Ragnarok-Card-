//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DisplayHiveAccountDetails : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}

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

    //void Start()
    //{
    //    profileFetcher = GetComponent<HiveProfileFetcher>();

    //    // Fetch the user profile data
    //    StartCoroutine(profileFetcher.FetchProfile((error,profileData) => {
    //        if(string.IsNullOrEmpty(error))
    //        {
    //            // Successfully fetched the profile data, now display it
    //            JObject postingJsonMetadata = JObject.Parse((string)profileData["posting_json_metadata"]);
    //            Debug.Log("posting_json_metadata: " + postingJsonMetadata);
    //            usernameText.text = (string)profileData["name"];
    //            string profileImageUrl = (string)postingJsonMetadata["profile"]["profile_image"];
    //            Debug.Log("Profile image URL: " + profileImageUrl);
    //            StartCoroutine(FetchProfilePicture(profileImageUrl));
    //        }
    //        else
    //        {
    //            // Failed to fetch the profile data, log an error
    //            Debug.LogError("Failed to fetch profile: " + error);
    //        }
    //    }));
    //}


    void Start()
    {
        profileFetcher = GetComponent<HiveProfileFetcher>();
        string account = "";
        if(PlayerPrefs.HasKey("account"))
        { 
            account = PlayerPrefs.GetString("account");
        }

        Debug.LogError("Account "+account);
        // Fetch the user profile data
        StartCoroutine(profileFetcher.FetchProfile((error,profileData) => {
            if(string.IsNullOrEmpty(error))
            {
                // Check if profileData contains the key "posting_json_metadata"
                if(profileData.ContainsKey("posting_json_metadata"))
                {
                    string postingJsonMetadataStr = (string)profileData["posting_json_metadata"];

                    // Check if postingJsonMetadataStr is not null or empty
                    if(!string.IsNullOrEmpty(postingJsonMetadataStr))
                    {
                        try
                        {
                            JObject postingJsonMetadata = JObject.Parse(postingJsonMetadataStr);
                           // Debug.Log("posting_json_metadata: " + postingJsonMetadata);
                            usernameText.text = (string)profileData["name"];
                            string profileImageUrl = (string)postingJsonMetadata["profile"]["profile_image"];
                           // Debug.Log("Profile image URL: " + profileImageUrl);
                            GameData.hasProfileImage = true;
                            StartCoroutine(FetchProfilePicture(profileImageUrl));
                        }
                        catch(System.Exception ex)
                        {
                            if(string.IsNullOrEmpty(account))
                            {
                               // Debug.LogError("Account not found");
                                GameData.hasProfileImage = false;
                                int randomprofileImageIndex = Random.Range(0,GameData.Get().DummyProfile.Count);
                                GameData.dummyProfileIndex = randomprofileImageIndex;

                                if(dummyProfileimages[randomprofileImageIndex])
                                    profileImageRegular.sprite = dummyProfileimages[randomprofileImageIndex];

                                usernameText.text = Photon.Pun.PhotonNetwork.LocalPlayer.NickName;
                            }
                            Debug.LogError("Error parsing posting_json_metadata: " + ex.Message);
                        }
                    }
                    else
                    {

                        GameData.hasProfileImage = false;
                        if(string.IsNullOrEmpty(account))
                        {
                            //Debug.LogError("Account not found");
                            GameData.hasProfileImage = false;
                            int randomprofileImageIndex = Random.Range(0,GameData.Get().DummyProfile.Count);
                            GameData.dummyProfileIndex = randomprofileImageIndex;

                            if(dummyProfileimages[randomprofileImageIndex])
                                profileImageRegular.sprite = dummyProfileimages[randomprofileImageIndex];


                            usernameText.text = Photon.Pun.PhotonNetwork.LocalPlayer.NickName;
                        }
                        //Debug.LogError("posting_json_metadata is empty or null.");
                    }
                }
                else
                {
                    if(string.IsNullOrEmpty(account))
                    {
                      //  Debug.LogError("Account not found");
                        GameData.hasProfileImage = false;
                        int randomprofileImageIndex = Random.Range(0,GameData.Get().DummyProfile.Count);
                        GameData.dummyProfileIndex = randomprofileImageIndex;

                        if(dummyProfileimages[randomprofileImageIndex])
                            profileImageRegular.sprite = dummyProfileimages[randomprofileImageIndex];

                        usernameText.text = Photon.Pun.PhotonNetwork.LocalPlayer.NickName;
                    }
                   // Debug.LogError("profileData does not contain the key 'posting_json_metadata'.");
                }
            }
            else
            {
                if(string.IsNullOrEmpty(account)) 
                {
                   // Debug.LogError("Account not found");
                    GameData.hasProfileImage = false;
                    int randomprofileImageIndex = Random.Range(0,GameData.Get().DummyProfile.Count);
                    GameData.dummyProfileIndex = randomprofileImageIndex;

                    if(dummyProfileimages[randomprofileImageIndex])
                        profileImageRegular.sprite = dummyProfileimages[randomprofileImageIndex];

                    usernameText.text = Photon.Pun.PhotonNetwork.LocalPlayer.NickName;
                }
                // Failed to fetch the profile data, log an error
              //  Debug.LogError("Failed to fetch profile: " + error);
            }
        }));
    }

    private IEnumerator FetchProfilePicture(string url)
    {
       // Debug.Log("Fetching profile picture from URL: " + url);
        GameData.playerProfileUrl = url;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if(www.result != UnityWebRequest.Result.Success)
        {
            GameData.hasProfileImage = false;
            int randomprofileImageIndex = Random.Range(0,GameData.Get().DummyProfile.Count);
            GameData.dummyProfileIndex = randomprofileImageIndex;
            if(dummyProfileimages[randomprofileImageIndex])
            profileImageRegular.sprite = dummyProfileimages[randomprofileImageIndex];
            //  Debug.Log("Failed to download profile picture: " + www.error);
        }
        else
        {
            Debug.Log("Successfully downloaded profile picture.");

            GameData.hasProfileImage = true;
            GameData.playerProfileUrl = url;
            Texture2D texture2D = ((DownloadHandlerTexture)www.downloadHandler).texture;
            GameData.playerProfileTexture = texture2D;
            profileImage.texture = texture2D; //((DownloadHandlerTexture)www.downloadHandler).texture;
           GameData.playerSprite= GameData.SpriteFromTexture2D(texture2D);
            profileImageRegular.sprite = GameData.playerSprite;
        }
    }
}
