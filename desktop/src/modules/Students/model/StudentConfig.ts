import type { CrudConfig } from '../../../shared/crud';
import type { Student, StudentRequest } from './Student';
import { StudentsService } from './StudentsService';

const GenderOptions = [
  { value: 'M', label: 'M' },
  { value: 'F', label: 'F' },
  { value: 'Other', label: 'Otro' },
];

const StatusOptions = [
  { value: 'Active', label: 'Activo' },
  { value: 'Inactive', label: 'Inactivo' },
  { value: 'Graduated', label: 'Egresado' },
  { value: 'Dropout', label: 'Baja' },
];

// CRUD de alumnos: la matrícula la genera la API, aquí solo se captura el resto.
export const StudentConfig: CrudConfig<Student, StudentRequest> = {
  Route: '/students',
  TitleKey: 'Students.Title',
  SubtitleKey: 'Students.Subtitle',
  NewKey: 'Students.New',
  EditKey: 'Students.Edit',
  EmptyKey: 'Students.Empty',
  SearchKey: 'Students.Search',

  PermissionView: 'Students.View',
  PermissionCreate: 'Students.Create',
  PermissionEdit: 'Students.Edit',
  PermissionDelete: 'Students.Delete',

  Filters: [
    { Field: 'CareerId', LabelKey: 'Students.Filters.Career', OptionsKey: 'Careers' },
    { Field: 'AcademicTermId', LabelKey: 'Students.Filters.Term', OptionsKey: 'Terms' },
    { Field: 'Status', LabelKey: 'Students.Filters.Status', OptionsKey: 'StudentStatuses' },
  ],

  Columns: [
    { Field: 'StudentNumber', HeaderKey: 'Students.Columns.StudentNumber' },
    { Field: 'FullName', HeaderKey: 'Students.Columns.FullName' },
    { Field: 'CareerName', HeaderKey: 'Students.Columns.Career' },
    { Field: 'Curp', HeaderKey: 'Students.Columns.Curp' },
    { Field: 'Email', HeaderKey: 'Students.Columns.Email' },
    { Field: 'Status', HeaderKey: 'Students.Columns.Status' },
  ],

  Fields: [
    { Name: 'CareerId', LabelKey: 'Students.Fields.CareerId', Type: 'number', Required: true },
    { Name: 'StudyPlanId', LabelKey: 'Students.Fields.StudyPlanId', Type: 'number', Required: true },
    { Name: 'EnrollmentTermId', LabelKey: 'Students.Fields.EnrollmentTermId', Type: 'number', Required: true },
    { Name: 'FirstName', LabelKey: 'Students.Fields.FirstName', Required: true },
    { Name: 'PaternalLastName', LabelKey: 'Students.Fields.PaternalLastName', Required: true },
    { Name: 'MaternalLastName', LabelKey: 'Students.Fields.MaternalLastName' },
    { Name: 'Curp', LabelKey: 'Students.Columns.Curp', Required: true },
    { Name: 'BirthDate', LabelKey: 'Students.Fields.BirthDate', Required: true },
    { Name: 'Gender', LabelKey: 'Students.Fields.Gender', Type: 'select', Options: GenderOptions, Required: true },
    { Name: 'Email', LabelKey: 'Students.Columns.Email', Type: 'email' },
    { Name: 'Phone', LabelKey: 'Students.Fields.Phone' },
    { Name: 'Status', LabelKey: 'Students.Columns.Status', Type: 'select', Options: StatusOptions },
  ],

  EmptyForm: {
    CareerId: '1',
    StudyPlanId: '4',
    EnrollmentTermId: '1',
    FirstName: '',
    PaternalLastName: '',
    MaternalLastName: '',
    Curp: '',
    BirthDate: '',
    Gender: 'F',
    Email: '',
    Phone: '',
    Status: 'Active',
  },

  List: (Page, PageSize, Search, Filters) => StudentsService.GetPaged(Page, PageSize, Search, Filters),
  Load: (Id) => StudentsService.GetById(Id),
  Create: (Request) => StudentsService.Create(Request),
  Update: (Id, Request) => StudentsService.Update(Id, Request),
  Remove: (Id) => StudentsService.Delete(Id),

  ToForm: (Row: Student) => ({
    CareerId: String(Row.CareerId),
    StudyPlanId: String(Row.StudyPlanId),
    EnrollmentTermId: String(Row.EnrollmentTermId),
    FirstName: Row.FirstName,
    PaternalLastName: Row.PaternalLastName,
    MaternalLastName: Row.MaternalLastName ?? '',
    Curp: Row.Curp,
    BirthDate: Row.BirthDate,
    Gender: Row.Gender,
    Email: Row.Email ?? '',
    Phone: Row.Phone ?? '',
    Status: Row.Status,
  }),

  FromForm: (Values) => ({
    CareerId: Number(Values.CareerId),
    StudyPlanId: Number(Values.StudyPlanId),
    EnrollmentTermId: Number(Values.EnrollmentTermId),
    FirstName: Values.FirstName.trim(),
    PaternalLastName: Values.PaternalLastName.trim(),
    MaternalLastName: Values.MaternalLastName.trim() === '' ? null : Values.MaternalLastName.trim(),
    Curp: Values.Curp.trim().toUpperCase(),
    BirthDate: Values.BirthDate,
    Gender: Values.Gender,
    Email: Values.Email.trim() === '' ? null : Values.Email.trim(),
    Phone: Values.Phone.trim() === '' ? null : Values.Phone.trim(),
    AddressLine: null,
    City: null,
    State: null,
    PostalCode: null,
    Status: Values.Status,
  }),
};
