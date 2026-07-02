-- ===== Extensiones =====
create extension if not exists pgcrypto;

-- ===== Enums =====
create type public.rol_jugador as enum ('jugador', 'game_master');
create type public.sync_mode as enum ('tiempo_real', 'asincrono');
create type public.tipo_transaccion as enum ('ingreso', 'gasto', 'transferencia');

-- ===== Helper: updated_at automático =====
create or replace function public.set_updated_at()
returns trigger
language plpgsql
as $$
begin
  new.updated_at = now();
  return new;
end;
$$;

-- ===== jugadores =====
-- Un registro por usuario autenticado (1:1 con auth.users).
create table public.jugadores (
  id uuid primary key references auth.users(id) on delete cascade,
  nombre_usuario text not null unique,
  rol public.rol_jugador not null default 'jugador',
  avatar_url text,
  created_at timestamptz not null default now()
);

-- Crea automáticamente el jugador al registrarse en Supabase Auth.
create or replace function public.handle_new_user()
returns trigger
language plpgsql
security definer set search_path = public
as $$
begin
  insert into public.jugadores (id, nombre_usuario)
  values (new.id, coalesce(new.raw_user_meta_data->>'nombre_usuario', split_part(new.email, '@', 1)));
  return new;
end;
$$;

create trigger on_auth_user_created
  after insert on auth.users
  for each row execute function public.handle_new_user();

-- ===== dimensiones (catálogo) =====
create table public.dimensiones (
  id uuid primary key default gen_random_uuid(),
  slug text not null unique,
  nombre text not null,
  descripcion text,
  sync_mode public.sync_mode not null default 'asincrono',
  activa boolean not null default true,
  created_at timestamptz not null default now()
);

-- ===== personajes =====
-- Identidad base de un personaje, transversal a dimensiones.
create table public.personajes (
  id uuid primary key default gen_random_uuid(),
  jugador_id uuid not null references public.jugadores(id) on delete cascade,
  nombre text not null,
  avatar_url text,
  descripcion text,
  created_at timestamptz not null default now()
);
create index on public.personajes (jugador_id);

-- ===== hojas_de_personaje =====
-- Instancia de un personaje dentro de una dimensión concreta (progreso, nivel, etc).
-- 'datos' es un cajón jsonb para campos específicos de cada dimensión que aún no
-- se modelan como columna propia — evita migraciones por cada mecánica nueva.
create table public.hojas_de_personaje (
  id uuid primary key default gen_random_uuid(),
  personaje_id uuid not null references public.personajes(id) on delete cascade,
  dimension_id uuid not null references public.dimensiones(id) on delete cascade,
  nivel int not null default 1,
  experiencia bigint not null default 0,
  datos jsonb not null default '{}'::jsonb,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  unique (personaje_id, dimension_id)
);
create index on public.hojas_de_personaje (personaje_id);
create index on public.hojas_de_personaje (dimension_id);
create trigger set_updated_at before update on public.hojas_de_personaje
  for each row execute function public.set_updated_at();

-- ===== stat_definiciones (catálogo extensible) =====
-- Catálogo de stats. dimension_id nulo = stat transversal (aplica a cualquier dimensión).
create table public.stat_definiciones (
  id uuid primary key default gen_random_uuid(),
  clave text not null unique,
  nombre text not null,
  tipo text not null default 'numero' check (tipo in ('numero', 'texto', 'booleano')),
  descripcion text,
  dimension_id uuid references public.dimensiones(id) on delete cascade,
  created_at timestamptz not null default now()
);
create index on public.stat_definiciones (dimension_id);

-- ===== hoja_stats (valores, patrón EAV) =====
-- Un valor por stat y hoja de personaje. Solo una de las 3 columnas de valor
-- se usa según stat_definiciones.tipo — permite agregar stats nuevos sin migrar schema.
create table public.hoja_stats (
  id uuid primary key default gen_random_uuid(),
  hoja_id uuid not null references public.hojas_de_personaje(id) on delete cascade,
  stat_id uuid not null references public.stat_definiciones(id) on delete cascade,
  valor_numero numeric,
  valor_texto text,
  valor_booleano boolean,
  updated_at timestamptz not null default now(),
  unique (hoja_id, stat_id)
);
create index on public.hoja_stats (hoja_id);
create trigger set_updated_at before update on public.hoja_stats
  for each row execute function public.set_updated_at();

-- ===== recursos (catálogo) =====
create table public.recursos (
  id uuid primary key default gen_random_uuid(),
  slug text not null unique,
  nombre text not null,
  descripcion text,
  icono_url text,
  created_at timestamptz not null default now()
);

-- ===== inventarios =====
-- Saldo actual de cada recurso por jugador (la economía es a nivel jugador,
-- no por personaje, ya que recursos/alianzas se comparten entre personajes).
create table public.inventarios (
  id uuid primary key default gen_random_uuid(),
  jugador_id uuid not null references public.jugadores(id) on delete cascade,
  recurso_id uuid not null references public.recursos(id) on delete cascade,
  cantidad numeric not null default 0 check (cantidad >= 0),
  updated_at timestamptz not null default now(),
  unique (jugador_id, recurso_id)
);
create index on public.inventarios (jugador_id);
create trigger set_updated_at before update on public.inventarios
  for each row execute function public.set_updated_at();

