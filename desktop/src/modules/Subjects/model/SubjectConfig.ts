import type { CrudConfig } from '../../../shared/crud';
import type { Subject, SubjectRequest } from './Subject';
import { SubjectsService } from './SubjectsService';

// Configuración del CRUD de materias: columnas, campos y llamadas a la API.
export const SubjectConfig: CrudConfig<Subject, SubjectRequest> = {
  Route: '/subjects',
  TitleKey: 'Subjects.Title',
  SubtitleKey: 'Subjects.Subtitle',
  NewKey: 'Subjects.New',
  EditKey: 'Subjects.Edit',
  EmptyKey: 'Subjects.Empty',
  SearchKey: 'Subjects.Search',

  PermissionView: 'Subjects.View',
  PermissionCreate: 'Subjects.Create',
  PermissionEdit: 'Subjects.Edit',
  PermissionDelete: 'Subjects.Delete',

  Columns: [
    { Field: 'Code', HeaderKey: 'Subjects.Columns.Code' },
    { Field: 'Name', HeaderKey: 'Subjects.Columns.Name' },
    { Field: 'Credits', HeaderKey: 'Subjects.Columns.Credits', Type: 'number' },
    { Field: 'TheoryHours', HeaderKey: 'Subjects.Columns.TheoryHours', Type: 'number' },
    { Field: 'PracticeHours', HeaderKey: 'Subjects.Columns.PracticeHours', Type: 'number' },
    { Field: 'IsActive', HeaderKey: 'Subjects.Columns.IsActive', Type: 'boolean' },
  ],

  Fields: [
    { Name: 'Code', LabelKey: 'Subjects.Columns.Code', Required: true },
    { Name: 'Name', LabelKey: 'Subjects.Columns.Name', Required: true },
    { Name: 'Credits', LabelKey: 'Subjects.Columns.Credits', Type: 'number', Required: true },
    { Name: 'TheoryHours', LabelKey: 'Subjects.Columns.TheoryHours', Type: 'number' },
    { Name: 'PracticeHours', LabelKey: 'Subjects.Columns.PracticeHours', Type: 'number' },
    { Name: 'IsActive', LabelKey: 'Subjects.Columns.IsActive', Type: 'switch' },
  ],

  EmptyForm: {
    Code: '',
    Name: '',
    Credits: '',
    TheoryHours: '',
    PracticeHours: '',
    IsActive: 'true',
  },

  List: (Page, PageSize, Search) => SubjectsService.GetPaged(Page, PageSize, Search),
  Load: (Id) => SubjectsService.GetById(Id),
  Create: (Request) => SubjectsService.Create(Request),
  Update: (Id, Request) => SubjectsService.Update(Id, Request),
  Remove: (Id) => SubjectsService.Delete(Id),

  ToForm: (Row: Subject) => ({
    Code: Row.Code,
    Name: Row.Name,
    Credits: String(Row.Credits),
    TheoryHours: String(Row.TheoryHours),
    PracticeHours: String(Row.PracticeHours),
    IsActive: Row.IsActive ? 'true' : 'false',
  }),

  FromForm: (Values) => ({
    Code: Values.Code.trim(),
    Name: Values.Name.trim(),
    Credits: Number(Values.Credits),
    TheoryHours: Number(Values.TheoryHours === '' ? 0 : Values.TheoryHours),
    PracticeHours: Number(Values.PracticeHours === '' ? 0 : Values.PracticeHours),
    IsActive: Values.IsActive === 'true',
  }),
};
