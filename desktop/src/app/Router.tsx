import { Navigate, Route, Routes } from 'react-router-dom';
import { AcademicTermFormPage } from '../modules/AcademicTerms/view/AcademicTermFormPage';
import { AcademicTermListPage } from '../modules/AcademicTerms/view/AcademicTermListPage';
import { AuditLogListPage } from '../modules/AuditLogs/view/AuditLogListPage';
import { ChangePasswordPage } from '../modules/Auth/view/ChangePasswordPage';
import { ForgotPasswordPage } from '../modules/Auth/view/ForgotPasswordPage';
import { LoginPage } from '../modules/Auth/view/LoginPage';
import { ResetPasswordPage } from '../modules/Auth/view/ResetPasswordPage';
import { ProfilePage } from '../modules/Auth/view/ProfilePage';
import { CareerFormPage } from '../modules/Careers/view/CareerFormPage';
import { CareerListPage } from '../modules/Careers/view/CareerListPage';
import { GradesPage } from '../modules/Grades/view/GradesPage';
import { HomePage } from '../modules/Home/view/HomePage';
import { NotificationFormPage } from '../modules/Notifications/view/NotificationFormPage';
import { NotificationListPage } from '../modules/Notifications/view/NotificationListPage';
import { ChargeListPage } from '../modules/Payments/view/ChargeListPage';
import { PaymentConceptFormPage } from '../modules/Payments/view/PaymentConceptFormPage';
import { PaymentConceptListPage } from '../modules/Payments/view/PaymentConceptListPage';
import { ProcedureListPage } from '../modules/Procedures/view/ProcedureListPage';
import { ProcedureTypeFormPage } from '../modules/Procedures/view/ProcedureTypeFormPage';
import { ProcedureTypeListPage } from '../modules/Procedures/view/ProcedureTypeListPage';
import { DashboardPage } from '../modules/Reports/view/DashboardPage';
import { ReEnrollmentPage } from '../modules/ReEnrollments/view/ReEnrollmentPage';
import { RoleFormPage } from '../modules/Roles/view/RoleFormPage';
import { RoleListPage } from '../modules/Roles/view/RoleListPage';
import { SettingsPage } from '../modules/Settings/view/SettingsPage';
import { StudyPlanFormPage } from '../modules/StudyPlans/view/StudyPlanFormPage';
import { StudyPlanListPage } from '../modules/StudyPlans/view/StudyPlanListPage';
import { SubjectFormPage } from '../modules/Subjects/view/SubjectFormPage';
import { SubjectListPage } from '../modules/Subjects/view/SubjectListPage';
import { ClassGroupFormPage } from '../modules/ClassGroups/view/ClassGroupFormPage';
import { ClassGroupListPage } from '../modules/ClassGroups/view/ClassGroupListPage';
import { CourseMaterialFormPage } from '../modules/CourseMaterials/view/CourseMaterialFormPage';
import { EnrollmentFormPage } from '../modules/Enrollments/view/EnrollmentFormPage';
import { EnrollmentWizardPage } from '../modules/Enrollments/view/EnrollmentWizardPage';
import { EnrollmentListPage } from '../modules/Enrollments/view/EnrollmentListPage';
import { CourseMaterialListPage } from '../modules/CourseMaterials/view/CourseMaterialListPage';
import { StudentFormPage } from '../modules/Students/view/StudentFormPage';
import { TrackingPage } from '../modules/Tracking/view/TrackingPage';
import { StudentListPage } from '../modules/Students/view/StudentListPage';
import { TeacherFormPage } from '../modules/Teachers/view/TeacherFormPage';
import { TeacherListPage } from '../modules/Teachers/view/TeacherListPage';
import { UserFormPage } from '../modules/Users/view/UserFormPage';
import { UserListPage } from '../modules/Users/view/UserListPage';
import { RequireAuth } from '../shared/auth/RequireAuth';
import { MainLayout } from './layout/MainLayout';

