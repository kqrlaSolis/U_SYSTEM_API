import type { CrudConfig } from '../../../shared/crud';
import type { PaymentConcept, PaymentConceptRequest } from './PaymentConcept';
import { PaymentsService } from './PaymentsService';

// CRUD de conceptos de pago.
export const PaymentConceptConfig: CrudConfig<PaymentConcept, PaymentConceptRequest> = {
  Route: '/payment-concepts',
  TitleKey: 'Payments.ConceptsTitle',
  SubtitleKey: 'Payments.ConceptsSubtitle',
  NewKey: 'Payments.NewConcept',
  EditKey: 'Payments.EditConcept',
  EmptyKey: 'Payments.EmptyConcepts',
  SearchKey: 'Payments.Search',

  PermissionView: 'Payments.View',
  PermissionCreate: 'Payments.Register',
  PermissionEdit: 'Payments.Register',
  PermissionDelete: 'Payments.Register',

  Columns: [
    { Field: 'Code', HeaderKey: 'Payments.Columns.Code' },
    { Field: 'Name', HeaderKey: 'Payments.Columns.Name' },
    { Field: 'BaseAmount', HeaderKey: 'Payments.Columns.BaseAmount', Type: 'number' },
    { Field: 'IsRecurring', HeaderKey: 'Payments.Columns.Recurring', Type: 'boolean' },
    { Field: 'IsActive', HeaderKey: 'Payments.Columns.IsActive', Type: 'boolean' },
  ],

  Fields: [
    { Name: 'Code', LabelKey: 'Payments.Columns.Code', Required: true },
    { Name: 'Name', LabelKey: 'Payments.Columns.Name', Required: true },
    { Name: 'BaseAmount', LabelKey: 'Payments.Columns.BaseAmount', Type: 'number', Required: true },
    { Name: 'IsRecurring', LabelKey: 'Payments.Columns.Recurring', Type: 'switch' },
    { Name: 'IsActive', LabelKey: 'Payments.Columns.IsActive', Type: 'switch' },
  ],

  EmptyForm: {
    Code: '',
    Name: '',
    BaseAmount: '0',
    IsRecurring: 'false',
    IsActive: 'true',
  },

  List: (Page, PageSize, Search) => PaymentsService.GetConcepts(Page, PageSize, Search),
  Load: (Id) => PaymentsService.GetConceptById(Id),
  Create: (Request) => PaymentsService.CreateConcept(Request),
  Update: (Id, Request) => PaymentsService.UpdateConcept(Id, Request),
  Remove: (Id) => PaymentsService.DeleteConcept(Id),

  ToForm: (Row: PaymentConcept) => ({
    Code: Row.Code,
    Name: Row.Name,
    BaseAmount: String(Row.BaseAmount),
    IsRecurring: Row.IsRecurring ? 'true' : 'false',
    IsActive: Row.IsActive ? 'true' : 'false',
  }),

  FromForm: (Values) => ({
    Code: Values.Code.trim(),
    Name: Values.Name.trim(),
    BaseAmount: Number(Values.BaseAmount === '' ? '0' : Values.BaseAmount),
    IsRecurring: Values.IsRecurring === 'true',
    IsActive: Values.IsActive === 'true',
  }),
};
