-- Fijar search_path explícito (evita hijacking vía search_path mutable).
create or replace function public.set_updated_at()
returns trigger
language plpgsql
set search_path = public
as $$
begin
  new.updated_at = now();
  return new;
end;
$$;

-- handle_new_user solo debe ejecutarse como trigger de auth.users, nunca como RPC público.
revoke execute on function public.handle_new_user() from public, anon, authenticated;

-- es_game_master se usa dentro de policies (necesita ser ejecutable por 'authenticated'),
-- pero no tiene sentido exponerlo como RPC anónimo.
revoke execute on function public.es_game_master() from public, anon;
