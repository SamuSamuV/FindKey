using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaClient : MonoBehaviour
{
    [Header("Ollama Settings")]
    [Tooltip("Modelo que usar (por ejemplo: llama3.1:8b)")]
    public string model = "llama3.1:8b";

    [Tooltip("URL del servidor Ollama. Por defecto: http://localhost:11434/api/generate")]
    public string baseUrl = "http://localhost:11434/api/generate";

    [Serializable]
    public class OllamaRequest
    {
        public string model;
        public string prompt;
        public bool stream;
    }

    /// <summary>
    /// Envía un prompt a Ollama y devuelve la respuesta en el callback onSuccess.
    /// </summary>
    public IEnumerator SendPrompt(string prompt, Action<string> onSuccess, Action<string> onError = null)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            onError?.Invoke("Base URL vacía o no configurada.");
            yield break;
        }

        if (string.IsNullOrEmpty(model))
        {
            onError?.Invoke("Modelo no configurado.");
            yield break;
        }

        // Construye el cuerpo de la petición (JSON válido)
        OllamaRequest payload = new OllamaRequest
        {
            model = model,
            prompt = prompt,
            stream = false // desactivamos el stream para que devuelva la respuesta completa
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

            //Extraemos el campo "response" correctamente con Regex
            Match match = Regex.Match(raw, "\"response\":\"(.*?)\"[,}]");
            if (match.Success)
            {
                string cleanResponse = match.Groups[1].Value;
                cleanResponse = Regex.Unescape(cleanResponse);
                onSuccess?.Invoke(cleanResponse.Trim());
            }
            else
            {
                onError?.Invoke("No se pudo leer el campo 'response'.");
            }
        }
    }
}