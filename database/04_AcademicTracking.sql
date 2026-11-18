-- =====================================================================
-- U SYSTEM | Seguimiento académico y vistas
-- Motor: MySQL 8.x | Tablas y columnas en PascalCase (inglés)
-- Regla del proyecto: solo los comentarios van en español
-- =====================================================================
USE USystem;

CREATE TABLE IF NOT EXISTS Grades (
  Id BIGINT NOT NULL AUTO_INCREMENT,
  EnrollmentId BIGINT NOT NULL,
  ClassGroupId INT NOT NULL,
  Partial1 DECIMAL(5,2) NULL,
  Partial2 DECIMAL(5,2) NULL,
  Partial3 DECIMAL(5,2) NULL,
  Ordinary DECIMAL(5,2) NULL,
  FinalScore DECIMAL(5,2) NULL,
  AttemptNumber TINYINT NOT NULL DEFAULT 1,
  EvaluationType VARCHAR(20) NOT NULL DEFAULT 'Ordinary',
  Status VARCHAR(20) NOT NULL DEFAULT 'InProgress',
  IsReportClosed TINYINT(1) NOT NULL DEFAULT 0,
  ClosedAt DATETIME NULL,
  ClosedBy BIGINT NULL,
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CreatedBy BIGINT NULL,
  UpdatedAt DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
  UpdatedBy BIGINT NULL,
  DeletedAt DATETIME NULL,
  PRIMARY KEY (Id),
  UNIQUE KEY UqGradesEnrollmentGroupAttempt (EnrollmentId, ClassGroupId, AttemptNumber),
  CONSTRAINT FkGradesEnrollmentId FOREIGN KEY (EnrollmentId) REFERENCES Enrollments (Id) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT FkGradesClassGroupId FOREIGN KEY (ClassGroupId) REFERENCES ClassGroups (Id) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT FkGradesClosedBy FOREIGN KEY (ClosedBy) REFERENCES Users (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


-- Kardex por alumno: una fila por materia cursada y evaluación
CREATE OR REPLACE VIEW TranscriptView AS
SELECT
  Student.Id AS StudentId,
  Student.StudentNumber,
  CONCAT_WS(' ', Student.FirstName, Student.PaternalLastName, Student.MaternalLastName) AS FullName,
  Career.Code AS CareerCode,
  AcademicTerm.Code AS AcademicTermCode,
  Subject.Code AS SubjectCode,
  Subject.Name AS SubjectName,
  Subject.Credits,
  Grade.FinalScore,
  Grade.EvaluationType,
  Grade.AttemptNumber,
  Grade.Status,
  Grade.IsReportClosed
FROM Grades AS Grade
JOIN Enrollments AS Enrollment ON Enrollment.Id = Grade.EnrollmentId
JOIN Students AS Student ON Student.Id = Enrollment.StudentId
JOIN Careers AS Career ON Career.Id = Student.CareerId
JOIN AcademicTerms AS AcademicTerm ON AcademicTerm.Id = Enrollment.AcademicTermId
JOIN ClassGroups AS ClassGroup ON ClassGroup.Id = Grade.ClassGroupId
JOIN Subjects AS Subject ON Subject.Id = ClassGroup.SubjectId
WHERE Enrollment.Status = 'Active'
  AND Enrollment.DeletedAt IS NULL
  AND Grade.DeletedAt IS NULL;

-- Avance académico: cada materia cuenta una sola vez con su mejor calificación
CREATE OR REPLACE VIEW AcademicProgressView AS
SELECT
  Student.Id AS StudentId,
  Student.StudentNumber,
  Student.CareerId,
  StudyPlan.TotalCredits AS PlanTotalCredits,
  COALESCE(SUM(CASE WHEN BestAttempt.IsPassed = 1 THEN BestAttempt.Credits ELSE 0 END), 0) AS ApprovedCredits,
  ROUND(COALESCE(SUM(CASE WHEN BestAttempt.IsPassed = 1 THEN BestAttempt.Credits ELSE 0 END), 0)
        / NULLIF(StudyPlan.TotalCredits, 0) * 100, 2) AS ProgressPercentage,
  COUNT(CASE WHEN BestAttempt.IsPassed = 1 THEN BestAttempt.SubjectId END) AS ApprovedSubjects,
  ROUND(AVG(BestAttempt.BestScore), 2) AS GradePointAverage
FROM Students AS Student
JOIN StudyPlans AS StudyPlan ON StudyPlan.Id = Student.StudyPlanId
LEFT JOIN (
  SELECT
    Enrollment.StudentId,
    Subject.Id AS SubjectId,
    Subject.Credits,
    MAX(Grade.FinalScore) AS BestScore,
    MAX(CASE WHEN Grade.Status = 'Passed' THEN 1 ELSE 0 END) AS IsPassed
  FROM Grades AS Grade
  JOIN Enrollments AS Enrollment ON Enrollment.Id = Grade.EnrollmentId AND Enrollment.Status = 'Active'
  JOIN ClassGroups AS ClassGroup ON ClassGroup.Id = Grade.ClassGroupId
  JOIN Subjects AS Subject ON Subject.Id = ClassGroup.SubjectId
  GROUP BY Enrollment.StudentId, Subject.Id, Subject.Credits
) AS BestAttempt ON BestAttempt.StudentId = Student.Id
WHERE Student.DeletedAt IS NULL
  AND Student.Status = 'Active'
GROUP BY Student.Id, Student.StudentNumber, Student.CareerId, StudyPlan.TotalCredits;
