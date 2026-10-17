import { CrudListPage } from '../../../shared/crud';
import { CareerConfig } from '../model/CareerConfig';

// Listado de carreras sobre el CRUD genérico.
export function CareerListPage() {
  return <CrudListPage Config={CareerConfig} />;
}
