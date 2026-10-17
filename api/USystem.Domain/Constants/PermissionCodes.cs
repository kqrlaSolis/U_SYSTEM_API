// Catálogo de permisos generado desde tools/DocsGenerator/Rbac.py.
// Formato Module.Action en inglés y PascalCase; solo los comentarios van en español.

namespace USystem.Domain.Constants;

public static class PermissionCodes
{
    public static class Users
    {
        // View users
        public const string View = "Users.View";
        // Create users
        public const string Create = "Users.Create";
        // Edit users and activate or deactivate accounts
        public const string Edit = "Users.Edit";
    }

    public static class Roles
    {
        // View roles and permissions
        public const string View = "Roles.View";
        // Create roles
        public const string Create = "Roles.Create";
        // Edit roles and their permissions
        public const string Edit = "Roles.Edit";
        // Delete or deactivate roles
        public const string Delete = "Roles.Delete";
        // Assign roles to users
        public const string Assign = "Roles.Assign";
    }

    public static class AuditLogs
    {
        // View the audit log
        public const string View = "AuditLogs.View";
    }

    public static class Careers
    {
        // View careers
        public const string View = "Careers.View";
        // Create careers
        public const string Create = "Careers.Create";
        // Edit careers
        public const string Edit = "Careers.Edit";
        // Delete or deactivate careers
        public const string Delete = "Careers.Delete";
    }

    public static class StudyPlans
    {
        // View study plans
        public const string View = "StudyPlans.View";
        // Create study plans
        public const string Create = "StudyPlans.Create";
        // Edit study plans, curriculum and prerequisites
        public const string Edit = "StudyPlans.Edit";
        // Delete or deactivate study plans
        public const string Delete = "StudyPlans.Delete";
    }

    public static class Subjects
    {
        // View subjects
        public const string View = "Subjects.View";
        // Create subjects
        public const string Create = "Subjects.Create";
        // Edit subjects
        public const string Edit = "Subjects.Edit";
        // Delete or deactivate subjects
        public const string Delete = "Subjects.Delete";
    }

    public static class AcademicTerms
    {
        // View academic terms
        public const string View = "AcademicTerms.View";
        // Create academic terms
        public const string Create = "AcademicTerms.Create";
        // Edit academic terms
        public const string Edit = "AcademicTerms.Edit";
        // Open an academic term for enrollment
        public const string Open = "AcademicTerms.Open";
        // Close an academic term
        public const string Close = "AcademicTerms.Close";
    }

    public static class CourseMaterials
    {
        // View course materials
        public const string View = "CourseMaterials.View";
        // Create course materials
        public const string Create = "CourseMaterials.Create";
        // Edit course materials
        public const string Edit = "CourseMaterials.Edit";
        // Delete or deactivate course materials
        public const string Delete = "CourseMaterials.Delete";
    }

    public static class Teachers
    {
        // View teachers
        public const string View = "Teachers.View";
        // Create teachers
        public const string Create = "Teachers.Create";
        // Edit teachers and their enabled subjects
        public const string Edit = "Teachers.Edit";
        // Delete or deactivate teachers
        public const string Delete = "Teachers.Delete";
    }

    public static class ClassGroups
    {
        // View class groups and schedules
        public const string View = "ClassGroups.View";
        // Create class groups
        public const string Create = "ClassGroups.Create";
        // Edit class groups and schedules
        public const string Edit = "ClassGroups.Edit";
        // Delete or cancel class groups
        public const string Delete = "ClassGroups.Delete";
    }

    public static class Students
    {
        // View students
        public const string View = "Students.View";
        // Create students
        public const string Create = "Students.Create";
        // Edit students and change their status
        public const string Edit = "Students.Edit";
        // Delete or deactivate students
        public const string Delete = "Students.Delete";
    }

    public static class StudentDocuments
    {
        // View student documents
        public const string View = "StudentDocuments.View";
        // Upload student documents
        public const string Upload = "StudentDocuments.Upload";
        // Validate student documents
        public const string Validate = "StudentDocuments.Validate";
        // Reject student documents
        public const string Reject = "StudentDocuments.Reject";
    }

    public static class Enrollments
    {
        // View enrollments
        public const string View = "Enrollments.View";
        // Create enrollments
        public const string Create = "Enrollments.Create";
        // Cancel enrollments
        public const string Cancel = "Enrollments.Cancel";
    }

    public static class ReEnrollments
    {
        // View re-enrollments
        public const string View = "ReEnrollments.View";
        // Create re-enrollments
        public const string Create = "ReEnrollments.Create";
        // Cancel re-enrollments
        public const string Cancel = "ReEnrollments.Cancel";
        // Authorize re-enrollment exceptions
        public const string AuthorizeException = "ReEnrollments.AuthorizeException";
    }

