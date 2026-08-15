# Plan de desarrollo — TP1 Camiones (adaptación móvil)

> Documento vivo. Cada sesión debe leerlo al empezar y actualizarlo al terminar.
> El plan maestro original (con todo el contexto, decisiones y hallazgos iniciales)
> vive fuera del repo en `C:\Users\Ian\.claude\plans\create-a-plan-to-tranquil-sifakis.md`.
> Este documento es el resumen ejecutable y la fuente de verdad **dentro** del repo.

**Fecha límite de la cátedra (PDF):** 03/10/2025 23:59hs — ya pasada.
**Fecha límite real:** 2026-08-15 (confirmada por el usuario, Sesión 3).

---

## Estado actual

**Etapa 0 cerrada y verificada.** El proyecto corre en Unity 6000.0.74f1,
tiene Input System instalado (en modo "Both", conviviendo con el Input
Manager legacy), TMP Essential Resources importado, y `Assets/Editor/BuildTools.cs`
como única herramienta de editor permanente. El ciclo de verificación headless
(`-batchmode -executeMethod BuildTools.CompileCheck`) está probado y funciona:
exit code 0, sin `error CS` en el log.

**Etapa 1 (código) cerrada; verificación en Play Mode pendiente.** Se
trabajaron los 26 ítems del Apéndice A (ver commit `221940e` y Sesión 2 en
`SESSION_LOG.md`) más la limpieza de código muerto y el cacheo de
`GetComponent`. `BuildTools.CompileCheck` headless: exit code 0, sin
`error CS` en el log, corrido dos veces (antes y después de matar el
SmokeTest fallido de esta sesión).

`BuildTools.SmokeTest` **sigue sin completarse, esta vez por una causa
distinta (Sesión 3, 2026-07-27):** se reintentó desde cero. El proceso pasó
el punto exacto donde se había colgado la Sesión 2 (`TrimDiskCacheJob:
Current cache size 6554mb`) y avanzó más: logueó
`SmokeTest: abriendo 'Assets/ESCENAS/conduccion9.unity'`, e indicios de que
sí llegó a tickear en Play Mode (varias líneas repetidas de
`Scanning for USB devices`, que no aparecen una sola vez sino varias,
consistente con múltiples pasadas del loop de `EditorApplication.update`).
El uso de CPU se mantuvo alto y multi-hilo de forma sostenida durante todo
el proceso (confirmado con muestras de `UserModeTime`/`KernelModeTime` cada
pocos minutos) — es decir, nunca estuvo realmente congelado. Sin embargo,
**a los ~79 minutos el proceso de shell en segundo plano que lo envolvía fue
terminado por la infraestructura de este entorno (no por decisión del
usuario ni por un `taskkill` manual)**, y como `Unity.exe` era un proceso
hijo de ese shell, se cayó con él. El log terminó en la línea 462 sin
`SmokeTest: OK` ni `SmokeTest: FALLO` — no hay veredicto de pass/fail, pero
tampoco apareció ningún `error CS`/`Exception`/`NullReference` nuevo en las
462 líneas (solo el error de licencia ya documentado, no bloqueante). Se
confirmó que no quedaba ningún `Unity.exe` corriendo (`tasklist`) y se borró
el `Temp/UnityLockfile` huérfano.

Esto cambia el diagnóstico: el riesgo ya no parece ser un cuelgue real del
`TrimDiskCacheJob` (esta vez sí lo cruzó), sino que **esta escena
(`conduccion9.unity`) headless tarda más de ~79 minutos en terminar los 600
frames de `SmokeTest`** — mucho más que lo esperable — y ese tiempo choca
con un límite de duración de este entorno de ejecución que corta procesos
en segundo plano antes de que termine.

**Dos experimentos adicionales en la misma Sesión 3 descartaron las dos
hipótesis más obvias:**
1. Se bajó `FramesPorEscena` de 600 a 60 (temporalmente, ya revertido) y
   se reintentó: el proceso se clavó en el mismo punto exacto
   (`TrimDiskCacheJob`) con la misma curva de CPU alta y sostenida que con
   600 frames. **Conclusión: el tiempo no depende de la cantidad de
   frames** — el costo es fijo, no por-frame, así que reducir el budget de
   `SmokeTest` no sirve para acortar esta espera.
