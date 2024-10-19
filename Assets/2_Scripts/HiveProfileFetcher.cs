///////////////////////////////////////////////////////////
/// HiveProfileFetcher.cs
/// 
/// This script is responsible for fetching the profile data of a Hive account.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class HiveProfileFetcher : MonoBehaviour
{
    public IEnumerator FetchProfile(string account, Action<string, JObject> callback)//Fetch profile data of the specified account
    {
        string apiUrl = "https://api.hive.blog";//Hive API URL
        string postData = "{\"jsonrpc\":\"2.0\", \"method\":\"condenser_api.get_accounts\", \"params\":[[\"" + account + "\"]], \"id\":1}";//API request data

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))//This part of the code sends a POST request to the Hive API
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(postData);//Convert the request data to bytes
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);//Set the request data
            request.downloadHandler = new DownloadHandlerBuffer();//Set the download handler
            request.SetRequestHeader("Content-Type", "application/json");//Set the request header

            yield return request.SendWebRequest();//Send the request

            if (request.result == UnityWebRequest.Result.Success)//If the request is successful
            {
                JObject jsonResponse = JObject.Parse(request.downloadHandler.text);//Parse the JSON response
                Debug.Log("API Response: " + jsonResponse.ToString());//Log the response
                JArray accounts = (JArray)jsonResponse["result"];//Get the account data from the response

                if (accounts.Count > 0)//If the account date is found
                {
                    JObject profileData = (JObject)accounts[0];//Get the profile data
                    callback(null, profileData);//Invoke the callback with the profile data
                }
                else
                {
                    callback("Account not found", null);//Invoke the callback with an error message
                }
            }
            else
            {
                callback(request.error, null);//Invoke the callback with an error message
            }
        }

    }

    ////////////////////////////////////////////////////////////////////////
    //This method fetches the profile data of the logged-in account
    //the difference between this method and the previous one is that it doesn't take an account name as a parameter
    //it fetches the profile data of the logged-in account
    ////////////////////////////////////////////////////////////////////////

    public IEnumerator FetchProfile(Action<string, JObject> callback)//Fetch profile data of the logged in account
    {
        string account = PlayerPrefs.GetString("account");//Get the logged-in account name
        string apiUrl = "https://api.hive.blog";//Hive API URL
        string postData = "{\"jsonrpc\":\"2.0\", \"method\":\"condenser_api.get_accounts\", \"params\":[[\"" + account + "\"]], \"id\":1}";//API request data

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))//This part of the code sends a POST request to the Hive API
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(postData);//Convert the request data to bytes
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);//Set the request data
            request.downloadHandler = new DownloadHandlerBuffer();//Set the download handler
            request.SetRequestHeader("Content-Type", "application/json");//Set the request header

            yield return request.SendWebRequest();//Send the request

            ////////////////
            //Request logic
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
