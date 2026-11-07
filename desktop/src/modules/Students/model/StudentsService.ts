import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { Student, StudentRequest } from './Student';

const StudentsPath = '/api/v1/students';

// Servicio del módulo: sin estado de UI y sin conocer React
export const StudentsService = {
  async GetPaged(
    Page: number,
    PageSize: number,
    Search: string,
    Filters: Record<string, string> = {},
  ): Promise<PagedResult<Student>> {
    return HttpClient.Get<PagedResult<Student>>(StudentsPath, { Page, PageSize, Search, ...Filters });
  },

  async GetById(Id: number): Promise<Student> {
    return HttpClient.Get<Student>(`${StudentsPath}/${Id}`);
  },

  async Create(Request: StudentRequest): Promise<Student> {
    return HttpClient.Post<Student>(StudentsPath, Request);
  },

  async Update(Id: number, Request: StudentRequest): Promise<Student> {
    return HttpClient.Put<Student>(`${StudentsPath}/${Id}`, Request);
  },

  async Delete(Id: number): Promise<void> {
    await HttpClient.Delete<boolean>(`${StudentsPath}/${Id}`);
  },
};