2. Se limpió a mano el caché de GI (`%LOCALAPPDATA%\Unity\Caches\GiCache`,
   6.5 GB → 0 MB, confirmado por el propio log:
   `TrimDiskCacheJob: Current cache size 0mb`) y se reintentó: **igual se
   clavó en el mismo punto**, con la misma curva de CPU. Conclusión: el
   `TrimDiskCacheJob` en sí no es lo que tarda — es solo la última línea
   que se loguea antes de que arranque otra cosa (todavía no identificada)
   que consume CPU multi-hilo sostenido durante varios minutos u horas sin
   loguear nada.
3. Se revisó `Settings.lighting` embebido en `conduccion9.unity`:
   `m_BakeOnSceneLoad: 0` (no debería re-bakear lightmaps solo al abrir la
   escena), `m_EnableRealtimeLightmaps: 0`, `m_EnableBakedLightmaps: 1`,
   `m_BakeBackend: 1` (Progressive CPU). No se encontró evidencia directa
   de auto-bake en el log (no aparece ninguna línea con "Bak" para
   Lightmap/Burst/IL2CPP en ninguno de los tres logs generados esta
   sesión), pero tampoco se pudo descartar del todo sin ver la UI del
   Editor (el log headless no muestra la barra de progreso de bakeo).

**No identificado todavía:** qué es exactamente lo que consume ese tiempo.
Candidatos no descartados: compilación nativa de Burst la primera vez que
se ejecuta un job (no debería depender del tamaño de frames si ocurre una
sola vez al entrar en Play Mode), algo del sistema de iluminación/GI que
no pasa por el log estándar, u otra cosa específica de esta escena. Pendiente
para la próxima sesión: decidir con el usuario si conviene (a) dejar correr
un intento sin interrumpir más allá de los ~80 min para confirmar si
eventualmente termina, (b) abrir el proyecto en el Editor gráfico
normalmente y observar qué barra de progreso/actividad aparece justo
después de abrir `conduccion9.unity` (esto revelaría en segundos lo que el
log headless no muestra), o (c) aceptar que la verificación de Play Mode
para esta etapa se haga a mano en el Editor gráfico en vez de por
`SmokeTest` headless, dejando `SmokeTest` como herramienta para más
adelante si se resuelve la causa.

Como el `SmokeTest` no dio un veredicto, tampoco se probó manualmente una
partida de 2 jugadores por teclado — eso también queda para la próxima
sesión, junto con la tarea pendiente en `TAREAS_EDITOR.md` (cablear
`Contr1`/`Contr2` en las 4 instancias de `Deposito2`, que ahora son campos
que hay que asignar a mano en vez de resolverse con `GameObject.Find`,
todavía sin hacer — confirmado leyendo `conduccion9.unity`, las 4 instancias
siguen en `{fileID: 0}`).

## Decisiones ya tomadas (no volver a discutir sin razón nueva)

| Tema | Decisión |
|---|---|
| Modo un jugador | **Solo contrarreloj** — cámara completa, un camión, mejor puntaje guardado. Sin IA. |
| Tutorial | **Dos etapas separadas**, como el original: descarga (mini-juego de pallets) y conducción (manejar hasta el trigger). En 2P ambos jugadores deben terminar antes de empezar la carrera. |
| Módulo Android | Se instala vía Unity Hub headless CLI (Etapa 7). |
| Alcance visual | **Solo UI, mínimo.** Menús y HUD prolijos y consistentes. La escena 3D, iluminación y materiales quedan como están. Sin trabajo de audio. |
| Versión de Unity | **6000.0.74f1** (ya instalada). Proyecto estaba en 6000.0.56f1. |
| Idioma del código | Se mantiene el vocabulario en español del proyecto original (`Camion`, `Bolsa`, `Pallet`, `Deposito`, `Dificultad`). Convenciones C# estándar en lo demás. |
| Herramientas de editor | Un solo archivo permanente: `Assets/Editor/BuildTools.cs`. Scripts de migración puntual van en `Assets/Editor/_Migraciones/` y se borran en la misma etapa que se usan. |
| Autoría de escena/UI | El agente escribe el código runtime con slots `[SerializeField]`; el usuario arma jerarquías de Canvas y prefabs visuales en el Editor siguiendo `TAREAS_EDITOR.md`. |

