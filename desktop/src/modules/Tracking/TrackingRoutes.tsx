import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { TrackingPage } from './view/TrackingPage';

// Kardex y avance académico: rutas y entradas del menú.
export const TrackingRoutes: ModuleRoutes = {
  Name: 'Tracking',
  Menu: [
    { Id: 'tracking', Path: '/tracking', LabelKey: 'Nav.Tracking', Icon: 'Tracking', Permission: 'Transcript.View' },
  ],
  Private: <Route path="tracking" element={<TrackingPage />} />,
};