// Rutas en minúsculas con guiones. /login es público; el resto exige sesión.
export function Router() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/forgot-password" element={<ForgotPasswordPage />} />
      <Route path="/reset-password" element={<ResetPasswordPage />} />

      <Route
        element={
          <RequireAuth>
            <MainLayout />
          </RequireAuth>
        }
      >
        <Route index element={<HomePage />} />
        <Route path="profile" element={<ProfilePage />} />
        <Route path="change-password" element={<ChangePasswordPage />} />
        <Route path="users" element={<UserListPage />} />
        <Route path="users/new" element={<UserFormPage />} />
        <Route path="users/:Id" element={<UserFormPage />} />
        <Route path="careers" element={<CareerListPage />} />
        <Route path="careers/new" element={<CareerFormPage />} />
        <Route path="careers/:Id" element={<CareerFormPage />} />
        <Route path="subjects" element={<SubjectListPage />} />
        <Route path="subjects/new" element={<SubjectFormPage />} />
        <Route path="subjects/:Id" element={<SubjectFormPage />} />
        <Route path="academic-terms" element={<AcademicTermListPage />} />
        <Route path="academic-terms/new" element={<AcademicTermFormPage />} />
        <Route path="academic-terms/:Id" element={<AcademicTermFormPage />} />
        <Route path="roles" element={<RoleListPage />} />
        <Route path="roles/new" element={<RoleFormPage />} />
        <Route path="roles/:Id" element={<RoleFormPage />} />
        <Route path="audit-logs" element={<AuditLogListPage />} />
        <Route path="study-plans" element={<StudyPlanListPage />} />
        <Route path="study-plans/new" element={<StudyPlanFormPage />} />
        <Route path="study-plans/:Id" element={<StudyPlanFormPage />} />
        <Route path="teachers" element={<TeacherListPage />} />
        <Route path="teachers/new" element={<TeacherFormPage />} />
        <Route path="teachers/:Id" element={<TeacherFormPage />} />
        <Route path="students" element={<StudentListPage />} />
        <Route path="students/new" element={<StudentFormPage />} />
        <Route path="students/:Id" element={<StudentFormPage />} />
        <Route path="course-materials" element={<CourseMaterialListPage />} />
        <Route path="course-materials/new" element={<CourseMaterialFormPage />} />
        <Route path="course-materials/:Id" element={<CourseMaterialFormPage />} />
        <Route path="enrollment-wizard" element={<EnrollmentWizardPage />} />
        <Route path="enrollments" element={<EnrollmentListPage />} />
        <Route path="enrollments/new" element={<EnrollmentFormPage />} />
        <Route path="enrollments/:Id" element={<EnrollmentFormPage />} />
        <Route path="class-groups" element={<ClassGroupListPage />} />
        <Route path="class-groups/new" element={<ClassGroupFormPage />} />
        <Route path="class-groups/:Id" element={<ClassGroupFormPage />} />
        <Route path="grades" element={<GradesPage />} />
        <Route path="tracking" element={<TrackingPage />} />
        <Route path="re-enrollments" element={<ReEnrollmentPage />} />
        <Route path="notifications" element={<NotificationListPage />} />
        <Route path="notifications/new" element={<NotificationFormPage />} />
        <Route path="notifications/:Id" element={<NotificationFormPage />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="charges" element={<ChargeListPage />} />
        <Route path="payment-concepts" element={<PaymentConceptListPage />} />
        <Route path="payment-concepts/new" element={<PaymentConceptFormPage />} />
        <Route path="payment-concepts/:Id" element={<PaymentConceptFormPage />} />
        <Route path="procedures" element={<ProcedureListPage />} />
        <Route path="procedure-types" element={<ProcedureTypeListPage />} />
        <Route path="procedure-types/new" element={<ProcedureTypeFormPage />} />
        <Route path="procedure-types/:Id" element={<ProcedureTypeFormPage />} />
        <Route path="settings" element={<SettingsPage />} />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
