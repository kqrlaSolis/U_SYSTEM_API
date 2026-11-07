import { CrudFormPage } from '../../../shared/crud';
import { StudentConfig } from '../model/StudentConfig';

// Alta y edición de alumnos sobre el CRUD genérico (la matrícula la asigna la API).
export function StudentFormPage() {
  return <CrudFormPage Config={StudentConfig} />;
}