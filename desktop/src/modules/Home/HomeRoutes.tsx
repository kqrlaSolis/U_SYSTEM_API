import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { HomePage } from './view/HomePage';

// Inicio: rutas y entradas del menú.
export const HomeRoutes: ModuleRoutes = {
  Name: 'Home',
  Menu: [
    { Id: 'home', Path: '/', LabelKey: 'Nav.Home', Icon: 'Home', Permission: null },
  ],
  Private: <Route index element={<HomePage />} />,
};
