import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { Charge, ChargeRequest, PaymentConcept, PaymentConceptRequest } from './PaymentConcept';

const ConceptsPath = '/api/v1/payment-concepts';
const ChargesPath = '/api/v1/charges';

// Servicio de finanzas: conceptos y cargos
export const PaymentsService = {
  async GetConcepts(Page: number, PageSize: number, Search: string): Promise<PagedResult<PaymentConcept>> {
    return HttpClient.Get<PagedResult<PaymentConcept>>(ConceptsPath, { Page, PageSize, Search });
  },

  async GetConceptById(Id: number): Promise<PaymentConcept> {
    return HttpClient.Get<PaymentConcept>(`${ConceptsPath}/${Id}`);
  },

  async CreateConcept(Request: PaymentConceptRequest): Promise<PaymentConcept> {
    return HttpClient.Post<PaymentConcept>(ConceptsPath, Request);
  },

  async UpdateConcept(Id: number, Request: PaymentConceptRequest): Promise<PaymentConcept> {
    return HttpClient.Put<PaymentConcept>(`${ConceptsPath}/${Id}`, Request);
  },

  async DeleteConcept(Id: number): Promise<void> {
    await HttpClient.Delete<boolean>(`${ConceptsPath}/${Id}`);
  },

  async GetCharges(Page: number, PageSize: number, Search: string): Promise<PagedResult<Charge>> {
    return HttpClient.Get<PagedResult<Charge>>(ChargesPath, { Page, PageSize, Search });
  },

  async CreateCharge(Request: ChargeRequest): Promise<Charge> {
    return HttpClient.Post<Charge>(ChargesPath, Request);
  },

  async CancelCharge(Id: number): Promise<Charge> {
    return HttpClient.Post<Charge>(`${ChargesPath}/${Id}/cancel`, {});
  },
};
