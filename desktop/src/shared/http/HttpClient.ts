import I18next, { BuildAcceptLanguage } from '../i18n/Config';
import { ApiErrorException } from './ApiErrorException';
import type { ApiError, ApiResponse } from './ApiResponse';

export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

export type QueryParameters = Record<string, string | number | boolean | null | undefined>;

export interface RequestOptions {
  Query?: QueryParameters;
  Body?: unknown;
  Signal?: AbortSignal;
  IsRetry?: boolean;
  // Token que se usa en el reintento tras un refresh, sin depender del almacén
  AccessTokenOverride?: string;
}

export interface HttpClientAuthConfig {
  GetAccessToken: () => string | null;
  RefreshAccessToken: () => Promise<string | null>;
}

// URL base configurada por entorno (Vite). El desktop nunca escribe la URL a mano.
export const ApiBaseUrl = (import.meta.env.VITE_API_BASE_URL as string | undefined) ?? 'http://localhost:5000';

const DefaultTimeoutMs = 20000;

export const RefreshTokenPath = '/api/v1/auth/refresh';

const ErrorKeysByStatus: Record<number, string> = {
  400: 'Errors.BadRequest',
  401: 'Errors.Unauthorized',
  403: 'Errors.Forbidden',
  404: 'Errors.NotFound',
  409: 'Errors.Conflict',
  500: 'Errors.Server',
};

const ErrorCodesByStatus: Record<number, string> = {
  400: 'Http.BadRequest',
  401: 'Http.Unauthorized',
  403: 'Http.Forbidden',
  404: 'Http.NotFound',
  409: 'Http.Conflict',
  500: 'Http.Server',
};

let AuthConfig: HttpClientAuthConfig | null = null;

function Translate(Key: string, Fallback: string): string {
  const Value = I18next.t(Key);
  return typeof Value === 'string' && Value.length > 0 && Value !== Key ? Value : Fallback;
}

function IsApiResponse(Value: unknown): Value is ApiResponse<unknown> {
  return (
    typeof Value === 'object' &&
    Value !== null &&
    'Success' in Value &&
    'Message' in Value &&
    'Data' in Value
  );
}

function IsArrayOfApiErrors(Value: unknown): Value is ApiError[] {
  return Array.isArray(Value);
}

function BuildUrl(Path: string, Query?: QueryParameters): string {
  const HasAbsolutePath = Path.startsWith('http://') || Path.startsWith('https://');
  const NormalizedPath = HasAbsolutePath
    ? Path
    : `${ApiBaseUrl.replace(/\/+$/, '')}${Path.startsWith('/') ? Path : `/${Path}`}`;

  if (Query === undefined) {
    return NormalizedPath;
  }

  const SearchParameters = new URLSearchParams();

  for (const [ParameterName, ParameterValue] of Object.entries(Query)) {
    if (ParameterValue === undefined || ParameterValue === null || ParameterValue === '') {
      continue;
    }
    SearchParameters.append(ParameterName, String(ParameterValue));
  }

  const QueryString = SearchParameters.toString();
  return QueryString.length > 0 ? `${NormalizedPath}?${QueryString}` : NormalizedPath;
}

function BuildHeaders(HasBody: boolean, AccessTokenOverride?: string): Record<string, string> {
  const RequestHeaders: Record<string, string> = {
    Accept: 'application/json',
    'Accept-Language': BuildAcceptLanguage(I18next.language),
  };

  if (HasBody) {
    RequestHeaders['Content-Type'] = 'application/json; charset=utf-8';
  }

  const AccessToken = AccessTokenOverride ?? AuthConfig?.GetAccessToken() ?? null;

  if (AccessToken !== null && AccessToken.length > 0) {
    RequestHeaders.Authorization = `Bearer ${AccessToken}`;
  }

  return RequestHeaders;
}

async function ReadPayload(Response: Response): Promise<unknown> {
  if (Response.status === 204 || Response.status === 205) {
    return null;
  }

  const RawBody = await Response.text();

  if (RawBody.length === 0) {
    return null;
  }

  try {
    return JSON.parse(RawBody) as unknown;
  } catch {
    // Una respuesta no JSON se trata como cuerpo vacío
    return null;
  }
}

