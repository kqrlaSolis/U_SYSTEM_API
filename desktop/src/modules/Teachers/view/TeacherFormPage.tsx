import { CrudFormPage } from '../../../shared/crud';
import { TeacherConfig } from '../model/TeacherConfig';

// Alta y edición de docentes sobre el CRUD genérico.
export function TeacherFormPage() {
  return <CrudFormPage Config={TeacherConfig} />;
}
