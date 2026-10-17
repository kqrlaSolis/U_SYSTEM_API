// Lista de módulos del menú lateral. El permiso es la única condición para verlos;
// la API siempre valida de nuevo (ver docs/Architecture/DesktopMvvm.md).
export type NavigationIconName = 'Home' | 'Users' | 'Careers' | 'Subjects' | 'AcademicTerms' | 'Roles' | 'AuditLogs' | 'Settings';

export interface NavigationModule {
  Id: string;
  Path: string;
  LabelKey: string;
  Icon: NavigationIconName;
  Permission: string | null;
}

export const NavigationModules: NavigationModule[] = [
  {
    Id: 'home',
    Path: '/',
    LabelKey: 'Nav.Home',
    Icon: 'Home',
    Permission: null,
  },
  {
    Id: 'users',
    Path: '/users',
    LabelKey: 'Nav.Users',
    Icon: 'Users',
    Permission: 'Users.View',
  },
  {
    Id: 'careers',
    Path: '/careers',
    LabelKey: 'Nav.Careers',
    Icon: 'Careers',
    Permission: 'Careers.View',
  },
  {
    Id: 'subjects',
    Path: '/subjects',
    LabelKey: 'Nav.Subjects',
    Icon: 'Subjects',
    Permission: 'Subjects.View',
  },
  {
    Id: 'academic-terms',
    Path: '/academic-terms',
    LabelKey: 'Nav.AcademicTerms',
    Icon: 'AcademicTerms',
    Permission: 'AcademicTerms.View',
  },
  {
    Id: 'roles',
    Path: '/roles',
    LabelKey: 'Nav.Roles',
    Icon: 'Roles',
    Permission: 'Roles.View',
  },
  {
    Id: 'audit-logs',
    Path: '/audit-logs',
    LabelKey: 'Nav.AuditLogs',
    Icon: 'AuditLogs',
    Permission: 'AuditLogs.View',
  },
  {
    Id: 'settings',
    Path: '/settings',
    LabelKey: 'Nav.Settings',
    Icon: 'Settings',
    Permission: 'Settings.View',
  },
];
