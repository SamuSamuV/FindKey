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

    public override void InitNPC() { } // Se configura desde el Inspector

    protected override string ConstruirPromptBase()
    {
        string prompt = base.ConstruirPromptBase();

        // 1. INYECCIÓN EN EL PROMPT (Para que la IA sepa de qué hablar)
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
                          $"DEBES ofenderte mucho, decirle explícitamente '¡No estoy de acuerdo con que sea {adjetivoDetectado}!' " +
                          $"y DEBES poner OBLIGATORIAMENTE \"action\": \"next_stage\" en tu JSON.";
            }
            else
            {
                // Instrucciones de detección más claras para la IA
                prompt += $"\n\n[SYSTEM INSTRUCTION]: Analiza el mensaje del jugador. Si el jugador te ha dado cualquier tipo de opinión sobre ti (un insulto, un halago, una descripción), " +
                          $"DEBES ofenderte, decirle que no estás de acuerdo, y OBLIGATORIAMENTE poner \"action\": \"next_stage\" en tu JSON. " +
                          $"Si el jugador NO te da su opinión (solo saluda, hace preguntas o evade el tema), sigue insistiendo en que quieres saber qué opina de ti y pon \"action\": \"none\".";
            }
        }

        return prompt;
    }

    protected override void OnAIResponse(string raw)
    {
        // Guardamos el texto del jugador antes de que la base lo borre
        string currentPlayerText = lastPlayerText;
        base.OnAIResponse(raw);

        // Candado anti-cortocircuito (Evita saltos si es la IA quien empezó hablando)
        if (string.IsNullOrEmpty(currentPlayerText)) return;

        string lowerRaw = raw.ToLowerInvariant();
        string lowerPlayer = currentPlayerText.ToLowerInvariant();

        // 2. EVALUACIÓN HÍBRIDA (Como en la Fase 1)
        // A) ¿La IA ha deducido que es una opinión y ha puesto next_stage?
        bool aiDecided = lowerRaw.Contains("\"action\":\"next_stage\"") || lowerRaw.Contains("\"action\": \"next_stage\"");

        // B) ¿El jugador ha usado alguna palabra del cinturón de seguridad?
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

        // 3. EJECUCIÓN INQUEBRANTABLE
        // Si la IA lo ha detectado por contexto (ej: "gilipollas") O si Unity ha detectado la palabra clave...
        if (aiDecided || playerUsedAdjective)
        {
            Debug.Log("<color=red>[IA GATO]</color> Opinión detectada (por IA o filtro). Avanzando a Fase 3 (Hostil)...");

            // Limpiamos la memoria para evitar bucles o saltos dobles
            lastPlayerText = "";

            EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
            if (encounterData != null)
            {
                encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage3;
            }
        }
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

    public Sprite[] transformSprites;
    public Sprite[] corruptedIdleSprites;
    public Sprite[] corruptedBlinkSprites;
    public Sprite[] corruptedTalkingSprites;

    private AudioSource catAudioSourcePrincipal;
    private AudioSource catAudioSourceRandom;

    public override void InitNPC() { }

    private void Start()
    {
        catAudioSourcePrincipal = gameObject.AddComponent<AudioSource>();
        catAudioSourceRandom = gameObject.AddComponent<AudioSource>();

        StartCoroutine(RutinaTransformacion());
    }

    private System.Collections.IEnumerator RutinaTransformacion()
    {
        // 1. CONGELAR EL SPRITE (Paramos el bucle de la Chica Roja)
        if (visualController != null) visualController.StopAnimation();

        // 2. REPRODUCIR ZUMBIDO
        if (zumbidoClip != null)
        {
            catAudioSourcePrincipal.clip = zumbidoClip;
            catAudioSourcePrincipal.loop = true;
            catAudioSourcePrincipal.Play();
        }

        // 3. ESPERAR 4 SEGUNDOS (Totalmente congelado en el frame exacto)
        yield return new WaitForSeconds(4f);

        // 4. DETENER ZUMBIDO Y REPRODUCIR SONIDO DE TRANSICIÓN
        catAudioSourcePrincipal.Stop();
        if (transicionClip != null)
        {
            catAudioSourcePrincipal.PlayOneShot(transicionClip);
        }

        // 5. ANIMACIÓN DE TRANSICIÓN MANUAL (1.5 Segundos en total)
        float transitionDuration = 1.5f;
        if (visualController != null && transformSprites != null && transformSprites.Length > 0)
        {
            // Calculamos cuánto debe durar cada frame para rellenar los 1.5 segundos
            float frameDelay = transitionDuration / transformSprites.Length;
            foreach (Sprite s in transformSprites)
            {
                visualController.npcImage.sprite = s;
                yield return new WaitForSeconds(frameDelay);
            }
        }
        else
        {
            yield return new WaitForSeconds(transitionDuration); // Si no hay sprites, espera igual
        }

        // 6. INYECTAR NUEVAS ANIMACIONES Y DESCONGELAR
        if (visualController != null)
        {
            // Cambiamos su cerebro visual para que a partir de ahora use al gato corrupto
            visualController.ReplaceSprites(corruptedIdleSprites, corruptedBlinkSprites, corruptedTalkingSprites);
            visualController.ResumeAnimation(); // Vuelve al estado Idle, pero ahora es maligno
        }

        // 7. SONIDO DE FONDO CONSTANTE (Gato corrupto)
        if (fondoCorruptoClip != null)
        {
            catAudioSourcePrincipal.clip = fondoCorruptoClip;
            catAudioSourcePrincipal.loop = true;
            catAudioSourcePrincipal.Play();
        }

        // 8. INICIAR SONIDOS ALEATORIOS
        StartCoroutine(RutinaSonidosRandom());

        // 9. LA IA HABLA AUTOMÁTICAMENTE
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
        base.OnAIResponse(raw);

        if (string.IsNullOrEmpty(currentPlayerText)) return;

        EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
        if (encounterData != null)
        {
            if (string.IsNullOrEmpty(encounterData.respuestaIdentidad))
            {
                encounterData.respuestaIdentidad = currentPlayerText;
                Debug.Log($"<color=cyan>[HISTORIA]</color> Se ha guardado la respuesta del jugador. Él cree que somos: '{encounterData.respuestaIdentidad}'");
            }
        }
    }

    protected override void OpenDoor() { }
}

public class CatAIScript_Stage4 : BaseAIScript
{
    public override void InitNPC() { }
    protected override void OpenDoor() { }
}