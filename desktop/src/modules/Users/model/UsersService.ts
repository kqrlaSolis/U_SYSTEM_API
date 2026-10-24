import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { User } from './User';
import type {
  CreateUserRequest,
  CreatedUserResponse,
  TemporaryPasswordResponse,
  UpdateUserRequest,
} from './UserRequests';

const UsersPath = '/api/v1/users';

// Servicio del módulo: sin estado de UI y sin conocer React
export const UsersService = {
  async GetPaged(Page: number, PageSize: number, Search: string): Promise<PagedResult<User>> {
    return HttpClient.Get<PagedResult<User>>(UsersPath, { Page, PageSize, Search });
  },

  async GetById(Id: number): Promise<User> {
    return HttpClient.Get<User>(`${UsersPath}/${Id}`);
  },

  async Create(Request: CreateUserRequest): Promise<CreatedUserResponse> {
    return HttpClient.Post<CreatedUserResponse>(UsersPath, Request);
  },

  async Update(Id: number, Request: UpdateUserRequest): Promise<User> {
    return HttpClient.Put<User>(`${UsersPath}/${Id}`, Request);
  },

  async SetStatus(Id: number, IsActive: boolean): Promise<User> {
    return HttpClient.Patch<User>(`${UsersPath}/${Id}/status`, { IsActive });
  },

  async ResetPassword(Id: number): Promise<TemporaryPasswordResponse> {
    return HttpClient.Post<TemporaryPasswordResponse>(`${UsersPath}/${Id}/reset-password`, {});
  },

  async AssignRoles(Id: number, RoleIds: number[]): Promise<User> {
    return HttpClient.Put<User>(`${UsersPath}/${Id}/roles`, { RoleIds });
  },
};
