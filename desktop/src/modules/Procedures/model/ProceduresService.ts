import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { Procedure, ProcedureType, ProcedureTypeRequest } from './ProcedureType';

const TypesPath = '/api/v1/procedure-types';
const ProceduresPath = '/api/v1/procedures';

// Servicio de trámites: tipos y solicitudes con folio
export const ProceduresService = {
  async GetTypes(Page: number, PageSize: number, Search: string): Promise<PagedResult<ProcedureType>> {
    return HttpClient.Get<PagedResult<ProcedureType>>(TypesPath, { Page, PageSize, Search });
  },

  async GetTypeById(Id: number): Promise<ProcedureType> {
    return HttpClient.Get<ProcedureType>(`${TypesPath}/${Id}`);
  },

  async CreateType(Request: ProcedureTypeRequest): Promise<ProcedureType> {
    return HttpClient.Post<ProcedureType>(TypesPath, Request);
  },

  async UpdateType(Id: number, Request: ProcedureTypeRequest): Promise<ProcedureType> {
    return HttpClient.Put<ProcedureType>(`${TypesPath}/${Id}`, Request);
  },

  async DeleteType(Id: number): Promise<void> {
    await HttpClient.Delete<boolean>(`${TypesPath}/${Id}`);
  },

  async GetProcedures(Page: number, PageSize: number, Search: string): Promise<PagedResult<Procedure>> {
    return HttpClient.Get<PagedResult<Procedure>>(ProceduresPath, { Page, PageSize, Search });
  },

  async CreateProcedure(ProcedureTypeId: number, StudentId: number): Promise<Procedure> {
    return HttpClient.Post<Procedure>(ProceduresPath, { ProcedureTypeId, StudentId });
  },

  async Issue(Id: number, FilePath: string | null): Promise<Procedure> {
    return HttpClient.Post<Procedure>(`${ProceduresPath}/${Id}/issue`, { FilePath });
  },
};
