using UnityEngine;

public class CatAIScript_Stage1 : BaseAIScript
{
    public override void InitNPC() { } // Se configura desde el Inspector

    protected override void OnAIResponse(string raw)
    {
        // 1. Guardamos lo que dijo el jugador ANTES de que el script base lo borre por seguridad
        string currentPlayerText = lastPlayerText;

        // 2. Dejamos que el script base haga su trabajo normal (escribir, animar, etc.)
        base.OnAIResponse(raw);

        // 3. CANDADO ANTI-CORTOCIRCUITO:
        // Si el jugador no ha escrito nada (es decir, la IA está enviando su primer mensaje de saludo),
        // es IMPOSIBLE avanzar de fase. Ignoramos cualquier locura que haya puesto la IA en el JSON.
        if (string.IsNullOrEmpty(currentPlayerText))
        {
            return;
        }

        // 4. Evaluación Híbrida (Solo se ejecuta si el jugador realmente ha hablado)
        string lowerRaw = raw.ToLowerInvariant();
        string lowerPlayer = currentPlayerText.ToLowerInvariant();

        // ¿La IA puso la acción en el JSON?
        bool aiDecided = lowerRaw.Contains("\"action\":\"next_stage\"") || lowerRaw.Contains("\"action\": \"next_stage\"");

        // ¿O el jugador dijo una palabra clave? (Red de seguridad)
        bool playerGreeted = lowerPlayer.Contains("hola") || lowerPlayer.Contains("buenas") ||
                             lowerPlayer.Contains("saludos") || lowerPlayer.Contains("hey") ||
                             lowerPlayer.Contains("miau");

        if (aiDecided || playerGreeted)
        {
            Debug.Log("<color=magenta>[IA GATO]</color> Saludo detectado tras hablar el jugador. Avanzando a Fase 2 en secreto...");

            EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
            if (encounterData != null)
            {
                encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage2;
            }
        }
    }

    protected override void OpenDoor() { }
}

public class CatAIScript_Stage2 : BaseAIScript
{
    public string[] adjetivosDeSeguridad;

    public override void InitNPC() { }

    protected override string ConstruirPromptBase()
    {
        string prompt = base.ConstruirPromptBase();

        if (!string.IsNullOrEmpty(lastPlayerText))
        {
            string lowerPlayer = lastPlayerText.ToLowerInvariant();
            bool detected = false;
            string adjetivoDetectado = "";

            if (adjetivosDeSeguridad != null)
            {
                foreach (string adj in adjetivosDeSeguridad)
                {
                    if (lowerPlayer.Contains(adj))
                    {
                        detected = true;
                        adjetivoDetectado = adj;
                        break;
                    }
                }
            }

            if (detected)
            {
                prompt += $"\n\n[SYSTEM OVERRIDE URGENTE]: El jugador te ha llamado '{adjetivoDetectado}'. " +
                          $"DEBES poner OBLIGATORIAMENTE \"action\": \"next_stage\" en tu JSON.";
            }
            else
            {
                prompt += $"\n\n[SYSTEM INSTRUCTION]: Analiza el mensaje del jugador. Si el jugador te ha dado cualquier tipo de opinión sobre ti (un insulto, un halago, una descripción), " +
                          $"OBLIGATORIAMENTE pon \"action\": \"next_stage\" en tu JSON. " +
                          $"Si el jugador NO te da su opinión (solo saluda o evade el tema), sigue insistiendo en que quieres saber qué opina de ti y pon \"action\": \"none\".";
            }
        }

        return prompt;
    }

    protected override void OnAIResponse(string raw)
    {
        string currentPlayerText = lastPlayerText;

        if (string.IsNullOrEmpty(currentPlayerText))
        {
            base.OnAIResponse(raw);
            return;
        }

        string lowerRaw = raw.ToLowerInvariant();
        string lowerPlayer = currentPlayerText.ToLowerInvariant();

        bool aiDecided = lowerRaw.Contains("\"action\":\"next_stage\"") || lowerRaw.Contains("\"action\": \"next_stage\"");

        bool playerUsedAdjective = false;
        if (adjetivosDeSeguridad != null)
        {
            foreach (string adj in adjetivosDeSeguridad)
            {
                if (lowerPlayer.Contains(adj))
                {
                    playerUsedAdjective = true;
                    break;
                }
            }
        }

        if (aiDecided || playerUsedAdjective)
        {
            Debug.Log("<color=red>[IA GATO]</color> Opinión detectada. Silenciando Fase 2 y saltando a Fase 3...");

            isThinking = false;
            if (thinkingPanel != null) thinkingPanel.SetActive(false);
            if (inputField != null) inputField.gameObject.SetActive(true);

            lastPlayerText = "";

            EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
            if (encounterData != null)
            {
                encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage3;
            }

            return;
        }

        base.OnAIResponse(raw);
    }

