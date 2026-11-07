import type { CrudConfig } from '../../../shared/crud';
import type { ClassGroup, ClassGroupRequest, ClassGroupSchedule } from './ClassGroup';
import { ClassGroupsService } from './ClassGroupsService';

const StatusOptions = [
  { value: 'Open', label: 'Abierto' },
  { value: 'Closed', label: 'Cerrado' },
  { value: 'Cancelled', label: 'Cancelado' },
];

// Horarios en formato "día inicio-fin" separados por punto y coma: 1 08:00-10:00; 3 08:00-10:00
const ParseSchedules = (Value: string): ClassGroupSchedule[] =>
  Value
    .split(';')
    .map((Item) => Item.trim())
    .filter((Item) => Item.length > 0)
    .map((Item) => {
      const [Day, Range] = Item.split(/\s+/);
      const [Start, End] = (Range ?? '').split('-');
      return {
        DayOfWeek: Number(Day),
        StartTime: (Start ?? '').trim(),
        EndTime: (End ?? '').trim(),
      };
    })
    .filter((Item) => Item.DayOfWeek >= 1 && Item.DayOfWeek <= 7 && Item.StartTime.length === 5 && Item.EndTime.length === 5);

const FormatSchedules = (Schedules: ClassGroupSchedule[]): string =>
  Schedules.map((Item) => `${Item.DayOfWeek} ${Item.StartTime}-${Item.EndTime}`).join('; ');

// CRUD de grupos de clase con horarios.
export const ClassGroupConfig: CrudConfig<ClassGroup, ClassGroupRequest> = {
  Route: '/class-groups',
  TitleKey: 'ClassGroups.Title',
  SubtitleKey: 'ClassGroups.Subtitle',
  NewKey: 'ClassGroups.New',
  EditKey: 'ClassGroups.Edit',
  EmptyKey: 'ClassGroups.Empty',
  SearchKey: 'ClassGroups.Search',

  PermissionView: 'ClassGroups.View',
  PermissionCreate: 'ClassGroups.Create',
  PermissionEdit: 'ClassGroups.Edit',
  PermissionDelete: 'ClassGroups.Delete',

  Columns: [
    { Field: 'Code', HeaderKey: 'ClassGroups.Columns.Code' },
    { Field: 'AcademicTermId', HeaderKey: 'ClassGroups.Columns.Term', Type: 'number' },
    { Field: 'SubjectId', HeaderKey: 'ClassGroups.Columns.Subject', Type: 'number' },
    { Field: 'TeacherId', HeaderKey: 'ClassGroups.Columns.Teacher', Type: 'number' },
    { Field: 'Classroom', HeaderKey: 'ClassGroups.Columns.Classroom' },
    { Field: 'Capacity', HeaderKey: 'ClassGroups.Columns.Capacity', Type: 'number' },
    { Field: 'EnrolledCount', HeaderKey: 'ClassGroups.Columns.Enrolled', Type: 'number' },
    { Field: 'Status', HeaderKey: 'ClassGroups.Columns.Status' },
  ],

  Fields: [
    { Name: 'Code', LabelKey: 'ClassGroups.Columns.Code', Required: true },
    { Name: 'AcademicTermId', LabelKey: 'ClassGroups.Columns.Term', Type: 'number', Required: true },
    { Name: 'SubjectId', LabelKey: 'ClassGroups.Columns.Subject', Type: 'number', Required: true },
    { Name: 'TeacherId', LabelKey: 'ClassGroups.Columns.Teacher', Type: 'number', Required: true },
    { Name: 'Classroom', LabelKey: 'ClassGroups.Columns.Classroom' },
    { Name: 'Capacity', LabelKey: 'ClassGroups.Columns.Capacity', Type: 'number', Required: true },
    { Name: 'Status', LabelKey: 'ClassGroups.Columns.Status', Type: 'select', Options: StatusOptions },
    { Name: 'Schedules', LabelKey: 'ClassGroups.Fields.Schedules' },
  ],

  EmptyForm: {
    Code: '',
    AcademicTermId: '1',
    SubjectId: '1',
    TeacherId: '1',
    Classroom: '',
    Capacity: '30',
    Status: 'Open',
    Schedules: '1 08:00-10:00',
  },

  List: (Page, PageSize, Search) => ClassGroupsService.GetPaged(Page, PageSize, Search),
  Load: (Id) => ClassGroupsService.GetById(Id),
  Create: (Request) => ClassGroupsService.Create(Request),
  Update: (Id, Request) => ClassGroupsService.Update(Id, Request),
  Remove: (Id) => ClassGroupsService.Delete(Id),

  ToForm: (Row: ClassGroup) => ({
    Code: Row.Code,
    AcademicTermId: String(Row.AcademicTermId),
    SubjectId: String(Row.SubjectId),
    TeacherId: String(Row.TeacherId),
    Classroom: Row.Classroom ?? '',
    Capacity: String(Row.Capacity),
    Status: Row.Status,
    Schedules: FormatSchedules(Row.Schedules),
  }),

  FromForm: (Values) => ({
    Code: Values.Code.trim(),
    AcademicTermId: Number(Values.AcademicTermId),
    SubjectId: Number(Values.SubjectId),
    TeacherId: Number(Values.TeacherId),
    Classroom: Values.Classroom.trim() === '' ? null : Values.Classroom.trim(),
    Capacity: Number(Values.Capacity),
    Status: Values.Status,
    Schedules: ParseSchedules(Values.Schedules),
  }),
};
