import { CrudFormPage } from '../../../shared/crud';
import { ProcedureTypeConfig } from '../model/ProcedureTypeConfig';

// Alta y edición de tipos de trámite.
export function ProcedureTypeFormPage() {
  return <CrudFormPage Config={ProcedureTypeConfig} />;
}
