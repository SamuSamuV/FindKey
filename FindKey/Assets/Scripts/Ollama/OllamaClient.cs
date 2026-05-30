/// <summary>
/// Class: OllamaClient
/// Description: This script defines the OllamaClient class, which is responsible for communicating with the Ollama API to generate responses based on prompts.
///              It includes configuration options for the model, context size, and other parameters that can be adjusted to optimize performance.
///              The class uses Unity's UnityWebRequest to send POST requests to the Ollama API and handles the responses accordingly.
///              It also includes error handling to manage potential issues during the API communication process.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaClient : MonoBehaviour
{
    [Header("Ollama Settings")]
    public string model; //Model name to use in Ollama (e.g., "llama3.1:8b", "llama3.2.3b", etc.)
    public string baseUrl = "http://localhost:11434/api/generate"; // Base URL of the Ollama API. Adjust if your Ollama server is running on a different address or port.

    [Range(1024, 8192)] public int contextSize = 2048;

    /// <summary>
    /// Class: OllamaOptions
    /// Description: This class defines the options that can be sent in the request to the Ollama API. It includes parameters for GPU usage, context size, and temperature,
    ///              which can be adjusted to optimize performance and control the randomness of the generated responses.
    /// </summary>
    [Serializable]
    public class OllamaOptions
    {
        public int num_gpu; // Set to 99 to use all available GPUs
        public int num_ctx; // Context size (e.g., 2048, 4096, etc.)
        public float temperature; // Controls randomness in the output. Higher values (e.g., 0.8) make output more random, while lower values (e.g., 0.2) make it more deterministic.
    }

    /// <summary>
    /// Class: OllamaRequest
    /// Description: This class defines the structure of the request that will be sent to the Ollama API. It includes the model name, the input prompt, whether to use streaming responses,
    ///              the keep-alive time for the connection, and the options defined in the OllamaOptions class.
    /// </summary>
    [Serializable]
    public class OllamaRequest
    {
        public string model; // Model name to use in Ollama
        public string prompt; // The input prompt for the model
        public bool stream; // Whether to receive the response as a stream (true) or as a single response (false). For simplicity, we use false here.
        public int keep_alive; // Time in seconds to keep the connection alive. Set to -1 for no timeout.
        public OllamaOptions options; // Additional options for the request, such as GPU usage, context size, and temperature.
    }

    /// <summary>
    /// Class: OllamaApiResponse
    /// Description: This class defines the structure of the response received from the Ollama API. It includes the generated response text and a boolean indicating whether the response is complete.
    /// </summary>
    [Serializable]
    private class OllamaApiResponse
    {
        public string response; // The generated response from the Ollama API
        public bool done; // Indicates whether the response is complete (useful for streaming responses, but we set stream to false in this implementation)
    }

/// <summary>
    /// Sends a prompt to the local Ollama API asynchronously and processes the JSON response.
    /// </summary>
    /// <param name="prompt">The input text or instruction to be sent to the language model.</param>
    /// <param name="onSuccess">Callback executed if the communication is successful, returning the generated response.</param>
    /// <param name="onError">Optional callback executed if an HTTP or parsing error occurs, returning the error message.</param>
    /// <returns>IEnumerator for the coroutine execution (UnityWebRequest).</returns>
    public IEnumerator SendPrompt(string prompt, Action<string> onSuccess, Action<string> onError = null) // Coroutine to send a prompt to the Ollama API and handle the response
    {
        if (string.IsNullOrEmpty(baseUrl)) { onError?.Invoke("Base URL vacía."); yield break; }

        OllamaOptions myOptions = new OllamaOptions // Set the options for the request, including GPU usage, context size, and temperature. Adjust these values as needed to optimize performance and control the randomness of the generated responses.
        {
            num_gpu = 99,
            num_ctx = contextSize,
            temperature = 0.7f
        };

        OllamaRequest payload = new OllamaRequest // Create the request payload with the specified model, prompt, streaming option, keep-alive time, and options.
        {
            model = model,
            prompt = prompt,
            stream = false,
            keep_alive = -1,
            options = myOptions
        };

        string jsonBody = JsonUtility.ToJson(payload); // Serialize the request payload to JSON format for sending to the Ollama API.

        // Debug.Log($"[Ollama Request]: {jsonBody}");

        using (UnityWebRequest request = new UnityWebRequest(baseUrl, "POST")) // Create a new UnityWebRequest for sending a POST request to the Ollama API with the specified base URL.
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody); // Convert the JSON body to a byte array for sending in the request.
            request.uploadHandler = new UploadHandlerRaw(bodyRaw); // Set the upload handler with the raw byte array of the JSON body.
            request.downloadHandler = new DownloadHandlerBuffer(); // Set the download handler to buffer the response from the Ollama API.
            request.SetRequestHeader("Content-Type", "application/json"); // Set the Content-Type header to indicate that the request body is in JSON format.
            request.timeout = 180; // Set a timeout for the request to prevent hanging indefinitely. Adjust this value as needed based on expected response times from the Ollama API.

            yield return request.SendWebRequest(); // Send the request and wait for the response from the Ollama API. This is a coroutine, so it will yield until the request is complete.

            if (request.result != UnityWebRequest.Result.Success) // Check if the request was successful. If not, invoke the onError callback with the error message and exit the coroutine.
            {
                onError?.Invoke($"HTTP Error: {request.error}");
                yield break;
            }

            string raw = request.downloadHandler.text; // Get the raw response text from the download handler. This should contain the JSON response from the Ollama API.

            try // Attempt to parse the JSON response into an OllamaApiResponse object. If successful and the response is not empty, invoke the onSuccess callback with the generated response text. If the response is empty, invoke the onError callback with an appropriate message.
            {
                OllamaApiResponse apiResponse = JsonUtility.FromJson<OllamaApiResponse>(raw); // Parse the JSON response from the Ollama API into an OllamaApiResponse object.

                if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.response)) // Check if the parsed response is not null and contains a non-empty response string. If so, invoke the onSuccess callback with the generated response text.
                {
                    onSuccess?.Invoke(apiResponse.response);
                }

                else // If the response is null or empty, invoke the onError callback with a message indicating that the response was empty.
                {
                    onError?.Invoke("Respuesta vacía.");
                }
            }

            catch (Exception e) // If an exception occurs during JSON parsing, invoke the onError callback with a message indicating that there was an error parsing the response, along with the exception message for debugging purposes.
            {
                onError?.Invoke($"Error parseando: {e.Message}");
            }
        }
    }
}