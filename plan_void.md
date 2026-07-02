# Plan de Desarrollo — Void
### App híbrida de gestión de recursos + juego de mesa virtual para "Dimensiones"

---

## 1. Resumen de la visión

Void combina dos capas que deben convivir en la misma arquitectura desde el día uno:

1. **Capa de gestión** — hojas de personajes, stats, bestiario, cartas, economía, alianzas, tratados, directivas, investigaciones, etc. (todo lo descrito en el documento de Dimensiones).
2. **Capa de juego virtual** — las dimensiones jugables en sí, cada una con su propia mecánica (recolección de recursos en mapa grande, cooperativo contra jefes con cooldowns por rondas, y las que vengan después).

Encima de ambas capas se sienta un **dashboard web para el Game Master**, que monitorea jugadores y avances sin necesitar estar en la app móvil.

La decisión clave que ya tomaste: **cliente móvil en Unity, dashboard web como proyecto aparte**, con apertura a migrar de engine si en el camino conviene más. Esto es razonable — vamos a construir la arquitectura de forma que el engine sea reemplazable sin reescribir todo lo demás.

---

## 2. Arquitectura general

```
┌─────────────────────┐        ┌──────────────────────┐
│   App Móvil (Unity)  │◄──────►│   Backend / API        │
│  - Dimensión 1        │        │  (Auth, DB, Realtime,  │
│  - Dimensión 2        │        │   Cloud Functions)     │
│  - Hoja de personaje  │        └──────────┬────────────┘
│  - Bestiario, cartas  │                   │
└─────────────────────┘                   │
                                             ▼
                                ┌──────────────────────┐
                                │  Dashboard Web (GM)    │
                                │  Next.js + Tailwind    │
                                └──────────────────────┘
```

**Principio de diseño clave:** el motor de juego (Unity) nunca es dueño de los datos. Es un cliente más. Todo el estado "de verdad" (personajes, recursos, stats, alianzas, economía) vive en el backend. Esto es lo que te permite:
- Que el dashboard web lea el mismo estado sin duplicar lógica.
- Migrar de Unity a otro engine en el futuro sin perder datos ni reescribir reglas de negocio.
- Sincronizar en tiempo real o de forma asíncrona según la dimensión, porque la lógica de sync vive en el backend, no en el cliente.

---

## 3. Stack recomendado

### 3.1 Backend — **Supabase** (recomendado sobre Firebase)

Dado el modelo de datos que describe tu documento (stats, hojas por dimensión, bestiario, cartas, alianzas, tratados, economía con transacciones) es un modelo **relacional** con muchas relaciones entre entidades. Firebase/Firestore (NoSQL) te haría sufrir con joins y consultas complejas conforme crezca. Supabase te da:

- Postgres real (tablas, relaciones, queries complejas — ideal para "jugador tiene N personajes, cada uno con stats, inventario, alianzas con otros jugadores").
- **Realtime** por tabla/canal — puedes activar tiempo real solo en las dimensiones que lo necesiten y dejar polling/consultas normales en las asíncronas.
- Auth integrado (login de jugadores + rol especial de "Game Master").
- Row Level Security — útil para que un jugador no pueda leer/editar los recursos de otro, pero el GM sí vea todo.
- Storage para assets (cartas, sprites, íconos de bestiario).
- Tiene **conector MCP oficial**, así que se puede administrar/consultar la base directamente desde acá cuando lo necesites.

Alternativa si en algún punto prefieres NoSQL más simple: Firebase. Lo dejo como plan B, no como default.

### 3.2 Cliente móvil — Unity (mantener, con arquitectura desacoplada)

