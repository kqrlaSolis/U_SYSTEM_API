import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { ProcedureListPage } from './view/ProcedureListPage';
import { ProcedureTypeFormPage } from './view/ProcedureTypeFormPage';
import { ProcedureTypeListPage } from './view/ProcedureTypeListPage';

// Tipos de trámite y trámites: rutas y entradas del menú.
export const ProceduresRoutes: ModuleRoutes = {
  Name: 'Procedures',
  Menu: [
    { Id: 'procedure-types', Path: '/procedure-types', LabelKey: 'Nav.Procedures', Icon: 'Procedures', Permission: 'Procedures.View' },
  ],
  Private: (
    <>
      <Route path="procedure-types" element={<ProcedureTypeListPage />} />
      <Route path="procedure-types/new" element={<ProcedureTypeFormPage />} />
      <Route path="procedure-types/:Id" element={<ProcedureTypeFormPage />} />
      <Route path="procedures" element={<ProcedureListPage />} />
    </>
  ),
};
