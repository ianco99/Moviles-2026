# Registro de sesiones

> Log de solo-agregado (append-only). Un bloque por sesión. No editar bloques
> anteriores — si algo estaba mal, corregirlo en un bloque nuevo.

---

## Sesión 1 — 2026-07-26 — Modelo: Sonnet 5 — Etapa: 0

**Objetivo:** Poner en marcha la infraestructura del proyecto: git, documentos
vivos, versión de Unity, herramientas de build headless, Input System, y
probar que el ciclo de verificación headless funciona.

**Hecho:**
- `git init` en la carpeta interna del proyecto (`MobileDevTP1-Public-master/MobileDevTP1-Public-master/`).
- Se agregó `/[Uu]ser[Ss]ettings/` al `.gitignore` (faltaba en el original) y
  se destrackearon esos archivos antes del commit inicial.
- Commit baseline del proyecto tal cual estaba entregado (`6d7b980`, 2403 archivos).
- Creados `Documentacion/DEV_PLAN.md`, `SESSION_LOG.md`, `TAREAS_EDITOR.md`
  (`3a11f7b`).
- Escritas 4 memorias del harness (`project-tp1-layout`,
  `project-tp1-headless-workflow`, `project-tp1-code-quality-bar`,
  `project-tp1-decisions`) + índice `MEMORY.md`.
- `ProjectSettings/ProjectVersion.txt` → `6000.0.74f1` (hash de revisión
  `7685f01dc6be`, tomado del propio `Unity.exe` instalado).
- `Packages/manifest.json` → agregado `com.unity.inputsystem: 1.14.0`
  (versión validada por uso en otros proyectos locales con Unity 6000.0.x).
- `ProjectSettings/ProjectSettings.asset` → agregado `activeInputHandler: 2`
  (Both), para no romper el `Input.*` legacy mientras la Etapa 3 no llegue.
- `Assets/Editor/BuildTools.cs` — único script de editor permanente:
  `CompileCheck`, `SmokeTest` (con máquina de estados para tickear Play Mode
  headless sin bloquear el loop de `EditorApplication.update`), y
  `BuildWindows`/`BuildAndroid` como stubs que tiran `NotImplementedException`
  hasta la Etapa 7.
- Import headless de TMP Essential Resources vía script descartable
  (`Assets/Editor/_Migraciones/ImportarTMPEssentials.cs`), corrido una vez y
  borrado después (regla del proyecto: esa carpeta queda vacía en cada corte
  de etapa).
- Commit final de la etapa (`330af36`): bump de versión, Input System,
  TMP Essentials, BuildTools.cs.

**No hecho / bloqueado:**
- La fecha límite real (distinta de la de la cátedra, ya vencida) queda
  pendiente de que el usuario la complete en `DEV_PLAN.md`.
- No se dejó terminar un `SmokeTest` completo (ver Verificación) — no era
  requisito de cierre de esta etapa, y de todos modos no tiene sentido
  correrlo hasta que la Etapa 1 limpie el logging por-frame heredado.

**Verificación:**
- `BuildTools.CompileCheck` headless (`Unity.exe -batchmode -nographics
  -executeMethod BuildTools.CompileCheck`): **exit code 0**, 0 ocurrencias de
  `error CS` en el log. Esto prueba el loop de verificación del que dependen
  las Etapas 1, 4 y 6.
- `BuildTools.SmokeTest` se probó por separado para confirmar que la
  herramienta en sí funciona (entra en Play Mode, tickea frames, detecta
  errores) — confirmado que sí funciona, pero se abortó a los ~5 minutos
  porque el juego heredado spamea un `print()` por frame (`Pallet.cs:54`,
  Apéndice A ítem 14) y el log había llegado a ~1 GB. Se mató el proceso
  Unity a mano (`Stop-Process -Force`) y se borró el `Temp/UnityLockfile`
  que quedó huérfano. Registrado en `DEV_PLAN.md § Hallazgos`.

**Para la próxima sesión:**
- Empezar la Etapa 1 (delegalizar y estabilizar) trabajando el Apéndice A
  del plan maestro en orden. Priorizar el `print("crudo")`/`print("smoot")`
  de `Pallet.cs` y los demás logs por-frame temprano en la etapa — así se
  puede volver a correr `BuildTools.SmokeTest` sin que genere gigabytes de
  log, y usarlo de ahí en adelante como verificación real de la etapa.
- Confirmar con el usuario la fecha límite real y completarla en
  `DEV_PLAN.md`.

---

## Sesión 2 — 2026-07-27 — Modelo: Sonnet 5 — Etapa: 1

**Objetivo:** Trabajar los 26 ítems del Apéndice A del plan maestro
(delegalizar y estabilizar) sin restructuración arquitectónica, y dejar el
proyecto listo para volver a correr `BuildTools.SmokeTest` sin la explosión
de log de la Sesión 1.

