import { CrudListPage } from '../../../shared/crud';
import { ProcedureTypeConfig } from '../model/ProcedureTypeConfig';

// Catálogo de tipos de trámite.
export function ProcedureTypeListPage() {
  return <CrudListPage Config={ProcedureTypeConfig} />;
}
