import { AppRoutes } from '../AppRoutes';

// Entradas del menú lateral. Cada módulo declara las suyas en modules/<Module>/<Module>Routes.tsx.
// El permiso es la única condición para verlas; la API siempre valida de nuevo
// (ver docs/Architecture/Desktop.md).
export type NavigationIconName = 'Home' | 'Dashboard' | 'Users' | 'Careers' | 'Subjects' | 'AcademicTerms' | 'StudyPlans' | 'Teachers' | 'Students' | 'CourseMaterials' | 'Enrollments' | 'ClassGroups' | 'Grades' | 'Tracking' | 'ReEnrollments' | 'Notifications' | 'Procedures' | 'Payments' | 'Roles' | 'AuditLogs' | 'Settings';

export interface NavigationModule {
  Id: string;
  Path: string;
  LabelKey: string;
  Icon: NavigationIconName;
  Permission: string | null;
}

// Se arma al llamarla (no al importar el archivo): Home usa el menú y a la vez es un módulo de la
// lista, así que leer AppRoutes al cargar este archivo fallaría por la dependencia circular.
export function GetNavigationModules(): NavigationModule[] {
  return AppRoutes.flatMap((Module) => Module.Menu ?? []);
}
