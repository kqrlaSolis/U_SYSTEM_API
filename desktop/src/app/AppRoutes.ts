import type { ModuleRoutes } from './ModuleRoutes';
import { AcademicTermsRoutes } from '../modules/AcademicTerms/AcademicTermsRoutes';
import { AuditLogsRoutes } from '../modules/AuditLogs/AuditLogsRoutes';
import { AuthRoutes } from '../modules/Auth/AuthRoutes';
import { CareersRoutes } from '../modules/Careers/CareersRoutes';
import { ClassGroupsRoutes } from '../modules/ClassGroups/ClassGroupsRoutes';
import { CourseMaterialsRoutes } from '../modules/CourseMaterials/CourseMaterialsRoutes';
import { EnrollmentsRoutes } from '../modules/Enrollments/EnrollmentsRoutes';
import { GradesRoutes } from '../modules/Grades/GradesRoutes';
import { HomeRoutes } from '../modules/Home/HomeRoutes';
import { NotificationsRoutes } from '../modules/Notifications/NotificationsRoutes';
import { PaymentsRoutes } from '../modules/Payments/PaymentsRoutes';
import { ProceduresRoutes } from '../modules/Procedures/ProceduresRoutes';
import { ReEnrollmentsRoutes } from '../modules/ReEnrollments/ReEnrollmentsRoutes';
import { ReportsRoutes } from '../modules/Reports/ReportsRoutes';
import { RolesRoutes } from '../modules/Roles/RolesRoutes';
import { SettingsRoutes } from '../modules/Settings/SettingsRoutes';
import { StudentsRoutes } from '../modules/Students/StudentsRoutes';
import { StudyPlansRoutes } from '../modules/StudyPlans/StudyPlansRoutes';
import { SubjectsRoutes } from '../modules/Subjects/SubjectsRoutes';
import { TeachersRoutes } from '../modules/Teachers/TeachersRoutes';
import { TrackingRoutes } from '../modules/Tracking/TrackingRoutes';
import { UsersRoutes } from '../modules/Users/UsersRoutes';

// Rutas de todos los módulos, en el orden en que aparecen en el menú lateral.
// Para agregar un módulo: crea modules/<Module>/<Module>Routes.tsx y súmalo aquí.
export const AppRoutes: ModuleRoutes[] = [
  HomeRoutes,
  ReportsRoutes,
  UsersRoutes,
  CareersRoutes,
  SubjectsRoutes,
  AcademicTermsRoutes,
  StudyPlansRoutes,
  TeachersRoutes,
  StudentsRoutes,
  CourseMaterialsRoutes,
  ClassGroupsRoutes,
  EnrollmentsRoutes,
  GradesRoutes,
  TrackingRoutes,
  ReEnrollmentsRoutes,
  NotificationsRoutes,
  ProceduresRoutes,
  PaymentsRoutes,
  RolesRoutes,
  AuditLogsRoutes,
  SettingsRoutes,
  AuthRoutes,
];