Ya tienes avance ahí, así que no lo tiro por la borda. Recomendaciones para que quede "migrable":
- Toda la lógica de reglas (cálculo de daño, cooldowns, costos de economía) debe vivir en el backend o en una capa de C# separable, **no** en scripts pegados a la UI de Unity.
- Usar un cliente HTTP/WebSocket delgado (Supabase C# SDK o REST) para que Unity sea "vista + input" y el backend sea "verdad".
- Existen **puentes MCP para Unity** (community Unity MCP servers) que permiten que yo pueda crear/editar scripts, escenas y prefabs directamente en tu proyecto Unity desde el chat o desde Claude Code. Esto es justo lo que mencionabas de vincular MCP — te conviene configurarlo pronto porque acelera mucho la iteración de las mecánicas por dimensión.

### 3.3 Dashboard Web (GM) — Next.js + Tailwind + Supabase client

- Next.js porque despliega fácil (Vercel), tiene buen soporte para datos en tiempo real vía suscripciones de Supabase, y es el estándar más simple de mantener para un dashboard tipo panel de control.
- Tailwind para no perder tiempo en CSS a mano dado que es un equipo chico.
- Este proyecto queda 100% independiente de Unity — solo consume la misma base de datos.

### 3.4 Herramientas de desarrollo colaborativo

- **GitHub** como repo central (Unity project + backend + dashboard como monorepo o 3 repos, a definir).
- **Claude Code** conectado vía MCP tanto al repo como al proyecto Unity — para que pueda ayudarte a escribir scripts, migraciones de base de datos y componentes del dashboard directamente.
- Supabase MCP conectado para poder revisar/ajustar esquema de base de datos en la conversación sin que tengas que copiar/pegar SQL.

---

## 4. Sincronización por dimensión (tu respuesta de "depende")

Como dijiste que depende de la mecánica, la arquitectura debe soportar dos modos, elegibles por dimensión:

| Modo | Cuándo usarlo | Cómo se implementa |
|---|---|---|
| **Tiempo real** | Dimensión 2 (cooperativo contra jefes, cooldowns por ronda) — el GM y los jugadores necesitan ver el estado del combate al instante | Supabase Realtime channels, un canal por partida/sesión |
| **Asíncrono** | Dimensión 1 (recolección de recursos en mapa grande) — no requiere que todos estén conectados a la vez | Escritura normal a la DB + el dashboard hace polling o se suscribe pero sin urgencia |

Esto significa que el modelo de datos debe tener un campo tipo `sync_mode` por dimensión, y el dashboard/cliente deciden si abren un canal realtime o simplemente consultan.

---

## 5. El desafío del arte — comparativa

Esto es lo más difícil como bien dices, principalmente porque el documento de Dimensiones lista una cantidad **enorme** de temáticas cruzadas (decenas de universos, cientos de personajes/criaturas potenciales). Esto cambia la pregunta de "qué estilo se ve mejor" a "qué estilo es sostenible a esa escala con pocas manos".

| Criterio | Pixel Art 2D | 2D Ilustrado propio | 3D estilizado/low-poly |
|---|---|---|---|
| Costo por asset | Bajo-medio | Alto (cada personaje único requiere ilustración completa) | Medio (pero reutilizable) |
| Escalabilidad a cientos de personajes | Buena si usas sistemas modulares (partes intercambiables) | Mala si son ilustraciones 1 a 1 | Muy buena — un rig/esqueleto sirve para muchos personajes con reskin |
| Consistencia visual entre temáticas tan distintas (Naruto, Marvel, sci-fi, etc.) | Alta — el pixel art "unifica" estéticamente cosas muy distintas | Baja, salvo que definas una guía de estilo muy estricta | Alta — el low-poly también unifica |
| Rendimiento en móvil | Excelente | Excelente (son sprites) | Bueno, pero requiere más optimización |
| Herramientas | Aseprite, Piskel | Procreate, Illustrator, Photoshop | Blender + Unity |
| Tiempo de producción por personaje | Horas | Días | Horas si hay sistema modular (partes, texturas, colores) reutilizado |
| Curva de aprendizaje para ti/equipo ocasional | Baja-media | Alta si buscas calidad | Media-alta |

**Mi recomendación honesta dado el volumen del proyecto:** con la cantidad de contenido que describe el documento (decenas de franquicias, cientos de entidades potenciales entre criaturas, personajes, objetos, armaduras...), un estilo **modular** — ya sea pixel art con sistema de partes intercambiables, o 3D low-poly con rig compartido — es lo único realmente sostenible para un equipo chico y ocasional. La ilustración 2D única por personaje es hermosa pero te puede consumir el proyecto entero solo en arte.

Sugerencia concreta: antes de decidir, hagan una **prueba de pipeline** — tomen un mismo personaje (por ejemplo, uno de la Dimensión 2, un "jefe bacteria") y háganlo en los 3 estilos con un límite de tiempo fijo (ej. 4 horas cada uno). Comparen resultado real y costo real, no solo referencia. Esto lo puedo ayudar a planificar como una fase corta antes de comprometerte.

> Nota aparte, no técnica: el documento incluye muchísimos personajes/franquicias con derechos de autor (Marvel, DC, Naruto, Pokémon, etc.). Para uso personal/familiar como está planteado no hay problema, pero si en algún momento piensan en publicar o monetizar Void, esas referencias directas serían un obstáculo legal real — vale la pena tenerlo en el radar para más adelante, no ahora.

---

## 6. Roadmap propuesto

**Fase 0 — Fundamentos (1-2 semanas)**
- Definir el modelo de datos unificado (jugador, personaje, stats, recurso, dimensión, sync_mode) a partir de lo que ya está en el documento.
- Configurar Supabase (schema inicial + auth + rol GM).
- Configurar MCP: Unity bridge + repo + Supabase.

**Fase 1 — Vertical slice técnico**
- Migrar Dimensión 1 y Dimensión 2 (las que ya tienes en Unity) para que lean/escriban del backend en vez de estado local.
- Confirmar que el modo tiempo real funciona en Dimensión 2 y el asíncrono en Dimensión 1.

**Fase 2 — Dashboard GM v1**
- Next.js mostrando jugadores conectados, su estado en Dimensión 1 y 2, con lectura en tiempo real donde aplique.

**Fase 3 — Prueba de pipeline de arte**
- Prototipo comparativo (los 3 estilos) sobre un mismo personaje, con tiempos y costos reales.
- Decisión de dirección de arte definitiva.

**Fase 4 — Sistemas transversales reutilizables**
- Cartas, economía, alianzas, tratados, bestiario como sistemas genéricos que cualquier dimensión futura pueda usar sin reinventarlos.

**Fase 5 — Contenido y expansión**
- Nuevas dimensiones usando el documento como guía de diseño (que sabemos que "puede evolucionar", así que conviene tratarlo como versión viva, no como spec cerrada).

**Fase 6 — Playtesting**
- Con tu grupo real (primos/hermanos), iterar sobre balance y experiencia.

---

## 7. Próximos pasos concretos

1. Confirmemos el modelo de datos base (te puedo armar el schema de Supabase a partir de las secciones del documento: personaje, stats, hoja, bestiario, carta, recurso, alianza, tratado).
2. Configuramos el MCP de Unity para que pueda ver y editar tu proyecto actual directamente.
3. Empezamos por Fase 0 con algo tangible: el schema de base de datos.

¿Con cuál de estos tres arrancamos primero?
