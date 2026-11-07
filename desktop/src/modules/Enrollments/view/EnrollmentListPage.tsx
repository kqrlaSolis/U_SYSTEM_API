import { CrudListPage } from '../../../shared/crud';
import { EnrollmentConfig } from '../model/EnrollmentConfig';

// Listado de inscripciones sobre el CRUD genérico.
export function EnrollmentListPage() {
  return <CrudListPage Config={EnrollmentConfig} />;
}