    public static class Grades
    {
        // View grades
        public const string View = "Grades.View";
        // Capture grades
        public const string Capture = "Grades.Capture";
        // Modify grades after capture
        public const string Modify = "Grades.Modify";
        // Close a grade report
        public const string CloseReport = "Grades.CloseReport";
        // Reopen a grade report
        public const string ReopenReport = "Grades.ReopenReport";
    }

    public static class Transcript
    {
        // View transcripts
        public const string View = "Transcript.View";
        // Export transcripts to PDF
        public const string Export = "Transcript.Export";
    }

    public static class AcademicProgress
    {
        // View academic progress
        public const string View = "AcademicProgress.View";
    }

    public static class Payments
    {
        // View charges and payments
        public const string View = "Payments.View";
        // Register payments and generate charges
        public const string Register = "Payments.Register";
        // Cancel payments
        public const string Cancel = "Payments.Cancel";
        // Waive charges
        public const string Waive = "Payments.Waive";
    }

    public static class Scholarships
    {
        // View scholarships
        public const string View = "Scholarships.View";
        // Create scholarships
        public const string Create = "Scholarships.Create";
        // Edit scholarships
        public const string Edit = "Scholarships.Edit";
        // Assign scholarships to students
        public const string Assign = "Scholarships.Assign";
    }

    public static class Procedures
    {
        // View procedures
        public const string View = "Procedures.View";
        // Request procedures
        public const string Request = "Procedures.Request";
        // Issue procedure documents
        public const string Issue = "Procedures.Issue";
    }

    public static class Notifications
    {
        // View notifications and templates
        public const string View = "Notifications.View";
        // Send notifications and edit templates
        public const string Send = "Notifications.Send";
    }

    public static class Reports
    {
        // View reports and the dashboard
        public const string View = "Reports.View";
        // Export reports
        public const string Export = "Reports.Export";
    }

    public static class Settings
    {
        // View system settings
        public const string View = "Settings.View";
        // Edit system settings
        public const string Edit = "Settings.Edit";
    }

    // Lista completa: se usa en pruebas y en la validación de roles
    public static readonly IReadOnlyList<string> All = new[]
    {
        "Users.View",
        "Users.Create",
        "Users.Edit",
        "Roles.View",
        "Roles.Create",
        "Roles.Edit",
        "Roles.Delete",
        "Roles.Assign",
        "AuditLogs.View",
        "Careers.View",
        "Careers.Create",
        "Careers.Edit",
        "Careers.Delete",
        "StudyPlans.View",
        "StudyPlans.Create",
        "StudyPlans.Edit",
        "StudyPlans.Delete",
        "Subjects.View",
        "Subjects.Create",
        "Subjects.Edit",
        "Subjects.Delete",
        "AcademicTerms.View",
        "AcademicTerms.Create",
        "AcademicTerms.Edit",
        "AcademicTerms.Open",
        "AcademicTerms.Close",
        "CourseMaterials.View",
        "CourseMaterials.Create",
        "CourseMaterials.Edit",
        "CourseMaterials.Delete",
        "Teachers.View",
        "Teachers.Create",
        "Teachers.Edit",
        "Teachers.Delete",
        "ClassGroups.View",
        "ClassGroups.Create",
        "ClassGroups.Edit",
        "ClassGroups.Delete",
        "Students.View",
        "Students.Create",
        "Students.Edit",
        "Students.Delete",
        "StudentDocuments.View",
        "StudentDocuments.Upload",
        "StudentDocuments.Validate",
        "StudentDocuments.Reject",
        "Enrollments.View",
        "Enrollments.Create",
        "Enrollments.Cancel",
        "ReEnrollments.View",
        "ReEnrollments.Create",
        "ReEnrollments.Cancel",
        "ReEnrollments.AuthorizeException",
        "Grades.View",
        "Grades.Capture",
        "Grades.Modify",
        "Grades.CloseReport",
        "Grades.ReopenReport",
        "Transcript.View",
        "Transcript.Export",
        "AcademicProgress.View",
        "Payments.View",
        "Payments.Register",
        "Payments.Cancel",
        "Payments.Waive",
        "Scholarships.View",
        "Scholarships.Create",
        "Scholarships.Edit",
        "Scholarships.Assign",
        "Procedures.View",
        "Procedures.Request",
        "Procedures.Issue",
        "Notifications.View",
        "Notifications.Send",
        "Reports.View",
        "Reports.Export",
        "Settings.View",
        "Settings.Edit",
    };
}
