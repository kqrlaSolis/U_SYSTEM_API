import { useCallback, useState } from 'react';
import { useAuth } from '../../../shared/auth/AuthContext';
import { ApiErrorException } from '../../../shared/http/ApiErrorException';
import I18next from '../../../shared/i18n/Config';

export interface LoginValidationErrors {
  Username?: string;
  Password?: string;
}

const MinimumPasswordLength = 6;

// ViewModel del login: estado del formulario, validación y envío. Sin JSX.
export function useLoginViewModel() {
  const Auth = useAuth();
  const [Username, SetUsername] = useState('');
  const [Password, SetPassword] = useState('');
  const [ValidationErrors, SetValidationErrors] = useState<LoginValidationErrors>({});
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [IsSubmitting, SetIsSubmitting] = useState(false);

  const Validate = useCallback((): LoginValidationErrors => {
    const Errors: LoginValidationErrors = {};

    if (Username.trim().length === 0) {
      Errors.Username = I18next.t('Validation.Required');
    }

    if (Password.length === 0) {
      Errors.Password = I18next.t('Validation.Required');
    } else if (Password.length < MinimumPasswordLength) {
      Errors.Password = I18next.t('Validation.MinLength', {
        Field: I18next.t('Login.Password'),
        Min: MinimumPasswordLength,
      });
    }

    return Errors;
  }, [Password, Username]);

  const Submit = useCallback(async () => {
    const Errors = Validate();
    SetValidationErrors(Errors);

    if (Object.keys(Errors).length > 0) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);

    try {
      await Auth.SignIn({ UsernameOrEmail: Username.trim(), Password });
    } catch (CaughtError) {
      // El mensaje ya viene localizado desde la API o desde el HttpClient
      if (CaughtError instanceof ApiErrorException) {
        SetErrorMessage(CaughtError.Message);
      } else {
        SetErrorMessage(I18next.t('Errors.Unknown'));
      }
    } finally {
      SetIsSubmitting(false);
    }
  }, [Auth, Password, Username, Validate]);

  return {
    Username,
    Password,
    ValidationErrors,
    ErrorMessage,
    IsSubmitting,
    SetUsername,
    SetPassword,
    Submit,
  };
}
