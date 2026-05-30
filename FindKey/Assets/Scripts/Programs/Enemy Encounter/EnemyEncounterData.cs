/// <summary>
/// Class: EnemyEncounterData
/// Description: This script serves as a data container for the enemy encounter in the FindKey game. It holds references to various data objects and configurations related to the
///              enemy encounter, such as MoveAppData, StoryLog, NPC profiles for different stages of the cat encounter, and a list of adjectives for the "Cinturón de Seguridad" phase.
///              The script is designed to be attached to a GameObject in the Unity scene, allowing other scripts to access and utilize this data during the enemy encounter sequence.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using System;
using UnityEngine;
using UnityEngine.Events;

public class EnemyEncounterData : MonoBehaviour
{
    public MoveAppData moveAppData;
    public StoryLog myAppStoryLog;

    [Header("Configuración de Fases del Gato")] // Cat stages
    public NPCProfile catStage1Profile;
    public NPCProfile catStage2Profile;
    public NPCProfile catStage3Profile;
    public NPCProfile catStage4Profile;

    [Header("Cinturón de Seguridad (Fase 2)")] // All the adjectives for the "Cinturón de Seguridad" phase, which is the second stage of the cat encounter. These adjetives was wrote by the designer.
    [SerializeField] public string[] adjetivosDeSeguridadFase2 = new string[] {
        "cabron", "cabrona", "gilipollas", "capullo", "capulla", "hijo de puta", "hija de puta",
"mamon", "mamona", "subnormal", "retrasado", "retrasada", "pringado", "pringada",
"payaso", "payasa", "imbecil", "idiota", "estupido", "estupida", "inutil",
"mierda", "basura", "escoria", "malparido", "malparida", "cretino", "cretina",
"zorra", "zorro", "puta", "puto", "mierdoso", "mierdosa", "pelotudo", "pelotuda",
"gilipuertas", "gilicachas", "atontado", "atontada", "lelo", "lela", "memo", "mema",
"tolai", "tontolaba", "bocachancla", "bocazas", "flipado", "flipada", "fantasma",
"pringao", "pringada", "patan", "paleto", "paleta", "cateto", "cateta",
"bestia", "animal", "burro", "burra", "zoquete", "tarado", "tarada",
"chalado", "chalada", "pirado", "pirada", "loco de mierda", "payaso de mierda",
"gilipollas integral", "cara dura", "sinverguenza", "desgraciado", "desgraciada",
"muerto de hambre", "fracasado", "fracasada", "patetico", "patetica",
"lamentable", "ridiculo", "ridicula", "asqueroso", "asquerosa", "repugnante",
"vomitivo", "vomitiva", "apestoso", "apestosa", "cerdo", "cerda",
"guarro", "guarra", "miserable", "rata", "rata inmunda", "escoria humana",
"pedazo de mierda", "trozo de mierda", "pedazo de inutil", "pedazo de cabron",
"payaso inutil", "gilipollas de mierda", "subnormal de mierda",
"idiota de mierda", "maldito idiota", "malnacido", "malnacida",
"engendro", "engendro humano", "engreido", "engreida", "farsante",
"falso", "falsa", "hipocrita", "cobarde", "rastrero", "rastrera",
"traidor", "traidora", "sanguijuela", "parasito", "parasita",
"chupoptero", "trepa", "fanfarron", "fanfarrona", "creido", "creida",
"prepotente", "arrogante", "egolatra", "narcisista", "lloron", "llorona",
"quejica", "pesado", "pesada", "plasta", "pelma", "tocapelotas",
"tocacojones", "rompehuevos", "aguafiestas", "cansino", "cansina",
"amargado", "amargada", "borde", "antipatico", "antipatica",
"despreciable", "despreciado", "despreciada", "desalmado", "desalmada",
"psicopata", "enfermo", "enferma", "perturbado", "perturbada",
"degenerado", "degenerada", "obsesionado", "obsesionada",
"manipulador", "manipuladora", "abusador", "abusadora",
"acosador", "acosadora", "violento", "violenta", "agresivo", "agresiva",
"bestia humana", "animal de mierda", "mono", "mona", "simio",
"payasin", "payasete", "pringadete", "bobalicon", "bobalicona",
"cara de culo", "cara de perro", "cara de amargado", "cara de idiota",
"cara de tonto", "cara de gilipollas", "cara de nada",
"me das asco", "me das pena", "me das verguenza", "me das miedo",
"me aterras", "me repugnas", "me enfermas", "me hartas",
"me sacas de quicio", "me sacas la paciencia", "no te aguanto",
"no te soporto", "no sirves para nada", "das pena", "das asco",
"eres basura", "eres una basura", "eres una mierda",
"eres un inutil", "eres patetico", "eres lamentable",
"eres un gilipollas", "eres un cabron", "eres una cabrona",
"eres un hijo de puta", "eres una hija de puta",
"eres insoportable", "eres insufrible", "eres retrasado",
"eres subnormal", "eres ridiculo", "eres un payaso",
"eres un enfermo", "eres un psicopata", "eres un monstruo",
"eres despreciable", "eres una verguenza", "eres un desastre",
"eres un problema", "eres toxico", "eres falso",
"eres un mentiroso", "eres una rata", "eres escoria",
"eres una decepcion", "das cringe", "das rabia",
"que asco me das", "que pena das", "que verguenza das",
"que ridiculo eres", "que pesado eres", "que insoportable eres",
"que patetico eres", "que estupido eres", "que inutil eres",
"das lastima", "me provocas rechazo", "me provocas odio",
"me produces asco", "me produces miedo", "me produces ansiedad",
"me caes fatal", "me caes como el culo", "me caes horrible",
"me pareces estupido", "me pareces ridiculo", "me pareces patetico",
"me pareces una basura", "me pareces insoportable",
"tu actitud apesta", "tu personalidad da asco",
"tu presencia incomoda", "tu voz molesta", "tu cara da miedo",
"ojala te calles", "callate", "callate ya", "vete a la mierda",
"largate", "pirate", "deja de hablar", "nadie te aguanta",
"nadie te quiere", "das pereza", "das mal rollo",
"maldito pesado", "puto pesado", "puto loco", "puto idiota",
"jodido imbecil", "jodete", "vete al carajo", "anda a freir esparragos",
"anda a la mierda", "que te den", "das grimita", "das repelus",
"eres repelente", "eres irritante", "eres cargante",
"eres insoportable de verdad", "eres lo peor", "eres nefasto",
"eres horrible", "eres un cero a la izquierda", "feo", "fea", "horrible", "asqueroso", "asquerosa", "repugnante", "desagradable", "tonto", "tonta",
"idiota", "imbecil", "estupido", "estupida", "torpe", "inutil", "pesado", "pesada", "molesto",
"molesta", "cansino", "cansina", "odioso", "odiosa", "amargado", "amargada", "grunon", "grunona",
"antipatico", "antipatica", "raro", "rara", "extrano", "extrana", "creepy", "loco", "loca",
"ridiculo", "ridicula", "patetico", "patetica", "lamentable", "mediocre", "cutre", "pobre",
"agresivo", "agresiva", "violento", "violenta", "malvado", "malvada", "cruel", "frio", "fria",
"insensible", "egoista", "narcisista", "arrogante", "presumido", "presumida", "engreido",
"engreida", "pesimista", "toxico", "toxica", "falso", "falsa", "hipocrita", "mentiroso",
"mentirosa", "traicionero", "traicionera", "celoso", "celosa", "envidioso", "envidiosa",
"vago", "vaga", "perezoso", "perezosa", "sucio", "sucia", "desordenado", "desordenada",
"descuidado", "descuidada", "maleducado", "maleducada", "grosero", "grosera", "brusco", "brusca",
"tosco", "tosca", "malhumorado", "malhumorada", "irritable", "caprichoso", "caprichosa",
"mandon", "mandona", "controlador", "controladora", "dominante", "manipulador", "manipuladora",
"abusivo", "abusiva", "cobarde", "debil", "lloron", "llorona", "dramatico", "dramatica",
"infantil", "inmaduro", "inmadura", "absurdo", "absurda", "vacio", "vacia", "superficial",
"desesperante", "aburrido", "aburrida", "soso", "sosa", "simplon", "simplona",
"gris", "monotono", "monotona", "callado", "callada", "serio", "seria", "introvertido",
"introvertida", "extrovertido", "extrovertida", "timido", "timida", "vergonzoso", "vergonzosa",
"nervioso", "nerviosa", "ansioso", "ansiosa", "paranoico", "paranoica", "obsesivo", "obsesiva",
"intenso", "intensa", "sensible", "emocional", "fragil", "fuerte", "valiente", "atrevido",
"atrevida", "audaz", "seguro", "segura", "confiado", "confiada", "valioso", "valiosa",
"importante", "especial", "unico", "unica", "curioso", "curiosa", "inteligente", "listo",
"lista", "sabio", "sabia", "genio", "brillante", "rapido", "rapida", "agil", "astuto",
"astuta", "ingenioso", "ingeniosa", "creativo", "creativa", "talentoso", "talentosa",
"habilidoso", "habilidosa", "capaz", "eficiente", "responsable", "disciplinado", "disciplinada",
"trabajador", "trabajadora", "productivo", "productiva", "organizado", "organizada", "limpio",
"limpia", "educado", "educada", "respetuoso", "respetuosa", "amable", "agradable", "simpatico",
"simpatica", "divertido", "divertida", "gracioso", "graciosa", "encantador", "encantadora",
"adorable", "lindo", "linda", "bonito", "bonita", "hermoso", "hermosa", "precioso", "preciosa",
"atractivo", "atractiva", "guapo", "guapa", "elegante", "refinado", "refinada", "carismatico",
"carismatica", "cool", "genial", "fantastico", "fantastica", "excelente", "perfecto",
"perfecta", "maravilloso", "maravillosa", "espectacular", "increible", "impresionante",
"magnifico", "magnifica", "extraordinario", "extraordinaria", "fascinante", "interesante",
"entretenido", "entretenida", "emocionante", "amigable", "carinoso", "carinosa", "dulce",
"tierna", "tierno", "amoroso", "amorosa", "fiel", "leal", "honesto", "honesta", "sincero",
"sincera", "humilde", "generoso", "generosa", "solidario", "solidaria", "empatico", "empatica",
"comprensivo", "comprensiva", "paciente", "tranquilo", "tranquila", "calmado", "calmada",
"relajado", "relajada", "positivo", "positiva", "optimista", "alegre", "feliz", "contento",
"contenta", "energetico", "energetica", "activo", "activa", "dinamico", "dinamica",
"aventurero", "aventurera", "apasionado", "apasionada", "motivado", "motivada", "ambicioso",
"ambiciosa", "persistente", "constante", "decidido", "decidida", "prudente", "maduro",
"madura", "apestoso", "apestosa", "guarro", "guarra", "baboso", "babosa", "payaso", "payasa",
"friki", "nerd", "empollon", "empollona", "cringe", "borde", "seca", "seco", "vacilon",
"vacilona", "chulo", "chula", "flipado", "flipada", "fanfarron", "fanfarrona",
"me das asco", "me das miedo", "me aterras", "me incomodas", "me preocupas",
"me caes mal", "me caes bien", "me encantas", "me fascinas", "me aburres",
"me desesperas", "me agotas", "me irritas", "me molestas", "me fastidias",
"me enfadas", "me haces reir", "me haces feliz", "me haces dano", "me haces sentir raro",
"me haces sentir incomodo", "me haces sentir seguro", "me haces sentir especial",
"me haces sentir inutil", "me haces sentir querido", "me haces sentir pequeno",
"me haces sentir importante", "me haces sentir vacio", "me haces sentir vivo",
"me provocas ansiedad", "me provocas ternura", "me provocas rechazo",
"me provocas curiosidad", "me provocas confianza", "me provocas tristeza",
"me provocas alegria", "me das ternura", "me das pena", "me das verguenza",
"me das cringe", "me das tranquilidad", "me das ansiedad", "me das rabia",
"me das confianza", "me das felicidad", "me das lastima", "me das curiosidad",
"me transmites paz", "me transmites malas vibras", "me transmites inseguridad",
"me transmites confianza", "me transmites miedo", "me transmites amor",
"me transmites odio", "me transmites tranquilidad", "me transmites peligro",
"me caes fatal", "me caes increible", "me caes raro", "me caes pesado",
"me caes genial", "me caes sospechoso", "me caes adorable", "me caes insufrible",
"me pareces raro", "me pareces interesante", "me pareces insoportable",
"me pareces atractivo", "me pareces desagradable", "me pareces ridiculo",
"me pareces inteligente", "me pareces inutil", "me pareces adorable",
"me pareces patetico", "me pareces misterioso", "me pareces peligroso",
"me pareces sospechoso", "me pareces aburrido", "me pareces divertido",
"me pareces intenso", "me pareces falso", "me pareces honesto",
"me pareces buena persona", "me pareces mala persona", "me pareces un genio",
"me pareces un idiota", "me pareces una basura", "me pareces increible",
"me vuelves loco", "me vuelves loca", "me vuelves feliz", "me vuelves inseguro",
"me vuelves insegura", "me vuelves nervioso", "me vuelves nerviosa",
"no te soporto", "no te aguanto", "no quiero verte", "no quiero hablar contigo",
"no me gustas", "no confio en ti", "no me inspiras confianza",
"eres insoportable", "eres adorable", "eres ridiculo", "eres increible",
"eres raro", "eres espeluznante", "eres aterrador", "eres incomodo",
"eres divertido", "eres aburrido", "eres molesto", "eres pesado",
"eres irritante", "eres genial", "eres una maravilla", "eres un desastre",
"eres una decepcion", "eres impresionante", "eres patetico",
"eres una verguenza", "eres un amor", "eres un encanto", "eres insufrible",
"eres toxico", "eres manipulador", "eres sospechoso", "eres confiable",
"eres falso", "eres honesto", "eres frio", "eres calido",
"eres distante", "eres cercano", "eres intenso", "eres dramatico",
"eres especial", "eres unico", "eres superficial", "eres profundo",
"eres inteligente", "eres tonto", "eres un genio", "eres un inutil",
"eres un crack", "eres basura", "eres oro", "eres arte", "eres caos",
"das miedo", "das asco", "das pena", "das rabia", "das verguenza",
"das cringe", "das tranquilidad", "das confianza", "das problemas",
"das malas vibras", "das buenas vibras", "das ternura", "das lastima",
"das ansiedad", "das felicidad", "das tristeza", "das estres",
"que asco me das", "que miedo me das", "que verguenza das",
"que pesado eres", "que insoportable eres", "que adorable eres",
"que lindo eres", "que molesto eres", "que raro eres",
"me sacas de quicio", "me sacas una sonrisa", "me sacas la paciencia",
"me rompes el corazon", "me rompes los nervios",
"me llenas de paz", "me llenas de ansiedad", "me llenas de rabia",
"me intimidas", "me inspiras", "me decepcionas", "me impresionas",
"me sorprendes", "me traumatizas", "me relajas", "me alteras",
"me gusta tu energia", "odio tu actitud", "amo tu personalidad",
"me enfermas", "me obsesionas", "me confundes",
"me hipnotizas", "me repeles", "me atraes", "me interesas",
"me desagradas", "me enamoras", "tu presencia incomoda",
"tu presencia tranquiliza", "tu presencia intimida",
"tu voz molesta", "tu voz calma", "tu mirada asusta",
"tu actitud apesta", "tu actitud encanta", "me das asco", "me das miedo", "me aterras", "me incomodas", "me preocupas",
"me caes mal", "me caes bien", "me encantas", "me fascinas", "me aburres",
"me desesperas", "me agotas", "me irritas", "me molestas", "me fastidias",
"me enfadas", "me haces reír", "me haces feliz", "me haces daño", "me haces sentir raro",
"me haces sentir incómodo", "me haces sentir seguro", "me haces sentir especial",
"me haces sentir inútil", "me haces sentir querido", "me haces sentir pequeño",
"me haces sentir importante", "me haces sentir vacío", "me haces sentir vivo",
"me provocas ansiedad", "me provocas ternura", "me provocas rechazo",
"me provocas curiosidad", "me provocas confianza", "me provocas tristeza",
"me provocas alegría", "me das ternura", "me das pena", "me das vergüenza",
"me das cringe", "me das tranquilidad", "me das ansiedad", "me das rabia",
"me das confianza", "me das felicidad", "me das lástima", "me das curiosidad",
"me transmites paz", "me transmites malas vibras", "me transmites inseguridad",
"me transmites confianza", "me transmites miedo", "me transmites amor",
"me transmites odio", "me transmites tranquilidad", "me transmites peligro",
"me caes fatal", "me caes increíble", "me caes raro", "me caes pesado",
"me caes genial", "me caes sospechoso", "me caes adorable", "me caes insufrible",
"me pareces raro", "me pareces interesante", "me pareces insoportable",
"me pareces atractivo", "me pareces desagradable", "me pareces ridículo",
"me pareces inteligente", "me pareces inútil", "me pareces adorable",
"me pareces patético", "me pareces misterioso", "me pareces peligroso",
"me pareces sospechoso", "me pareces aburrido", "me pareces divertido",
"me pareces intenso", "me pareces falso", "me pareces honesto",
"me pareces buena persona", "me pareces mala persona", "me pareces un genio",
"me pareces un idiota", "me pareces una basura", "me pareces increíble",
"me pareces especial", "me pareces corriente", "me pareces deprimente",
"me vuelves loco", "me vuelves loca", "me vuelves feliz", "me vuelves inseguro",
"me vuelves insegura", "me vuelves nervioso", "me vuelves nerviosa",
"me vuelves paranoico", "me vuelves paranoica", "me vuelves débil",
"me vuelves fuerte", "me vuelves agresivo", "me vuelves agresiva",
"me vuelves mejor", "me vuelves peor", "me vuelves tóxico", "me vuelves tóxica",
"no te soporto", "no te aguanto", "no quiero verte", "no quiero hablar contigo",
"no me gustas", "no confío en ti", "no me inspiras confianza",
"no me haces gracia", "no me interesas", "no significas nada para mí",
"no quiero estar contigo", "no me siento cómodo contigo",
"no me siento segura contigo", "no me siento seguro contigo",
"no me siento bien contigo", "no me siento feliz contigo",
"no me das buena espina", "no me transmites nada bueno",
"no me gustas nada", "no tienes gracia", "no vales nada",
"eres insoportable", "eres adorable", "eres ridículo", "eres increíble",
"eres raro", "eres extrañamente agradable", "eres espeluznante",
"eres aterrador", "eres incómodo", "eres divertido", "eres aburrido",
"eres molesto", "eres pesado", "eres irritante", "eres genial",
"eres una maravilla", "eres un desastre", "eres un problema",
"eres una decepción", "eres impresionante", "eres patético",
"eres lamentable", "eres una vergüenza", "eres una joya",
"eres un amor", "eres un encanto", "eres insufrible", "eres tóxico",
"eres manipulador", "eres sospechoso", "eres confiable", "eres falso",
"eres honesto", "eres frío", "eres cálido", "eres distante",
"eres cercano", "eres intenso", "eres dramático", "eres especial",
"eres único", "eres corriente", "eres superficial", "eres profundo",
"eres inteligente", "eres tonto", "eres un genio", "eres un inútil",
"eres un crack", "eres basura", "eres oro", "eres arte", "eres caos",
"das miedo", "das asco", "das pena", "das rabia", "das vergüenza",
"das cringe", "das tranquilidad", "das confianza", "das problemas",
"das malas vibras", "das buenas vibras", "das ternura", "das lástima",
"das ansiedad", "das felicidad", "das tristeza", "das estrés",
"das pereza", "das alegría", "das curiosidad", "das incomodidad",
"qué asco me das", "qué miedo me das", "qué vergüenza das",
"qué pesado eres", "qué insoportable eres", "qué adorable eres",
"qué lindo eres", "qué molesto eres", "qué raro eres",
"qué persona tan desagradable", "qué persona tan agradable",
"qué persona tan falsa", "qué persona tan increíble",
"qué persona tan intensa", "qué persona tan aburrida",
"qué persona tan divertida", "qué persona tan deprimente",
"me sacas de quicio", "me sacas una sonrisa", "me sacas la paciencia",
"me sacas lo peor", "me sacas lo mejor", "me sacas de onda",
"me rompes la cabeza", "me rompes el corazón", "me rompes los nervios",
"me llenas de paz", "me llenas de ansiedad", "me llenas de rabia",
"me llenas de felicidad", "me llenas de dudas", "me llenas de miedo",
"me intimidas", "me inspiras", "me decepcionas", "me impresionas",
"me sorprendes", "me traumatizas", "me relajas", "me alteras",
"me perturba tu presencia", "me gusta tu energía", "odio tu actitud",
"amo tu personalidad", "detesto cómo hablas", "adoro cómo eres",
"me enfermas", "me encariño contigo", "me obsesionas", "me confundes",
"me hipnotizas", "me repeles", "me atraes", "me interesas",
"me desagradas", "me enamoras", "me decepcionas siempre",
"me haces perder la paciencia", "me haces sentir querido",
"me haces sentir basura", "me haces sentir invisible",
"me haces sentir importante", "me haces sentir incómodo",
"me haces sentir protegido", "me haces sentir atacado",
"me haces sentir observado", "me haces sentir acompañado",
"me haces sentir solo", "me haces sentir raro",
"tu presencia incomoda", "tu presencia tranquiliza",
"tu presencia intimida", "tu presencia relaja",
"tu voz molesta", "tu voz calma", "tu mirada asusta",
"tu mirada tranquiliza", "tu actitud apesta", "tu actitud encanta",
"tu energía es horrible", "tu energía es increíble",
"tu personalidad abruma", "tu personalidad enamora",
"tu comportamiento da miedo", "tu comportamiento preocupa",
"tu forma de hablar irrita", "tu forma de hablar encanta", "angelical", "angelicales", "apestado", "apestada", "abominable", "abrumador", "abrumadora",
"absorbente", "abusón", "abusona", "académico", "académica", "aceptable", "ácido", "ácida",
"acomplejado", "acomplejada", "acogedor", "acogedora", "acosador", "acosadora", "activo",
"activa", "adorado", "adorada", "afectuoso", "afectuosa", "afortunado", "afortunada",
"agitado", "agitada", "agonizante", "agradable", "agrio", "agria", "aguafiestas", "agudo",
"aguda", "alborotador", "alborotadora", "alegre", "alérgico", "alérgica", "alienado",
"alienada", "altanero", "altanera", "alucinante", "amargado", "amargada", "ameno", "amena",
"amigable", "amistoso", "amistosa", "amorfo", "amorfa", "analfabeto", "analfabeta",
"analítico", "analítica", "anárquico", "anárquica", "anciano", "anciana", "animado",
"animada", "anormal", "ansioso", "ansiosa", "anticuado", "anticuada", "antisocial",
"apagado", "apagada", "apasionado", "apasionada", "apestoso", "apestosa", "apocado",
"apocada", "apuesto", "apuesta", "arisco", "arisca", "arrogante", "artístico", "artística",
"asocial", "asombroso", "asombrosa", "áspero", "áspera", "asqueroso", "asquerosa",
"astuto", "astuta", "atlético", "atlética", "atractivo", "atractiva", "atroz", "audaz",
"auténtico", "auténtica", "avaricioso", "avariciosa", "aventurero", "aventurera", "avispado",
"avispada", "baboso", "babosa", "bacán", "barato", "barata", "barbudo", "barbuda",
"bárbaro", "bárbara", "basto", "basta", "bellaco", "bellaca", "bello", "bella", "berrinchudo",
"berrinchuda", "bestia", "bestial", "bipolar", "bizco", "bizca", "blandengue", "blando",
"blanda", "bochornoso", "bochornosa", "bonachón", "bonachona", "bonito", "bonita",
"borracho", "borracha", "bravo", "brava", "brillante", "brusco", "brusca", "brutal",
"bueno", "buena", "burlón", "burlona", "caballeroso", "caballerosa", "cabizbajo",
"cabizbaja", "cabezota", "calculador", "calculadora", "caliente", "calmado", "calmada",
"cansado", "cansada", "cansino", "cansina", "caprichoso", "caprichosa", "carismático",
"carismática", "cariñoso", "cariñosa", "carnicero", "carnicera", "castroso", "castrosa",
"cateto", "cateta", "caótico", "caótica", "capaz", "celestial", "celoso", "celosa",
"cenizo", "ceniza", "cerdo", "cerda", "chiflado", "chiflada", "chillón", "chillona",
"chismoso", "chismosa", "chulo", "chula", "cínico", "cínica", "civilizado", "civilizada",
"clásico", "clásica", "cochino", "cochina", "colérico", "colérica", "comelón", "comelona",
"cómico", "cómica", "compasivo", "compasiva", "competente", "complejo", "compleja",
"complicado", "complicada", "comprensivo", "comprensiva", "común", "condenado",
"condenada", "conflictivo", "conflictiva", "conformista", "confundido", "confundida",
"conservador", "conservadora", "considerado", "considerada", "constante", "contento",
"contenta", "controlador", "controladora", "cool", "correcto", "correcta", "corrupto",
"corrupta", "cortante", "cotilla", "crédulo", "crédula", "creativo", "creativa",
"creepy", "cringe", "crítico", "crítica", "cruel", "cuadrado", "cuadrada", "culto",
"culta", "curioso", "curiosa", "cutre", "cálido", "cálida", "débil", "decente",
"decidido", "decidida", "decoroso", "decorosa", "deforme", "delgado", "delgada",
"delicioso", "deliciosa", "delirante", "demencial", "demoniaco", "demoniaca", "dependiente",
"deplorable", "depresivo", "depresiva", "desagradable", "desalmado", "desalmada",
"descarado", "descarada", "desconfiado", "desconfiada", "desconsiderado", "desconsiderada",
"descontrolado", "descontrolada", "descortés", "descuidado", "descuidada", "desesperado",
"desesperada", "desgraciado", "desgraciada", "deshonesto", "deshonesta", "desleal",
"deslumbrante", "desordenado", "desordenada", "despiadado", "despiadada", "despreciable",
"desquiciado", "desquiciada", "desternillante", "desubicado", "desubicada", "detallista",
"devastador", "devastadora", "diabólico", "diabólica", "difícil", "digno", "digna",
"discreto", "discreta", "distraído", "distraída", "divertido", "divertida", "divino",
"divina", "dominante", "dormilón", "dormilona", "dramático", "dramática", "duro", "dura",
"educado", "educada", "eficiente", "egoísta", "elegante", "elitista", "embaucador",
"embaucadora", "emocionante", "empalagoso", "empalagosa", "empático", "empática",
"empollón", "empollona", "encantador", "encantadora", "encantado", "encantada",
"enérgico", "enérgica", "enfadica", "enfadico", "engañoso", "engañosa", "engreído",
"engreída", "enigmático", "enigmática", "enorme", "envidioso", "envidiosa", "épico",
"épica", "equilibrado", "equilibrada", "errático", "errática", "escandaloso", "escandalosa",
"escéptico", "escéptica", "esforzado", "esforzada", "esnob", "espabilado", "espabilada",
"especial", "espeluznante", "espontáneo", "espontánea", "espléndido", "espléndida",
"espiritual", "espumoso", "espumosa", "estable", "estafador", "estafadora", "estiloso",
"estilosa", "estirado", "estirada", "estresante", "estresado", "estresada", "estúpido",
"estúpida", "etéreo", "etérea", "eufórico", "eufórica", "exagerado", "exagerada",
"excelente", "excéntrico", "excéntrica", "excitante", "exigente", "exótico", "exótica",
"explosivo", "explosiva", "extraño", "extraña", "extravagante", "extrovertido",
"extrovertida", "fabuloso", "fabulosa", "facha", "fácil", "falso", "falsa", "fanático",
"fanática", "fantástico", "fantástica", "fantasioso", "fantasiosa", "fastidioso",
"fastidiosa", "fatal", "feo", "fea", "feroz", "fiel", "fiestero", "fiestera", "fino",
"fina", "firme", "flipado", "flipada", "flojo", "floja", "formal", "formidable",
"forzudo", "forzuda", "fracasado", "fracasada", "frágil", "franco", "franca", "friki",
"frío", "fría", "frustrante", "fumado", "fumada", "furioso", "furiosa", "futurista",
"galante", "gamberro", "gamberra", "ganador", "ganadora", "genial", "genio", "gigante",
"gilipollas", "glorioso", "gloriosa", "gordo", "gorda", "grosero", "grosera", "gruñón",
"gruñona", "guapo", "guapa", "guarro", "guarra", "hablador", "habladora", "habilidoso",
"habilidosa", "hambriento", "hambrienta", "hediondo", "hedionda", "hermoso", "hermosa",
"heroico", "heroica", "hipócrita", "histérico", "histérica", "honesto", "honesta",
"horrendo", "horrenda", "horrible", "hostil", "humilde", "humillante", "idealista",
"idiota", "ignorante", "ilegal", "ilógico", "ilógica", "imbécil", "impaciente",
"impaciente", "imperdonable", "imponente", "impresionante", "impulsivo", "impulsiva",
"inadaptado", "inadaptada", "incómodo", "incómoda", "incompetente", "increíble",
"indeciso", "indecisa", "independiente", "indiferente", "indigno", "indigna",
"indisciplinado", "indisciplinada", "indomable", "ineficiente", "infantil", "infernal",
"inferior", "influyente", "ingenioso", "ingeniosa", "inhumano", "inhumana", "inmaduro",
"inmadura", "inofensivo", "inofensiva", "inquietante", "inseguro", "insegura",
"insensible", "insistente", "insolente", "inteligente", "intenso", "intensa",
"interesante", "introvertido", "introvertida", "inútil", "irónico", "irónica",
"irrespetuoso", "irrespetuosa", "irritable", "jodido", "jodida", "jovial", "joven",
"juguetón", "juguetona", "justo", "justa", "lamentable", "largo", "larga", "legendario",
"legendaria", "leal", "lento", "lenta", "libre", "limpio", "limpia", "lindo", "linda",
"listillo", "listilla", "listo", "lista", "llorón", "llorona", "loco", "loca", "lógico",
"lógica", "luminoso", "luminosa", "lunático", "lunática", "macabro", "macabra",
"maduro", "madura", "mágico", "mágica", "magnífico", "magnífica", "majestuoso",
"majestuosa", "malcriado", "malcriada", "maldito", "maldita", "maleducado",
"maleducada", "malévolo", "malévola", "malhumorado", "malhumorada", "malicioso",
"maliciosa", "mandón", "mandona", "manipulador", "manipuladora", "maravilloso",
"maravillosa", "masivo", "masiva", "mediocre", "melancólico", "melancólica",
"mentiroso", "mentirosa", "mezquino", "mezquina", "miedoso", "miedosa", "milagroso",
"milagrosa", "mimado", "mimada", "miserable", "místico", "mística", "moderno",
"moderna", "molesto", "molesta", "mono", "mona", "monstruoso", "monstruosa",
"morboso", "morbosa", "motivado", "motivada", "mudo", "muda", "mugriento", "mugrienta",
"mágnetico", "mágnetica", "narcisista", "nauseabundo", "nauseabunda", "negativo",
"negativa", "nervioso", "nerviosa", "neutral", "noble", "normal", "novato", "novata",
"obsesivo", "obsesiva", "odioso", "odiosa", "ofensivo", "ofensiva", "optimista",
"ordinario", "ordinaria", "orgulloso", "orgullosa", "oscuro", "oscura", "oso", "osa",
"paciente", "palurdo", "palurda", "paranoico", "paranoica", "parlanchín", "parlanchina",
"patético", "patética", "payaso", "payasa", "pedorro", "pedorra", "peligroso",
"peligrosa", "pelmazo", "pelmaza", "pensativo", "pensativa", "pequeño", "pequeña",
"perfecto", "perfecta", "perezoso", "perezosa", "persistente", "pesado", "pesada",
"pesimista", "picante", "pícaro", "pícara", "pijo", "pija", "pirado", "pirada",
"placentero", "placentera", "plasta", "poderoso", "poderosa", "poético", "poética",
"positivo", "positiva", "potente", "precioso", "preciosa", "presumido", "presumida",
"primitivo", "primitiva", "problemático", "problemática", "profesional", "profundo",
"profunda", "prolijo", "prolija", "prudente", "psicópata", "pulcro", "pulcra",
"puntilloso", "puntillosa", "querido", "querida", "quisquilloso", "quisquillosa",
"radiante", "radical", "raro", "rara", "racional", "realista", "rebelde", "refinado",
"refinada", "relajado", "relajada", "repelente", "repugnante", "respetuoso",
"respetuosa", "responsable", "retorcido", "retorcida", "ridículo", "ridícula",
"rígido", "rígida", "robusto", "robusta", "romántico", "romántica", "rudo", "ruda",
"ruidoso", "ruidosa", "ruin", "sabio", "sabia", "sabelotodo", "sabroso", "sabrosa",
"sádico", "sádica", "salvaje", "sanguinario", "sanguinaria", "sarcástico",
"sarcástica", "seductor", "seductora", "seguro", "segura", "sensato", "sensata",
"sensible", "sensual", "sereno", "serena", "serio", "seria", "severo", "severa",
"sexi", "silencioso", "silenciosa", "simpático", "simpática", "simple", "sincero",
"sincera", "siniestro", "siniestra", "sobrio", "sobria", "sociable", "sofisticado",
"sofisticada", "soñador", "soñadora", "soso", "sosa", "suave", "sucio", "sucia",
"sufrido", "sufrida", "superficial", "superior", "suspicaz", "talentoso", "talentosa",
"tacaño", "tacaña", "temperamental", "tenaz", "tenso", "tensa", "terco", "terca",
"terrible", "terrorífico", "terrorífica", "testarudo", "testaruda", "tierno", "tierna",
"tímido", "tímida", "tonto", "tonta", "torpe", "tóxico", "tóxica", "trabajador",
"trabajadora", "tramposo", "tramposa", "tranquilo", "tranquila", "traicionero",
"traicionera", "travieso", "traviesa", "triste", "triunfador", "triunfadora",
"turulato", "turulata", "único", "única", "vacilón", "vacilona", "vacío", "vacía",
"vago", "vaga", "valiente", "valioso", "valiosa", "vandalico", "vandálica", "vanidoso",
"vanidosa", "veloz", "vengativo", "vengativa", "vergonzoso", "vergonzosa", "violento",
"violenta", "virtuoso", "virtuosa", "visionario", "visionaria", "vital", "vividor",
"vividora", "vulgar", "vulnerable", "zafio", "zafia", "zalamero", "zalamera", "zombi"
    };

