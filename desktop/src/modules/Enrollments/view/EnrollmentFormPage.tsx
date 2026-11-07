import { CrudFormPage } from '../../../shared/crud';
import { EnrollmentConfig } from '../model/EnrollmentConfig';

// Alta de inscripción sobre el CRUD genérico (solo lectura una vez creada).
export function EnrollmentFormPage() {
  return <CrudFormPage Config={EnrollmentConfig} />;
}
