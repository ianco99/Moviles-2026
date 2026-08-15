using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Herramientas de compilación, verificación en Play Mode y build, pensadas
/// para ejecutarse en modo headless:
///
///   Unity.exe -batchmode -nographics -projectPath "..." -logFile "..." -executeMethod BuildTools.NombreDelMetodo
///
/// Es el único script de Editor permanente del proyecto (ver
/// Documentacion/DEV_PLAN.md, sección "Herramientas de editor"): cualquier
/// otro script de Editor que se necesite para una tarea puntual (migrar una
/// escena, mover assets) va en Assets/Editor/_Migraciones/ y se borra en la
/// misma etapa en la que se usa.
///
/// Ninguno de estos métodos se invoca junto con el flag "-quit": cada uno
/// corta el proceso con EditorApplication.Exit una vez termina su trabajo
/// (que puede ser asincrónico, como entrar en Play Mode), así Unity no
/// termina el proceso antes de tiempo.
/// </summary>
public static class BuildTools
{
    private const int FramesPorEscena = 600;

    /// <summary>
    /// Confirma que el proyecto compiló sin errores. Si hubiera errores de
    /// compilación, Unity ni siquiera llega a ejecutar este método con
    /// -executeMethod, así que solo hace falta llegar hasta acá y salir con
    /// código 0.
    /// </summary>
    public static void CompileCheck()
    {
        Debug.Log("BuildTools.CompileCheck: compilación OK.");
        EditorApplication.Exit(0);
    }

    private enum Fase { EntrandoAPlayMode, Jugando, SaliendoDePlayMode }

    private static Queue<string> escenasPendientes;
    private static Fase fase;
    private static int framesTranscurridos;
    private static bool huboError;

    /// <summary>
    /// Carga cada escena habilitada en Build Settings, la corre en Play Mode
    /// durante <see cref="FramesPorEscena"/> frames, y falla (código de
    /// salida 1) si en el camino se logueó algún error o excepción.
    /// </summary>
    public static void SmokeTest()
    {
        var rutas = EditorBuildSettings.scenes.Where(e => e.enabled).Select(e => e.path).ToList();
        if (rutas.Count == 0)
        {
            Debug.LogError("SmokeTest: no hay escenas habilitadas en Build Settings.");
            EditorApplication.Exit(1);
            return;
        }

        escenasPendientes = new Queue<string>(rutas);
        huboError = false;
        Application.logMessageReceived += RegistrarError;
        EditorApplication.update += Avanzar;
        AbrirSiguienteEscena();
    }

    private static void AbrirSiguienteEscena()
    {
        var ruta = escenasPendientes.Dequeue();
        Debug.Log($"SmokeTest: abriendo '{ruta}'");
        EditorSceneManager.OpenScene(ruta, OpenSceneMode.Single);
        framesTranscurridos = 0;
        fase = Fase.EntrandoAPlayMode;
        EditorApplication.isPlaying = true;
    }

    private static void Avanzar()
    {
        switch (fase)
        {
            case Fase.EntrandoAPlayMode:
                if (EditorApplication.isPlaying)
                    fase = Fase.Jugando;
                break;

            case Fase.Jugando:
                framesTranscurridos++;
                if (framesTranscurridos >= FramesPorEscena)
                {
                    fase = Fase.SaliendoDePlayMode;
                    EditorApplication.isPlaying = false;
                }
                break;

            case Fase.SaliendoDePlayMode:
                if (!EditorApplication.isPlaying)
                {
                    if (escenasPendientes.Count > 0)
                        AbrirSiguienteEscena();
                    else
                        TerminarSmokeTest();
                }
                break;
        }
    }

    private static void RegistrarError(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception) return;
        huboError = true;
        Debug.LogWarning($"SmokeTest: error detectado -> {condition}");
    }

    private static void TerminarSmokeTest()
    {
        EditorApplication.update -= Avanzar;
        Application.logMessageReceived -= RegistrarError;
        Debug.Log(huboError ? "SmokeTest: FALLO (hubo errores)" : "SmokeTest: OK");
        EditorApplication.Exit(huboError ? 1 : 0);
    }

    // --- Builds finales — se completan en la Etapa 7 del plan (rendimiento móvil + build Android) ---

    /// <summary>Genera Builds/Windows/Camiones.exe. Sin configurar todavía.</summary>
    public static void BuildWindows()
    {
        throw new System.NotImplementedException(
            "BuildTools.BuildWindows: se configura en la Etapa 7 del plan (rendimiento móvil + build).");
    }

    /// <summary>Genera Builds/Android/Camiones.apk. Sin configurar todavía.</summary>
    public static void BuildAndroid()
    {
        throw new System.NotImplementedException(
            "BuildTools.BuildAndroid: se configura en la Etapa 7 del plan (rendimiento móvil + build).");
    }
}
