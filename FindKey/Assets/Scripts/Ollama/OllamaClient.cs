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
    }

    // Clase interna para leer el "sobre" de la API de Ollama
    [Serializable]
    private class OllamaApiResponse
    {
        public string response; // El texto real que generó la IA
        public bool done;
    }

    public IEnumerator SendPrompt(string prompt, Action<string> onSuccess, Action<string> onError = null)
    {
        if (string.IsNullOrEmpty(baseUrl)) { onError?.Invoke("Base URL vacía."); yield break; }
        if (string.IsNullOrEmpty(model)) { onError?.Invoke("Modelo no configurado."); yield break; }

        // Pedimos stream false para recibir todo de golpe
        OllamaRequest payload = new OllamaRequest { model = model, prompt = prompt, stream = false };
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

            // DEBUG: Respuesta cruda de la API de Ollama (el JSON envoltorio)
            Debug.Log($"[OllamaClient] Respuesta cruda de la API de Ollama:\n{raw}");

            // 1) Desempaquetar la respuesta de la API de Ollama
            try
            {
                OllamaApiResponse apiResponse = JsonUtility.FromJson<OllamaApiResponse>(raw);
                if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.response))
                {
                    // DEBUG: El JSON interno (respuesta limpia de la IA) que se pasa a BaseAIScript
                    Debug.Log($"[OllamaClient] Respuesta de la IA (JSON interno):\n{apiResponse.response}");

                    // ¡Éxito! Pasamos solo el contenido interno (el JSON de la IA)
                    onSuccess?.Invoke(apiResponse.response);
                }
                else
                {
                    onError?.Invoke("La API de Ollama respondió, pero el campo 'response' estaba vacío.");
                }
            }
            catch (Exception e)
            {
                onError?.Invoke($"Error parseando respuesta de Ollama: {e.Message}. Raw: {raw}");
            }
        }
    }
}