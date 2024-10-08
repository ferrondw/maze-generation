#if UNITY_EDITOR
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;

public static class Ollama
{
    public const string ApiUrl = "http://localhost:11434/api/chat";

    private static readonly List<ChatMessage> conversationHistory = new()
    {
        new ChatMessage { role = "system", content = "" }
    };

    public static async Task Generate(string userMessage)
    {
        conversationHistory.Add(new ChatMessage { role = "user", content = userMessage });
        
        ChatRequestBody requestBody = new ChatRequestBody
        {
            model = "llama3.1",
            messages = conversationHistory,
            stream = false
        };
        await GenerateResponse(requestBody);
    }

    public static void SetSystemPrompt(string prompt) => conversationHistory[0] = new ChatMessage { role = "system", content = prompt };
    public static void Reset() => conversationHistory.Clear();

    public static string LastMessage()
    {
        for (int i = conversationHistory.Count - 1; i >= 0; i--)
        {
            if (conversationHistory[i].role == "assistant")
            {
                return conversationHistory[i].content;
            }
        }

        return string.Empty;
    }

    private static async Task GenerateResponse(ChatRequestBody requestBody)
    {
        var jsonBody = JsonUtility.ToJson(requestBody);

        using UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        var rawBody = Encoding.UTF8.GetBytes(jsonBody);
        
        request.uploadHandler = new UploadHandlerRaw(rawBody);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        var asyncOperation = request.SendWebRequest();
        while (!asyncOperation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success) return;
        
        var responseBody = JsonUtility.FromJson<ChatResponseBody>(request.downloadHandler.text);
        if (responseBody?.message == null) return;
        conversationHistory.Add(new ChatMessage { role = "assistant", content = responseBody.message.content });
    }

    [System.Serializable]
    private class ChatRequestBody
    {
        public string model;
        public List<ChatMessage> messages;
        public bool stream;
    }

    [System.Serializable]
    private class ChatResponseBody
    {
        public ChatMessage message;
    }

    [System.Serializable]
    private class ChatMessage
    {
        public string role;
        public string content;
    }
}
#endif