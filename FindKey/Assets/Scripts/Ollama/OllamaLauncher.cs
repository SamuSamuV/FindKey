using UnityEngine;
using System.Diagnostics;
using System.IO;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Class: OllamaLauncher
/// Description: This script is responsible for launching the Ollama server when the game starts. It checks for the presence of the ollama.exe file in the StreamingAssets/Ollama
///              folder and starts the server as a background process. The script also sets up environment variables to specify the models directory and GPU usage for Ollama.
///              It includes functionality to kill any existing Ollama processes before starting a new one, ensuring that there are no conflicts. Additionally,
///              it preloads a specified model into VRAM by sending a request to the Ollama API after the server has started.
///              The script also handles cleanup by killing the Ollama process when the application quits or when the object is destroyed.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class OllamaLauncher : MonoBehaviour
{
    [Header("Configuración")]
    public string modelToLoad; // Model name to preload into VRAM (must be in the 'models' folder)
    public bool showDebugLogs = true; // Toggle to show or hide Ollama server logs in the Unity console, this is very useful for debugging but can be overwhelming, so you can turn it off if you want a cleaner console.

    private Process ollamaProcess; // Reference to the Ollama server process
    public static bool IsServerRunning = false; // Static flag to indicate if the Ollama server is currently running

    void Awake()
    {
        KillOllamaByForce(); // Ensure no existing Ollama processes are running before starting a new one

        StartOllamaServer(); // Start the Ollama server as a background process
        DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed when loading new scenes, so the server keeps running in the background
    }

/// <summary>
    /// Starts the Ollama server as a background process. 
    /// Sets up the necessary environment variables (OLLAMA_MODELS, OLLAMA_NUM_GPU) to force GPU usage and redirects the models directory to StreamingAssets.
    /// </summary>
    void StartOllamaServer() // Method to start the Ollama server
    {
        string streamingPath = Application.streamingAssetsPath; // Get the path to the StreamingAssets folder

        string ollamaPath = Path.Combine(streamingPath, "Ollama", "ollama.exe"); // Construct the full path to the ollama.exe file
        string modelsPath = Path.Combine(streamingPath, "Ollama", "models"); // Construct the full path to the models directory where Ollama will look for models

        if (!File.Exists(ollamaPath)) // Check if the ollama.exe file exists at the specified path
        {
            UnityEngine.Debug.LogError("FATAL: No se encontró ollama.exe en: " + ollamaPath);
            return;
        }

        if (!Directory.Exists(modelsPath)) // Check if the models directory exists, if not, create it
        {
            UnityEngine.Debug.LogWarning("[Launcher] Carpeta 'models' no encontrada. Creándola en: " + modelsPath);
            Directory.CreateDirectory(modelsPath);
        }

        ProcessStartInfo startInfo = new ProcessStartInfo // Configure the process start info for launching the Ollama server
        {
            FileName = ollamaPath, // Set the executable to run (ollama.exe)
            Arguments = "serve", // Set the arguments to start the server
            UseShellExecute = false, // Required to redirect output and error streams
            RedirectStandardOutput = true, // Redirect the standard output to capture server logs
            RedirectStandardError = true, // Redirect the standard error to capture server error logs
            CreateNoWindow = true, // Do not create a console window for the server process
            WindowStyle = ProcessWindowStyle.Hidden // Ensure the process window is hidden
        };

        if (startInfo.EnvironmentVariables.ContainsKey("OLLAMA_MODELS")) // Remove existing OLLAMA_MODELS environment variable if it exists to avoid conflicts
        {
            startInfo.EnvironmentVariables.Remove("OLLAMA_MODELS");
        }

        startInfo.EnvironmentVariables.Add("OLLAMA_MODELS", modelsPath);
        UnityEngine.Debug.Log($"[Launcher] Configurando OLLAMA_MODELS a: {modelsPath}");

        startInfo.EnvironmentVariables["CUDA_VISIBLE_DEVICES"] = "0";
        startInfo.EnvironmentVariables["OLLAMA_NUM_GPU"] = "99";

        try // Try to start the Ollama server process and set up event handlers for output and error logging
        {
            ollamaProcess = new Process { StartInfo = startInfo }; // Create a new process instance with the configured start info

            ollamaProcess.OutputDataReceived += (sender, e) => // Event handler for capturing standard output from the Ollama server
            {
                if (!string.IsNullOrEmpty(e.Data) && showDebugLogs) // Check if the output data is not null or empty and if debug logs are enabled
                {
                    if (!e.Data.Contains("blobs")) // Filter out less relevant logs that contain "blobs" to reduce console clutter, you can remove this filter if you want to see all logs
                        UnityEngine.Debug.Log("[Ollama Server]: " + e.Data);
                }
            };

            ollamaProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    UnityEngine.Debug.LogWarning("[Ollama Log]: " + e.Data);
            }; // Event handler for capturing standard error output from the Ollama server, these are usually warnings or important logs that you might want to see even if debug logs are turned off

            ollamaProcess.Start(); // Start the Ollama server process
            ollamaProcess.BeginOutputReadLine(); // Begin asynchronous reading of the standard output stream to capture server logs without blocking the main thread
            ollamaProcess.BeginErrorReadLine(); // Begin asynchronous reading of the standard error stream to capture server error logs without blocking the main thread

            IsServerRunning = true;

            StartCoroutine(PreloadModel()); // Start a coroutine to preload the specified model into VRAM by sending a request to the Ollama API after the server has started, this helps reduce latency when generating responses for the first time
        }

        catch (System.Exception e) // Catch any exceptions that occur while trying to start the Ollama server and log an error message to the Unity console
        {
            UnityEngine.Debug.LogError("Error al iniciar Ollama: " + e.Message); // Log the error message to the Unity console for debugging purposes
        }
    }

    /// <summary>
    /// Forcefully kills any orphaned or existing 'ollama.exe' processes in the system using console commands (taskkill).
    /// Ensures a clean startup and prevents port conflicts.
    /// </summary>
    public void KillOllamaByForce() // Method to forcefully kill any existing Ollama server processes, this is useful to ensure that there are no conflicts when starting a new server instance
    {
        try // Try to execute the taskkill command to kill any existing ollama.exe processes, this is a forceful way to ensure that no Ollama server instances are left running in the background
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = "/F /IM ollama.exe /T",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi).WaitForExit();
        }

        catch (System.Exception e) { } // Catch any exceptions that occur while trying to kill the Ollama processes, we can ignore these exceptions because if the process is not found it will throw an error, but that just means there are no existing processes running, which is what we want

        IsServerRunning = false;
        ollamaProcess = null;
    }

    /// <summary>
    /// Preloads the specified language model into the GPU VRAM by sending a silent request to the API.
    /// This drastically reduces the latency (response time) of the player's first interaction.
    /// </summary>
    /// <returns>IEnumerator for the asynchronous coroutine wait.</returns>
    IEnumerator PreloadModel() // Coroutine to preload the specified model into VRAM by sending a request to the Ollama API after the server has started, this helps reduce latency when generating responses for the first time
    {
        yield return new WaitForSeconds(2.0f);
        string url = "http://127.0.0.1:11434/api/generate"; // URL of the Ollama API endpoint for generating responses, we will send a request to this endpoint with the specified model to preload it into VRAM

        string jsonBody = $"{{\"model\": \"{modelToLoad}\", \"prompt\": \"hi\", \"stream\": false, \"keep_alive\": -1, \"options\": {{\"num_ctx\": 2048}}}}"; // Construct the JSON body for the POST request to the Ollama API, this includes the model name to preload, a simple prompt, and options to keep the model alive in VRAM with a long context length, you can adjust these parameters as needed for your specific use case

        using (UnityWebRequest request = new UnityWebRequest(url, "POST")) // Create a new UnityWebRequest to send a POST request to the Ollama API endpoint with the specified JSON body to preload the model into VRAM
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody); // Convert the JSON body string into a byte array to be sent in the request
            request.uploadHandler = new UploadHandlerRaw(bodyRaw); // Set the upload handler of the request to send the raw byte array as the request body
            request.downloadHandler = new DownloadHandlerBuffer(); // Set the download handler to capture the response from the Ollama API, we will read this response to check if the model was loaded successfully
            request.SetRequestHeader("Content-Type", "application/json"); // Set the content type header to indicate that we are sending JSON data in the request body

            UnityEngine.Debug.Log($">> CALENTANDO MODELO: {modelToLoad} (Espere unos segundos)...");
            yield return request.SendWebRequest(); // Send the request to the Ollama API and wait for the response, this will preload the specified model into VRAM, reducing latency for future requests that use this model

            if (request.result == UnityWebRequest.Result.Success)
            {
                UnityEngine.Debug.Log(">>IA LISTA Y CARGADA EN VRAM.");
            }

            else
            {
                UnityEngine.Debug.LogError($">>Error cargando modelo. Código: {request.responseCode}. Error: {request.error}");
                UnityEngine.Debug.LogError(">> SUGERENCIA: Revisa que la carpeta 'blobs' y 'manifests' estén dentro de StreamingAssets/Ollama/models");
            }
        }
    }

    void OnApplicationQuit() // Method called when the application is quitting, we use this to ensure that we kill the Ollama server process when the game is closed to prevent it from running in the background after the game has exited
    {
        KillOllamaByForce();
    }

    void OnDestroy() // Method called when this object is destroyed, we also use this to ensure that we kill the Ollama server process if this object is destroyed for any reason while the game is running, this is a safety measure to prevent orphaned processes
    {
        if (IsServerRunning)
        {
            KillOllamaByForce();
        }
    }
}