**Hecho:**
- Los 26 ítems del Apéndice A, archivo por archivo (ver el cuerpo del
  commit `221940e` para el detalle completo). Puntos más críticos:
  - `GameManager.FinCalibracion()` llamaba `CambiarACarrera()` en vez de
    `CambiarATutorial()` — el tutorial de conducción nunca aparecía.
    Corregido.
  - NRE de `GameManager.Update()` antes de que ambos jugadores se unieran
    (`PlayerInfo1`/`PlayerInfo2` arrancaban `null`) — corregido
    inicializándolos con `PJ = null` en vez del objeto entero en `null`.
  - `Bolsa.Start()` pisaba el `Monto` del inspector con `Valor2` fijo (las
    68 bolsas valían lo mismo); `Desaparecer()` se llamaba dos veces por
    pickup y desreferenciaba `Particulas` sin guard.
  - `Cinta.cs` desactivaba el pallet equivocado en el loop de movimiento;
    el parpadeo de `Cinta`/`Estanteria` comparaba `Color` (float) contra
    `Color32` y casi nunca coincidía — reemplazado por un estado `bool`
    explícito en vez de leer el color de vuelta del material.
  - `CollContraObst` comparaba `name == "Camion1"` (nunca coincide con
    `"Camion - new 1/2"`) — reemplazado por `Player.IdPlayer`.
  - `MngPts.PrepararNumeros` (vía `new Visualizacion()`) dejaba el puntaje
    en blanco salvo que tuviera exactamente 6 o 7 dígitos — generalizado
    para separar miles con cualquier cantidad de dígitos, y el método pasó
    a `static` (ya no hace falta instanciar un `MonoBehaviour` con `new`).
  - `GUISkin` se mutaba en runtime directo sobre el asset importado en
    `Visualizacion.cs` y `MngPts.cs` (dirtying permanente del proyecto si
    se jugaba en el Editor) — ahora se clona una vez en `Awake()` y se
    muta solo la copia.
  - Steering drop: `ControlDireccion` escribía `giro` en `Update()`
    mientras `CarController.FixedUpdate()` lo consume y lo pisa a 0 en
    cada paso físico — movido a `FixedUpdate()` para que ambos corran al
    mismo ritmo. De paso se notó que el campo `Giro` (usado por
    `Visualizacion` para rotar el volante en HUD) nunca se actualizaba
    para teclado/mouse en el original — solo para Kinect — así que el
    volante en pantalla nunca rotaba con teclado; corregido de paso.
  - `Application.LoadLevel`/`loadedLevel` → `SceneManager`;
    `SetActiveRecursively` (31 usos) → `SetActive`; `Screen.lockCursor`
    eliminado junto con el script Kinect que lo usaba.
  - Debug keys que quedaban activas en builds (reinicio, skip
    tutorial/carrera, animaciones de depósito) gateadas detrás de
    `#if UNITY_EDITOR`. Logging por-frame eliminado de
    `Pallet.LateUpdate`, `ManejoPallets.Recibir` y `CollContraObst` — esto
    es lo que generaba ~1 GB de log en la Sesión 1.
  - Código muerto de Kinect eliminado: `Aceleracion.cs`, `Direccion.cs`,
    `ManejadorKinectCalib.cs`, `ControlDireccion.TipoInput.Kinect` +
    `ManoDer`/`ManoIzq`/`Angulo()`, rama `Calibrando` de
    `PantallaCalibTuto.cs`, `GameManager.Esqueleto1/2` + `PosEsqsCarrera`.
    Además se encontraron y borraron 3 scripts completamente huérfanos
    (no referenciados en ninguna escena/prefab, confirmado por GUID):
    `VidIntrMgr.cs`, `JuegoEscMgr.cs`, `AcelerAuto.cs` — se borraron en
    vez de parchear el bug que pedía el Apéndice A porque el código era
    inalcanzable de todos modos.
  - `GetComponent` cacheado en `Awake()` en los 7 scripts que pedía el
    plan (`ControlDireccion`, `Frenado`, `Cinta`, `Estanteria`, `Respawn`,
    `Obstaculo`, `Bolsa`) más `AnimMngDesc` y `ContrCalibracion`.
  - Todos los bloques de código comentado grandes (`/* ... */`) removidos
    del árbol `Assets/SCRIPTS` y `Assets/PREFABS`.
- `GameObject.Find("GameMgr")` reemplazado por `GameManager.Instancia`
  (desviación deliberada del plan, que pedía `[SerializeField]`; ver
  `DEV_PLAN.md § Decisiones`). `Deposito2.Contr1`/`Contr2` sí se dejaron
  como campos públicos para cablear a mano — tarea agregada a
  `TAREAS_EDITOR.md`.
