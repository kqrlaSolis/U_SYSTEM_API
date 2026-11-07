import { CrudListPage } from '../../../shared/crud';
import { StudentConfig } from '../model/StudentConfig';

// Listado de alumnos sobre el CRUD genérico.
export function StudentListPage() {
  return <CrudListPage Config={StudentConfig} />;
}