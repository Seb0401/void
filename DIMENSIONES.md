# Dimensiones — Mecánicas y Estado Técnico

> Documento vivo. Antes de implementar una dimensión nueva, se planea acá primero:
> mecánica, recursos que usa, y qué del framework compartido (ver sección 0) puede
> reutilizar sin escribir código nuevo. Actualizar cada vez que se audita o modifica
> una dimensión.

**Última actualización:** 2026-07-02 — primera auditoría técnica completa de HadicEon, HadicEon 2 y ArcaicEon.

---

## 0. Framework compartido entre dimensiones

Al auditar las 3 escenas existentes se encontró que **ArcaicEon y HadicEon 2 comparten
literalmente la misma plantilla de sistemas** (mismos nombres de GameObjects, misma
estructura de Popups), solo cambia el "skin" de selección de personaje (razas vs.
colores). Esto es información clave para diseñar dimensiones futuras: en vez de crear
UI desde cero, conviene clonar esta plantilla y solo cambiar el contenido.

### 0.1 — Controladores raíz (uno por escena)

| GameObject | Script | Rol |
|---|---|---|
| `Controlador_General` | `Controlador.cs` | Flujo de escena: cargar escena, panel de "ganar", pasar a la siguiente escena (`nomEscena`, `sigEscena`, `panelGanar`). |
| `Controlador_Personaje` | `ControladorVida.cs` | HP del personaje: vida actual/máxima, barra, invulnerabilidad temporal tras recibir daño. |
| `Controlador_Moleculas` | *(sin script — pendiente, ver 4.1)* | Se asume orquestador general de moléculas, no identificado aún. |
| `Controlador_Cristales` | *(sin script — pendiente, ver 4.1)* | Se asume orquestador general de cristales, no identificado aún. |

### 0.2 — Sistema de "Cristales" (Popup_Cristal → Popup_Cirstales)

Popup con 8 elementos fijos: **Viento, Tierra, Fuego, Agua, Natura, Oscuridad, Luz, Hielo**.
Cada elemento tiene su propio contador independiente:

- GameObject `<Elemento>/Controlador` → **`Casillas_Contador.cs`**
- Estructura por elemento: texto (`C_<Elemento>`) + botón `Casilla` (subir) + botón
  `Casilla (1)` (bajar) + el `Controlador` con el script.
- `cuadroTexto` del script apunta al texto `C_<Elemento>` hermano.

### 0.3 — Sistema de "Moléculas" (Popup_Moleculas)

Tres categorías, cada una con su propia mecánica de conteo:

| Categoría | Elementos | Script en el `GameObject` hijo | Patrón |
|---|---|---|---|
| **Natural** | Carbono, Hidrógeno, Oxígeno, Nitrógeno, Fósforo, Azufre | **`Conteo.cs`** | Barra + botón; al llenarse resta de `ContoladorContador` (ver abajo). |
| **Especial** | Uranio, Mercurio, Helio, Radón | **`Conteo1.cs`** | Barra + botón; al llenarse oculta el botón y revela un botón de "acción" (reset). |
| **Resistencia** | Veneno (ResVen), Peste (ResPet), Fuego (ResFue), Eléctrico (ResElec) | **`Casillas_Contador.cs`** | Mismo patrón +/- que los Cristales (medidor circular con botones subir/bajar). |

`Popup_Moleculas/Natural/ContoladorContador` → **`ConteoSinBarra.cs`**: es el total
agregado. Cada `Conteo.cs` de la categoría Natural tiene un campo `conteoSinBarra` que
apunta acá — al llenar un contador individual, se descuenta del total.

Los botones `Btn_Nat`, `Btn_Esp`, `Btn_Res` (pestañas Natural/Especial/Resistencia)
tienen un `GameObject` hijo con script roto que **no se pudo identificar** — ver 4.1.

### 0.4 — Selección de personaje

`Seleccion de Personaje/<Opción>/Controlador` → **`SeleccionPersonaje.cs`** — al hacer
clic, copia el sprite/color del botón elegido al retrato del personaje. El contenido de
`<Opción>` es lo único que cambia entre dimensiones:
- ArcaicEon: razas (Arquea, Hongo, Protozoo, Probionte, Virus — dominios de vida
  primitivos, coherente con el nombre "Arcaico").
- HadicEon 2: colores (Rojo, Azul, Amarillo, Verde, Morado, Anaranjado, Rosado,
  Celeste, Marrón).

### 0.5 — Combate (visto en ArcaicEon)

