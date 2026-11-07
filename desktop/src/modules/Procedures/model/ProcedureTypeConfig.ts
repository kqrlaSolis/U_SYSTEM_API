import type { CrudConfig } from '../../../shared/crud';
import type { ProcedureType, ProcedureTypeRequest } from './ProcedureType';
import { ProceduresService } from './ProceduresService';

// CRUD de tipos de trámite (catálogo de constancias y solicitudes).
export const ProcedureTypeConfig: CrudConfig<ProcedureType, ProcedureTypeRequest> = {
  Route: '/procedure-types',
  TitleKey: 'Procedures.TypesTitle',
  SubtitleKey: 'Procedures.TypesSubtitle',
  NewKey: 'Procedures.NewType',
  EditKey: 'Procedures.EditType',
  EmptyKey: 'Procedures.EmptyTypes',
  SearchKey: 'Procedures.Search',

  PermissionView: 'Procedures.View',
  PermissionCreate: 'Procedures.Issue',
  PermissionEdit: 'Procedures.Issue',
  PermissionDelete: 'Procedures.Issue',

  Columns: [
    { Field: 'Code', HeaderKey: 'Procedures.Columns.Code' },
    { Field: 'Name', HeaderKey: 'Procedures.Columns.Name' },
    { Field: 'PaymentConceptId', HeaderKey: 'Procedures.Columns.Concept', Type: 'number' },
    { Field: 'IsActive', HeaderKey: 'Procedures.Columns.IsActive', Type: 'boolean' },
  ],

  Fields: [
    { Name: 'Code', LabelKey: 'Procedures.Columns.Code', Required: true },
    { Name: 'Name', LabelKey: 'Procedures.Columns.Name', Required: true },
    { Name: 'PaymentConceptId', LabelKey: 'Procedures.Columns.Concept', Type: 'number' },
    { Name: 'IsActive', LabelKey: 'Procedures.Columns.IsActive', Type: 'switch' },
  ],

  EmptyForm: {
    Code: '',
    Name: '',
    PaymentConceptId: '',
    IsActive: 'true',
  },

  List: (Page, PageSize, Search) => ProceduresService.GetTypes(Page, PageSize, Search),
  Load: (Id) => ProceduresService.GetTypeById(Id),
  Create: (Request) => ProceduresService.CreateType(Request),
  Update: (Id, Request) => ProceduresService.UpdateType(Id, Request),
  Remove: (Id) => ProceduresService.DeleteType(Id),

  ToForm: (Row: ProcedureType) => ({
    Code: Row.Code,
    Name: Row.Name,
    PaymentConceptId: Row.PaymentConceptId === null ? '' : String(Row.PaymentConceptId),
    IsActive: Row.IsActive ? 'true' : 'false',
  }),

  FromForm: (Values) => ({
    Code: Values.Code.trim(),
    Name: Values.Name.trim(),
    PaymentConceptId:
      Values.PaymentConceptId.trim() === '' ? null : Number(Values.PaymentConceptId),
    IsActive: Values.IsActive === 'true',
  }),
};
