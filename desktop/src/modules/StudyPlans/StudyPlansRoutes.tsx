import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { StudyPlanFormPage } from './view/StudyPlanFormPage';
import { StudyPlanListPage } from './view/StudyPlanListPage';

// Planes de estudio: rutas y entradas del menú.
export const StudyPlansRoutes: ModuleRoutes = {
  Name: 'StudyPlans',
  Menu: [
    { Id: 'study-plans', Path: '/study-plans', LabelKey: 'Nav.StudyPlans', Icon: 'StudyPlans', Permission: 'StudyPlans.View' },
  ],
  Private: (
    <>
      <Route path="study-plans" element={<StudyPlanListPage />} />
      <Route path="study-plans/new" element={<StudyPlanFormPage />} />
      <Route path="study-plans/:Id" element={<StudyPlanFormPage />} />
    </>
  ),
};
