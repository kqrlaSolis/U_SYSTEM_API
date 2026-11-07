import type { CrudConfig } from '../../../shared/crud';
import type { Teacher, TeacherRequest } from './Teacher';
import { TeachersService } from './TeachersService';

// Configuración del CRUD de docentes: columnas, campos y llamadas a la API.
export const TeacherConfig: CrudConfig<Teacher, TeacherRequest> = {
  Route: '/teachers',
  TitleKey: 'Teachers.Title',
  SubtitleKey: 'Teachers.Subtitle',
  NewKey: 'Teachers.New',
  EditKey: 'Teachers.Edit',
  EmptyKey: 'Teachers.Empty',
  SearchKey: 'Teachers.Search',

  PermissionView: 'Teachers.View',
  PermissionCreate: 'Teachers.Create',
  PermissionEdit: 'Teachers.Edit',
  PermissionDelete: 'Teachers.Delete',

  Columns: [
    { Field: 'EmployeeNumber', HeaderKey: 'Teachers.Columns.EmployeeNumber' },
    { Field: 'FullName', HeaderKey: 'Teachers.Columns.FullName' },
    { Field: 'Email', HeaderKey: 'Teachers.Columns.Email' },
    { Field: 'Phone', HeaderKey: 'Teachers.Columns.Phone' },
    { Field: 'Specialty', HeaderKey: 'Teachers.Columns.Specialty' },
    { Field: 'IsActive', HeaderKey: 'Teachers.Columns.IsActive', Type: 'boolean' },
  ],

  Fields: [
    { Name: 'EmployeeNumber', LabelKey: 'Teachers.Columns.EmployeeNumber', Required: true },
    { Name: 'FirstName', LabelKey: 'Teachers.Fields.FirstName', Required: true },
    { Name: 'PaternalLastName', LabelKey: 'Teachers.Fields.PaternalLastName', Required: true },
    { Name: 'MaternalLastName', LabelKey: 'Teachers.Fields.MaternalLastName' },
    { Name: 'Email', LabelKey: 'Teachers.Columns.Email', Type: 'email', Required: true },
    { Name: 'Phone', LabelKey: 'Teachers.Columns.Phone' },
    { Name: 'Specialty', LabelKey: 'Teachers.Columns.Specialty' },
    { Name: 'IsActive', LabelKey: 'Teachers.Columns.IsActive', Type: 'switch' },
  ],

  EmptyForm: {
    EmployeeNumber: '',
    FirstName: '',
    PaternalLastName: '',
    MaternalLastName: '',
    Email: '',
    Phone: '',
    Specialty: '',
    IsActive: 'true',
  },

  List: (Page, PageSize, Search) => TeachersService.GetPaged(Page, PageSize, Search),
  Load: (Id) => TeachersService.GetById(Id),
  Create: (Request) => TeachersService.Create(Request),
  Update: (Id, Request) => TeachersService.Update(Id, Request),
  Remove: (Id) => TeachersService.Delete(Id),

  ToForm: (Row: Teacher) => ({
    EmployeeNumber: Row.EmployeeNumber,
    FirstName: Row.FirstName,
    PaternalLastName: Row.PaternalLastName,
    MaternalLastName: Row.MaternalLastName ?? '',
    Email: Row.Email,
    Phone: Row.Phone ?? '',
    Specialty: Row.Specialty ?? '',
    IsActive: Row.IsActive ? 'true' : 'false',
  }),

  FromForm: (Values) => ({
    EmployeeNumber: Values.EmployeeNumber.trim(),
    FirstName: Values.FirstName.trim(),
    PaternalLastName: Values.PaternalLastName.trim(),
    MaternalLastName: Values.MaternalLastName.trim() === '' ? null : Values.MaternalLastName.trim(),
    Email: Values.Email.trim(),
    Phone: Values.Phone.trim() === '' ? null : Values.Phone.trim(),
    Specialty: Values.Specialty.trim() === '' ? null : Values.Specialty.trim(),
    UserId: null,
    IsActive: Values.IsActive === 'true',
  }),
};
