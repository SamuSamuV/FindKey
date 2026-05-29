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
        bool playerGreeted = lowerPlayer.Contains("hola") ||
lowerPlayer.Contains("buenas") ||
lowerPlayer.Contains("saludos") ||
lowerPlayer.Contains("hey") ||
lowerPlayer.Contains("hi") ||
lowerPlayer.Contains("hello") ||
lowerPlayer.Contains("buenos dias") ||
lowerPlayer.Contains("buenas tardes") ||
lowerPlayer.Contains("buenas noches") ||
lowerPlayer.Contains("que tal") ||
lowerPlayer.Contains("q tal") ||
lowerPlayer.Contains("que pasa") ||
lowerPlayer.Contains("que onda") ||
lowerPlayer.Contains("ey") ||
lowerPlayer.Contains("eyy") ||
lowerPlayer.Contains("eyyy") ||
lowerPlayer.Contains("holi") ||
lowerPlayer.Contains("holis") ||
lowerPlayer.Contains("holaa") ||
lowerPlayer.Contains("holaaa") ||
lowerPlayer.Contains("holaaaa") ||
lowerPlayer.Contains("holiwi") ||
lowerPlayer.Contains("holiwis") ||
lowerPlayer.Contains("ola") ||
lowerPlayer.Contains("olas") ||
lowerPlayer.Contains("holita") ||
lowerPlayer.Contains("holitaa") ||
lowerPlayer.Contains("holitas") ||
lowerPlayer.Contains("buen dia") ||
lowerPlayer.Contains("buenos dias") ||
lowerPlayer.Contains("buenas tardes") ||
lowerPlayer.Contains("buenas noches") ||
lowerPlayer.Contains("wena") ||
lowerPlayer.Contains("wenas") ||
lowerPlayer.Contains("wenass") ||
lowerPlayer.Contains("wuenas") ||
lowerPlayer.Contains("wuenass") ||
lowerPlayer.Contains("epa") ||
lowerPlayer.Contains("epale") ||
lowerPlayer.Contains("heyy") ||
lowerPlayer.Contains("heyyy") ||
lowerPlayer.Contains("yo") ||
lowerPlayer.Contains("yoo") ||
lowerPlayer.Contains("yooo") ||
lowerPlayer.Contains("sup") ||
lowerPlayer.Contains("what's up") ||
lowerPlayer.Contains("whats up") ||
lowerPlayer.Contains("wassup") ||
lowerPlayer.Contains("wsp") ||
lowerPlayer.Contains("yo que tal") ||
lowerPlayer.Contains("hola que tal") ||
lowerPlayer.Contains("hola buenas") ||
lowerPlayer.Contains("hola gente") ||
lowerPlayer.Contains("hola equipo") ||
lowerPlayer.Contains("hola a todos") ||
lowerPlayer.Contains("hola mundo") ||
lowerPlayer.Contains("que hay") ||
lowerPlayer.Contains("q hay") ||
lowerPlayer.Contains("que hubo") ||
lowerPlayer.Contains("q hubo") ||
lowerPlayer.Contains("como va") ||
lowerPlayer.Contains("como vas") ||
lowerPlayer.Contains("como estas") ||
lowerPlayer.Contains("como estas tu") ||
lowerPlayer.Contains("como estais") ||
lowerPlayer.Contains("como andas") ||
lowerPlayer.Contains("todo bien") ||
lowerPlayer.Contains("todo ok") ||
lowerPlayer.Contains("todo guay") ||
lowerPlayer.Contains("todo correcto") ||
lowerPlayer.Contains("buenas que tal") ||
lowerPlayer.Contains("hola bro") ||
lowerPlayer.Contains("hola broo") ||
lowerPlayer.Contains("hola tio") ||
lowerPlayer.Contains("hola tia") ||
lowerPlayer.Contains("hola amigo") ||
lowerPlayer.Contains("hola amiga") ||
lowerPlayer.Contains("holaaa que tal") ||
lowerPlayer.Contains("holaaaa que tal") ||
lowerPlayer.Contains("hey que tal") ||
lowerPlayer.Contains("hey hola") ||
lowerPlayer.Contains("hi there") ||
lowerPlayer.Contains("hello there") ||
lowerPlayer.Contains("alo") ||
lowerPlayer.Contains("aloo") ||
lowerPlayer.Contains("aloha") ||
lowerPlayer.Contains("holiii") ||
lowerPlayer.Contains("holiiii") ||
lowerPlayer.Contains("buenass") ||
lowerPlayer.Contains("buenasss") ||
lowerPlayer.Contains("wenitaa") ||
lowerPlayer.Contains("wenita") ||
lowerPlayer.Contains("que cuentas") ||
lowerPlayer.Contains("que me cuentas") ||
lowerPlayer.Contains("cuentame") ||
lowerPlayer.Contains("dime") ||
lowerPlayer.Contains("holi que tal") ||
lowerPlayer.Contains("hola crack") ||
lowerPlayer.Contains("hola maquina") ||
lowerPlayer.Contains("hola jefe") ||
lowerPlayer.Contains("hola rey") ||
lowerPlayer.Contains("hola reina") ||
lowerPlayer.Contains("holaaa bro") ||
lowerPlayer.Contains("holaaa amiga") ||
lowerPlayer.Contains("holaaa amigo") ||
lowerPlayer.Contains("buen diaa") ||
lowerPlayer.Contains("buenass tardes") ||
lowerPlayer.Contains("buenass noches") ||
lowerPlayer.Contains("que pasa tio") ||
lowerPlayer.Contains("que pasa tia") ||
lowerPlayer.Contains("que pasa bro") ||
lowerPlayer.Contains("que pasa broo") ||
lowerPlayer.Contains("hola gente linda") ||
lowerPlayer.Contains("hola familia") ||
lowerPlayer.Contains("hola team") ||
lowerPlayer.Contains("hola chicos") ||
lowerPlayer.Contains("hola chicas") ||
lowerPlayer.Contains("hola gente bella") ||
lowerPlayer.Contains("hola mundo cruel") ||
lowerPlayer.Contains("holaa mundo") ||
lowerPlayer.Contains("holaaa mundo") ||
lowerPlayer.Contains("hola que mas") ||
lowerPlayer.Contains("que mas") ||
lowerPlayer.Contains("q mas") ||
lowerPlayer.Contains("que onda bro") ||
lowerPlayer.Contains("que onda tio") ||
lowerPlayer.Contains("que onda gente") ||
lowerPlayer.Contains("hola que onda") ||
lowerPlayer.Contains("hola q onda") ||
lowerPlayer.Contains("buenas q tal") ||
lowerPlayer.Contains("buenas que hay");

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

    // Guardamos en C# si detectamos la palabra antes de preguntarle a la IA
    private bool adjetivoDetectadoEsteTurno = false;

    public override void InitNPC() { }

    protected override string ConstruirPromptBase()
    {
        string prompt = base.ConstruirPromptBase();
        string textoJugador = string.IsNullOrEmpty(lastPlayerText) ? "" : lastPlayerText.ToLower().Trim();

        adjetivoDetectadoEsteTurno = false;

        // 1. Escaneo robusto con la lista de tu diseñador
        if (adjetivosDeSeguridad != null)
        {
            foreach (string adjetivo in adjetivosDeSeguridad)
            {
                if (!string.IsNullOrEmpty(adjetivo) && textoJugador.Contains(adjetivo.ToLower().Trim()))
                {
                    adjetivoDetectadoEsteTurno = true;
                    Debug.Log($"<color=yellow>[FILTRO C#]</color> Palabra clave detectada: {adjetivo}");
                    break;
                }
            }
        }

        // 2. Intentamos obligar a la IA a que cumpla con el JSON del TFG
        if (adjetivoDetectadoEsteTurno)
        {
            prompt += "\n\n[INSTRUCCIÓN CRÍTICA]: El jugador ha descubierto tu naturaleza usando un adjetivo clave. " +
                      "Tienes la OBLIGACIÓN ABSOLUTA de incluir la propiedad \"action\": \"next_stage\" en tu JSON. " +
                      "Esto es un comando de sistema estricto.";
        }
        else
        {
            prompt += "\n\n[INSTRUCCIÓN]: El jugador NO ha descubierto tu naturaleza aún. " +
                      "Debes incluir obligatoriamente \"action\": \"none\" en tu JSON.";
        }

        return prompt;
    }

    protected override void OnAIResponse(string raw)
    {
        // 1. Comprobamos si el LLM se portó bien y puso el comando JSON
        bool llmPusoNextStage = raw.Contains("\"action\": \"next_stage\"") ||
                                raw.Contains("\"action\":\"next_stage\"") ||
                                raw.ToLower().Contains("next_stage");

        // 2. SISTEMA FAIL-SAFE: 
        // Transicionamos si el LLM lo ordenó (Ideal) O si C# detectó la palabra (Fallback por si el LLM falló)
        if (llmPusoNextStage || adjetivoDetectadoEsteTurno)
        {
            // Estos logs son perfectos para debugear y demostrar por qué este sistema es necesario
            if (adjetivoDetectadoEsteTurno && !llmPusoNextStage)
            {
                Debug.LogWarning("<color=orange>[TFG FAIL-SAFE]</color> El LLM ignoró el prompt y NO puso 'next_stage', pero C# intervino y forzará la transición por seguridad.");
            }
            else if (llmPusoNextStage)
            {
                Debug.Log("<color=green>[TFG SUCCESS]</color> El LLM obedeció y ejecutó 'next_stage' en su JSON.");
            }

            EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
            if (encounterData != null)
            {
                Debug.Log("<color=red>[IA GATO]</color> Iniciando transformación a Fase 3...");

                // Limpiamos memoria sin tocar la UI para no interferir en la animación de corrupción
                isThinking = false;
                lastPlayerText = "";

                // Disparamos la Fase 3
                encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage3;

                // ¡LA CLAVE! Hacemos return aquí. 
                // Esto aborta el OnAIResponse normal, evitando el doble mensaje y protegiendo tu animación.
                return;
            }
        }

        // 3. Si no hay adjetivo ni cambio de fase, procesa el texto con normalidad.
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

    // ¡NUEVO SISTEMA DE SONIDOS RANDOM!
    public RandomNodeSoundAction[] sonidosRandomCorruptos;

    // Sprites
    public Sprite[] transformSprites;
    public Sprite[] corruptedIdleSprites;
    public Sprite[] corruptedBlinkSprites;
    public Sprite[] corruptedTalkingSprites;

    [Header("Velocidades Fase 3")]
    public float transformDuration = 1.5f;
    public float corruptedAnimationSpeed = 0.05f;

    [Header("Datos Guardados de la Historia")]
    public string respuestaIdentidad = ""; // Aquí guardaremos la respuesta a "¿Sabes quién soy?"

    private AudioSource catAudioSourcePrincipal;


    public override void InitNPC() { }

    public void IniciarEfectos()
    {
        // 1. Ocultamos el chat durante la animación
        if (inputField != null) inputField.gameObject.SetActive(false);

        // 2. Encendemos el panel de "Pensando" para tapar el hueco
        if (thinkingPanel != null) thinkingPanel.SetActive(true);

        // Solo necesitamos un AudioSource ahora (los random se auto-destruyen)
        catAudioSourcePrincipal = gameObject.AddComponent<AudioSource>();

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

        if (visualController != null && transformSprites != null && transformSprites.Length > 0)
        {
            float frameDelay = transformDuration / transformSprites.Length;
            foreach (Sprite s in transformSprites)
            {
                visualController.npcImage.sprite = s;
                yield return new WaitForSeconds(frameDelay);
            }
        }

        else
        {
            yield return new WaitForSeconds(transformDuration);
        }

        if (visualController != null)
        {
            visualController.ReplaceSprites(corruptedIdleSprites, corruptedBlinkSprites, corruptedTalkingSprites);

            visualController.animationSpeed = corruptedAnimationSpeed;
            visualController.ResumeAnimation();
        }

        if (fondoCorruptoClip != null)
        {
            catAudioSourcePrincipal.clip = fondoCorruptoClip;
            catAudioSourcePrincipal.loop = true;
            catAudioSourcePrincipal.Play();
        }

        // --- LANZAMOS LOS SONIDOS RANDOM AVANZADOS INDEPENDIENTES ---
        if (sonidosRandomCorruptos != null)
        {
            foreach (var randomSound in sonidosRandomCorruptos)
            {
                if (randomSound != null && randomSound.soundSettings != null && randomSound.soundSettings.IsValid())
                {
                    StartCoroutine(LoopRandomSound(randomSound));
                }
            }
        }

        ForceProactiveMessage("Acabas de sufrir una transformación gráfica muy dolorosa revelando tu forma corrupta. " +
                              "Dirígete al jugador, dile que NO te ha gustado NADA lo que acaba de decir sobre ti, " +
                              "y pregúntale de forma amenazante e inquietante: '¿Acaso sabes quién soy yo?'. " +
                              "DEBES mantener OBLIGATORIAMENTE el formato JSON.");
    }

    private System.Collections.IEnumerator LoopRandomSound(RandomNodeSoundAction randomData)
    {
        while (true)
        {
            float safeMin = Mathf.Max(0f, randomData.minInterval);
            float safeMax = Mathf.Max(safeMin, randomData.maxInterval);
            float waitTime = Random.Range(safeMin, safeMax);

            if (waitTime > 0) yield return new WaitForSeconds(waitTime);

            float roll = Random.value;

            // Si supera el porcentaje de probabilidad y tiene clip, suena
            if (roll <= randomData.playChance)
            {
                GameObject targetObj = GetChannelObject(randomData.channel);
                if (targetObj == null) targetObj = gameObject;

                // Creamos un objeto volátil para este sonido y lo destruimos al terminar (como en los nodos)
                GameObject soundObj = new GameObject("CatRandomAudio_" + randomData.soundSettings.clip.name);
                soundObj.transform.SetParent(targetObj.transform, false);

                AudioSource newSource = soundObj.AddComponent<AudioSource>();
                if (randomData.channel != AudioChannel.Master)
                {
                    newSource.spatialBlend = 1f; // Lo volvemos 3D
                }

                randomData.soundSettings.PlayOn(newSource, false);
                newSource.loop = false;

                float duration = randomData.soundSettings.clip.length / Mathf.Max(randomData.soundSettings.pitch, 0.01f);
                float totalTime = duration + randomData.soundSettings.fadeInDuration + randomData.soundSettings.fadeOutDuration + 0.5f;
                Destroy(soundObj, totalTime);
            }
        }
    }

    private GameObject GetChannelObject(AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Front: return GameObject.Find("FrontSoundBox");
            case AudioChannel.Back: return GameObject.Find("BackSoundBox");
            case AudioChannel.Left: return GameObject.Find("LeftSoundBox");
            case AudioChannel.Right: return GameObject.Find("RightSoundBox");
            case AudioChannel.Master:
            default: return GameObject.Find("Main Camera");
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

        if (string.IsNullOrEmpty(currentPlayerText))
        {
            base.OnAIResponse(raw);
            return;
        }

        EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
        if (encounterData != null)
        {
            if (string.IsNullOrEmpty(encounterData.respuestaIdentidad))
            {
                encounterData.respuestaIdentidad = currentPlayerText;
                Debug.Log($"<color=cyan>[HISTORIA]</color> Se ha guardado la respuesta del jugador. Él cree que somos: '{encounterData.respuestaIdentidad}'");

                isThinking = false;
                if (thinkingPanel != null) thinkingPanel.SetActive(false);
                if (inputField != null) inputField.gameObject.SetActive(true);

                lastPlayerText = "";

                Debug.Log("<color=red>[IA GATO]</color> Identidad revelada. Pasando el control a la Fase 4...");
                encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage4;

                return;
            }
        }

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

        private void Start()
        {
            base.Start();

            if (visualController != null)
            {
                // Buscamos los datos configurados en el Inspector de este mismo objeto
                EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();

                if (encounterData != null)
                {
                    visualController.animationSpeed = encounterData.stage4AnimationSpeed;
                }
                else
                {
                    visualController.animationSpeed = 0.03f; // Valor por defecto por si acaso
                }
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