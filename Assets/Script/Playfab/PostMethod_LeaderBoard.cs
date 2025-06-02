﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using static GlobalResponseData_Login;

public class PostMethod_LeaderBoard : MonoBehaviour
{
    public TMP_InputField outputArea; // TextMeshPro element to display status messages

    // UI Text elements for the top 5 players
    public TextMeshProUGUI[] fullNameTexts;
    public TextMeshProUGUI[] scoreTexts;
    public TextMeshProUGUI[] student_idTexts;
    public TextMeshProUGUI[] MedalTexts;

    private string apiUrl = "http://1.55.212.49:8098/DemoBackend3D_API/player/getLeaderboard";

    void Start()
    {
        if (outputArea == null)
        {
            outputArea = GameObject.Find("OutputArea").GetComponent<TMP_InputField>();
        }
    }

    public void LoadLeaderboardData()
    {
        StartCoroutine(LoadLeaderboardData_Coroutine());
    }

    IEnumerator LoadLeaderboardData_Coroutine()
    {
        outputArea.text = "Loading leaderboard data...";

        // Create session data object
        SessionData sessionData = new SessionData
        {
            session_id = GlobalResponseData.session_id
        };

        // Convert session data to JSON
        string jsonData = JsonUtility.ToJson(sessionData);

        // Create UnityWebRequest for POST
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for response
        yield return request.SendWebRequest();

        // Handle errors or successful response
        if (request.isNetworkError || request.isHttpError)
        {
            outputArea.text = request.error;
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            LeaderboardResponse responseData = JsonUtility.FromJson<LeaderboardResponse>(jsonResponse);

            if (responseData.code == 200)
            {
                outputArea.text = "Top 5 Players Loaded Successfully!";

                // Sort the leaderboard by score in descending order and take the top 5 players
                List<LeaderboardPlayer> topPlayers = responseData.leaderboard_player
     .OrderByDescending(player => player.details_state.Medal) // Sort by Medal first
     .ThenByDescending(player => player.score) // Sort by score if Medal is the same
     .Take(5)
     .ToList();

                // Update UI Text elements for the top 5 players
                for (int i = 0; i < topPlayers.Count; i++)
                {
                    fullNameTexts[i].text = topPlayers[i].fullname + "-" + topPlayers[i].student_id;
        
                    scoreTexts[i].text = topPlayers[i].score.ToString();
                    MedalTexts[i].text = topPlayers[i].details_state.Medal.ToString();
                }
            }
            else
            {
                outputArea.text = $"Error {responseData.code}: {responseData.desc}";
            }
        }
    }

    [System.Serializable]
    public class SessionData
    {
        public string session_id;
    }

    [System.Serializable]
    public class LeaderboardResponse
    {
        public int code;
        public string desc;
        public List<LeaderboardPlayer> leaderboard_player;
    }

    [System.Serializable]
    public class LeaderboardPlayer
    {
        public int ID;
        public string name;
        public string last_login;
        public string fullname;
        public string student_id;
        public int score;
        public Details_state details_state; // This should match the response
    }

    [System.Serializable]
    public class Details_state
    {
        public int Medal;
    }

}
