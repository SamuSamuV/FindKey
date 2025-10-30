using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaChat : MonoBehaviour
{
    [System.Serializable]
    public class OllamaResponse
    {
        public string response;
    }

    public string model = "llama3.1:8b";

    public IEnumerator SendMessageToOllama(string prompt, System.Action<string> callback)
    {
        string url = "http://localhost:11434/api/generate";

        string jsonBody = JsonUtility.ToJson(new
        {
            model = model,
            prompt = prompt
        });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawText = request.downloadHandler.text;
                OllamaResponse response = JsonUtility.FromJson<OllamaResponse>(rawText);
                callback?.Invoke(response.response);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                callback?.Invoke(null);
            }
        }
    }
}