## Etapas

| # | Etapa | Estado | Sesión | Fecha |
|---|---|---|---|---|
| 0 | Infraestructura y baseline | ✅ | 1 | 2026-07-26 |
| 1 | Delegalizar y estabilizar | 🟡 | 2 | 2026-07-27 |
| 2 | Sistemas núcleo y patrones | ⬜ | — | — |
| 3 | Input: teclado/gamepad/táctil | ⬜ | — | — |
| 4 | Reestructura de escenas + carga async | ⬜ | — | — |
| 5 | UI (uGUI + TMP) y adaptación de resolución | ⬜ | — | — |
| 6 | Dificultad + carga/descarga runtime de prefabs | ⬜ | — | — |
| 7 | Rendimiento móvil + build Android | ⬜ | — | — |
| 8 | Entregables y QA final | ⬜ | — | — |

Leyenda: ⬜ no empezada · 🟡 en curso · ✅ verificada y cerrada.

## Hallazgos

*(Registro de solo-lectura de descubrimientos que cambian el plan. Cada entrada
con fecha y etapa. No editar entradas viejas — agregar nuevas.)*

- **2026-07-26 (Etapa 0, exploración previa a este documento):** El proyecto NO
  era un repositorio git. La carpeta de Unity está anidada un nivel:
  `MobileDevTP1-Public-master/MobileDevTP1-Public-master/`. Ya existe un
  `.gitignore` de Unity correcto, salvo que le faltaba `UserSettings/`
  (agregado antes del commit inicial).
- **2026-07-26:** No hay ningún editor de Unity con el módulo de Android Build
  Support instalado (6000.0.33f1, 6000.0.74f1, 6000.3.3f1, 6000.3.11f1 — todos
  solo con `windowsstandalonesupport`). Se instalará en la Etapa 7 vía Hub CLI.
- **2026-07-26:** Lista completa de defectos verificados en el código original
  (26 ítems) está en el Apéndice A del plan maestro. Los más críticos:
  - `GameManager.cs:525` llama a `CambiarACarrera()` donde el código comentado
    indica que debía ser `CambiarATutorial()` — **por esto el tutorial de
    conducción nunca aparece.**
  - `Bolsa.cs:19` pisa el valor `Monto` asignado en el inspector con
    `Valores.Valor2` — **las 68 bolsas de la escena valen todas lo mismo.**
  - `Application.LoadLevel(3)` referenciado en 3 scripts apunta a un build
    index que no existe (solo hay 2 escenas).
- **2026-07-26 (Etapa 0, verificación de `BuildTools.SmokeTest`):** correr
  `SmokeTest` contra las escenas tal cual están hoy es impracticable: el
  `print("crudo")` por-frame de `Pallet.cs:54` (Apéndice A ítem 14) generó
  ~1 GB de log en menos de 5 minutos en modo headless y hubo que matar el
  proceso a mano. La Etapa 1 debe eliminar ese y los demás `print`/`Debug.Log`
  por-frame **antes** de intentar correr un SmokeTest completo; no vale la
  pena intentarlo de nuevo hasta entonces.
- **2026-07-26:** Al correr Unity headless aparece en el log la línea
  `[Licensing::Module] Error: Access token is unavailable; failed to update`.
  No impidió que CompileCheck ni el import de TMP terminaran con éxito
  (exit code 0 en ambos) — parece ser solo el intento de refresco de token
  de licencia online, no bloqueante mientras haya una licencia ya activada
  en la máquina. Si algún build headless futuro falla por licencia, empezar
  a investigar por acá.
