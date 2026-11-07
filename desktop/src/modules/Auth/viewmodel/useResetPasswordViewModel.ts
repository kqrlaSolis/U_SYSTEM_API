import { useCallback, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import I18next from '../../../shared/i18n/Config';
import { AuthService } from '../model/AuthService';

const MinimumPasswordLength = 8;

// ViewModel de "restablecer contraseña" con el token de un solo uso.
export function useResetPasswordViewModel() {
  const [Email, SetEmail] = useState('');
  const [Token, SetToken] = useState('');
  const [NewPassword, SetNewPassword] = useState('');
  const [ConfirmPassword, SetConfirmPassword] = useState('');
  const [Errors, SetErrors] = useState<Record<string, string>>({});
  const [IsSubmitting, SetIsSubmitting] = useState(false);
  const [IsDone, SetIsDone] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Submit = useCallback(async () => {
    const ValidationErrors: Record<string, string> = {};

    if (Email.trim().length === 0 || !Email.includes('@')) {
      ValidationErrors.Email = I18next.t('Validation.InvalidEmail');
    }
    if (Token.trim().length === 0) {
      ValidationErrors.Token = I18next.t('Validation.Required');
    }
    if (NewPassword.length < MinimumPasswordLength) {
      ValidationErrors.NewPassword = I18next.t('Validation.MinLength', {
        Field: I18next.t('ResetPassword.New'),
        Min: MinimumPasswordLength,
      });
    }
    if (ConfirmPassword !== NewPassword) {
      ValidationErrors.ConfirmPassword = I18next.t('ChangePassword.Mismatch');
    }

    SetErrors(ValidationErrors);
    if (Object.keys(ValidationErrors).length > 0) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);

    try {
      await AuthService.ResetPasswordAsync({
        Email: Email.trim(),
        Token: Token.trim(),
        NewPassword,
      });
      SetIsDone(true);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [ConfirmPassword, Email, NewPassword, Token]);

  return {
    Email,
    Token,
    NewPassword,
    ConfirmPassword,
    Errors,
    IsSubmitting,
    IsDone,
    ErrorMessage,
    SetEmail,
    SetToken,
    SetNewPassword,
    SetConfirmPassword,
    Submit,
  };
}