-- ===== transacciones_economia (log inmutable) =====
-- origen/destino nulos = movimiento desde/hacia "el sistema" (recompensas, costos fijos).
create table public.transacciones_economia (
  id uuid primary key default gen_random_uuid(),
  tipo public.tipo_transaccion not null,
  recurso_id uuid not null references public.recursos(id),
  cantidad numeric not null check (cantidad > 0),
  origen_jugador_id uuid references public.jugadores(id),
  destino_jugador_id uuid references public.jugadores(id),
  dimension_id uuid references public.dimensiones(id),
  motivo text,
  created_at timestamptz not null default now()
);
create index on public.transacciones_economia (origen_jugador_id);
create index on public.transacciones_economia (destino_jugador_id);

-- ===== Row Level Security =====

-- Helper para políticas: evita recursión en RLS de jugadores gracias a security definer.
create or replace function public.es_game_master()
returns boolean
language sql
stable
security definer set search_path = public
as $$
  select exists (
    select 1 from public.jugadores
    where id = auth.uid() and rol = 'game_master'
  );
$$;

alter table public.jugadores enable row level security;
alter table public.dimensiones enable row level security;
alter table public.personajes enable row level security;
alter table public.hojas_de_personaje enable row level security;
alter table public.stat_definiciones enable row level security;
alter table public.hoja_stats enable row level security;
alter table public.recursos enable row level security;
alter table public.inventarios enable row level security;
alter table public.transacciones_economia enable row level security;

-- jugadores: cada quien ve/edita su fila, el GM ve todas.
create policy "jugadores_select_propio_o_gm" on public.jugadores
  for select using (id = auth.uid() or public.es_game_master());
create policy "jugadores_update_propio" on public.jugadores
  for update using (id = auth.uid()) with check (id = auth.uid() and rol = 'jugador');

-- catálogos: lectura abierta a cualquier usuario autenticado, escritura solo GM.
create policy "dimensiones_select_auth" on public.dimensiones
  for select to authenticated using (true);
create policy "dimensiones_write_gm" on public.dimensiones
  for all using (public.es_game_master()) with check (public.es_game_master());

create policy "stat_definiciones_select_auth" on public.stat_definiciones
  for select to authenticated using (true);
create policy "stat_definiciones_write_gm" on public.stat_definiciones
  for all using (public.es_game_master()) with check (public.es_game_master());

create policy "recursos_select_auth" on public.recursos
  for select to authenticated using (true);
create policy "recursos_write_gm" on public.recursos
  for all using (public.es_game_master()) with check (public.es_game_master());

-- personajes: dueño (via jugador_id) o GM.
create policy "personajes_all_propio_o_gm" on public.personajes
  for all using (jugador_id = auth.uid() or public.es_game_master())
  with check (jugador_id = auth.uid() or public.es_game_master());

-- hojas_de_personaje / hoja_stats: dueño del personaje asociado, o GM.
create policy "hojas_all_propio_o_gm" on public.hojas_de_personaje
  for all using (
    public.es_game_master()
    or exists (select 1 from public.personajes p where p.id = personaje_id and p.jugador_id = auth.uid())
  )
  with check (
    public.es_game_master()
    or exists (select 1 from public.personajes p where p.id = personaje_id and p.jugador_id = auth.uid())
  );

create policy "hoja_stats_all_propio_o_gm" on public.hoja_stats
  for all using (
    public.es_game_master()
    or exists (
      select 1 from public.hojas_de_personaje h
      join public.personajes p on p.id = h.personaje_id
      where h.id = hoja_id and p.jugador_id = auth.uid()
    )
  )
  with check (
    public.es_game_master()
    or exists (
      select 1 from public.hojas_de_personaje h
      join public.personajes p on p.id = h.personaje_id
      where h.id = hoja_id and p.jugador_id = auth.uid()
    )
  );

-- inventarios: dueño o GM. Sin policy de insert/update para 'jugador' normal
-- más allá de lo que ya cubre "propio" — los cambios de saldo reales deberían
-- pasar por una función validada (Edge Function/RPC), no escritura directa libre.
create policy "inventarios_select_propio_o_gm" on public.inventarios
  for select using (jugador_id = auth.uid() or public.es_game_master());
create policy "inventarios_write_gm" on public.inventarios
  for all using (public.es_game_master()) with check (public.es_game_master());

-- transacciones_economia: solo lectura de las propias (como origen o destino) o GM.
-- Sin policy de insert para clientes: las transacciones se crean vía función validada.
create policy "transacciones_select_propias_o_gm" on public.transacciones_economia
  for select using (
    origen_jugador_id = auth.uid() or destino_jugador_id = auth.uid() or public.es_game_master()
  );
