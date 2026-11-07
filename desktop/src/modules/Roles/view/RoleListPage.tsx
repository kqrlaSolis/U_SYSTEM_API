import { CrudListPage } from '../../../shared/crud';
import { RoleConfig } from '../model/RoleConfig';

// Listado de roles: usa el CRUD genérico y el formulario propio con editor de permisos.
export function RoleListPage() {
  return <CrudListPage Config={RoleConfig} />;
}
