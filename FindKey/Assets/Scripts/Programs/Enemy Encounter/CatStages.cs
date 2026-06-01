using UnityEngine;

/// <summary>
/// Class: CatAIScript_Stage1
/// Description: CatAIScript_Stage1 represents the first phase of the cat NPC encounter in the FindKey game. In this initial stage, the cat will appear in its most innocent and
///              friendly state, engaging with the player in a seemingly harmless manner. The AI will have the autonomy to decide when to advance to the next stage based on its own criteria,
///              but we will also secretly monitor the player's input for greetings. If the player greets the cat, we will advance to the next stage in secret, without informing the LLM, as a
///              sort of "Easter egg" that rewards players for trying to interact with the cat in a friendly way. This design allows for a more dynamic and immersive encounter, where player choices
///              can influence the progression of the narrative in unexpected ways.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class CatAIScript_Stage1 : BaseAIScript // In this first phase, the cat will be in its most innocent and friendly state. The AI will have the freedom to decide when to advance to the next stage based on its own criteria, but we will also secretly monitor the player's input for greetings. If the player greets the cat, we will advance to the next stage in secret, without telling the LLM, as a sort of "Easter egg" that rewards players for trying to interact with the cat in a friendly way.
{
    public override void InitNPC() { } // No necesary, we will trigger the phase change with a player greeting or if the LLM decides to do it proactively.

    protected override void OnAIResponse(string raw) // In this first phase, we will allow the LLM to decide if it wants to advance to the next stage on its own, but we will also secretly monitor the player's input for greetings. If the player greets the cat, we will advance to the next stage in secret, without telling the LLM, as a sort of "Easter egg" that rewards players for trying to interact with the cat in a friendly way.
    {
        string currentPlayerText = lastPlayerText;

        base.OnAIResponse(raw); // We call the base method to ensure any existing functionality is preserved, but we will add our own logic after it.

        if (string.IsNullOrEmpty(currentPlayerText)) // If the player hasn't said anything, we won't check for greetings or stage advancement, and we'll just return.
        {
            return;
        }

        string lowerRaw = raw.ToLowerInvariant();
        string lowerPlayer = currentPlayerText.ToLowerInvariant();

        bool aiDecided = lowerRaw.Contains("\"action\":\"next_stage\"") || lowerRaw.Contains("\"action\": \"next_stage\"");

        bool playerGreeted = lowerPlayer.Contains("hola") || // We check for a wide variety of common greetings in Spanish, as well as some in English, to cover different player behaviors. This is not an exhaustive list, but it should catch most typical greetings.
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

        if (aiDecided || playerGreeted) // If the AI has already decided to advance, we won't interfere. But if the player greeted and the AI hasn't decided yet, we will secretly advance to the next stage without telling the AI.
        {
            Debug.Log("<color=magenta>[IA GATO]</color> Saludo detectado tras hablar el jugador. Avanzando a Fase 2 en secreto...");

            EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
            if (encounterData != null)
            {
                encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage2; // We change the encounter type to CatStage2, which will cause the system to switch the AI script to CatAIScript_Stage2 on the next turn. The LLM will not be aware of this change until it receives the new prompt in the next turn, so it will be a surprise for it as well.
            }
        }
    }

    protected override void OpenDoor() { }
}

