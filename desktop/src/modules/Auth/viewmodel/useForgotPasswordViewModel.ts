import { useCallback, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import I18next from '../../../shared/i18n/Config';
import { AuthService } from '../model/AuthService';

// ViewModel de "olvidé mi contraseña": envía el correo y muestra un mensaje genérico.
export function useForgotPasswordViewModel() {
  const [Email, SetEmail] = useState('');
  const [IsSubmitting, SetIsSubmitting] = useState(false);
  const [IsSent, SetIsSent] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [EmailError, SetEmailError] = useState<string | null>(null);

  const Submit = useCallback(async () => {
    if (Email.trim().length === 0 || !Email.includes('@')) {
      SetEmailError(I18next.t('Validation.InvalidEmail'));
      return;
    }

    SetEmailError(null);
    SetIsSubmitting(true);
    SetErrorMessage(null);

    try {
      await AuthService.ForgotPasswordAsync({ Email: Email.trim() });
      SetIsSent(true);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [Email]);

  return {
    Email,
    IsSubmitting,
    IsSent,
    ErrorMessage,
    EmailError,
    SetEmail,
    Submit,
  };
}
