import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { Career, CareerRequest } from './Career';

const CareersPath = '/api/v1/careers';

// Servicio del módulo: sin estado de UI y sin conocer React
export const CareersService = {
  async GetPaged(Page: number, PageSize: number, Search: string): Promise<PagedResult<Career>> {
    return HttpClient.Get<PagedResult<Career>>(CareersPath, { Page, PageSize, Search });
  },

  async GetById(Id: number): Promise<Career> {
    return HttpClient.Get<Career>(`${CareersPath}/${Id}`);
  },

  async Create(Request: CareerRequest): Promise<Career> {
    return HttpClient.Post<Career>(CareersPath, Request);
  },

  async Update(Id: number, Request: CareerRequest): Promise<Career> {
    return HttpClient.Put<Career>(`${CareersPath}/${Id}`, Request);
  },

  async Delete(Id: number): Promise<void> {
    await HttpClient.Delete<boolean>(`${CareersPath}/${Id}`);
  },
};
