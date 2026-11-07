import { CrudListPage } from '../../../shared/crud';
import { ClassGroupConfig } from '../model/ClassGroupConfig';

// Listado de grupos de clase sobre el CRUD genérico.
export function ClassGroupListPage() {
  return <CrudListPage Config={ClassGroupConfig} />;
}
