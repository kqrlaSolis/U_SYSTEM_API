import { CrudFormPage } from '../../../shared/crud';
import { PaymentConceptConfig } from '../model/PaymentConceptConfig';

// Alta y edición de conceptos de pago.
export function PaymentConceptFormPage() {
  return <CrudFormPage Config={PaymentConceptConfig} />;
}