- **2026-07-27 (Etapa 1):** Varios scripts del Apéndice A resultaron estar
  completamente huérfanos (no referenciados por ningún GameObject en
  ninguna escena/prefab, verificado buscando su GUID): `VidIntrMgr.cs`
  (pertenecía a una escena de video-intro que ya no existe),
  `JuegoEscMgr.cs` (mismo caso) y `AcelerAuto.cs` (su único llamador
  vivía comentado en `ReductorVelColl.cs`). Se borraron en vez de
  parchear el bug que pedía el Apéndice A en cada uno, porque parchear
  código inalcanzable no cambia el comportamiento del juego y deja el
  proyecto más difícil de defender. Confirmado con
  `grep` del GUID del `.meta` contra `Assets/ESCENAS/*.unity` y los
  prefabs relevantes antes de borrar cada uno.
- **2026-07-27 (Etapa 1):** Corriendo `BuildTools.SmokeTest` headless por
  primera vez tras la Etapa 1, el proceso quedó ~45 minutos sin avanzar
  justo después de abrir `conduccion9.unity` (primera vez que esa escena
  se carga bajo 6000.0.74f1) y de loguear
  `TrimDiskCacheJob: Current cache size 6554mb`. El uso de CPU mostraba
  un solo hilo ocupado de forma sostenida (no una explosión de log ni un
  proceso caído/deadlockeado sin actividad) — consistente con una limpieza
  de caché de GI de Unity, no con el código cambiado esta sesión (nunca
  llegó a entrar en Play Mode). Se mató el proceso a mano. Pendiente
  confirmar en la próxima sesión si vuelve a pasar con el caché ya limpio.
- **2026-07-27 (Etapa 1, Sesión 3):** Se reintentó `SmokeTest` y esta vez
  sí cruzó el punto del `TrimDiskCacheJob` y llegó a abrir la escena y dar
  señales de estar tickeando en Play Mode (`Scanning for USB devices`
  repetido varias veces). CPU alto y multi-hilo sostenido durante ~79
  minutos seguidos (muestreado con `wmic ... get UserModeTime,
  KernelModeTime` cada pocos minutos) — nunca se vio una señal de cuelgue
  real. A los ~79 minutos el proceso fue terminado por un límite de
  duración del entorno de ejecución en segundo plano (no por
  `taskkill` manual ni por decisión explícita), sin que `SmokeTest`
  llegara a loguear `OK` ni `FALLO`. Conclusión: el cuelgue de la Sesión 2
  y este caso probablemente **no son el mismo fenómeno** — esta escena
  parece tardar de verdad más de ~79 minutos en completar 600 frames
  headless sin GPU (`-nographics`), lo cual excede los límites prácticos
  de este flujo de verificación tal como está planteado.

## Decisiones

*(Decisiones tomadas durante la ejecución que no estaban en el plan original,
con su razón. Cualquier cosa que contradiga el plan maestro va acá Y se le
avisa al usuario explícitamente.)*

- **2026-07-26:** El proyecto está en modo Auto (el usuario supervisa pero deja
  avanzar a los agentes). Se procede con la Etapa 0 sin bloquear en la fecha
  límite real; queda pendiente que el usuario la complete arriba.
- **2026-07-27 (Etapa 1):** El Apéndice A pedía reemplazar
  `GameObject.Find("GameMgr")` en `ContrCalibracion.cs`/`ContrTutorial.cs`
  por referencias `[SerializeField]`. En cambio se usó
  `GameManager.Instancia` (el singleton estático que `GameManager.Awake()`
  ya setea, y que `TaxiComp.cs` ya usaba para lo mismo) — mismo resultado,
  cero trabajo de Editor para el usuario, y más consistente con el propio
  código del proyecto. Para `Deposito2.Contr1`/`Contr2` sí se siguió la
  recomendación literal del plan (son 2 instancias por jugador, no un
  singleton): se sacó el `GameObject.Find`, los campos quedan `public` para
  cablear a mano — tarea agregada a `TAREAS_EDITOR.md`.
- **2026-07-27 (Etapa 1):** `BuildTools.SmokeTest` no se corrió hasta el
  final esta sesión (ver Hallazgos). Se decidió no bloquear el cierre de la
  Etapa 1 en el código por esto — `CompileCheck` pasa limpio dos veces y el
  trabajo de código está completo — pero la Etapa 1 queda marcada 🟡 (no ✅)
  hasta correr el SmokeTest completo y una partida manual de 2 jugadores.
