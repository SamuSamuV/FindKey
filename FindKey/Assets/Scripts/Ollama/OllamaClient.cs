using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaClient : MonoBehaviour
{
    [Header("Ollama Settings")]
    public string model = "llama3.1:8b";
    public string baseUrl = "http://localhost:11434/api/generate";

    [Serializable]
    public class OllamaRequest
    {
        public string model;
        public string prompt;
        public bool stream;
        // IMPORTANTE: Este campo evita que la IA se descargue de la RAM
        public int keep_alive;
    }

    [Serializable]
    private class OllamaApiResponse
    {
        public string response;
        public bool done;
    }

    public IEnumerator SendPrompt(string prompt, Action<string> onSuccess, Action<string> onError = null)
    {
        if (string.IsNullOrEmpty(baseUrl)) { onError?.Invoke("Base URL vacía."); yield break; }
        if (string.IsNullOrEmpty(model)) { onError?.Invoke("Modelo no configurado."); yield break; }

        // CAMBIO CLAVE: keep_alive = -1 significa "mantener en RAM para siempre"
        OllamaRequest payload = new OllamaRequest
        {
            model = model,
            prompt = prompt,
            stream = false,
            keep_alive = -1
        };

        string jsonBody = JsonUtility.ToJson(payload);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"HTTP Error: {request.error}");
                yield break;
            }

            string raw = request.downloadHandler.text;

            try
            {
                OllamaApiResponse apiResponse = JsonUtility.FromJson<OllamaApiResponse>(raw);
                if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.response))
                {
                    onSuccess?.Invoke(apiResponse.response);
                }
                else
                {
                    onError?.Invoke("Respuesta vacía de Ollama.");
                }
            }
            catch (Exception e)
            {
                onError?.Invoke($"Error parseando: {e.Message}");
            }
        }
    }
}