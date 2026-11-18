import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { ChargeListPage } from './view/ChargeListPage';
import { PaymentConceptFormPage } from './view/PaymentConceptFormPage';
import { PaymentConceptListPage } from './view/PaymentConceptListPage';

// Conceptos de pago y cargos: rutas y entradas del menú.
export const PaymentsRoutes: ModuleRoutes = {
  Name: 'Payments',
  Menu: [
    { Id: 'payment-concepts', Path: '/payment-concepts', LabelKey: 'Nav.Payments', Icon: 'Payments', Permission: 'Payments.View' },
  ],
  Private: (
    <>
      <Route path="payment-concepts" element={<PaymentConceptListPage />} />
      <Route path="payment-concepts/new" element={<PaymentConceptFormPage />} />
      <Route path="payment-concepts/:Id" element={<PaymentConceptFormPage />} />
      <Route path="charges" element={<ChargeListPage />} />
    </>
  ),
};