- **2026-07-27 (Etapa 1, Sesión 3):** Después de descartar cantidad de
  frames y tamaño de caché de GI como causa del cuelgue headless (ver
  Hallazgos), se le preguntó al usuario cómo seguir. Decisión: el usuario
  va a abrir el proyecto en el Editor gráfico él mismo y observar qué
  aparece justo después de que carga `conduccion9.unity` (barra de
  progreso, indicador de bakeo, etc.), ya que eso puede revelar en segundos
  lo que el log headless no mostró en 79+ minutos. Queda pendiente su
  reporte para la próxima sesión antes de decidir el siguiente paso con
  `SmokeTest`.

## Deuda técnica

*(Atajos deliberados tomados a sabiendas, para pagar más adelante.)*

- Ninguna todavía.

## Riesgos abiertos

- El Samsung Galaxy S3 (2012, API 16–18) no puede ejecutar ningún build de
  Unity 6 (mínimo API 22). Se re-encuadra como "30 fps estables dentro de un
  presupuesto de gama baja" — ver Etapa 7 del plan maestro. Hay que plantearlo
  también al profesor.
- El primer reimport tras el bump de versión de Unity puede tardar 10-20 min;
  no confundir con un cuelgue. (Confirmado en la práctica en la Sesión 1.)
- ~~Correr `BuildTools.SmokeTest` en modo headless con los scripts
  pre-Etapa 1 podía generar logs de cientos de MB/GB por el logging
  por-frame heredado.~~ **Resuelto en la Etapa 1**: se eliminó todo el
  logging por-frame (`Pallet.LateUpdate`, `ManejoPallets.Recibir`,
  `CollContraObst`).
- ~~La primera vez que `SmokeTest` abre `conduccion9.unity` bajo
  6000.0.74f1, el proceso headless puede quedar colgado ~45+ min en una
  limpieza de caché de GI (`TrimDiskCacheJob`) antes de siquiera entrar en
  Play Mode.~~ **Reevaluado en la Sesión 3:** al reintentar, el proceso sí
  cruzó ese punto — no era un cuelgue real del trim de GI.
- **Nuevo (Etapa 1, Sesión 3):** `BuildTools.SmokeTest` contra
  `conduccion9.unity` headless parece tardar más de ~79 minutos en
  completar los 600 frames configurados en `FramesPorEscena`, sin llegar a
  un veredicto — y ese tiempo excede el límite de duración de procesos en
  segundo plano de este entorno de ejecución, que los termina antes de que
  Unity termine solo. Se probaron y descartaron dos hipótesis: **no es**
  por cantidad de frames (con 60 frames se clavó en el mismo punto e
  igual de rápido/lento) ni por tamaño del caché de GI (con el caché en
  0 MB se clavó igual). El cuello de botella real todavía no está
  identificado — pasa justo después de que se loguea `TrimDiskCacheJob`,
  consume CPU multi-hilo sostenido, y no aparece nada más en el log
  headless durante ese tramo. Candidato principal sin confirmar: algo del
  sistema de iluminación/GI o compilación nativa (Burst) que no loguea vía
  `Debug.Log`. Antes de reintentar sin más: abrir el proyecto en el Editor
  gráfico y observar qué actividad/barra de progreso aparece justo después
  de abrir `conduccion9.unity` — eso revelaría en segundos lo que 79
  minutos de log headless no mostraron.
- **Cronograma ajustado (Sesión 3, 2026-07-27):** con la fecha límite real
  confirmada en 2026-08-15 (19 días desde hoy) y solo la Etapa 0 cerrada +
  Etapa 1 en verificación, quedan 6 etapas y media en ~2.5 semanas. Vigilar
  el ritmo sesión a sesión; si para mediados de Etapa 3-4 el avance no
  acompaña, replantear alcance con el usuario (candidatos a recortar:
  pulido de Etapa 5, profundidad de Etapa 7) en vez de dejarlo para el final.