- `Ataques/AtaqueBasico/Controlador` → **`AtaquePredeterminado.cs``**: botón de ataque
  que copia sprite/color de un objeto Y a un objeto X (selección de ataque activo).
- `animacionesEnemigo.cs` + `detectorEnemigoPersonaje.cs`: cuando el jugador entra en
  el trigger de un enemigo, este ataca en bucle cada `tiempoAtaque1` segundos.
- **`Habilidades.cs`** existe en el proyecto (cooldown de habilidad medido en rondas:
  `PasarRonda` descuenta, al llegar a 1 reactiva el botón) pero **no está atado a
  ningún GameObject en ninguna escena actual** — es la "otra mecánica" de cooldowns
  por ronda mencionada en `plan_void.md` para combate cooperativo, todavía sin
  implementar visualmente.

---

## 1. Dimensión 1, Parte A — HadicEon

**Concepto (según lo definido):** tablero con punto de inicio A y punto de fin B
definidos. Las casillas pueden tener efectos típicos de juego de mesa: avanzar
casillas, retroceder casillas, etc.

**Estado técnico encontrado:**
- La escena es un esqueleto: `Personaje` (sprite estático, sin física), un contenedor
  `Objetos` vacío (sin casillas colocadas todavía), y un `Controlador` raíz que tenía
  el script roto (**ya arreglado**, ahora tiene `Controlador.cs` con `sigEscena =
  "HadicEon 2"`).
- Existen 2 botones `Casilla`/`Casilla (1)` con hijos de texto "Aumentar"/"Disminuir"
  y 2 textos `Numero de Casilla` — parece el esqueleto de un contador de casilla
  manual (tipo "en qué casilla física estoy"), pero **no tienen ningún script
  atado todavía** (ni roto ni funcional) — es contenido sin terminar, no un bug.
- **No existe ningún GameObject de "casilla" individual con efecto** en la escena —
  el sistema de efectos hay que construirlo y poblarlo.

**Lo que se agregó esta sesión (nuevo, no existía antes):**
- `Casillas_Contador.cs` ahora expone `onValorCambiado` (evento con el nuevo valor)
  cada vez que el contador sube o baja.
- **`CasillaEfecto.cs`** (nuevo): componente reutilizable que se suscribe a ese
  evento y aplica un desplazamiento automático (avanzar o retroceder N casillas)
  cuando el contador llega a un número de casilla configurado en una lista
  `efectos` (`casilla` → `desplazamiento`). Pensado para engancharse al mismo
  `Casillas_Contador` que ya usan Cristales y Resistencia — no duplica lógica.

**Pendiente de diseño (no se puede resolver sin tu input):**
- ¿El tablero es un camino lineal de N casillas o tiene ramificaciones?
- ¿El "punto B" dispara automáticamente `Controlador.FinEscena()` (ya existe el
  método) o hace falta otra condición?
- Definir la lista real de casillas con efecto (`CasillaEfecto.efectos`) una vez
  que el tablero visual esté armado en el Editor.

---

## 2. Dimensión 1, Parte B — HadicEon 2

**Concepto:** recolección de recursos a lo largo de un mapa más abierto, con
casillas hexagonales.

**Estado técnico encontrado:**
- Comparte el framework completo de la sección 0 (Cristales, Moléculas, Selección
  de personaje por color). **Todos los scripts rotos de esta escena se repararon**
  esta sesión (39 referencias).
- La grilla hexagonal en sí no tiene scripts dedicados — probablemente se resuelve
  con un `Tilemap` de Unity en modo hexagonal (hay una carpeta `Assets/Tiles`), sin
  necesitar código de pathfinding custom todavía.
- No hay ningún script de movimiento del jugador sobre el mapa ni de detección de
  "pisar una casilla hexagonal para recolectar" — es contenido pendiente de
  construir, similar a HadicEon.

**Pendiente de diseño:**
- ¿La recolección es por clic (como los contadores +/- que ya existen) o por
  movimiento físico del personaje sobre el hexágono?
- Mapeo real de qué molécula/cristal se obtiene en qué zona del mapa.

---

## 3. Dimensión 2 — ArcaicEon

**Concepto:** el usuario selecciona una raza/clase con sus habilidades propias y
enfrenta jefes a lo largo de un mapa similar al de HadicEon 2.

**Estado técnico encontrado:**
- Comparte el mismo framework de la sección 0, con razas microorganismo (Arquea,
  Hongo, Protozoo, Probionte, Virus) en vez de colores. **Los 41 scripts rotos de
  esta escena se repararon** esta sesión.
- Tiene además el sistema de `Ataques/AtaqueBasico` (sección 0.5) que no existe en
  HadicEon 2.
- El sistema de combate contra jefes (`animacionesEnemigo` +
  `detectorEnemigoPersonaje`) está en los scripts del proyecto pero no se confirmó
  atado a un enemigo real en esta escena durante la auditoría — revisar si hay un
  jefe colocado en el mapa.
- `Habilidades.cs` (cooldown por rondas) sigue sin estar atado a ningún botón de
  habilidad — es la pieza que falta para que "habilidades de la raza/clase" con
  cooldown funcionen.

**Pendiente de diseño:**
- Definir qué habilidad tiene cada raza (actualmente la selección de raza solo
  cambia el retrato, no hay diferenciación mecánica todavía).
- Conectar `Habilidades.cs` a los botones de habilidad reales.
- Confirmar la relación entre el mapa de ArcaicEon y el de HadicEon 2 ("similar al
  mapa de la segunda parte de la primera dimensión" — ¿mismo sistema hexagonal?).

---

## 4. Deuda técnica encontrada en la auditoría

### 4.1 — Sin resolver (requieren decisión de diseño, no se adivinó)

- `Controlador_Moleculas` y `Controlador_Cristales` (raíz de cada escena, x2
  escenas): tenían script roto, se limpió la referencia rota pero **no se
  reasignó ningún script** porque ninguno de los 23 scripts existentes encaja
  claramente con "controlador general de moléculas/cristales". Candidatos: un
  script nuevo que sume los totales de todas las categorías, o que no haga falta
  si `ContoladorContador` ya cumple ese rol.
- `Popup_Moleculas/Btn_Nat`, `Btn_Esp`, `Btn_Res` (botones de pestaña, x2 escenas):
  mismo caso — script roto limpiado, sin reemplazo. Es casi seguro un script de
  "cambiar de pestaña visible" simple (mostrar Natural/ocultar Especial y
  Resistencia, etc.), similar a `Selector_Tema.cs` pero para 3 opciones en vez de 2.

### 4.2 — Scripts duplicados / sin uso (candidatos a limpieza, no se tocaron)

- `Contoler.cs` (nombre con typo, clase `Controler`) es casi idéntico a
  `ControladorVida.cs` — parece el original antes de renombrarse, quedó huérfano
  sin ningún GameObject que lo use. Candidato a eliminar una vez confirmado que
  `ControladorVida` es la versión "buena".
- `Mov.cs`, `Salto.cs`, `DetectorSalt.cs`: mecánica de plataformas (correr + saltar
  con física), no están atados a ningún GameObject en las 3 escenas auditadas.
  Es probable que vengan del "otro proyecto" que se fusionó y no correspondan a
  ninguna dimensión de Void. Revisar antes de borrar por si pertenecen a una
  dimensión futura de plataformas.
- `ejemplo.cs`: script de prueba (`Debug.Log("Prueba")`), sin uso real.

### 4.3 — Reparado esta sesión

- HadicEon: 1 script roto (`Controlador` raíz) → reparado.
- HadicEon 2: 39 scripts rotos → reparados y reconectados a sus clases correctas.
- ArcaicEon: 41 scripts rotos → reparados y reconectados a sus clases correctas.
- Los scripts recién atados a `Casillas_Contador` en Cristales tienen su
  `cuadroTexto` conectado al texto correspondiente; el resto de campos
  (`objetoActivar`, límites, referencias de `Conteo`/`Conteo1`/`ControladorVida`)
  quedan en blanco — hay que revisarlos y conectarlos a mano en el Inspector de
  Unity, porque requieren criterio visual (qué imagen, qué texto exacto) que no se
  puede adivinar de forma segura por código.

---

## 5. Checklist para planear una dimensión nueva

Antes de escribir código, completar esto acá:

- [ ] Nombre y tema de la dimensión
- [ ] Mecánica central en 1-2 frases
- [ ] ¿Reutiliza el framework de la sección 0 (cristales/moléculas/selección de
      personaje) o necesita un sistema nuevo?
- [ ] ¿Tiempo real o asíncrona? (afecta `dimensiones.sync_mode` en Supabase)
- [ ] Recursos/stats que introduce — ¿van al modelo genérico de Supabase
      (`recursos`, `stat_definiciones`) o son exclusivos de esta dimensión?
- [ ] Mapa: lineal (tipo HadicEon), hexagonal abierto (tipo HadicEon 2), u otro
- [ ] ¿Tiene jefes / combate? ¿Reutiliza `Habilidades.cs` +
      `detectorEnemigoPersonaje.cs`?