- Commit `221940e` (38 archivos: 27 modificados, 5 borrados con sus
  `.meta`, más `TAREAS_EDITOR.md`).

**No hecho / bloqueado:**
- `BuildTools.SmokeTest` no llegó a completarse: el proceso headless quedó
  ~45 minutos sin avanzar justo después de abrir `conduccion9.unity` por
  primera vez bajo 6000.0.74f1 (ver Verificación). Se mató a mano por
  decisión del usuario, sin llegar a confirmar si el código de la Etapa 1
  tickea 600 frames sin errores.
- Por lo anterior, tampoco se probó manualmente una partida de 2 jugadores
  por teclado (criterio de cierre de la Etapa 1).
- La tarea de `TAREAS_EDITOR.md` (cablear `Contr1`/`Contr2` en las 4
  instancias de `Deposito2`) sigue pendiente del usuario.
- La fecha límite real sigue sin completarse en `DEV_PLAN.md`.

**Verificación:**
- `BuildTools.CompileCheck` headless, corrido dos veces (antes y después
  de matar el SmokeTest fallido): **exit code 0** ambas veces, 0
  ocurrencias de `error CS`/`Exception`/`NullReference` en el log.
- `BuildTools.SmokeTest` headless: **no completado**. El log quedó
  detenido después de `TrimDiskCacheJob: Current cache size 6554mb` (limpieza
  del caché de disco de GI de Unity), con un solo hilo de CPU ocupado de
  forma sostenida durante ~45 min (no una explosión de log — eso ya se
  descartó como causa) y sin haber llegado a entrar en Play Mode. Matado a
  mano (`taskkill /F`) y borrado el `Temp/UnityLockfile` huérfano después
  de confirmar que no quedaba ningún proceso `Unity.exe` corriendo.
  Registrado en `DEV_PLAN.md § Hallazgos` y `§ Riesgos abiertos`.

**Para la próxima sesión:**
- Primera acción: volver a correr `BuildTools.SmokeTest` headless. Si el
  caché de GI ya quedó limpio, debería completarse rápido esta vez; si
  vuelve a colgarse en el mismo punto, es reproducible y hay que
  investigar más (posiblemente limpiar `%LOCALAPPDATA%\Unity\Caches\GiCache`
  a mano, o revisar si algo del proyecto fuerza ese trim en cada apertura
  de escena).
- Si `SmokeTest` pasa limpio: hacer una partida manual de 2 jugadores por
  teclado para confirmar el criterio de cierre de la Etapa 1, y recién ahí
  marcarla ✅ en `DEV_PLAN.md`.
- Recordarle al usuario la tarea pendiente en `TAREAS_EDITOR.md` (cablear
  `Contr1`/`Contr2` en `Deposito2`) antes de dar por buena una partida con
  descarga en depósito.
- Confirmar con el usuario la fecha límite real.

---

## Sesión 3 — 2026-07-27 — Modelo: Sonnet 5 — Etapa: 1

**Objetivo:** Retomar donde quedó la Sesión 2: primero reintentar
`BuildTools.SmokeTest` headless; si pasa, hacer la verificación manual de
2 jugadores y cerrar la Etapa 1. Además, cerrar el pendiente de la fecha
límite real.

**Hecho:**
- Confirmada la fecha límite real con el usuario: **2026-08-15**. Escrita
  en `DEV_PLAN.md` (reemplaza el placeholder pendiente desde la Sesión 1).
  Agregado un riesgo nuevo en `DEV_PLAN.md § Riesgos abiertos` por el
  cronograma ajustado (19 días para 6.5 etapas restantes).
- Confirmado por `git status`/`tasklist` al empezar: árbol de trabajo
  limpio, sin `Unity.exe` corriendo, sin `Temp/UnityLockfile` (nada
  quedó a medio terminar de la Sesión 2).
- Se reintentó `BuildTools.SmokeTest` headless desde cero. A diferencia de
  la Sesión 2, esta vez el proceso **sí cruzó** el punto exacto donde se
  había colgado antes (`TrimDiskCacheJob: Current cache size 6554mb`):
  logueó `SmokeTest: abriendo 'Assets/ESCENAS/conduccion9.unity'` y dio
  señales de estar tickeando en Play Mode (líneas repetidas de
  `Scanning for USB devices`). Uso de CPU alto y multi-hilo sostenido
  confirmado con varias muestras de `wmic ... get UserModeTime,
  KernelModeTime` a lo largo de todo el proceso (nunca hubo una señal real
  de cuelgue/congelamiento).
- A los ~79 minutos, el proceso de shell en segundo plano que envolvía a
  `Unity.exe` fue terminado por un límite de duración del entorno de
  ejecución (no fue un `taskkill` manual ni una decisión del usuario).
  `Unity.exe`, al ser un proceso hijo de ese shell, se cayó con él, sin
  llegar a loguear `SmokeTest: OK` ni `FALLO`.