/// <summary>
/// Class: CatAIScript_Stage2
/// Description: CatAIScript_Stage2 represents the second phase of the cat NPC encounter in the FindKey game. In this stage, the cat will start to show subtle hints of its true nature,
///              becoming more mysterious and less friendly. The AI will still have the autonomy to decide when to advance to the next stage based on its own criteria, but we will also monitor the
///              player's input for specific adjectives that might indicate they are starting to suspect the cat's true nature. If we detect any of those adjectives, we will force the AI to
///              transition to the next stage by including "action": "next_stage" in the prompt, even if the AI doesn't decide to do it on its own. This is a sort of fail-safe mechanism to ensure
///              that if the player is on the right track, they will be able to progress through the encounter without getting stuck due to an uncooperative AI. This design allows for a more
///              dynamic and immersive encounter, where player choices can influence the progression of the narrative in unexpected ways, while also ensuring that players who are on the right
///              track can continue progressing even if the LLM fails to respond as expected.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class CatAIScript_Stage2 : BaseAIScript // In this second phase, we will monitor the player's input for specific adjectives that might indicate they are starting to suspect the cat's true nature. If we detect any of those adjectives, we will force the AI to transition to the next stage by including "action": "next_stage" in the prompt, even if the AI doesn't decide to do it on its own. This is a sort of fail-safe mechanism to ensure that if the player is on the right track, they will be able to progress through the encounter without getting stuck due to an uncooperative AI.
{
    public string[] adjetivosDeSeguridad; // We will populate this array in the Inspector with adjectives that we consider "red flags" that indicate the player is starting to suspect the cat's true nature. For example, words like "sombra", "oscuro", "misterioso", "extraño", "raro", "siniestro", "tenebroso", "escondido", "oculto", "desconocido", etc. The more adjectives we include, the more likely we are to catch players who are on the right track, but we should also be careful not to include too many or too vague ones that could trigger false positives.

    private bool adjetivoDetectadoEsteTurno = false;

    public override void InitNPC() { } // No necessary, we will trigger the phase change with player input or if the LLM decides to do it proactively, but we will also have a fail-safe mechanism that forces the transition if we detect certain keywords in the player's input.

    protected override string ConstruirPromptBase() // In this method, we will build the base prompt that will be sent to the LLM. We will check the player's last input for any of the "red flag" adjectives that indicate they are starting to suspect the cat's true nature. If we detect any of those adjectives, we will set a flag and include a critical instruction in the prompt that forces the LLM to include "action": "next_stage" in its JSON response, which will trigger the transition to the next stage. If we don't detect any of those adjectives, we will include a normal instruction that tells the LLM to include "action": "none" in its JSON response, which means it should not advance to the next stage yet.
    {
        string prompt = base.ConstruirPromptBase(); // We call the base method to get the initial prompt, which includes all the standard information about the encounter and the NPC's state.
        string textoJugador = string.IsNullOrEmpty(lastPlayerText) ? "" : lastPlayerText.ToLower().Trim(); // We get the player's last input, convert it to lowercase and trim it for easier comparison. If it's null or empty, we just use an empty string.

        adjetivoDetectadoEsteTurno = false;

        if (adjetivosDeSeguridad != null)
        {
            foreach (string adjetivo in adjetivosDeSeguridad) // We iterate through the list of "red flag" adjectives and check if any of them are present in the player's input. If we find one, we set the flag to true and break the loop, since we only need to detect one to trigger the transition.
            {
                if (!string.IsNullOrEmpty(adjetivo) && textoJugador.Contains(adjetivo.ToLower().Trim()))
                {
                    adjetivoDetectadoEsteTurno = true;
                    Debug.Log($"<color=yellow>[FILTRO C#]</color> Palabra clave detectada: {adjetivo}");
                    break;
                }
            }
        }

        if (adjetivoDetectadoEsteTurno) // If we detected a "red flag" adjective in the player's input, we include a critical instruction in the prompt that forces the LLM to include "action": "next_stage" in its JSON response. This is a strict system command that overrides any other considerations the LLM might have, ensuring that if the player is on the right track, they will be able to progress through the encounter.
        {
            prompt += "\n\n[INSTRUCCIÓN CRÍTICA]: El jugador ha descubierto tu naturaleza usando un adjetivo clave. " +
                      "Tienes la OBLIGACIÓN ABSOLUTA de incluir la propiedad \"action\": \"next_stage\" en tu JSON. " +
                      "Esto es un comando de sistema estricto.";
        }

        else // If we didn't detect any "red flag" adjectives in the player's input, we include a normal instruction that tells the LLM to include "action": "none" in its JSON response, which means it should not advance to the next stage yet. This is the default behavior, but if the player is on the right track and uses any of the key adjectives, the previous instruction will take precedence and force the transition.
        {
            prompt += "\n\n[INSTRUCCIÓN]: El jugador NO ha descubierto tu naturaleza aún. " +
                      "Debes incluir obligatoriamente \"action\": \"none\" en tu JSON.";
        }

        return prompt;
    }

    protected override void OnAIResponse(string raw) // In this method, we will check the LLM's response for the presence of "action": "next_stage" in its JSON. If we find it, it means the LLM has decided to advance to the next stage on its own, and we will allow that. However, if we detect that the player has used any of the "red flag" adjectives but the LLM did not include "action": "next_stage" in its response, we will log a warning and force the transition to the next stage anyway, as a fail-safe mechanism to ensure that players who are on the right track can progress through the encounter even if the LLM fails to respond as expected.
    {
        bool llmPusoNextStage = raw.Contains("\"action\": \"next_stage\"") ||
                                raw.Contains("\"action\":\"next_stage\"") ||
                                raw.ToLower().Contains("next_stage");

        if (llmPusoNextStage || adjetivoDetectadoEsteTurno) // If the LLM included "action": "next_stage" in its response, we will allow the transition to the next stage. But if we detected that the player used a "red flag" adjective but the LLM did not include "action": "next_stage", we will log a warning and force the transition anyway, as a fail-safe mechanism.
        {
            if (adjetivoDetectadoEsteTurno && !llmPusoNextStage) // This means the player used a "red flag" adjective that should have triggered the transition, but the LLM did not include "action": "next_stage" in its response. This is a failure of the LLM to follow the critical instruction we included in the prompt, but we will not let that stop the encounter from progressing, so we will log a warning and force the transition to the next stage anyway.
            {
                Debug.LogWarning("<color=orange>[TFG FAIL-SAFE]</color> El LLM ignoró el prompt y NO puso 'next_stage', pero C# intervino y forzará la transición por seguridad.");
            }

            else if (llmPusoNextStage) // This means the LLM correctly included "action": "next_stage" in its response, either because it decided to do it on its own or because it followed the critical instruction we included in the prompt. In any case, we will allow the transition to the next stage and log a success message.
            {
                Debug.Log("<color=green>[TFG SUCCESS]</color> El LLM obedeció y ejecutó 'next_stage' en su JSON.");
            }

            EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
            if (encounterData != null) // We change the encounter type to CatStage3, which will cause the system to switch the AI script to CatAIScript_Stage3 on the next turn. The LLM will not be aware of this change until it receives the new prompt in the next turn, so it will be a surprise for it as well.
            {
                Debug.Log("<color=red>[IA GATO]</color> Iniciando transformación a Fase 3...");

                isThinking = false;
                lastPlayerText = "";

                encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage3;

                return;
            }
        }

        base.OnAIResponse(raw); // If we didn't detect "action": "next_stage" in the LLM's response and we also didn't detect any "red flag" adjectives in the player's input, we will just proceed with the normal response handling without changing stages.
    }

    protected override void OpenDoor() { }
}

