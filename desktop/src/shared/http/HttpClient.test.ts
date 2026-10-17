import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ApiErrorException } from './ApiErrorException';
import { ApiBaseUrl, HttpClient, RefreshTokenPath } from './HttpClient';

interface FakeResponse {
  status: number;
  ok: boolean;
  text: () => Promise<string>;
}

function BuildResponse(Status: number, Body: unknown): Response {
  const Fake: FakeResponse = {
    status: Status,
    ok: Status >= 200 && Status < 300,
    text: async () => (Body === null ? '' : JSON.stringify(Body)),
  };

  return Fake as unknown as Response;
}

function BuildSuccess(Data: unknown): Response {
  return BuildResponse(200, {
    Success: true,
    Message: 'OK',
    Data,
    Errors: [],
    TraceId: null,
  });
}

function BuildFailure(Status: number, Message: string, Code: string, Field = ''): Response {
  return BuildResponse(Status, {
    Success: false,
    Message,
    Data: null,
    Errors: [{ Code, Field, Message }],
    TraceId: '00-test',
  });
}

const FetchMock = vi.fn();

describe('HttpClient', () => {
  beforeEach(() => {
    FetchMock.mockReset();
    vi.stubGlobal('fetch', FetchMock);
    HttpClient.ConfigureAuth(null);
  });

  it('envía Accept-Language, Authorization y la query en PascalCase', async () => {
    FetchMock.mockResolvedValue(BuildSuccess({ Id: 7 }));
    HttpClient.ConfigureAuth({
      GetAccessToken: () => 'Token-Abc',
      RefreshAccessToken: async () => null,
    });

    const Result = await HttpClient.Get<{ Id: number }>('/api/v1/users', {
      Page: 2,
      PageSize: 20,
      Search: '',
    });

    expect(Result.Id).toBe(7);
    expect(FetchMock).toHaveBeenCalledTimes(1);

    const RequestUrl = String(FetchMock.mock.calls[0][0]);
    const RequestHeaders = FetchMock.mock.calls[0][1].headers as Record<string, string>;

    expect(RequestUrl).toBe(`${ApiBaseUrl}/api/v1/users?Page=2&PageSize=20`);
    expect(RequestHeaders['Accept-Language']).toContain('es-MX');
    expect(RequestHeaders.Authorization).toBe('Bearer Token-Abc');
    expect(RequestHeaders.Accept).toBe('application/json');
  });

  it('reintenta una sola vez con el refresh token cuando la API responde 401', async () => {
    const RefreshAccessToken = vi.fn(async () => 'Token-New');

    FetchMock.mockResolvedValueOnce(
      BuildFailure(401, 'Sesión expirada', 'Auth.Unauthorized'),
    ).mockResolvedValueOnce(BuildSuccess({ Id: 1 }));

    HttpClient.ConfigureAuth({
      GetAccessToken: () => 'Token-Old',
      RefreshAccessToken,
    });

    const Result = await HttpClient.Get<{ Id: number }>('/api/v1/users/1');

    expect(Result.Id).toBe(1);
    expect(RefreshAccessToken).toHaveBeenCalledTimes(1);
    expect(FetchMock).toHaveBeenCalledTimes(2);

    const SecondHeaders = FetchMock.mock.calls[1][1].headers as Record<string, string>;
    expect(SecondHeaders.Authorization).toBe('Bearer Token-New');
  });

  it('no reintenta si el refresh no devuelve token y propaga el error localizado', async () => {
    FetchMock.mockResolvedValue(BuildFailure(401, 'Sesión expirada', 'Auth.Unauthorized'));

    HttpClient.ConfigureAuth({
      GetAccessToken: () => 'Token-Old',
      RefreshAccessToken: async () => null,
    });

    await expect(HttpClient.Get('/api/v1/users')).rejects.toMatchObject({
      Code: 'Auth.Unauthorized',
      Status: 401,
      Message: 'Sesión expirada',
    });
  });

  it('no intenta refrescar el propio endpoint de refresh', async () => {
    const RefreshAccessToken = vi.fn(async () => 'Token-New');

    FetchMock.mockResolvedValue(BuildFailure(401, 'Sesión expirada', 'Auth.Unauthorized'));

    HttpClient.ConfigureAuth({
      GetAccessToken: () => 'Token-Old',
      RefreshAccessToken,
    });

    await expect(
      HttpClient.Post(RefreshTokenPath, { RefreshToken: 'Refresh-Old' }),
    ).rejects.toBeInstanceOf(ApiErrorException);
    expect(RefreshAccessToken).not.toHaveBeenCalled();
  });

  it('mapea un error de negocio de la API a ApiErrorException', async () => {
    FetchMock.mockResolvedValue(
      BuildFailure(409, 'Ya existe un usuario con el correo ana@usystem.local.', 'Users.DuplicateEmail', 'Email'),
    );

    const CaughtError = await HttpClient.Post('/api/v1/users', { Email: 'ana@usystem.local' }).catch(
      (ErrorThrown: unknown) => ErrorThrown,
    );

    expect(CaughtError).toBeInstanceOf(ApiErrorException);
    const ApiError = CaughtError as ApiErrorException;
    expect(ApiError.Code).toBe('Users.DuplicateEmail');
    expect(ApiError.Message).toContain('ana@usystem.local');
    expect(ApiError.Status).toBe(409);
    expect(ApiError.TraceId).toBe('00-test');
    expect(ApiError.GetErrorCodesByField()).toEqual({ Email: ['Users.DuplicateEmail'] });
  });

  it('traduce a ApiErrorException un fallo de red', async () => {
    FetchMock.mockRejectedValue(new TypeError('Failed to fetch'));

    const CaughtError = await HttpClient.Get('/api/v1/users').catch(
      (ErrorThrown: unknown) => ErrorThrown,
    );

    expect(CaughtError).toBeInstanceOf(ApiErrorException);
    expect((CaughtError as ApiErrorException).Code).toBe('Http.Network');
    expect((CaughtError as ApiErrorException).IsNetworkError).toBe(true);
  });
});
