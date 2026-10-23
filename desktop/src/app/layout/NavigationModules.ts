// Lista de módulos del menú lateral. El permiso es la única condición para verlos;
// la API siempre valida de nuevo (ver docs/Architecture/DesktopMvvm.md).
export type NavigationIconName = 'Home' | 'Dashboard' | 'Users' | 'Careers' | 'Subjects' | 'AcademicTerms' | 'StudyPlans' | 'Teachers' | 'Students' | 'CourseMaterials' | 'Enrollments' | 'ClassGroups' | 'Grades' | 'Tracking' | 'ReEnrollments' | 'Notifications' | 'Procedures' | 'Payments' | 'Roles' | 'AuditLogs' | 'Settings';

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
    Id: 'dashboard',
    Path: '/dashboard',
    LabelKey: 'Nav.Dashboard',
    Icon: 'Dashboard',
    Permission: 'Reports.View',
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
    Id: 'study-plans',
    Path: '/study-plans',
    LabelKey: 'Nav.StudyPlans',
    Icon: 'StudyPlans',
    Permission: 'StudyPlans.View',
  },
  {
    Id: 'teachers',
    Path: '/teachers',
    LabelKey: 'Nav.Teachers',
    Icon: 'Teachers',
    Permission: 'Teachers.View',
  },
  {
    Id: 'students',
    Path: '/students',
    LabelKey: 'Nav.Students',
    Icon: 'Students',
    Permission: 'Students.View',
  },
  {
    Id: 'course-materials',
    Path: '/course-materials',
    LabelKey: 'Nav.CourseMaterials',
    Icon: 'CourseMaterials',
    Permission: 'CourseMaterials.View',
  },
  {
    Id: 'enrollments',
    Path: '/enrollments',
    LabelKey: 'Nav.Enrollments',
    Icon: 'Enrollments',
    Permission: 'Enrollments.View',
  },
  {
    Id: 'class-groups',
    Path: '/class-groups',
    LabelKey: 'Nav.ClassGroups',
    Icon: 'ClassGroups',
    Permission: 'ClassGroups.View',
  },
  {
    Id: 'enrollment-wizard',
    Path: '/enrollment-wizard',
    LabelKey: 'Nav.EnrollmentWizard',
    Icon: 'Enrollments',
    Permission: 'Enrollments.Create',
  },
  {
    Id: 'grades',
    Path: '/grades',
    LabelKey: 'Nav.Grades',
    Icon: 'Grades',
    Permission: 'Grades.View',
  },
  {
    Id: 'tracking',
    Path: '/tracking',
    LabelKey: 'Nav.Tracking',
    Icon: 'Tracking',
    Permission: 'Grades.View',
  },
  {
    Id: 're-enrollments',
    Path: '/re-enrollments',
    LabelKey: 'Nav.ReEnrollments',
    Icon: 'ReEnrollments',
    Permission: 'Enrollments.Create',
  },
  {
    Id: 'notifications',
    Path: '/notifications',
    LabelKey: 'Nav.Notifications',
    Icon: 'Notifications',
    Permission: 'Notifications.View',
  },
  {
    Id: 'procedure-types',
    Path: '/procedure-types',
    LabelKey: 'Nav.Procedures',
    Icon: 'Procedures',
    Permission: 'Procedures.View',
  },
  {
    Id: 'payment-concepts',
    Path: '/payment-concepts',
    LabelKey: 'Nav.Payments',
    Icon: 'Payments',
    Permission: 'Payments.View',
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
