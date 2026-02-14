using UnityEngine;
using System.Diagnostics;
using System.IO;
using UnityEngine.Networking;
using System.Collections;

public class OllamaLauncher : MonoBehaviour
{
    [Header("Configuración")]
    // IMPORTANTE: Asegúrate de que este nombre coincida con la carpeta dentro de 'manifests'
    public string modelToLoad = "llama3.2:3b";
    public bool showDebugLogs = true;

    private Process ollamaProcess;
    public static bool IsServerRunning = false;

    void Awake()
    {
        // 1. LIMPIEZA INICIAL: Matar cualquier proceso previo
        KillOllamaByForce();

        // 2. Arrancar servidor
        StartOllamaServer();
        DontDestroyOnLoad(gameObject);
    }

    void StartOllamaServer()
    {
        string streamingPath = Application.streamingAssetsPath;

        // Rutas absolutas para el ejecutable y la carpeta de modelos
        string ollamaPath = Path.Combine(streamingPath, "Ollama", "ollama.exe");
        string modelsPath = Path.Combine(streamingPath, "Ollama", "models");

        // --- VERIFICACIONES DE SEGURIDAD ---
        if (!File.Exists(ollamaPath))
        {
            UnityEngine.Debug.LogError("FATAL: No se encontró ollama.exe en: " + ollamaPath);
            return;
        }

        // Crear carpeta de modelos si no existe (evita errores de variable de entorno)
        if (!Directory.Exists(modelsPath))
        {
            UnityEngine.Debug.LogWarning("[Launcher] Carpeta 'models' no encontrada. Creándola en: " + modelsPath);
            Directory.CreateDirectory(modelsPath);
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ollamaPath,
            Arguments = "serve",
            UseShellExecute = false, // OBLIGATORIO false para modificar variables de entorno
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        // --- CONFIGURACIÓN PORTABLE (LA SOLUCIÓN AL ERROR 404) ---

        // 1. Limpiamos la variable por si Windows tiene una global
        if (startInfo.EnvironmentVariables.ContainsKey("OLLAMA_MODELS"))
        {
            startInfo.EnvironmentVariables.Remove("OLLAMA_MODELS");
        }

        // 2. Inyectamos nuestra ruta local
        startInfo.EnvironmentVariables.Add("OLLAMA_MODELS", modelsPath);
        UnityEngine.Debug.Log($"[Launcher] Configurando OLLAMA_MODELS a: {modelsPath}");

        // --- OPTIMIZACIÓN GPU (Tu configuración original) ---
        startInfo.EnvironmentVariables["CUDA_VISIBLE_DEVICES"] = "0";
        startInfo.EnvironmentVariables["OLLAMA_NUM_GPU"] = "99";

        // Opcional: Forzar host local explícitamente
        // startInfo.EnvironmentVariables["OLLAMA_HOST"] = "127.0.0.1:11434";

        try
        {
            ollamaProcess = new Process { StartInfo = startInfo };

            // Captura de logs
            ollamaProcess.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data) && showDebugLogs)
                {
                    // Filtramos logs irrelevantes para no ensuciar la consola
                    if (!e.Data.Contains("blobs"))
                        UnityEngine.Debug.Log("[Ollama Server]: " + e.Data);
                }
            };

            // Captura de errores / avisos del servidor
            ollamaProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    UnityEngine.Debug.LogWarning("[Ollama Log]: " + e.Data);
            };

            ollamaProcess.Start();
            ollamaProcess.BeginOutputReadLine();
            ollamaProcess.BeginErrorReadLine();

            IsServerRunning = true;

            StartCoroutine(PreloadModel());
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error al iniciar Ollama: " + e.Message);
        }
    }

    // --- LA SOLUCIÓN NUCLEAR (TaskKill) ---
    public void KillOllamaByForce()
    {
        // Solo logueamos si realmente vamos a matar algo, para no spamear al iniciar
        // UnityEngine.Debug.Log(">> EJECUTANDO TASKKILL PARA OLLAMA...");

        try
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
        catch (System.Exception e)
        {
            // Ignoramos errores aquí, usualmente es porque no había proceso que matar
        }

        IsServerRunning = false;
        ollamaProcess = null;
    }

    IEnumerator PreloadModel()
    {
        yield return new WaitForSeconds(2.0f); // Damos un segundo extra para que lea los archivos
        string url = "http://127.0.0.1:11434/api/generate";

        // NOTA: 'stream': false es importante para pre-cargas simples
        string jsonBody = $"{{\"model\": \"{modelToLoad}\", \"prompt\": \"hi\", \"stream\": false, \"keep_alive\": -1, \"options\": {{\"num_ctx\": 2048}}}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            UnityEngine.Debug.Log($">> CALENTANDO MODELO: {modelToLoad} (Espere unos segundos)...");
            yield return request.SendWebRequest();

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

    void OnApplicationQuit()
    {
        KillOllamaByForce();
    }

    void OnDestroy()
    {
        if (IsServerRunning)
        {
            KillOllamaByForce();
        }
    }
}