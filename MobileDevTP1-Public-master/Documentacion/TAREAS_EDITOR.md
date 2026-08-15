# Tareas para hacer en el Editor de Unity

> Cola de trabajo visual/manual que le corresponde al usuario, no a los
> agentes. Cada ítem: qué escena/prefab, pasos exactos, resultado esperado, y
> cómo lo va a verificar el agente. Tachar (`~~texto~~`) cuando esté hecho y
> verificado.

Por qué esto existe: los agentes escriben el código C# runtime (con slots
`[SerializeField]` para lo que haga falta conectar), pero el armado visual de
Canvas, prefabs de UI, materiales, cámaras y disposición de objetos en escena
lo hace el usuario a mano en el Editor. Así el proyecto queda prolijo,
explicable y no lleno de jerarquías generadas por script que nadie puede
defender en una devolución oral.

---

## Pendientes

### 1. Cablear `Contr1`/`Contr2` en las instancias de `Deposito2` (Etapa 1)

**Por qué:** `Deposito2.cs` resolvía estas dos referencias con
`GameObject.Find("ContrDesc1")` / `GameObject.Find("ContrDesc2")` en
`Start()` (Apéndice A ítem 19). Ya se quitó ese código: `Contr1` y `Contr2`
son campos `public` normales, así que ahora hay que asignarlos a mano en el
Inspector, una sola vez, en la escena.

**Pasos:**
1. Abrir `Assets/ESCENAS/conduccion9.unity`.
2. En la Hierarchy, buscar todas las instancias que usan el prefab
   `Assets/PREFABS/Deposito/Deposito2.prefab` (buscar "Deposito2" en el
   buscador de la Hierarchy). Son 4 en la escena actual.
3. Seleccionarlas todas juntas (click + Ctrl/Shift-click) para editarlas de
   una sola vez en el Inspector (Unity permite multi-edición de campos
   iguales).
4. Arrastrar el GameObject **`ContrDesc1`** (tiene el componente
   `ControladorDeDescarga` del jugador 1) al campo **`Contr 1`**.
5. Arrastrar el GameObject **`ContrDesc2`** (tiene el componente
   `ControladorDeDescarga` del jugador 2) al campo **`Contr 2`**.
6. Guardar la escena.

**Cómo lo verifica el agente:** leyendo
`Assets/ESCENAS/conduccion9.unity` y confirmando que las 4 instancias de
`Deposito2` tienen `Contr1`/`Contr2` con un `fileID` distinto de `0`
(ya no `{fileID: 0}`), y corriendo `BuildTools.SmokeTest` para confirmar que
un camión puede entrar y salir de un depósito sin error.

## Hechos

*(Ninguno todavía.)*
