# Void — Roadmap y Seguimiento del Proyecto

> Este documento es la fuente de verdad del estado del proyecto. Vive en la raíz del repo (o de cada repo si se separan). Se actualiza en cada sesión de trabajo, no solo se lee.

---

## Cómo usar este documento con Claude Code

- Al empezar una sesión, pedir a Claude Code que **lea este archivo primero** antes de tocar código.
- Cada vez que se termina una tarea, marcarla `[x]` y mover el estado de la fase si corresponde.
- Cada vez que aparece un bloqueador o una decisión pendiente nueva, se anota en su sección — no se deja flotando en la conversación.
- La sección "Estado actual" se actualiza al final de cada sesión con 2-3 líneas de qué se hizo y qué sigue.
- No se borran tareas completadas ni fases viejas — se archivan al final del documento para tener histórico.

---

## Leyenda de estados

| Símbolo | Significado |
|---|---|
| ⬜ | No iniciado |
| 🟨 | En progreso |
| ✅ | Completado |
| 🔒 | Bloqueado (ver sección Bloqueadores) |
| ❓ | Pendiente de decisión |

Prioridad: 🔴 Alta · 🟠 Media · 🟢 Baja

---

## Estado actual (resumen rápido)

**Última actualización:** 2026-07-01

**Fase activa:** Fase 0 — Fundamentos y configuración

**Resumen:** Los 3 MCP (Unity, GitHub, Supabase) ya están conectados y funcionando. Modelo de datos base (0.2) aplicado en el proyecto Supabase "Void" (`ca-central-1`) con RLS activo — ver detalle en 0.2. El repo se reestructuró como monorepo sectorizado: proyecto Unity en `unity/`, migraciones de base de datos en `supabase/migrations/`, raíz reservada para docs y la futura carpeta `dashboard/`. Repo pusheado a `https://github.com/Seb0401/void.git` (rama `main`). Existe avance previo en Unity (UI + scripts de Dimensión 1 y 2, principalmente movimiento/UI genérica) que todavía no está conectado al backend.

**Siguiente paso inmediato:** Configurar Auth (0.3) y auditar el código Unity existente de Dimensión 1 y 2 (Fase 1) para ver qué se puede conectar al modelo de datos ya creado.

---

## Fase 0 — Fundamentos y configuración

Estado de fase: 🟨 En progreso

### 0.1 — Prerrequisitos de herramientas (MCP no instalado aún)

> ⚠️ Nada de lo que dependa de MCP puede ejecutarse hasta completar esto. Hasta entonces, el trabajo se hace en modo manual (compartiendo código/archivos directamente en la conversación).

- [x] 🔴 Instalar MCP de GitHub — conectado como `Seb0401`
- [x] 🔴 Instalar MCP de Unity (puente Unity Editor ↔ Claude Code) — conectado (`UnityMCP`, HTTP local `127.0.0.1:8080`)
- [x] 🟠 Instalar MCP de Supabase — conectado
- [x] 🟢 Evaluar si conviene monorepo o repos separados — decidido: **monorepo sectorizado** (`unity/`, `dashboard/`, `supabase/` en `Seb0401/void`)

### 0.2 — Modelo de datos base

- [x] 🔴 Definir schema inicial: jugador, personaje, dimensión, stat, hoja_de_personaje
- [x] 🔴 Definir schema de: recurso, inventario, economía (transacciones)
- [ ] 🟠 Definir schema de: bestiario, carta
- [ ] 🟠 Definir schema de: alianza, tratado, directiva
- [x] 🟢 Definir campo `sync_mode` por dimensión (tiempo real / asíncrono)

**Detalle del modelo base (aplicado en Supabase, migraciones en `supabase/migrations/`):**
- `jugadores` — 1:1 con `auth.users`, se crea automáticamente vía trigger al registrarse (`rol`: `jugador` | `game_master`).
- `dimensiones` — catálogo, incluye `sync_mode` y `activa`.
- `personajes` — identidad base de un personaje, transversal a dimensiones.
- `hojas_de_personaje` — instancia de un personaje dentro de una dimensión (nivel, experiencia, y un campo `datos jsonb` abierto para lo específico de cada mecánica que aún no se modela como columna).
- `stat_definiciones` + `hoja_stats` — catálogo de stats (EAV) para poder agregar stats nuevos sin migrar el schema; `stat_definiciones.dimension_id` nulo = stat transversal.
- `recursos`, `inventarios` (saldo por jugador), `transacciones_economia` (log inmutable, sin insert directo desde cliente — pensado para pasar por una función validada más adelante).
- RLS activo en las 9 tablas: cada jugador ve/edita lo suyo, el GM ve todo; los catálogos son de lectura abierta y escritura solo GM.
- Pendiente de decisión futura: cómo evoluciona `datos jsonb` de `hojas_de_personaje` hacia columnas propias a medida que se conocen mejor las mecánicas por dimensión — se dejó flexible a propósito porque el documento de Dimensiones puede seguir cambiando.

### 0.3 — Backend

- [x] 🔴 Crear proyecto en Supabase — proyecto "Void" (`ca-central-1`) activo
- [ ] 🔴 Configurar Auth (proveedor email/password, confirmaciones, etc. — la tabla `jugadores` y el trigger de auto-creación con `rol` ya están listos a nivel de datos)
- [x] 🟠 Configurar Row Level Security básica (jugador solo ve/edita lo suyo, GM ve todo) — aplicado en las 9 tablas
- [ ] 🟢 Configurar Storage para assets (cartas, sprites, íconos)

