import { Navigate, Route, Routes } from 'react-router-dom';
import { AcademicTermFormPage } from '../modules/AcademicTerms/view/AcademicTermFormPage';
import { AcademicTermListPage } from '../modules/AcademicTerms/view/AcademicTermListPage';
import { AuditLogListPage } from '../modules/AuditLogs/view/AuditLogListPage';
import { ChangePasswordPage } from '../modules/Auth/view/ChangePasswordPage';
import { LoginPage } from '../modules/Auth/view/LoginPage';
import { ProfilePage } from '../modules/Auth/view/ProfilePage';
import { CareerFormPage } from '../modules/Careers/view/CareerFormPage';
import { CareerListPage } from '../modules/Careers/view/CareerListPage';
import { HomePage } from '../modules/Home/view/HomePage';
import { RoleFormPage } from '../modules/Roles/view/RoleFormPage';
import { RoleListPage } from '../modules/Roles/view/RoleListPage';
import { SettingsPage } from '../modules/Settings/view/SettingsPage';
import { SubjectFormPage } from '../modules/Subjects/view/SubjectFormPage';
import { SubjectListPage } from '../modules/Subjects/view/SubjectListPage';
import { UserFormPage } from '../modules/Users/view/UserFormPage';
import { UserListPage } from '../modules/Users/view/UserListPage';
import { RequireAuth } from '../shared/auth/RequireAuth';
import { MainLayout } from './layout/MainLayout';

// Rutas en minúsculas con guiones. /login es público; el resto exige sesión.
export function Router() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

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
        <Route path="settings" element={<SettingsPage />} />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
