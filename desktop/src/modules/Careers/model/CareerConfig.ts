import type { CrudConfig } from '../../../shared/crud';
import type { Career, CareerRequest } from './Career';
import { CareersService } from './CareersService';

// Configuración del CRUD de carreras: columnas, campos y llamadas a la API.
export const CareerConfig: CrudConfig<Career, CareerRequest> = {
  Route: '/careers',
  TitleKey: 'Careers.Title',
  SubtitleKey: 'Careers.Subtitle',
  NewKey: 'Careers.New',
  EditKey: 'Careers.Edit',
  EmptyKey: 'Careers.Empty',
  SearchKey: 'Careers.Search',

  PermissionView: 'Careers.View',
  PermissionCreate: 'Careers.Create',
  PermissionEdit: 'Careers.Edit',
  PermissionDelete: 'Careers.Delete',

  Columns: [
    { Field: 'Code', HeaderKey: 'Careers.Columns.Code' },
    { Field: 'Name', HeaderKey: 'Careers.Columns.Name' },
    { Field: 'Level', HeaderKey: 'Careers.Columns.Level' },
    { Field: 'TermCount', HeaderKey: 'Careers.Columns.TermCount', Type: 'number' },
    { Field: 'TotalCredits', HeaderKey: 'Careers.Columns.TotalCredits', Type: 'number' },
    { Field: 'Modality', HeaderKey: 'Careers.Columns.Modality' },
    { Field: 'IsActive', HeaderKey: 'Careers.Columns.IsActive', Type: 'boolean' },
  ],

  Fields: [
    { Name: 'Code', LabelKey: 'Careers.Columns.Code', Required: true },
    { Name: 'Name', LabelKey: 'Careers.Columns.Name', Required: true },
    { Name: 'Level', LabelKey: 'Careers.Columns.Level', Required: true },
    { Name: 'TermCount', LabelKey: 'Careers.Columns.TermCount', Type: 'number', Required: true },
    { Name: 'TotalCredits', LabelKey: 'Careers.Columns.TotalCredits', Type: 'number', Required: true },
    { Name: 'Modality', LabelKey: 'Careers.Columns.Modality', Required: true },
    { Name: 'IsActive', LabelKey: 'Careers.Columns.IsActive', Type: 'switch' },
  ],

  EmptyForm: {
    Code: '',
    Name: '',
    Level: '',
    TermCount: '',
    TotalCredits: '',
    Modality: '',
    IsActive: 'true',
  },

  List: (Page, PageSize, Search) => CareersService.GetPaged(Page, PageSize, Search),
  Load: (Id) => CareersService.GetById(Id),
  Create: (Request) => CareersService.Create(Request),
  Update: (Id, Request) => CareersService.Update(Id, Request),
  Remove: (Id) => CareersService.Delete(Id),

  ToForm: (Row: Career) => ({
    Code: Row.Code,
    Name: Row.Name,
    Level: Row.Level,
    TermCount: String(Row.TermCount),
    TotalCredits: String(Row.TotalCredits),
    Modality: Row.Modality,
    IsActive: Row.IsActive ? 'true' : 'false',
  }),

  FromForm: (Values) => ({
    Code: Values.Code.trim(),
    Name: Values.Name.trim(),
    Level: Values.Level.trim(),
    TermCount: Number(Values.TermCount),
    TotalCredits: Number(Values.TotalCredits),
    Modality: Values.Modality.trim(),
    IsActive: Values.IsActive === 'true',
  }),
};
