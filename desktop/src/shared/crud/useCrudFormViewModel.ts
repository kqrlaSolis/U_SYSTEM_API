import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { GetErrorMessage } from '../http/GetErrorMessage';
import I18next from '../i18n/Config';
import type { CrudConfig, CrudRow } from './CrudTypes';

// ViewModel genérico de alta/edición: valores, validación de obligatorios y envío.
export function useCrudFormViewModel<TRow extends CrudRow, TRequest>(
  Config: CrudConfig<TRow, TRequest>,
  Id?: number,
) {
  const Navigator = useNavigate();
  const IsEditing = Id !== undefined;

  const [Values, SetValues] = useState<Record<string, string>>(Config.EmptyForm);
  const [Errors, SetErrors] = useState<Record<string, string>>({});
  const [IsLoading, SetIsLoading] = useState(IsEditing);
  const [IsSubmitting, SetIsSubmitting] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    if (!IsEditing || Id === undefined) {
      SetValues(Config.EmptyForm);
      SetIsLoading(false);
      return;
    }

    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const Row = await Config.Load(Id);
      SetValues(Config.ToForm(Row));
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [Config, Id, IsEditing]);

  useEffect(() => {
    void Load();
  }, [Load]);

  const SetValue = useCallback((Name: string, Value: string) => {
    SetValues((Current) => ({ ...Current, [Name]: Value }));
  }, []);

  const Submit = useCallback(async () => {
    const ValidationErrors: Record<string, string> = {};

    for (const Field of Config.Fields) {
      const Value = (Values[Field.Name] ?? '').trim();
      if (Field.Required === true && Value.length === 0) {
        ValidationErrors[Field.Name] = I18next.t('Validation.Required');
      }
    }

    SetErrors(ValidationErrors);
    if (Object.keys(ValidationErrors).length > 0) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);

    try {
      const Request = Config.FromForm(Values);

      if (!IsEditing || Id === undefined) {
        await Config.Create(Request);
      } else {
        await Config.Update(Id, Request);
      }

      Navigator(Config.Route, { replace: true });
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [Config, Id, IsEditing, Navigator, Values]);

  return {
    IsEditing,
    Values,
    Errors,
    IsLoading,
    IsSubmitting,
    ErrorMessage,
    SetValue,
    Submit,
    Reload: Load,
  };
}
