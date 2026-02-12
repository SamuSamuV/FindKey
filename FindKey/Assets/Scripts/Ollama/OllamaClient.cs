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
    
    // REDUCIDO: 2048 tokens suele caber en gráficas de 6GB-8GB. 
    // Si sigue fallando, prueba a bajarlo a 1024.
    [Range(1024, 8192)] public int contextSize = 2048; 

    [Serializable]
    public class OllamaOptions
    {
        public int num_gpu;       // -1 = Todo a la GPU
        public int num_ctx;       // Tamaño de la "memoria" de conversación
        public float temperature; // Creatividad
    }

    [Serializable]
    public class OllamaRequest
    {
        public string model;
        public string prompt;
        public bool stream;
        public int keep_alive;
        public OllamaOptions options; // <--- AÑADIDO: Opciones avanzadas
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

        // CONFIGURACIÓN DE RENDIMIENTO
        OllamaOptions myOptions = new OllamaOptions
        {
            num_gpu = 99,           // Forzamos 99 capas a la GPU (básicamente "todo")
            num_ctx = contextSize,  // Limitamos el contexto para que quepa en VRAM
            temperature = 0.7f
        };

        OllamaRequest payload = new OllamaRequest
        {
            model = model,
            prompt = prompt,
            stream = false,
            keep_alive = -1,        // Mantiene el modelo cargado
            options = myOptions
        };

        string jsonBody = JsonUtility.ToJson(payload);

        // DEBUG: Ver qué estamos enviando para asegurar que las opciones van bien
        // Debug.Log($"[Ollama Request]: {jsonBody}");

        using (UnityWebRequest request = new UnityWebRequest(baseUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 120; // 2 minutos de espera máx

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
                    onError?.Invoke("Respuesta vacía.");
                }
            }
            catch (Exception e)
            {
                onError?.Invoke($"Error parseando: {e.Message}");
            }
        }
    }
}