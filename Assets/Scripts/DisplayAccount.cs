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
    public Text profileName;
    public RawImage profileImage;
    public static string HiveProfileName="";
    private string account;

    private HiveProfileFetcher hiveProfileFetcher;
    public HiveProfileData hiveProfile;
    private void Start()
    {
        hiveProfileFetcher = FindObjectOfType<HiveProfileFetcher>();
        account = PlayerPrefs.GetString("account");
        Debug.LogError(account);
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
        Debug.LogError(profileData.ToString());
      // hiveProfile = JsonUtility.FromJson<HiveProfileData>(profileData.ToString());
        JObject jsonMetadata = null;
        JObject parsedProfileData = null;
      hiveProfile=  JsonConvert.DeserializeObject<HiveProfileData>(profileData.ToString());
        //if(profileData["json_metadata"] != null)
        //{
        //    jsonMetadata = JObject.Parse((string)profileData["json_metadata"]);
        //    parsedProfileData = jsonMetadata["profile"] as JObject;
        //}

        //if(parsedProfileData == null && profileData["posting_json_metadata"] != null)
        //{
        //    jsonMetadata = JObject.Parse((string)profileData["posting_json_metadata"]);
        //    parsedProfileData = jsonMetadata["profile"] as JObject;
        //}

        //if(parsedProfileData == null)
        //{
        //    Debug.LogError("JSON metadata is missing 'profile' property.");
        //    return;
        //}

        //account = (string)parsedProfileData["account"];
        //profileName.text = (string)parsedProfileData["name"];
        Debug.LogError(hiveProfile.name);
        account = hiveProfile.id.ToString();
        profileName.text = hiveProfile.name.ToString(); //(string)parsedProfileData["name"];
        PlayerPrefs.SetString("HiveProfileName",profileName.text.ToString());
       
        HiveProfileName = profileName.text.ToString();
        
        //StartCoroutine(LoadProfileImage((string)parsedProfileData["profile_image"]));
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
//Hive profile class

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

[System.Serializable]
public class Active
{
    public int weight_threshold { get; set; }
    public List<object> account_auths { get; set; }
    public List<List<object>> key_auths { get; set; }
}

[System.Serializable]
public class DownvoteManabar
{
    public int current_mana { get; set; }
    public int last_update_time { get; set; }
}

[System.Serializable]
public class Owner
{
    public int weight_threshold { get; set; }
    public List<object> account_auths { get; set; }
    public List<List<object>> key_auths { get; set; }
}

[System.Serializable]
public class Posting
{
    public int weight_threshold { get; set; }
    public List<object> account_auths { get; set; }
    public List<List<object>> key_auths { get; set; }
}

[System.Serializable]
public class HiveProfileData
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
public class VotingManabar
{
    public int current_mana { get; set; }
    public int last_update_time { get; set; }
}

