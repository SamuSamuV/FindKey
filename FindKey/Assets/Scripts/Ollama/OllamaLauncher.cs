using UnityEngine;
using System.Diagnostics;
using System.IO;

public class OllamaLauncher : MonoBehaviour
{
    private Process ollamaProcess;
    public static bool IsServerRunning = false;

    void Awake()
    {
        // Solo lanzamos el servidor si no está ya corriendo
        if (!IsServerRunning)
        {
            StartOllamaServer();
        }
        DontDestroyOnLoad(gameObject);
    }

    void StartOllamaServer()
    {
        string streamingPath = Application.streamingAssetsPath;
        string ollamaPath = Path.Combine(streamingPath, "Ollama", "ollama.exe");
        string modelsPath = Path.Combine(streamingPath, "Ollama", "models");

        // Verificamos que exista el ejecutable
        if (!File.Exists(ollamaPath))
        {
            UnityEngine.Debug.LogError("No se encontró ollama.exe en: " + ollamaPath);
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ollamaPath,
            Arguments = "serve", // Comando para iniciar el servidor
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true, // IMPORTANTE: Para que no salga la ventanita negra
            WindowStyle = ProcessWindowStyle.Hidden
        };

        // --- MAGIA AQUÍ ---
        // Le decimos a Ollama que busque los modelos DENTRO del juego
        startInfo.EnvironmentVariables["OLLAMA_MODELS"] = modelsPath;
        // Le decimos que escuche en el puerto por defecto
        startInfo.EnvironmentVariables["OLLAMA_HOST"] = "127.0.0.1:11434";

        try
        {
            ollamaProcess = new Process();
            ollamaProcess.StartInfo = startInfo;
            ollamaProcess.EnableRaisingEvents = true;

            ollamaProcess.OutputDataReceived += (sender, e) => {
                if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.Log("[Ollama]: " + e.Data);
            };
            ollamaProcess.ErrorDataReceived += (sender, e) => {
                if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.LogWarning("[Ollama Error]: " + e.Data);
            };

            ollamaProcess.Start();
            ollamaProcess.BeginOutputReadLine();
            ollamaProcess.BeginErrorReadLine();

            IsServerRunning = true;
            UnityEngine.Debug.Log("Servidor Ollama Local Iniciado correctamente.");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error al iniciar Ollama: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        // MATAR EL PROCESO AL CERRAR EL JUEGO
        // Esto es vital, si no, ollama.exe se quedará comiendo RAM en el PC del usuario
        if (ollamaProcess != null && !ollamaProcess.HasExited)
        {
            UnityEngine.Debug.Log("Cerrando servidor Ollama...");
            ollamaProcess.Kill();
            ollamaProcess.WaitForExit();
        }
    }
}