    protected override void OpenDoor() { }
}

public class CatAIScript_Stage3 : BaseAIScript
{
    // Audios
    public AudioClip zumbidoClip;
    public AudioClip transicionClip;
    public AudioClip fondoCorruptoClip;
    public AudioClip[] sonidosRandomCorruptos;

    // Sprites
    public Sprite[] transformSprites;
    public Sprite[] corruptedIdleSprites;
    public Sprite[] corruptedBlinkSprites;
    public Sprite[] corruptedTalkingSprites;

    private AudioSource catAudioSourcePrincipal;
    private AudioSource catAudioSourceRandom;

    public override void InitNPC() { }

    // ¡CAMBIO CLAVE! Renombramos Start() a IniciarEfectos() para no aplastar a la base
    public void IniciarEfectos()
    {
        // 1. Ocultamos el chat durante la animación para que el jugador no pueda interrumpir
        if (inputField != null) inputField.gameObject.SetActive(false);

        catAudioSourcePrincipal = gameObject.AddComponent<AudioSource>();
        catAudioSourceRandom = gameObject.AddComponent<AudioSource>();

        StartCoroutine(RutinaTransformacion());
    }

    private System.Collections.IEnumerator RutinaTransformacion()
    {
        if (visualController != null) visualController.StopAnimation();

        if (zumbidoClip != null)
        {
            catAudioSourcePrincipal.clip = zumbidoClip;
            catAudioSourcePrincipal.loop = true;
            catAudioSourcePrincipal.Play();
        }

        yield return new WaitForSeconds(4f);

        catAudioSourcePrincipal.Stop();
        if (transicionClip != null)
        {
            catAudioSourcePrincipal.PlayOneShot(transicionClip);
        }

        float transitionDuration = 1.5f;
        if (visualController != null && transformSprites != null && transformSprites.Length > 0)
        {
            float frameDelay = transitionDuration / transformSprites.Length;
            foreach (Sprite s in transformSprites)
            {
                visualController.npcImage.sprite = s;
                yield return new WaitForSeconds(frameDelay);
            }
        }
        else
        {
            yield return new WaitForSeconds(transitionDuration);
        }

        if (visualController != null)
        {
            visualController.ReplaceSprites(corruptedIdleSprites, corruptedBlinkSprites, corruptedTalkingSprites);
            visualController.ResumeAnimation();
        }

        if (fondoCorruptoClip != null)
        {
            catAudioSourcePrincipal.clip = fondoCorruptoClip;
            catAudioSourcePrincipal.loop = true;
            catAudioSourcePrincipal.Play();
        }

        StartCoroutine(RutinaSonidosRandom());

        // Al lanzar este mensaje proactivo, el sistema volverá a encender la caja de texto automáticamente al terminar
        ForceProactiveMessage("Acabas de sufrir una transformación gráfica muy dolorosa revelando tu forma corrupta. " +
                              "Dirígete al jugador, dile que NO te ha gustado NADA lo que acaba de decir sobre ti, " +
                              "y pregúntale de forma amenazante e inquietante: '¿Acaso sabes quién soy yo?'. " +
                              "DEBES mantener OBLIGATORIAMENTE el formato JSON.");
    }

