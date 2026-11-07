import type { CrudConfig } from '../../../shared/crud';
import type { RoleRequest } from './Permission';
import type { Role } from './Role';
import { RolesService } from './RolesService';

// Los roles usan el listado del CRUD genérico y un formulario propio (editor de permisos).
export const RoleConfig: CrudConfig<Role, RoleRequest> = {
  Route: '/roles',
  TitleKey: 'Roles.Title',
  SubtitleKey: 'Roles.Subtitle',
  NewKey: 'Roles.New',
  EditKey: 'Roles.Edit',
  EmptyKey: 'Roles.Empty',
  SearchKey: 'Roles.Search',

  PermissionView: 'Roles.View',
  PermissionCreate: 'Roles.Create',
  PermissionEdit: 'Roles.Edit',
  PermissionDelete: 'Roles.Delete',

  Columns: [
    { Field: 'Name', HeaderKey: 'Roles.Columns.Name' },
    { Field: 'Description', HeaderKey: 'Roles.Columns.Description' },
    { Field: 'UserCount', HeaderKey: 'Roles.Columns.UserCount', Type: 'number' },
    { Field: 'PermissionCount', HeaderKey: 'Roles.Columns.PermissionCount', Type: 'number' },
    { Field: 'IsSystem', HeaderKey: 'Roles.Columns.IsSystem', Type: 'boolean' },
    { Field: 'IsActive', HeaderKey: 'Roles.Columns.IsActive', Type: 'boolean' },
  ],

  Fields: [
    { Name: 'Name', LabelKey: 'Roles.Fields.Name', Required: true },
    { Name: 'Description', LabelKey: 'Roles.Fields.Description' },
    { Name: 'IsActive', LabelKey: 'Roles.Fields.IsActive', Type: 'switch' },
  ],

  EmptyForm: { Name: '', Description: '', IsActive: 'true' },

  List: (Page, PageSize, Search) => RolesService.GetPaged(Page, PageSize, Search),
  Load: (Id) => RolesService.GetById(Id),
  Create: (Request) => RolesService.Create(Request),
  Update: (Id, Request) => RolesService.Update(Id, Request),
  Remove: (Id) => RolesService.Delete(Id),

  ToForm: (Row: Role) => ({
    Name: Row.Name,
    Description: Row.Description ?? '',
    IsActive: Row.IsActive ? 'true' : 'false',
  }),

  FromForm: (Values) => ({
    Name: Values.Name.trim(),
    Description: Values.Description.trim() === '' ? null : Values.Description.trim(),
    IsActive: Values.IsActive === 'true',
    PermissionCodes: [],
  }),
};
