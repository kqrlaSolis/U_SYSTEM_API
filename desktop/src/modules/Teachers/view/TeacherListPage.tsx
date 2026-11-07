import { CrudListPage } from '../../../shared/crud';
import { TeacherConfig } from '../model/TeacherConfig';

// Listado de docentes sobre el CRUD genérico.
export function TeacherListPage() {
  return <CrudListPage Config={TeacherConfig} />;
}
