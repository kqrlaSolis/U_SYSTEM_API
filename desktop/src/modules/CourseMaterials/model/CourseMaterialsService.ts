import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { CourseMaterial, CourseMaterialRequest } from './CourseMaterial';

const MaterialsPath = '/api/v1/course-materials';

// Servicio del módulo: sin estado de UI y sin conocer React
export const CourseMaterialsService = {
  async GetPaged(Page: number, PageSize: number, Search: string): Promise<PagedResult<CourseMaterial>> {
    return HttpClient.Get<PagedResult<CourseMaterial>>(MaterialsPath, { Page, PageSize, Search });
  },

  async GetById(Id: number): Promise<CourseMaterial> {
    return HttpClient.Get<CourseMaterial>(`${MaterialsPath}/${Id}`);
  },

  async Create(Request: CourseMaterialRequest): Promise<CourseMaterial> {
    return HttpClient.Post<CourseMaterial>(MaterialsPath, Request);
  },

  async Update(Id: number, Request: CourseMaterialRequest): Promise<CourseMaterial> {
    return HttpClient.Put<CourseMaterial>(`${MaterialsPath}/${Id}`, Request);
  },

  async Delete(Id: number): Promise<void> {
    await HttpClient.Delete<boolean>(`${MaterialsPath}/${Id}`);
  },
};
