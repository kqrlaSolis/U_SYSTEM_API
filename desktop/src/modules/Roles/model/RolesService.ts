import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { PermissionModule, RoleRequest } from './Permission';
import type { Role } from './Role';

const RolesPath = '/api/v1/roles';
const PermissionsPath = '/api/v1/permissions';

// Servicio del módulo: sin estado de UI y sin conocer React
export const RolesService = {
  async GetAll(): Promise<Role[]> {
    const Page = await HttpClient.Get<PagedResult<Role>>(RolesPath, { Page: 1, PageSize: 100 });
    return Page.Items;
  },

  async GetPaged(Page: number, PageSize: number, Search: string): Promise<PagedResult<Role>> {
    return HttpClient.Get<PagedResult<Role>>(RolesPath, { Page, PageSize, Search });
  },

  async GetById(Id: number): Promise<Role> {
    return HttpClient.Get<Role>(`${RolesPath}/${Id}`);
  },

  async GetPermissionCatalog(): Promise<PermissionModule[]> {
    return HttpClient.Get<PermissionModule[]>(PermissionsPath);
  },

  async Create(Request: RoleRequest): Promise<Role> {
    return HttpClient.Post<Role>(RolesPath, Request);
  },

  async Update(Id: number, Request: RoleRequest): Promise<Role> {
    return HttpClient.Put<Role>(`${RolesPath}/${Id}`, Request);
  },

  async Delete(Id: number): Promise<void> {
    await HttpClient.Delete<boolean>(`${RolesPath}/${Id}`);
  },
};
