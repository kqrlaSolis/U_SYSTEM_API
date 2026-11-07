import { CrudFormPage } from '../../../shared/crud';
import { SubjectConfig } from '../model/SubjectConfig';

// Alta y edición de materias sobre el CRUD genérico.
export function SubjectFormPage() {
  return <CrudFormPage Config={SubjectConfig} />;
}
