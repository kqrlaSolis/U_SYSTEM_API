import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { ClassGroup, ClassGroupRequest } from './ClassGroup';

const GroupsPath = '/api/v1/class-groups';

// Servicio del módulo: sin estado de UI y sin conocer React
export const ClassGroupsService = {
  async GetPaged(Page: number, PageSize: number, Search: string): Promise<PagedResult<ClassGroup>> {
    return HttpClient.Get<PagedResult<ClassGroup>>(GroupsPath, { Page, PageSize, Search });
  },

  async GetById(Id: number): Promise<ClassGroup> {
    return HttpClient.Get<ClassGroup>(`${GroupsPath}/${Id}`);
  },

  async Create(Request: ClassGroupRequest): Promise<ClassGroup> {
    return HttpClient.Post<ClassGroup>(GroupsPath, Request);
  },

  async Update(Id: number, Request: ClassGroupRequest): Promise<ClassGroup> {
    return HttpClient.Put<ClassGroup>(`${GroupsPath}/${Id}`, Request);
  },

  async Delete(Id: number): Promise<void> {
    await HttpClient.Delete<boolean>(`${GroupsPath}/${Id}`);
  },
};