/// <summary>
/// Class: CatAIScript_Stage3
/// Description: CatAIScript_Stage3 represents the third and final phase of the cat NPC encounter in the FindKey game. In this stage, we will trigger a transformation
///              sequence where the cat reveals its true corrupted form. This will involve changing the cat's sprites to more disturbing and corrupted versions, playing ominous sound
///              effects, and forcing a proactive message that describes the transformation in a dramatic way. The LLM will not be aware of the transformation until it receives the new
///              prompt in the next turn, so it will be a surprise for it as well. After the transformation, we will also monitor the player's response to the cat's question
///              "¿Acaso sabes quién soy yo?" and save that response for later use in the final stage of the encounter. This design allows for a more dynamic and immersive encounter,
///              where player choices can influence the progression of the narrative in unexpected ways, while also providing a dramatic climax to the cat's storyline.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class CatAIScript_Stage3 : BaseAIScript // In this third phase, we will trigger a transformation sequence where the cat reveals its true corrupted form. This will involve changing the cat's sprites to more disturbing and corrupted versions, playing ominous sound effects, and forcing a proactive message that describes the transformation in a dramatic way. The LLM will not be aware of the transformation until it receives the new prompt in the next turn, so it will be a surprise for it as well. After the transformation, we will also monitor the player's response to the cat's question "¿Acaso sabes quién soy yo?" and save that response for later use in the final stage of the encounter.
{
    public AudioClip zumbidoClip;
    public AudioClip transicionClip;
    public AudioClip fondoCorruptoClip;

    public RandomNodeSoundAction[] sonidosRandomCorruptos;

    public Sprite[] transformSprites;
    public Sprite[] corruptedIdleSprites;
    public Sprite[] corruptedBlinkSprites;
    public Sprite[] corruptedTalkingSprites;

    [Header("Velocidades Fase 3")]
    public float transformDuration = 1.5f;
    public float corruptedAnimationSpeed = 0.05f;

    [Header("Datos Guardados de la Historia")]
    public string respuestaIdentidad = "";

    private AudioSource catAudioSourcePrincipal;


    public override void InitNPC() { }

    public void IniciarEfectos() // This method will be called externally (probably from the EnemyEncounterManager) when we want to start the transformation effects of Phase 3. We separate it from InitNPC because we want to have more control over when exactly the transformation starts, and we might want to trigger it at a specific moment rather than immediately when the stage starts.
    {
        if (inputField != null) inputField.gameObject.SetActive(false);

        if (thinkingPanel != null) thinkingPanel.SetActive(true);

        catAudioSourcePrincipal = gameObject.AddComponent<AudioSource>();

        StartCoroutine(RutinaTransformacion()); // We start the transformation routine, which will handle all the visual and audio effects, as well as the proactive message and the timing of the transformation. This routine will run independently and will not block the main thread, allowing the encounter to continue smoothly while the transformation is happening.
    }

    private System.Collections.IEnumerator RutinaTransformacion() // This coroutine will handle the entire transformation sequence of Phase 3, including stopping any existing animations, playing the transformation sound effects, changing the sprites to the corrupted versions, and forcing the proactive message that describes the transformation. We use a coroutine to manage the timing of all these events in a smooth and sequential way.
    {
        if (visualController != null) visualController.StopAnimation();

        if (zumbidoClip != null)
        {
            catAudioSourcePrincipal.clip = zumbidoClip;
            catAudioSourcePrincipal.loop = true;
            catAudioSourcePrincipal.Play();
        }

        yield return new WaitForSeconds(4f); // We let the ominous buzzing sound play for a few seconds before starting the transformation, to build tension and anticipation.

        catAudioSourcePrincipal.Stop();
        if (transicionClip != null)
        {
            catAudioSourcePrincipal.PlayOneShot(transicionClip);
        }

        if (visualController != null && transformSprites != null && transformSprites.Length > 0) // If we have a visual controller and a set of transformation sprites, we will play the transformation animation by cycling through the transformSprites array with a delay between each frame. This will create a smooth animation effect of the cat transforming into its corrupted form. The duration of the entire transformation will be determined by the transformDuration variable, which we can adjust in the Inspector to make the transformation faster or slower.
        {
            float frameDelay = transformDuration / transformSprites.Length;
            foreach (Sprite s in transformSprites) // We iterate through the transformSprites array and set each sprite as the npcImage's sprite with a delay in between, creating the animation effect. We also check if the visualController is still valid before setting the sprite, in case something happened to it during the transformation.
            {
                visualController.npcImage.sprite = s;
                yield return new WaitForSeconds(frameDelay);
            }
        }

        else
        {
            yield return new WaitForSeconds(transformDuration); // If we don't have transformation sprites, we will just wait for the duration of the transformation before proceeding to change the sprites to the corrupted versions. This way we still maintain the timing of the transformation even without the visual animation.
        }

        if (visualController != null) // After the transformation animation is complete, we will change the cat's sprites to the corrupted versions, which will reflect its true nature. We will also set the animation speed to a new value that fits the corrupted form, and then we will resume the animation so that the cat starts animating with its new corrupted sprites.
        {
            visualController.ReplaceSprites(corruptedIdleSprites, corruptedBlinkSprites, corruptedTalkingSprites);

            visualController.animationSpeed = corruptedAnimationSpeed;
            visualController.ResumeAnimation();
        }

        if (fondoCorruptoClip != null) // We will also start playing a new background music or ambient sound that fits the corrupted atmosphere of the cat's true form. This will enhance the immersion and the emotional impact of the transformation. We set it to loop so that it continues playing in the background during the rest of the encounter.
        {
            catAudioSourcePrincipal.clip = fondoCorruptoClip;
            catAudioSourcePrincipal.loop = true;
            catAudioSourcePrincipal.Play();
        }

        if (sonidosRandomCorruptos != null) // If we have a set of random sound actions for the corrupted phase, we will start a coroutine for each of them that will handle the random playing of those sounds according to their settings. This will add more variety and unpredictability to the audio experience during the corrupted phase, making it more dynamic and engaging.
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
                              "y pregúntale de forma amenazante e inquietante algo similar a: '¿Acaso sabes quién soy yo?'. pero que haga alusión a su respuesta" +
                              "DEBES mantener OBLIGATORIAMENTE el formato JSON."); // We force a proactive message that describes the transformation in a dramatic way and asks the player the critical question "¿Acaso sabes quién soy yo?" that will be important for the final stage of the encounter. We also remind the LLM to maintain the JSON format in its response, as always.
    }

    private System.Collections.IEnumerator LoopRandomSound(RandomNodeSoundAction randomData) // This coroutine will handle the random playing of a specific sound according to the settings defined in the RandomNodeSoundAction. It will run in an infinite loop, waiting for a random interval between plays, and then deciding randomly whether to play the sound based on the playChance. If it decides to play the sound, it will create a new GameObject with an AudioSource, play the sound, and destroy the GameObject after the sound has finished playing. This allows us to have multiple random sounds playing independently without interfering with each other.
    {
        while (true)
        {
            float safeMin = Mathf.Max(0f, randomData.minInterval);
            float safeMax = Mathf.Max(safeMin, randomData.maxInterval);
            float waitTime = Random.Range(safeMin, safeMax);

            if (waitTime > 0) yield return new WaitForSeconds(waitTime); // We wait for a random interval before attempting to play the sound again, to create the randomness in the timing of the sounds.

            float roll = Random.value;

            if (roll <= randomData.playChance) // We roll a random value between 0 and 1, and if it's less than or equal to the playChance defined in the randomData, we will play the sound. This allows us to have a certain probability of the sound playing each time we check, adding to the unpredictability of the audio experience.
            {
                GameObject targetObj = GetChannelObject(randomData.channel);
                if (targetObj == null) targetObj = gameObject;

                GameObject soundObj = new GameObject("CatRandomAudio_" + randomData.soundSettings.clip.name);
                soundObj.transform.SetParent(targetObj.transform, false);

                AudioSource newSource = soundObj.AddComponent<AudioSource>();
                if (randomData.channel != AudioChannel.Master) // If the sound is not on the Master channel, we will set it to be 3D so that it has spatial properties and can create a more immersive experience. If it's on the Master channel, we will keep it as 2D so that it plays uniformly regardless of the player's position.
                {
                    newSource.spatialBlend = 1f;
                }

                randomData.soundSettings.PlayOn(newSource, false);
                newSource.loop = false;

                float duration = randomData.soundSettings.clip.length / Mathf.Max(randomData.soundSettings.pitch, 0.01f);
                float totalTime = duration + randomData.soundSettings.fadeInDuration + randomData.soundSettings.fadeOutDuration + 0.5f;
                Destroy(soundObj, totalTime); // We destroy the sound GameObject after the sound has finished playing, plus a small buffer time to ensure it doesn't get cut off prematurely. This way we keep the scene clean and avoid having too many lingering GameObjects from the random sounds. We also make sure to calculate the total time based on the clip length and the pitch, as well as any fade in or fade out durations that might be defined in the SoundSettings.
            }
        }
    }

    private GameObject GetChannelObject(AudioChannel channel) // This method returns the GameObject that corresponds to the specified audio channel. This allows us to parent the random sound GameObjects to the appropriate objects in the scene, which can help with organization and also with spatial audio if the channels are set up that way. We assume that there are GameObjects named "FrontSoundBox", "BackSoundBox", "LeftSoundBox", "RightSoundBox" in the scene for the respective channels, and we use the "Main Camera" for the Master channel as a fallback.
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

    protected override string ConstruirPromptBase() // In this method, we will build the base prompt for Phase 3. We will include a specific instruction that tells the LLM to react to the player's response to the question "¿Acaso sabes quién soy yo?" in a cryptic way, and to include "action": "none" in its JSON response for now. We will also make sure to maintain the standard information about the encounter and the NPC's state by calling the base method.
    {
        string prompt = base.ConstruirPromptBase();

        prompt += "\n\n[SYSTEM INSTRUCTION]: Eres una entidad oscura. Tu objetivo principal ahora es escuchar la respuesta del jugador " +
                  "a tu pregunta, que es algo similar a'¿Acaso sabes quién soy yo?' y reaccionar de forma críptica. Pon \"action\": \"none\" en tu JSON por ahora.";

        return prompt;
    }

    protected override void OnAIResponse(string raw) // In this method, we will check the player's last input for their response to the question "¿Acaso sabes quién soy yo?" and save that response in the respuestaIdentidad variable for later use in the final stage of the encounter. This way, we can have a more personalized and reactive experience in the final stage, where the cat can refer to the player's guess about its identity. After saving the player's response, we will proceed with the normal response handling without changing stages, as the transition to the next stage will be triggered by other conditions in the final stage.
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

