import type { CrudConfig } from '../../../shared/crud';
import type { CourseMaterial, CourseMaterialRequest } from './CourseMaterial';
import { CourseMaterialsService } from './CourseMaterialsService';

const TypeOptions = [
  { value: 'Syllabus', label: 'Programa' },
  { value: 'Reading', label: 'Lectura' },
  { value: 'Video', label: 'Video' },
  { value: 'Link', label: 'Enlace' },
  { value: 'File', label: 'Archivo' },
  { value: 'Other', label: 'Otro' },
];

// CRUD de material didáctico por materia.
export const CourseMaterialConfig: CrudConfig<CourseMaterial, CourseMaterialRequest> = {
  Route: '/course-materials',
  TitleKey: 'CourseMaterials.Title',
  SubtitleKey: 'CourseMaterials.Subtitle',
  NewKey: 'CourseMaterials.New',
  EditKey: 'CourseMaterials.Edit',
  EmptyKey: 'CourseMaterials.Empty',
  SearchKey: 'CourseMaterials.Search',

  PermissionView: 'CourseMaterials.View',
  PermissionCreate: 'CourseMaterials.Create',
  PermissionEdit: 'CourseMaterials.Edit',
  PermissionDelete: 'CourseMaterials.Delete',

  Columns: [
    { Field: 'Title', HeaderKey: 'CourseMaterials.Columns.Title' },
    { Field: 'Type', HeaderKey: 'CourseMaterials.Columns.Type' },
    { Field: 'SubjectId', HeaderKey: 'CourseMaterials.Columns.Subject' },
    { Field: 'AcademicTermId', HeaderKey: 'CourseMaterials.Columns.Term' },
    { Field: 'Url', HeaderKey: 'CourseMaterials.Columns.Url' },
    { Field: 'IsActive', HeaderKey: 'CourseMaterials.Columns.IsActive', Type: 'boolean' },
  ],

  Fields: [
    { Name: 'SubjectId', LabelKey: 'CourseMaterials.Columns.Subject', Type: 'number', Required: true },
    { Name: 'AcademicTermId', LabelKey: 'CourseMaterials.Columns.Term', Type: 'number' },
    { Name: 'Type', LabelKey: 'CourseMaterials.Columns.Type', Type: 'select', Options: TypeOptions, Required: true },
    { Name: 'Title', LabelKey: 'CourseMaterials.Columns.Title', Required: true },
    { Name: 'Description', LabelKey: 'CourseMaterials.Fields.Description' },
    { Name: 'Url', LabelKey: 'CourseMaterials.Columns.Url' },
    { Name: 'FilePath', LabelKey: 'CourseMaterials.Fields.FilePath' },
    { Name: 'IsActive', LabelKey: 'CourseMaterials.Columns.IsActive', Type: 'switch' },
  ],

  EmptyForm: {
    SubjectId: '1',
    AcademicTermId: '1',
    Type: 'Reading',
    Title: '',
    Description: '',
    Url: '',
    FilePath: '',
    IsActive: 'true',
  },

  List: (Page, PageSize, Search) => CourseMaterialsService.GetPaged(Page, PageSize, Search),
  Load: (Id) => CourseMaterialsService.GetById(Id),
  Create: (Request) => CourseMaterialsService.Create(Request),
  Update: (Id, Request) => CourseMaterialsService.Update(Id, Request),
  Remove: (Id) => CourseMaterialsService.Delete(Id),

  ToForm: (Row: CourseMaterial) => ({
    SubjectId: String(Row.SubjectId),
    AcademicTermId: Row.AcademicTermId === null ? '' : String(Row.AcademicTermId),
    Type: Row.Type,
    Title: Row.Title,
    Description: Row.Description ?? '',
    Url: Row.Url ?? '',
    FilePath: Row.FilePath ?? '',
    IsActive: Row.IsActive ? 'true' : 'false',
  }),

  FromForm: (Values) => ({
    SubjectId: Number(Values.SubjectId),
    AcademicTermId: Values.AcademicTermId.trim() === '' ? null : Number(Values.AcademicTermId),
    Type: Values.Type,
    Title: Values.Title.trim(),
    Description: Values.Description.trim() === '' ? null : Values.Description.trim(),
    Url: Values.Url.trim() === '' ? null : Values.Url.trim(),
    FilePath: Values.FilePath.trim() === '' ? null : Values.FilePath.trim(),
    IsActive: Values.IsActive === 'true',
  }),
};
