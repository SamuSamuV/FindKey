using UnityEngine;
using System.Diagnostics;
using System.IO;
using UnityEngine.Networking;
using System.Collections;

public class OllamaLauncher : MonoBehaviour
{
    [Header("Configuración")]
    public string modelToLoad = "llama3.2:3b";
    public bool showDebugLogs = true;

    // Guardamos referencia, pero usaremos TaskKill para cerrar
    private Process ollamaProcess;
    public static bool IsServerRunning = false;

    void Awake()
    {
        // 1. LIMPIEZA INICIAL: Matar cualquier cosa que huela a Ollama
        KillOllamaByForce();

        // 2. Arrancar nuestro servidor
        StartOllamaServer();
        DontDestroyOnLoad(gameObject);
    }

    void StartOllamaServer()
    {
        string streamingPath = Application.streamingAssetsPath;
        string ollamaPath = Path.Combine(streamingPath, "Ollama", "ollama.exe");
        string modelsPath = Path.Combine(streamingPath, "Ollama", "models");

        if (!File.Exists(ollamaPath))
        {
            UnityEngine.Debug.LogError("FATAL: No se encontró ollama.exe en: " + ollamaPath);
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ollamaPath,
            Arguments = "serve",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        // Variables de entorno para modo portable
        startInfo.EnvironmentVariables["OLLAMA_MODELS"] = modelsPath;
        startInfo.EnvironmentVariables["OLLAMA_HOST"] = "127.0.0.1:11434";
        // IMPORTANTE: Evita que Ollama intente leer config de tu usuario de Windows
        startInfo.EnvironmentVariables["OLLAMA_ORIGINS"] = "*"; 

        try
        {
            ollamaProcess = new Process();
            ollamaProcess.StartInfo = startInfo;
            ollamaProcess.EnableRaisingEvents = true;

            if (showDebugLogs)
            {
                ollamaProcess.OutputDataReceived += (sender, e) => {
                    if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.Log("[Ollama]: " + e.Data);
                };
            }

            ollamaProcess.Start();
            ollamaProcess.BeginOutputReadLine();
            ollamaProcess.BeginErrorReadLine();

            IsServerRunning = true;
            UnityEngine.Debug.Log($"Servidor Ollama Iniciado (PID: {ollamaProcess.Id}). Pre-cargando modelo...");

            StartCoroutine(PreloadModel());
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error al iniciar Ollama: " + e.Message);
        }
    }

    // --- LA SOLUCIÓN NUCLEAR ---
    public void KillOllamaByForce()
    {
        UnityEngine.Debug.Log(">> EJECUTANDO TASKKILL PARA OLLAMA...");

        try
        {
            // Ejecutamos un comando de CMD para forzar el cierre
            // /F = Fuerza bruta
            // /IM ollama.exe = Busca por nombre de imagen
            // /T = Mata también a los hijos (Tree)
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
            UnityEngine.Debug.LogWarning("No se pudo ejecutar TaskKill (quizás ya estaba cerrado): " + e.Message);
        }

        IsServerRunning = false;
        ollamaProcess = null;
    }

    IEnumerator PreloadModel()
    {
        yield return new WaitForSeconds(1.0f); // Espera breve a que el servidor arranque
        string url = "http://127.0.0.1:11434/api/generate";

        // Enviamos "hi" y forzamos contexto pequeño también aquí para reservar la VRAM justa
        string jsonBody = $"{{\"model\": \"{modelToLoad}\", \"prompt\": \"hi\", \"keep_alive\": -1, \"options\": {{\"num_ctx\": 2048}}}}";

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        UnityEngine.Debug.Log(">> CALENTANDO MOTORES DE IA (Esto puede tardar unos segundos)...");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            UnityEngine.Debug.Log(">> IA LISTA Y CARGADA EN VRAM.");
        else
            UnityEngine.Debug.LogWarning(">> Pre-carga incompleta: " + request.error);
    }

    // --- TRIGGERS DE CIERRE ---
    void OnApplicationQuit()
    {
        KillOllamaByForce();
    }

    void OnDestroy()
    {
        // Solo matamos si somos el objeto original para evitar cierres accidentales al cambiar de escena
        if (IsServerRunning) 
        {
            KillOllamaByForce();
        }
    }
}