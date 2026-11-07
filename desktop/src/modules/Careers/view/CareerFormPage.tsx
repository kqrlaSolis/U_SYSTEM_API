import { CrudFormPage } from '../../../shared/crud';
import { CareerConfig } from '../model/CareerConfig';

// Alta y edición de carreras sobre el CRUD genérico.
export function CareerFormPage() {
  return <CrudFormPage Config={CareerConfig} />;
}
