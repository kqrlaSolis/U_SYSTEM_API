import { useCallback, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../../shared/auth/AuthContext';
import I18next from '../../../shared/i18n/Config';
import { AuthService } from '../model/AuthService';

export interface ChangePasswordValidationErrors {
  CurrentPassword?: string;
  NewPassword?: string;
  ConfirmPassword?: string;
}

const MinimumPasswordLength = 8;

// ViewModel del cambio de contraseña (obligatorio en el primer ingreso o voluntario).
export function useChangePasswordViewModel() {
  const Auth = useAuth();
  const Navigator = useNavigate();
  const [CurrentPassword, SetCurrentPassword] = useState('');
  const [NewPassword, SetNewPassword] = useState('');
  const [ConfirmPassword, SetConfirmPassword] = useState('');
  const [ValidationErrors, SetValidationErrors] = useState<ChangePasswordValidationErrors>({});
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [IsSubmitting, SetIsSubmitting] = useState(false);

  const Validate = useCallback((): ChangePasswordValidationErrors => {
    const Errors: ChangePasswordValidationErrors = {};

    if (CurrentPassword.length === 0) {
      Errors.CurrentPassword = I18next.t('Validation.Required');
    }

    if (NewPassword.length < MinimumPasswordLength) {
      Errors.NewPassword = I18next.t('Validation.MinLength', {
        Field: I18next.t('ChangePassword.New'),
        Min: MinimumPasswordLength,
      });
    } else if (NewPassword === CurrentPassword) {
      Errors.NewPassword = I18next.t('ChangePassword.Reused');
    }

    if (ConfirmPassword !== NewPassword) {
      Errors.ConfirmPassword = I18next.t('ChangePassword.Mismatch');
    }

    return Errors;
  }, [ConfirmPassword, CurrentPassword, NewPassword]);

  const Submit = useCallback(async () => {
    const Errors = Validate();
    SetValidationErrors(Errors);

    if (Object.keys(Errors).length > 0) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);

    try {
      await AuthService.ChangePasswordAsync({ CurrentPassword, NewPassword });
      Auth.MarkPasswordChanged();
      Navigator('/', { replace: true });
    } catch (CaughtError) {
      // El mensaje ya viene localizado desde la API
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [Auth, CurrentPassword, Navigator, NewPassword, Validate]);

  return {
    CurrentPassword,
    NewPassword,
    ConfirmPassword,
    ValidationErrors,
    ErrorMessage,
    IsSubmitting,
    SetCurrentPassword,
    SetNewPassword,
    SetConfirmPassword,
    Submit,
  };
}