function BuildExceptionFromResponse(Status: number, Payload: unknown): ApiErrorException {
  const Envelope = IsApiResponse(Payload) ? Payload : null;
  const Errors = IsArrayOfApiErrors(Envelope?.Errors) ? Envelope.Errors : [];
  const ApiMessage = typeof Envelope?.Message === 'string' ? Envelope.Message.trim() : '';
  const FallbackKey = ErrorKeysByStatus[Status] ?? 'Errors.Unknown';
  const Message =
    ApiMessage.length > 0
      ? ApiMessage
      : Translate(FallbackKey, 'Ocurrió un error inesperado');
  const Code = Errors[0]?.Code ?? ErrorCodesByStatus[Status] ?? 'Http.Unknown';

  return new ApiErrorException(Message, Code, Status, Errors, Envelope?.TraceId ?? null);
}

async function RequestTokenRefresh(): Promise<string | null> {
  if (AuthConfig === null) {
    return null;
  }

  try {
    return await AuthConfig.RefreshAccessToken();
  } catch {
    // Si el refresh falla, el 401 original se propaga con su mensaje localizado
    return null;
  }
}

async function SendRequest<T>(
  Method: HttpMethod,
  Path: string,
  Options: RequestOptions = {},
): Promise<T> {
  const RequestUrl = BuildUrl(Path, Options.Query);
  const Controller = new AbortController();
  const TimeoutIdentifier = setTimeout(() => Controller.abort(), DefaultTimeoutMs);
  const ExternalSignal = Options.Signal;
  const AbortByExternalSignal = () => Controller.abort();

  ExternalSignal?.addEventListener('abort', AbortByExternalSignal);

  try {
    const HasBody = Options.Body !== undefined && Options.Body !== null;
    const Response = await fetch(RequestUrl, {
      method: Method,
      headers: BuildHeaders(HasBody, Options.AccessTokenOverride),
      body: HasBody ? JSON.stringify(Options.Body) : undefined,
      signal: Controller.signal,
    });

    const Payload = await ReadPayload(Response);

    // Un solo reintento con refresh token cuando la API responde 401
    if (Response.status === 401 && Options.IsRetry !== true && Path !== RefreshTokenPath) {
      const NewAccessToken = await RequestTokenRefresh();

      if (NewAccessToken !== null && NewAccessToken.length > 0) {
        return await SendRequest<T>(Method, Path, {
          ...Options,
          IsRetry: true,
          AccessTokenOverride: NewAccessToken,
        });
      }
    }

    if (!Response.ok) {
      throw BuildExceptionFromResponse(Response.status, Payload);
    }

    if (IsApiResponse(Payload)) {
      if (!Payload.Success) {
        throw BuildExceptionFromResponse(Response.status, Payload);
      }

      return Payload.Data as T;
    }

    return Payload as T;
  } catch (CaughtError) {
    if (CaughtError instanceof ApiErrorException) {
      throw CaughtError;
    }

    const IsAborted = (CaughtError as Error | null)?.name === 'AbortError';
    const IsTimeout = IsAborted && ExternalSignal?.aborted !== true;
    const Message = IsTimeout
      ? Translate('Errors.Timeout', 'La operación tardó demasiado')
      : Translate('Errors.Network', 'No se pudo conectar con el servidor');

    throw new ApiErrorException(
      Message,
      IsTimeout ? 'Http.Timeout' : 'Http.Network',
      0,
      [],
      null,
      true,
    );
  } finally {
    clearTimeout(TimeoutIdentifier);
    ExternalSignal?.removeEventListener('abort', AbortByExternalSignal);
  }
}

// Cliente HTTP tipado: JWT, refresh en 401, Accept-Language y errores localizados.
export const HttpClient = {
  ConfigureAuth(Config: HttpClientAuthConfig | null): void {
    AuthConfig = Config;
  },

  Get<T>(Path: string, Query?: QueryParameters): Promise<T> {
    return SendRequest<T>('GET', Path, { Query });
  },

  Post<T>(Path: string, Body?: unknown, Query?: QueryParameters): Promise<T> {
    return SendRequest<T>('POST', Path, { Body, Query });
  },

  Put<T>(Path: string, Body?: unknown, Query?: QueryParameters): Promise<T> {
    return SendRequest<T>('PUT', Path, { Body, Query });
  },

  Patch<T>(Path: string, Body?: unknown, Query?: QueryParameters): Promise<T> {
    return SendRequest<T>('PATCH', Path, { Body, Query });
  },

  Delete<T>(Path: string, Query?: QueryParameters): Promise<T> {
    return SendRequest<T>('DELETE', Path, { Query });
  },
};
