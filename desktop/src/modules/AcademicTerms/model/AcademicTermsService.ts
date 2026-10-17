import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { AcademicTerm, AcademicTermRequest } from './AcademicTerm';

const TermsPath = '/api/v1/academic-terms';

// Servicio del módulo: sin estado de UI y sin conocer React
export const AcademicTermsService = {
  async GetPaged(Page: number, PageSize: number, Search: string): Promise<PagedResult<AcademicTerm>> {
    return HttpClient.Get<PagedResult<AcademicTerm>>(TermsPath, { Page, PageSize, Search });
  },

  async GetById(Id: number): Promise<AcademicTerm> {
    return HttpClient.Get<AcademicTerm>(`${TermsPath}/${Id}`);
  },

  // Devuelve null cuando todavía no hay periodo en curso (la API responde 404)
  async GetCurrent(): Promise<AcademicTerm | null> {
    try {
      return await HttpClient.Get<AcademicTerm>(`${TermsPath}/current`);
    } catch {
      return null;
    }
  },

  async Create(Request: AcademicTermRequest): Promise<AcademicTerm> {
    return HttpClient.Post<AcademicTerm>(TermsPath, Request);
  },

  async Update(Id: number, Request: AcademicTermRequest): Promise<AcademicTerm> {
    return HttpClient.Put<AcademicTerm>(`${TermsPath}/${Id}`, Request);
  },

  async Open(Id: number): Promise<AcademicTerm> {
    return HttpClient.Post<AcademicTerm>(`${TermsPath}/${Id}/open`, {});
  },

  async Close(Id: number): Promise<AcademicTerm> {
    return HttpClient.Post<AcademicTerm>(`${TermsPath}/${Id}/close`, {});
  },
};
