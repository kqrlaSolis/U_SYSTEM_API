import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { AcademicTermFormPage } from './view/AcademicTermFormPage';
import { AcademicTermListPage } from './view/AcademicTermListPage';

// Periodos: rutas y entradas del menú.
export const AcademicTermsRoutes: ModuleRoutes = {
  Name: 'AcademicTerms',
  Menu: [
    { Id: 'academic-terms', Path: '/academic-terms', LabelKey: 'Nav.AcademicTerms', Icon: 'AcademicTerms', Permission: 'AcademicTerms.View' },
  ],
  Private: (
    <>
      <Route path="academic-terms" element={<AcademicTermListPage />} />
      <Route path="academic-terms/new" element={<AcademicTermFormPage />} />
      <Route path="academic-terms/:Id" element={<AcademicTermFormPage />} />
    </>
  ),
};