- Verificado que las 462 líneas del log no contienen ningún `error CS`,
  `Exception` ni `NullReference` nuevo (solo el error de licencia ya
  documentado, no bloqueante).
- Confirmado con `tasklist` que no quedó ningún `Unity.exe` corriendo, y
  borrado el `Temp/UnityLockfile` huérfano resultante.
- Actualizado `DEV_PLAN.md` (`§ Estado actual`, `§ Hallazgos`,
  `§ Riesgos abiertos`) con el diagnóstico revisado: el cuelgue de la
  Sesión 2 y este caso probablemente no son el mismo fenómeno; esta escena
  parece tardar de verdad más de ~79 minutos en completar 600 frames
  headless sin GPU, lo cual excede los límites prácticos de este flujo de
  verificación tal como está planteado hoy.
- Preguntado al usuario cómo seguir; eligió bajar `FramesPorEscena`
  temporalmente. Se cambió `BuildTools.cs` de 600 a 60 frames y se
  reintentó `SmokeTest`: **se clavó en el mismo punto exacto**
  (`TrimDiskCacheJob`) con la misma curva de CPU alta y sostenida —
  descarta que el tiempo dependa de la cantidad de frames.
- Se mató ese intento (`taskkill`, confirmado sin `Unity.exe` corriendo,
  `Temp/UnityLockfile` borrado) y se limpió a mano el caché de GI
  (`C:/Users/Ian/AppData/LocalLow/Unity/Caches/GiCache`, 6.5 GB → 0).
  Se reintentó `SmokeTest` (todavía con 60 frames): el log mostró
  `TrimDiskCacheJob: Current cache size 0mb` pero **igual se clavó en el
  mismo punto** — descarta que el trim del caché de GI en sí sea el
  cuello de botella; esa línea es solo lo último que se loguea antes de
  que arranque otra cosa no identificada.
- Revisado `Settings.lighting` embebido en `conduccion9.unity`:
  `m_BakeOnSceneLoad: 0`, `m_EnableRealtimeLightmaps: 0`,
  `m_EnableBakedLightmaps: 1`, `m_BakeBackend: 1` (Progressive CPU). No
  se encontró ninguna línea de Lightmap/Burst/IL2CPP en los tres logs
  generados esta sesión, así que no se pudo confirmar ni descartar del
  todo un bakeo de iluminación como causa sin ver la UI del Editor.
- Matado ese segundo intento también (mismo procedimiento de limpieza) y
  revertido `BuildTools.cs` a `FramesPorEscena = 600` (confirmado por
  `git diff --stat`: sin cambios en ese archivo, solo en `Documentacion/`).

**No hecho / bloqueado:**
- `BuildTools.SmokeTest` sigue sin dar un veredicto pass/fail. La Etapa 1
  sigue 🟡, no ✅.
- Por lo mismo, tampoco se hizo la partida manual de 2 jugadores por
  teclado (criterio de cierre de la Etapa 1).
- La tarea de `TAREAS_EDITOR.md` (cablear `Contr1`/`Contr2` en las 4
  instancias de `Deposito2`) sigue pendiente del usuario — confirmado
  releyendo `conduccion9.unity`, las 4 instancias siguen en
  `{fileID: 0}`.

**Verificación:**
- `BuildTools.SmokeTest` headless: **sin veredicto** (terminado
  externamente a los ~79 min, sin `OK`/`FALLO` en el log). Sin errores de
  compilación ni excepciones nuevas en las 462 líneas que sí se generaron.
  `Temp/UnityLockfile` huérfano confirmado y borrado tras confirmar por
  `tasklist` que no quedaba ningún proceso `Unity.exe`.

**Para la próxima sesión:**
- Ya se descartaron cantidad de frames y tamaño del caché de GI como
  causa. Recomendado como primer paso: abrir el proyecto en el Editor
  gráfico (no headless) y observar qué aparece justo después de abrir
  `conduccion9.unity` — cualquier barra de progreso o indicador (bakeo de
  luz, compilación de Burst, import de algo) va a revelar en segundos lo
  que 79+ minutos de log headless no mostraron.
- Alternativa si el usuario prefiere no esperar más al headless: aceptar
  que la verificación de Play Mode de esta etapa se haga a mano en el
  Editor gráfico (jugar la escena directamente), y dejar `SmokeTest`
  headless para revisitar más adelante si se identifica y resuelve la
  causa real.
- Si se logra un `SmokeTest` con veredicto `OK`: hacer la partida manual de
  2 jugadores por teclado y recién ahí cerrar la Etapa 1 en `DEV_PLAN.md`.
- Recordarle al usuario la tarea pendiente en `TAREAS_EDITOR.md` (cablear
  `Contr1`/`Contr2` en `Deposito2`) antes de dar por buena una partida con
  descarga en depósito.
