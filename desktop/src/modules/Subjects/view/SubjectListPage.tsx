import { CrudListPage } from '../../../shared/crud';
import { SubjectConfig } from '../model/SubjectConfig';

// Listado de materias sobre el CRUD genérico.
export function SubjectListPage() {
  return <CrudListPage Config={SubjectConfig} />;
}
