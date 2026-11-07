import { CrudFormPage } from '../../../shared/crud';
import { ClassGroupConfig } from '../model/ClassGroupConfig';

// Alta y edición de grupos con horarios ("1 08:00-10:00; 3 08:00-10:00").
export function ClassGroupFormPage() {
  return <CrudFormPage Config={ClassGroupConfig} />;
}
