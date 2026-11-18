import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { CareerFormPage } from './view/CareerFormPage';
import { CareerListPage } from './view/CareerListPage';

// Carreras: rutas y entradas del menú.
export const CareersRoutes: ModuleRoutes = {
  Name: 'Careers',
  Menu: [
    { Id: 'careers', Path: '/careers', LabelKey: 'Nav.Careers', Icon: 'Careers', Permission: 'Careers.View' },
  ],
  Private: (
    <>
      <Route path="careers" element={<CareerListPage />} />
      <Route path="careers/new" element={<CareerFormPage />} />
      <Route path="careers/:Id" element={<CareerFormPage />} />
    </>
  ),
};
