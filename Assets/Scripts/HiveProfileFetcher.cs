using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class HiveProfileFetcher : MonoBehaviour
{
    public IEnumerator FetchProfile(string account, Action<string, JObject> callback)
    {
        string apiUrl = "https://api.hive.blog";
        string postData = "{\"jsonrpc\":\"2.0\", \"method\":\"condenser_api.get_accounts\", \"params\":[[\"" + account + "\"]], \"id\":1}";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(postData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                JObject jsonResponse = JObject.Parse(request.downloadHandler.text);
                Debug.Log("API Response: " + jsonResponse.ToString());
                JArray accounts = (JArray)jsonResponse["result"];

                if (accounts.Count > 0)
                {
                    JObject profileData = (JObject)accounts[0];
                    callback(null, profileData);
                }
                else
                {
                    callback("Account not found", null);
                }
            }
            else
            {
                callback(request.error, null);
            }
        }
    }
}
