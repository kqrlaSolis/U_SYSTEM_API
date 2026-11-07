import { CrudListPage } from '../../../shared/crud';
import { PaymentConceptConfig } from '../model/PaymentConceptConfig';

// Catálogo de conceptos de pago.
export function PaymentConceptListPage() {
  return <CrudListPage Config={PaymentConceptConfig} />;
}
