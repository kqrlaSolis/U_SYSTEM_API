import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { CourseMaterialFormPage } from './view/CourseMaterialFormPage';
import { CourseMaterialListPage } from './view/CourseMaterialListPage';

// Material didáctico: rutas y entradas del menú.
export const CourseMaterialsRoutes: ModuleRoutes = {
  Name: 'CourseMaterials',
  Menu: [
    { Id: 'course-materials', Path: '/course-materials', LabelKey: 'Nav.CourseMaterials', Icon: 'CourseMaterials', Permission: 'CourseMaterials.View' },
  ],
  Private: (
    <>
      <Route path="course-materials" element={<CourseMaterialListPage />} />
      <Route path="course-materials/new" element={<CourseMaterialFormPage />} />
      <Route path="course-materials/:Id" element={<CourseMaterialFormPage />} />
    </>
  ),
};
