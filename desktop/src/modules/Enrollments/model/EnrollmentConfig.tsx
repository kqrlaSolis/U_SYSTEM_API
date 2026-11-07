import type { CrudConfig } from '../../../shared/crud';
import { AppButton } from '../../../shared/components';
import I18next from '../../../shared/i18n/Config';
import type { Enrollment, EnrollmentRequest } from './Enrollment';
import { EnrollmentsService } from './EnrollmentsService';

const StatusOptions = [
  { value: 'Draft', label: 'Borrador' },
  { value: 'Confirmed', label: 'Confirmada' },
  { value: 'Cancelled', label: 'Cancelada' },
];

// Inscripción de primer ingreso: los grupos se capturan como lista separada por comas.
export const EnrollmentConfig: CrudConfig<Enrollment, EnrollmentRequest> = {
  Route: '/enrollments',
  TitleKey: 'Enrollments.Title',
  SubtitleKey: 'Enrollments.Subtitle',
  NewKey: 'Enrollments.New',
  EditKey: 'Enrollments.Edit',
  EmptyKey: 'Enrollments.Empty',
  SearchKey: 'Enrollments.Search',

  PermissionView: 'Enrollments.View',
  PermissionCreate: 'Enrollments.Create',
  PermissionEdit: 'Enrollments.Create',

  Columns: [
    { Field: 'Id', HeaderKey: 'Enrollments.Columns.Id' },
    { Field: 'StudentId', HeaderKey: 'Enrollments.Columns.Student', Type: 'number' },
    { Field: 'AcademicTermId', HeaderKey: 'Enrollments.Columns.Term', Type: 'number' },
    { Field: 'CareerId', HeaderKey: 'Enrollments.Columns.Career', Type: 'number' },
    { Field: 'Type', HeaderKey: 'Enrollments.Columns.Type' },
    { Field: 'Status', HeaderKey: 'Enrollments.Columns.Status' },
  ],

  Fields: [
    { Name: 'StudentId', LabelKey: 'Enrollments.Columns.Student', Type: 'number', Required: true },
    { Name: 'AcademicTermId', LabelKey: 'Enrollments.Columns.Term', Type: 'number', Required: true },
    { Name: 'CareerId', LabelKey: 'Enrollments.Columns.Career', Type: 'number', Required: true },
    { Name: 'ClassGroupIds', LabelKey: 'Enrollments.Fields.ClassGroupIds', Required: true },
    { Name: 'Notes', LabelKey: 'Enrollments.Fields.Notes' },
    { Name: 'Status', LabelKey: 'Enrollments.Columns.Status', Type: 'select', Options: StatusOptions },
  ],

  EmptyForm: {
    StudentId: '1',
    AcademicTermId: '1',
    CareerId: '1',
    ClassGroupIds: '1',
    Notes: '',
    Status: 'Draft',
  },

  List: (Page, PageSize, Search) => EnrollmentsService.GetPaged(Page, PageSize, Search),
  Load: (Id) => EnrollmentsService.GetById(Id),
  Create: (Request) => EnrollmentsService.Create(Request),

  // Cancelar libera cupo en la API; el listado se recarga al terminar
  RowActions: (Row, Reload) => {
    if (Row.Status === 'Cancelled') {
      return null;
    }

    return (
      <AppButton
        Label={I18next.t('Enrollments.CancelAction')}
        Variant="text"
        Size="sm"
        Color="danger"
        OnClick={() => {
          void EnrollmentsService.Cancel(Row.Id, 'Cancelada desde el listado').then(() => Reload());
        }}
      />
    );
  },
  // La inscripción no se edita: se cancela desde la API (POST /enrollments/{Id}/cancel)
  Update: async () => {
    throw new Error('EnrollmentsAreImmutable');
  },

  ToForm: (Row: Enrollment) => ({
    StudentId: String(Row.StudentId),
    AcademicTermId: String(Row.AcademicTermId),
    CareerId: String(Row.CareerId),
    ClassGroupIds: Row.ClassGroupIds.join(', '),
    Notes: Row.Notes ?? '',
    Status: Row.Status,
  }),

  FromForm: (Values) => ({
    StudentId: Number(Values.StudentId),
    AcademicTermId: Number(Values.AcademicTermId),
    CareerId: Number(Values.CareerId),
    Notes: Values.Notes.trim() === '' ? null : Values.Notes.trim(),
    ClassGroupIds: Values.ClassGroupIds
      .split(',')
      .map((Item) => Number(Item.trim()))
      .filter((Item) => Number.isFinite(Item) && Item > 0),
  }),
};