    [Header("Efectos Fase 3 (Corrupción)")]
    public AudioClip zumbidoClip;
    public AudioClip transicionClip;
    public AudioClip fondoCorruptoClip;
    public RandomNodeSoundAction[] sonidosRandomCorruptos;

    [Header("Animaciones Fase 3 (Sprites)")]
    public Sprite[] transformSprites;
    public Sprite[] corruptedIdleSprites;
    public Sprite[] corruptedBlinkSprites;
    public Sprite[] corruptedTalkingSprites;

    [Header("Velocidades Fase 3")]
    [Tooltip("Tiempo total (en segundos) que tarda en pasar todos los sprites de transformación.")]
    public float transformDuration = 1.5f;
    [Tooltip("Nueva velocidad de parpadeo/habla del gato una vez ya está corrupto.")]
    public float corruptedAnimationSpeed = 0.05f;

    [Header("Velocidades Fase 4")]
    [Tooltip("Velocidad de animación (parpadeo/habla) del gato hostil en la Fase 4.")]
    public float stage4AnimationSpeed = 0.03f;

    [Header("Datos Guardados de la Historia")]
    public string respuestaIdentidad = ""; // Here we will store the player's answer about the cat's identity, which will influence the final outcome.

    [Header("Fase 4 (Final)")]
    [Tooltip("Lista de lugares o passwords correctos para terminar la demo.")] // All the passwords for the stage 4. These adjetives was wrote by the designer.
    public string[] passwordsUbicacionFase4 = new string[] { "estamos en udit", "estoy en udit", "estoi en udit", "estoy en UDIT", "estoy en la udit", "estamos en la udit", "somos en udit", "somos en la udit",
"esto es udit", "esto es UDIT", "esto es la udit", "esto es universidad", "esto es la universidad", "esto es la uni", "es udit", "es UDIT", "es la udit", "es la universidad",
"estoy en la match in games", "estamos en la match in games", "estoi en la match in games", "estamos en match in games", "estoy en match in games", "en la match in games", "en match in games", "match in games", "MATCH IN GAMES", "match ing games", "matchin games", "matching games", "matchin games",
"estoy en la feria", "estamos en la feria", "estoi en la feria", "en la feria gaming", "feria gaming", "feria de videojuegos", "feria de videogames", "evento gaming", "evento de videojuegos",
"estoy en udit en la match in games", "estamos en udit en la match in games", "estoi en udit en la match in games", "estoy en udit feria", "estamos en udit feria", "udit feria gaming", "udit feria", "udit match in games", "udit matching games", "udit matchin games", "udit matchingames", "udit match ing games",
"estoy en la uni", "estamos en la uni", "estoi en la uni", "la uni", "uni", "universidad", "universidad de diseño", "universidad de diseño y tecnología", "UDIT universidad", "udit universidad",
"toy en udit", "toy en la udit", "ando en udit", "ando en la udit", "ando en la match", "toy en la match", "toy en match in games", "ando en match in games", "toy en la feria", "ando en la feria",
"estamos en udi", "estoy en udi", "udid", "udit.", "u d i t", "u-d-i-t",
"estamos en la match", "estoy en la match", "match", "la match", "la matching", "matching games", "match games", "matchgame", "match game" };