**Bloqueadores de esta fase:** ninguno.

---

## Fase 1 — Vertical slice técnico (Dimensión 1 y 2)

Estado de fase: ⬜ No iniciado — depende de Fase 0

- [ ] 🔴 Auditar el código Unity existente de Dimensión 1 (recolección de recursos en mapa grande) y documentar qué hay hecho
- [ ] 🔴 Auditar el código Unity existente de Dimensión 2 (cooperativo vs. jefes, cooldowns por ronda) y documentar qué hay hecho
- [ ] 🟠 Separar lógica de reglas (cálculos, cooldowns, costos) de la UI en ambas dimensiones — paso previo obligatorio para poder migrar al backend sin reescribir todo
- [ ] 🔴 Conectar Dimensión 1 al backend (modo asíncrono)
- [ ] 🔴 Conectar Dimensión 2 al backend (modo tiempo real)
- [ ] 🟠 Probar con 2+ dispositivos/jugadores simultáneos en Dimensión 2 para validar el modo tiempo real

**Bloqueadores de esta fase:** requiere Fase 0 completa (backend + modelo de datos + MCP de Unity idealmente, aunque no es estrictamente obligatorio si se trabaja manual).

---

## Fase 2 — Dashboard GM v1

Estado de fase: ⬜ No iniciado — depende de Fase 1

- [ ] 🔴 Inicializar proyecto Next.js + Tailwind
- [ ] 🔴 Conectar a Supabase (lectura de jugadores y su estado)
- [ ] 🟠 Vista de jugadores conectados y su progreso en Dimensión 1
- [ ] 🟠 Vista en tiempo real del estado de combate en Dimensión 2
- [ ] 🟢 Autenticación de GM (solo el GM puede entrar al dashboard)

---

## Fase 3 — Prueba de pipeline de arte

Estado de fase: ⬜ No iniciado — puede correr en paralelo a Fase 1/2, no depende de MCP

- [ ] 🔴 Elegir un personaje de prueba (sugerido: jefe bacteria de Dimensión 2)
- [ ] 🔴 Prototipo en pixel art (tiempo limitado, ej. 4h) — registrar tiempo real y resultado
- [ ] 🔴 Prototipo en 2D ilustrado (mismo límite de tiempo)
- [ ] 🔴 Prototipo en 3D low-poly (mismo límite de tiempo)
- [ ] ❓ Decisión final de dirección de arte con base en los 3 prototipos
- [ ] 🟢 Si se elige un estilo modular (pixel art con partes o 3D con rig compartido), definir el sistema de partes intercambiables

**Nota:** esta fase es la más urgente en términos de riesgo del proyecto — es lo que el usuario identificó como el mayor desafío. Conviene no postergarla mucho aunque no dependa técnicamente de las otras fases.

---

## Fase 4 — Sistemas transversales reutilizables

Estado de fase: ⬜ No iniciado — depende de Fase 1 y de la decisión de arte (Fase 3)

- [ ] 🟠 Sistema de cartas genérico (usable por cualquier dimensión)
- [ ] 🟠 Sistema de economía genérico (monedas, trueque, banco, préstamos)
- [ ] 🟢 Sistema de alianzas y tratados
- [ ] 🟢 Sistema de directivas / eventos tipo "updates"
- [ ] 🟢 Bestiario como sistema transversal (con sub-secciones por dimensión)

---

## Fase 5 — Contenido y expansión

Estado de fase: ⬜ No iniciado — depende de Fase 4

- [ ] 🟢 Definir criterio para priorizar qué dimensiones nuevas se desarrollan primero
- [ ] 🟢 Documentar cada nueva dimensión con: mecánica única, mapa/tablero, recursos propios, si es tiempo real o asíncrona

---

## Fase 6 — Playtesting

Estado de fase: ⬜ No iniciado — depende de tener al menos 2 dimensiones jugables end-to-end

- [ ] 🟢 Sesión de prueba con primos/hermanos
- [ ] 🟢 Recoger feedback de balance y experiencia
- [ ] 🟢 Iterar

---

## Backlog / Decisiones pendientes

- ❓ Dirección de arte final (depende de Fase 3)
- ❓ Si se migra de Unity a otro engine en algún punto (por ahora: no, pero queda abierto)
- ❓ Alcance real del documento "Dimensiones" — el usuario confirmó que **puede seguir evolucionando**, así que tratarlo como versión viva, no como spec cerrada

---

## Bloqueadores actuales

Ninguno.

---

## Registro de cambios

| Fecha | Cambio |
|---|---|
| 2026-07-01 | Documento creado. Plan de arquitectura definido (Unity móvil + Supabase + dashboard Next.js). Fase 0 iniciada. |
| 2026-07-01 | MCP de GitHub, Unity y Supabase conectados. Proyecto Supabase "Void" creado. Repo reestructurado como monorepo (`unity/` para el proyecto Unity, raíz para docs), inicializado y pusheado a `https://github.com/Seb0401/void.git`. |
| 2026-07-02 | Modelo de datos base aplicado en Supabase: `jugadores`, `dimensiones`, `personajes`, `hojas_de_personaje`, `stat_definiciones`/`hoja_stats` (EAV), `recursos`, `inventarios`, `transacciones_economia`. RLS activo en las 9 tablas. Migraciones guardadas en `supabase/migrations/`. |