/// <summary>
/// Class: CatAIScript_Stage4
/// Description: CatAIScript_Stage4 represents the fourth and final phase of the cat NPC encounter in the FindKey game. In this stage,
///              the cat will have fully revealed its corrupted form and will challenge the player to guess the location where they are.
///              The player will have to use the clues given by the cat throughout the encounter, as well as their own intuition and reasoning, to figure out the correct location.
///              We will monitor the player's input for specific keywords that indicate they are guessing the location, and if they guess correctly,
///              we will trigger a final event that ends the encounter in a dramatic way. We will also refer to the player's previous guess about the cat's identity
///              (saved in respuestaIdentidad) to make the interaction more personalized and reactive.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class CatAIScript_Stage4 : BaseAIScript // In this fourth and final phase, the cat will have fully revealed its corrupted form and will challenge the player to guess the location where they are. The player will have to use the clues given by the cat throughout the encounter, as well as their own intuition and reasoning, to figure out the correct location. We will monitor the player's input for specific keywords that indicate they are guessing the location, and if they guess correctly, we will trigger a final event that ends the encounter in a dramatic way. We will also refer to the player's previous guess about the cat's identity (saved in respuestaIdentidad) to make the interaction more personalized and reactive.
{
    public string[] passwordsUbicacion;
    public string respuestaIdentidadJugador;

    public GameEvent eventoFinalDemo;

    public override void InitNPC() { }

    private void Start() // In the Start method of Phase 4, we will set up the cat's behavior and appearance for the final confrontation. We will adjust the animation speed to make it more menacing, and we will force a proactive message that taunts the player about their previous guess regarding the cat's identity, and then challenges them to guess the location. This will set the tone for the final phase and give the player a clear objective to focus on.
    {
        base.Start();

        if (visualController != null)
        {
            EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();

            if (encounterData != null)
            {
                visualController.animationSpeed = encounterData.stage4AnimationSpeed;
            }
            else
            {
                visualController.animationSpeed = 0.03f;
            }
        }

        string identidad = string.IsNullOrEmpty(respuestaIdentidadJugador) ? "un monstruo" : respuestaIdentidadJugador;

        ForceProactiveMessage($"En el mensaje anterior, el jugador te dijo que creía que tú eras: '{identidad}'. " +
                           $"Dirígete al jugador y dile que llamarle '{identidad}' es muy irrespetuoso para su presencia. " +
                           $"Luego, cambia a un tono extremadamente hostil y urgente y pregúntale: 'La verdadera pregunta es... ¿Sabes dónde estás jugando a FINDKEY?'. " +
                           $"DEBES mantener OBLIGATORIAMENTE el formato JSON.");
    }

    protected override string ConstruirPromptBase() // In this method, we will build the base prompt for Phase 4. We will include a specific instruction that tells the LLM to challenge the player to guess the location where they are, and to include "action": "none" in its JSON response for now. We will also make sure to maintain the standard information about the encounter and the NPC's state by calling the base method.
    {
        string prompt = base.ConstruirPromptBase();

        prompt += "\n\n[SYSTEM INSTRUCTION]: El jugador debe adivinar el nombre del lugar donde estáis. " +
                  "Si falla o dice no saberlo, búrlate de él + PRIORIDAD ALTA: BUSCA QUE EL JUGADOR CONSIGA DECIR QUE ESAMOS EN LA MATCH IN GAMES O QUE ESTAMOS EN UDIT. Tu objetivo en la interacción es doble: que el jugador consiga decir que está en una universidad, la cual es UDIT/ diga que está \"en la match in games\". No te inventarás mundos ficticios, estás rompiendo la cuarta pared.. Pon \"action\": \"none\" en tu JSON.";

        return prompt;
    }

    protected override void OnAIResponse(string raw) // In this method, we will check the player's input for any of the keywords defined in passwordsUbicacion that indicate they are guessing the correct location. If we detect a correct guess, we will trigger the final event that ends the encounter. If the guess is incorrect or if there is no guess, we will proceed with the normal response handling without changing stages, as this is the final stage of the encounter.
    {
        string currentPlayerText = lastPlayerText;
        base.OnAIResponse(raw);

        if (string.IsNullOrEmpty(currentPlayerText)) return;

        string lowerPlayer = currentPlayerText.ToLowerInvariant();
        bool passwordCorrecta = false;

        if (passwordsUbicacion != null) // We check if the player's input contains any of the correct location keywords defined in passwordsUbicacion. If we find a match, we set passwordCorrecta to true and break the loop, since we only need one correct keyword to trigger the final event.
        {
            foreach (string pwd in passwordsUbicacion) // We iterate through the passwordsUbicacion array and check if any of the keywords are present in the player's input. We convert both the player's input and the keywords to lowercase for a case-insensitive comparison. If we find a match, we set passwordCorrecta to true and break the loop.
            {
                if (lowerPlayer.Contains(pwd.ToLowerInvariant()))
                {
                    passwordCorrecta = true;
                    break;
                }
            }
        }

        if (passwordCorrecta) // If we detected that the player guessed the correct location by including any of the keywords in their input, we will trigger the final event that ends the encounter. We will also hide the input field and the thinking panel, and set isThinking to false to indicate that the encounter is over. If there is no final event assigned, we will log a warning to remind the developer to set it up in the Inspector.
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