    [Tooltip("El GameEvent que se lanzará al adivinar la palabra")]
    public GameEvent eventoFinalDemo;

    public enum NPCType { None, CatStage1, CatStage2, CatStage3, CatStage4 } // Enum to select the NPC type and its corresponding AI behavior.

    [SerializeField]
    private NPCType selectedType = NPCType.None; // This variable will determine which AI script is applied to the NPC. It can be set in the Inspector or changed at runtime.

    public NPCType CurrentType // Property to get/set the current NPC type. When set, it will apply the corresponding AI script if it's different from the current one or if no AI is currently applied.
    {
        get { return selectedType; } // Getter to return the currently selected NPC type.

        set // Setter to change the NPC type. It checks if the new value is different from the current one or if there is no AI applied. If so, it updates the selectedType and applies the corresponding AI script.
        {
            if (selectedType != value || currentAI == null)
            {
                selectedType = value;
                if (selectedType != NPCType.None)
                    ApplyAI();
            }
        }
    }

    private BaseAIScript currentAI;

    void Awake() { InitCheck(); }
    void OnEnable() { InitCheck(); }
    void Start() { InitCheck(); }

    void Update()
    {
        if (moveAppData != null && moveAppData.playerIsFrontCat && selectedType == NPCType.None)
        {
            CurrentType = NPCType.CatStage1;
        }
    }

