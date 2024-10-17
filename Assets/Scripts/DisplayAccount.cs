/////////////////////////////////////////////////////////////////////
///DisplayAccount.cs
///
///This script is responsible for displaying the account name and profile image of the user.

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
using System.Collections.Generic;
using Newtonsoft.Json;

public class DisplayAccount : MonoBehaviour
{
    public Text profileName;//Text to display the profile name
    public RawImage profileImage;//Raw image to display the profile image
    public static string HiveProfileName = "";//Static string to store the profile name
    private string account;//String to store the account name
    private HiveProfileFetcher hiveProfileFetcher;//HiveProfileFetcher object to fetch the profile data
    public HiveProfileData hiveProfile;//HiveProfileData object to store the profile data
    private void Start()
    {
        hiveProfileFetcher = FindObjectOfType<HiveProfileFetcher>();//Find the HiveProfileFetcher object
        account = PlayerPrefs.GetString("account");//Get the account name from the player prefs
        //Debug.LogError(account);//Log the account name
        StartCoroutine(hiveProfileFetcher.FetchProfile(account, HandleProfileData));//Fetch the profile data
    }

    private void HandleProfileData(string error, JObject profileData)//Handle the profile data
    {
        if (error != null)
        {
            Debug.LogError("Failed to fetch profile data: " + error);//Log the error
            return;
        }

        if (profileData == null)
        {
            Debug.LogError("Profile data is null.");//Log the error
            return;
        }
        Debug.LogError(profileData.ToString());//Log the profile data
        hiveProfile = JsonConvert.DeserializeObject<HiveProfileData>(profileData.ToString());//Deserialize the profile data
        Debug.LogError(hiveProfile.name);//Log the profile name
        account = hiveProfile.id.ToString();//Get the account name
        profileName.text = hiveProfile.name.ToString();//Set the profile name text
        PlayerPrefs.SetString("HiveProfileName", profileName.text.ToString());//Set the profile name in the player prefs
        HiveProfileName = profileName.text.ToString();//Set the profile name
    }

    private IEnumerator LoadProfileImage(string url)//Load the profile image
    {
        yield return LoadImage(url, (error, texture) =>//Load the image
        {
            if (error == null)
            {
                profileImage.texture = texture;//Set the profile image texture
            }
            else
            {
                Debug.LogError("Failed to load profile image: " + error);//Log the error
            }
        });
    }

    private IEnumerator LoadImage(string url, Action<string, Texture2D> onCompleted)
    {
        if (string.IsNullOrEmpty(url))//Check if the URL is empty
        {
            yield break;
        }

        Debug.Log("Loading image from URL: " + url);//Log the URL

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))//Load the image
        {
            yield return www.SendWebRequest();//Send the request

            if (www.result != UnityWebRequest.Result.Success)//Check if the request is successful
            {
                Debug.LogError("Failed to load image: " + www.error);//Log the error
                onCompleted?.Invoke(www.error, null);//Invoke the callback
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);//Get the texture
                onCompleted?.Invoke(null, texture);//Invoke the callback
            }
        }
    }
}

[System.Serializable]
public class Active//Here we define the Active class which contains the properties of the active data.
{
    public int weight_threshold { get; set; }
    public List<object> account_auths { get; set; }
    public List<List<object>> key_auths { get; set; }
}

[System.Serializable]
public class DownvoteManabar//Here we define the DownvoteManabar class which contains the properties
{
    public int current_mana { get; set; }
    public int last_update_time { get; set; }
}

[System.Serializable]
public class Owner//Here we define the Owner class which contains the properties of the owner data.
{
    public int weight_threshold { get; set; }
    public List<object> account_auths { get; set; }
    public List<List<object>> key_auths { get; set; }
}

[System.Serializable]
public class Posting//Here we define the Posting class which contains the properties of the posting data.
{
    public int weight_threshold { get; set; }
    public List<object> account_auths { get; set; }
    public List<List<object>> key_auths { get; set; }
}

[System.Serializable]
public class HiveProfileData//Here we define the HiveProfileData class which contains the properties of the Hive profile data.
{
    public int id { get; set; }
    public string name { get; set; }
    public Owner owner { get; set; }
    public Active active { get; set; }
    public Posting posting { get; set; }
    public string memo_key { get; set; }
    public string json_metadata { get; set; }
    public string posting_json_metadata { get; set; }
    public string proxy { get; set; }
    public string previous_owner_update { get; set; }
    public string last_owner_update { get; set; }
    public string last_account_update { get; set; }
    public string created { get; set; }
    public bool mined { get; set; }
    public string recovery_account { get; set; }
    public string last_account_recovery { get; set; }
    public string reset_account { get; set; }
    public int comment_count { get; set; }
    public int lifetime_vote_count { get; set; }
    public int post_count { get; set; }
    public bool can_vote { get; set; }
    public VotingManabar voting_manabar { get; set; }
    public DownvoteManabar downvote_manabar { get; set; }
    public int voting_power { get; set; }
    public string balance { get; set; }
    public string savings_balance { get; set; }
    public string hbd_balance { get; set; }
    public string hbd_seconds { get; set; }
    public string hbd_seconds_last_update { get; set; }
    public string hbd_last_interest_payment { get; set; }
    public string savings_hbd_balance { get; set; }
    public string savings_hbd_seconds { get; set; }
    public string savings_hbd_seconds_last_update { get; set; }
    public string savings_hbd_last_interest_payment { get; set; }
    public int savings_withdraw_requests { get; set; }
    public string reward_hbd_balance { get; set; }
    public string reward_hive_balance { get; set; }
    public string reward_vesting_balance { get; set; }
    public string reward_vesting_hive { get; set; }
    public string vesting_shares { get; set; }
    public string delegated_vesting_shares { get; set; }
    public string received_vesting_shares { get; set; }
    public string vesting_withdraw_rate { get; set; }
    public string post_voting_power { get; set; }
    public string next_vesting_withdrawal { get; set; }
    public int withdrawn { get; set; }
    public int to_withdraw { get; set; }
    public int withdraw_routes { get; set; }
    public int pending_transfers { get; set; }
    public int curation_rewards { get; set; }
    public int posting_rewards { get; set; }
    public List<int> proxied_vsf_votes { get; set; }
    public int witnesses_voted_for { get; set; }
    public string last_post { get; set; }
    public string last_root_post { get; set; }
    public string last_vote_time { get; set; }
    public int post_bandwidth { get; set; }
    public int pending_claimed_accounts { get; set; }
    public string governance_vote_expiration_ts { get; set; }
    public List<object> delayed_votes { get; set; }
    public int open_recurrent_transfers { get; set; }
    public string vesting_balance { get; set; }
    public int reputation { get; set; }
    public List<object> transfer_history { get; set; }
    public List<object> market_history { get; set; }
    public List<object> post_history { get; set; }
    public List<object> vote_history { get; set; }
    public List<object> other_history { get; set; }
    public List<object> witness_votes { get; set; }
    public List<object> tags_usage { get; set; }
    public List<object> guest_bloggers { get; set; }
}

[System.Serializable]
public class VotingManabar//Here we define the VotingManabar class which contains the properties of the voting manabar.
{
    public int current_mana { get; set; }
    public int last_update_time { get; set; }
}

