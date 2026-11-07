import { CrudFormPage } from '../../../shared/crud';
import { CourseMaterialConfig } from '../model/CourseMaterialConfig';

// Alta y edición de material didáctico sobre el CRUD genérico.
export function CourseMaterialFormPage() {
  return <CrudFormPage Config={CourseMaterialConfig} />;
}