    private void InitCheck() // This method checks if the MoveAppData reference is assigned and if the player is in front of the cat. If so, it sets the NPC type to CatStage1. It also checks if there is no AI applied and a type is selected, in which case it applies the corresponding AI script.
    {
        if (moveAppData == null)
        {
            GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
            if (goMoveAppData != null) moveAppData = goMoveAppData.GetComponent<MoveAppData>();
        }

        if (moveAppData != null && moveAppData.playerIsFrontCat && selectedType == NPCType.None)
        {
            CurrentType = NPCType.CatStage1;
        }

        else if (currentAI == null && selectedType != NPCType.None)
        {
            ApplyAI();
        }
    }

    private void ApplyAI() // This method applies the corresponding AI script to the NPC based on the selectedType. It first destroys any existing AI script, then adds the new one and sets up its references and properties according to the NPCProfile and other settings defined in this class.
    {
        if (currentAI != null) Destroy(currentAI);

        switch (selectedType)
        {
            case NPCType.CatStage1:
                var c1 = gameObject.AddComponent<CatAIScript_Stage1>();
                SetupAIReferences(c1, catStage1Profile);
                currentAI = c1;
                break;

            case NPCType.CatStage2:
                var c2 = gameObject.AddComponent<CatAIScript_Stage2>();
                SetupAIReferences(c2, catStage2Profile);
                c2.isProactiveTriggered = true;
                c2.adjetivosDeSeguridad = adjetivosDeSeguridadFase2;
                currentAI = c2;
                break;

            case NPCType.CatStage3:
                var c3 = gameObject.AddComponent<CatAIScript_Stage3>();
                SetupAIReferences(c3, catStage3Profile);
                c3.isProactiveTriggered = true;

                c3.zumbidoClip = zumbidoClip;
                c3.transicionClip = transicionClip;
                c3.fondoCorruptoClip = fondoCorruptoClip;
                c3.sonidosRandomCorruptos = sonidosRandomCorruptos;

                c3.transformSprites = transformSprites;
                c3.corruptedIdleSprites = corruptedIdleSprites;
                c3.corruptedBlinkSprites = corruptedBlinkSprites;
                c3.corruptedTalkingSprites = corruptedTalkingSprites;

                c3.transformDuration = transformDuration;
                c3.corruptedAnimationSpeed = corruptedAnimationSpeed;

                c3.IniciarEfectos();

                currentAI = c3;
                break;

            case NPCType.CatStage4:
                var c4 = gameObject.AddComponent<CatAIScript_Stage4>();
                SetupAIReferences(c4, catStage4Profile);
                c4.isProactiveTriggered = true;

                c4.passwordsUbicacion = passwordsUbicacionFase4;
                c4.respuestaIdentidadJugador = respuestaIdentidad;

                c4.eventoFinalDemo = eventoFinalDemo;

                currentAI = c4;
                break;
        }
    }