    private System.Collections.IEnumerator RutinaSonidosRandom()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 8f));

            if (sonidosRandomCorruptos != null && sonidosRandomCorruptos.Length > 0)
            {
                AudioClip clip = sonidosRandomCorruptos[Random.Range(0, sonidosRandomCorruptos.Length)];
                catAudioSourceRandom.PlayOneShot(clip);
            }
        }
    }

    protected override string ConstruirPromptBase()
    {
        string prompt = base.ConstruirPromptBase();

        prompt += "\n\n[SYSTEM INSTRUCTION]: Eres una entidad oscura. Tu objetivo principal ahora es escuchar la respuesta del jugador " +
                  "a tu pregunta '¿Acaso sabes quién soy yo?' y reaccionar de forma críptica. Pon \"action\": \"none\" en tu JSON por ahora.";

        return prompt;
    }

    protected override void OnAIResponse(string raw)
    {
        string currentPlayerText = lastPlayerText;

        // Si el jugador no ha escrito nada (es la IA lanzando su mensaje inicial proactivo)
        if (string.IsNullOrEmpty(currentPlayerText))
        {
            base.OnAIResponse(raw);
            return;
        }

        // Si el jugador responde a la pregunta de identidad...
        EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
        if (encounterData != null)
        {
            if (string.IsNullOrEmpty(encounterData.respuestaIdentidad))
            {
                // 1. Guardamos lo que cree que somos
                encounterData.respuestaIdentidad = currentPlayerText;
                Debug.Log($"<color=cyan>[HISTORIA]</color> Se ha guardado la respuesta del jugador. Él cree que somos: '{encounterData.respuestaIdentidad}'");

                // 2. Silenciamos el procesamiento normal de la Fase 3
                isThinking = false;
                if (thinkingPanel != null) thinkingPanel.SetActive(false);
                if (inputField != null) inputField.gameObject.SetActive(true);

                lastPlayerText = ""; // Limpiamos para que la Fase 4 lo coja fresco

                // 3. ¡Saltamos a la Fase 4!
                Debug.Log("<color=red>[IA GATO]</color> Identidad revelada. Pasando el control a la Fase 4...");
                encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage4;

                return; // Evitamos ejecutar base.OnAIResponse(raw)
            }
        }

        // Por si acaso cayera aquí
        base.OnAIResponse(raw);
    }

    protected override void OpenDoor() { }
}

public class CatAIScript_Stage4 : BaseAIScript
{
    public string[] passwordsUbicacion;
    public string respuestaIdentidadJugador;

    // Aquí recibimos tu ScriptableObject del GameEvent
    public GameEvent eventoFinalDemo;

    public override void InitNPC() { }

    void Start()
    {
        base.Start();

        if (visualController != null)
        {
            visualController.animationSpeed = 0.03f;
        }

        string identidad = string.IsNullOrEmpty(respuestaIdentidadJugador) ? "un monstruo" : respuestaIdentidadJugador;

        ForceProactiveMessage($"En el mensaje anterior, el jugador te dijo que creía que tú eras: '{identidad}'. " +
                              $"Dirígete al jugador y dile: '¿Así que crees que soy {identidad}...? Qué ingenuidad tan deliciosa.'. " +
                              $"Luego, cambia a un tono extremadamente hostil y urgente y pregúntale: 'La verdadera pregunta es... ¿Sabes acaso DÓNDE estamos?'. " +
                              $"DEBES mantener OBLIGATORIAMENTE el formato JSON.");
    }

    protected override string ConstruirPromptBase()
    {
        string prompt = base.ConstruirPromptBase();

        prompt += "\n\n[SYSTEM INSTRUCTION]: El jugador debe adivinar el nombre del lugar donde estáis. " +
                  "Si falla o dice no saberlo, búrlate de su ignorancia y dile que jamás escapará. Pon \"action\": \"none\" en tu JSON.";

        return prompt;
    }

    protected override void OnAIResponse(string raw)
    {
        string currentPlayerText = lastPlayerText;
        base.OnAIResponse(raw);

        if (string.IsNullOrEmpty(currentPlayerText)) return;

        string lowerPlayer = currentPlayerText.ToLowerInvariant();
        bool passwordCorrecta = false;

        if (passwordsUbicacion != null)
        {
            foreach (string pwd in passwordsUbicacion)
            {
                if (lowerPlayer.Contains(pwd.ToLowerInvariant()))
                {
                    passwordCorrecta = true;
                    break;
                }
            }
        }

        if (passwordCorrecta)
        {
            Debug.Log("<color=red>[FINAL]</color> Ubicación adivinada. Lanzando evento al EventManager...");

            if (inputField != null) inputField.gameObject.SetActive(false);
            isThinking = false;
            if (thinkingPanel != null) thinkingPanel.SetActive(false);

            if (eventoFinalDemo != null)
            {
                EventManager.Instance.TriggerEvent(eventoFinalDemo);
            }
            else
            {
                Debug.LogWarning("[AVISO] El jugador acertó, pero no has arrastrado ningún GameEvent al Inspector del EnemyEncounterData.");
            }
        }
    }

    protected override void OpenDoor() { }
}