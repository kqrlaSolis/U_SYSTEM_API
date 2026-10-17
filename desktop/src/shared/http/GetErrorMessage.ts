import I18next from '../i18n/Config';
import { ApiErrorException } from './ApiErrorException';

// Mensaje localizado del error: la API ya traduce y el HttpClient normaliza.
// Si el error no viene de la API, se usa su mensaje; si no hay, el catálogo i18n.
export function GetErrorMessage(CaughtError: unknown): string {
  if (CaughtError instanceof ApiErrorException) {
    return CaughtError.Message;
  }

  if (CaughtError instanceof Error && CaughtError.message.length > 0) {
    return CaughtError.message;
  }

  return I18next.t('Errors.Unknown');
}