    private void SetupAIReferences(BaseAIScript newAI, NPCProfile profile) // This method sets up the references and properties of the newly added AI script based on the provided NPCProfile and other settings. It loads the profile into the AI, assigns the visual controller, and tries to find the necessary UI components from the DesktopManager's open windows. It also includes fallbacks to find the OllamaClient and StoryLog if they were not assigned from the windows.
    {
        if (profile != null) newAI.LoadProfile(profile);

        newAI.visualController = GetComponentInChildren<NPCVisualController>(true);

        if (selectedType == NPCType.CatStage1 && newAI.visualController != null)
        {
            newAI.visualController.RestoreDefaultSprites();
        }

        DesktopManager dm = FindObjectOfType<DesktopManager>();
        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if ((data.label.Contains("FindKey") || data.label.Contains("Move")) && data.isOpen && data.windowInstance != null)
                {
                    AI_References refs = data.windowInstance.GetComponentInChildren<AI_References>(true);
                    if (refs != null)
                    {
                        newAI.inputField = refs.inputField;
                        newAI.chatOutput = refs.chatOutput;
                        newAI.ollamaClient = refs.ollamaClient;
                        newAI.storyLog = refs.storyLog;
                        newAI.thinkingPanel = refs.thinkingPanel;
                    }
                    break;
                }
            }
        }

        // Fallbacks just in case
        if (newAI.ollamaClient == null) newAI.ollamaClient = FindObjectOfType<OllamaClient>(true);
        if (newAI.storyLog == null) newAI.storyLog = FindObjectOfType<StoryLog>(true);
    }

    public void ResetNPC() // This method resets the NPC to its initial state. It looks for the "Buscador Enemigos" or "Enemy Encounter" window in the DesktopManager and activates the nonEnemyFindedPanel if it finds it. Then it destroys any existing AI script, sets the currentAI reference to null, and resets the selectedType to None.
    {
        DesktopManager dm = FindObjectOfType<DesktopManager>();
    
        if (dm != null && dm.iconsToSpawn != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if ((data.label == "Buscador Enemigos" || data.label == "Enemy Encounter") && data.isOpen && data.windowInstance != null)
                {
                    BaseEnemyEncounter baseEnemyEncounter = data.windowInstance.GetComponent<BaseEnemyEncounter>();
                    if (baseEnemyEncounter != null) baseEnemyEncounter.nonEnemyFindedPanel.SetActive(true);
                    break;
                }
            }
        }
    
        if (currentAI != null) Destroy(currentAI);
        currentAI = null;
        selectedType = NPCType.None;
    }
}

/// <summary>
/// Class: NPCProfile
/// Description: This class is a serializable data structure that holds the profile information for an NPC (Non-Player Character). It includes fields for the NPC's name, an optional
///              password (if the NPC is not peaceful), a personality prompt to guide the AI's behavior, a first message that the NPC will say when interacted with, and system instructions that
///              can be used to provide additional context or rules for the AI. This profile can be loaded into different AI scripts to create varied NPC behaviors based on the same underlying data.
/// </summary>
[Serializable]
public class NPCProfile
{
    public string npcName;
    [Tooltip("Déjalo vacío si es pacífico")]
    public string password;

    [TextArea(4, 10)]
    public string personalityPrompt;

    [TextArea(2, 5)]
    public string firstMessage;

    [TextArea(4, 10)]
    public string systemInstruction;
}