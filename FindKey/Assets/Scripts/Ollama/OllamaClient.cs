using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
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

    /// <summary>
    /// Envía prompt a Ollama. onSuccess recibe la respuesta (campo "response") ya des-escaped.
    /// </summary>
    public IEnumerator SendPrompt(string prompt, Action<string> onSuccess, Action<string> onError = null)
    {
        if (string.IsNullOrEmpty(baseUrl)) { onError?.Invoke("Base URL vacía."); yield break; }
        if (string.IsNullOrEmpty(model)) { onError?.Invoke("Modelo no configurado."); yield break; }

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

            // Extraemos "response" robustamente y decodificamos secuencias escapadas
            var m = Regex.Match(raw, "\"response\":\"(.*?)\"[,}]");
            if (m.Success)
            {
                string clean = m.Groups[1].Value;
                clean = Regex.Unescape(clean);
                onSuccess?.Invoke(clean.Trim());
            }
            else
            {
                // fallback: si no encuentra "response", enviar raw
                onError?.Invoke("No se pudo extraer 'response' del JSON: " + raw);
            }
        }
    }
}