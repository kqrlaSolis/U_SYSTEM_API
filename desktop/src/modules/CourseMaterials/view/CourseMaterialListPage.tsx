import { CrudListPage } from '../../../shared/crud';
import { CourseMaterialConfig } from '../model/CourseMaterialConfig';

// Listado de material didáctico sobre el CRUD genérico.
export function CourseMaterialListPage() {
  return <CrudListPage Config={CourseMaterialConfig} />;
}
