import type { ApiError } from './ApiResponse';

// Error normalizado de la capa HTTP: Message llega localizado desde la API o desde
// el catálogo i18n; Code es estable y es lo que sirve para decidir el flujo.
export class ApiErrorException extends Error {
  public readonly Code: string;

  public readonly Status: number;

  public readonly Errors: ApiError[];

  public readonly TraceId: string | null;

  public readonly Message: string;

  public readonly IsNetworkError: boolean;

  constructor(
    Message: string,
    Code: string,
    Status: number,
    Errors: ApiError[] = [],
    TraceId: string | null = null,
    IsNetworkError = false,
  ) {
    super(Message);
    this.name = 'ApiErrorException';
    this.Message = Message;
    this.Code = Code;
    this.Status = Status;
    this.Errors = Errors;
    this.TraceId = TraceId;
    this.IsNetworkError = IsNetworkError;
  }

  // Códigos de los campos que fallaron, útil para pintar errores por input
  public GetErrorCodesByField(): Record<string, string[]> {
    const CodesByField: Record<string, string[]> = {};

    for (const ErrorItem of this.Errors) {
      if (!ErrorItem.Field) {
        continue;
      }
      CodesByField[ErrorItem.Field] = [...(CodesByField[ErrorItem.Field] ?? []), ErrorItem.Code];
    }

    return CodesByField;
  }
}
