import { useCallback, useEffect, useMemo, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import { useNavigate } from 'react-router-dom';
import I18next from '../../../shared/i18n/Config';
import type { Role } from '../../Roles/model/Role';
import { RolesService } from '../../Roles/model/RolesService';
import type { User } from '../model/User';
import { UsersService } from '../model/UsersService';

export interface UserFormState {
  FirstName: string;
  PaternalLastName: string;
  MaternalLastName: string;
  Email: string;
  Username: string;
  Phone: string;
  TemporaryPassword: string;
}

export type UserFormErrors = Partial<Record<keyof UserFormState | 'RoleIds', string>>;

const EmptyForm: UserFormState = {
  FirstName: '',
  PaternalLastName: '',
  MaternalLastName: '',
  Email: '',
  Username: '',
  Phone: '',
  TemporaryPassword: '',
};

// ViewModel del formulario de usuarios: alta, edición, roles, estado y contraseña temporal.
export function useUserFormViewModel(UserId?: number) {
  const Navigator = useNavigate();
  const IsEditing = UserId !== undefined;

  const [Form, SetForm] = useState<UserFormState>(EmptyForm);
  const [Roles, SetRoles] = useState<Role[]>([]);
  const [SelectedRoleIds, SetSelectedRoleIds] = useState<number[]>([]);
  const [IsActive, SetIsActive] = useState(true);
  const [IsLoading, SetIsLoading] = useState(true);
  const [IsSubmitting, SetIsSubmitting] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [TemporaryPassword, SetTemporaryPassword] = useState<string | null>(null);
  const [ValidationErrors, SetValidationErrors] = useState<UserFormErrors>({});
  const [User, SetUser] = useState<User | null>(null);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const AvailableRoles = await RolesService.GetAll();
      SetRoles(AvailableRoles);

      if (IsEditing && UserId !== undefined) {
        const Existing = await UsersService.GetById(UserId);
        SetUser(Existing);
        SetForm({
          FirstName: Existing.FirstName,
          PaternalLastName: Existing.PaternalLastName,
          MaternalLastName: Existing.MaternalLastName ?? '',
          Email: Existing.Email,
          Username: Existing.Username,
          Phone: Existing.Phone ?? '',
          TemporaryPassword: '',
        });
        SetIsActive(Existing.IsActive);
        SetSelectedRoleIds(
          AvailableRoles.filter((Role) => Existing.Roles.includes(Role.Name)).map((Role) => Role.Id),
        );
      }
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [IsEditing, UserId]);

  useEffect(() => {
    void Load();
  }, [Load]);

  const SetField = useCallback((Field: keyof UserFormState, Value: string) => {
    SetForm((Current) => ({ ...Current, [Field]: Value }));
  }, []);

  const Validate = useCallback((): UserFormErrors => {
    const Errors: UserFormErrors = {};

    if (Form.FirstName.trim().length === 0) {
      Errors.FirstName = I18next.t('Validation.Required');
    }

    if (Form.PaternalLastName.trim().length === 0) {
      Errors.PaternalLastName = I18next.t('Validation.Required');
    }

    if (Form.Email.trim().length === 0) {
      Errors.Email = I18next.t('Validation.Required');
    } else if (!Form.Email.includes('@')) {
      Errors.Email = I18next.t('Validation.InvalidEmail');
    }

    if (Form.Username.trim().length < 3) {
      Errors.Username = I18next.t('Validation.Required');
    }

    if (!IsEditing && SelectedRoleIds.length === 0) {
      Errors.RoleIds = I18next.t('Validation.Required');
    }

    return Errors;
  }, [Form, IsEditing, SelectedRoleIds]);

  const Submit = useCallback(async () => {
    const Errors = Validate();
    SetValidationErrors(Errors);

    if (Object.keys(Errors).length > 0) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);

    const Request = {
      FirstName: Form.FirstName.trim(),
      PaternalLastName: Form.PaternalLastName.trim(),
      MaternalLastName: Form.MaternalLastName.trim() === '' ? null : Form.MaternalLastName.trim(),
      Email: Form.Email.trim(),
      Username: Form.Username.trim(),
      Phone: Form.Phone.trim() === '' ? null : Form.Phone.trim(),
    };

    try {
      if (!IsEditing || UserId === undefined) {
        const Created = await UsersService.Create({
          ...Request,
          TemporaryPassword:
            Form.TemporaryPassword.trim() === '' ? null : Form.TemporaryPassword.trim(),
          RoleIds: SelectedRoleIds,
        });
        SetTemporaryPassword(Created.TemporaryPassword);
        Navigator('/users', { replace: true });
      } else {
        await UsersService.Update(UserId, Request);
        await UsersService.AssignRoles(UserId, SelectedRoleIds);
        Navigator('/users', { replace: true });
      }
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [Form, IsEditing, Navigator, SelectedRoleIds, UserId, Validate]);

  const ToggleStatus = useCallback(async () => {
    if (UserId === undefined) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);

    try {
      const Updated = await UsersService.SetStatus(UserId, !IsActive);
      SetIsActive(Updated.IsActive);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [IsActive, UserId]);

  const ResetPassword = useCallback(async () => {
    if (UserId === undefined) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);

    try {
      const Response = await UsersService.ResetPassword(UserId);
      SetTemporaryPassword(Response.TemporaryPassword);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [UserId]);

  const RoleOptions = useMemo(
    () => Roles.map((Role) => ({ value: Role.Id, label: Role.Name })),
    [Roles],
  );

  return {
    IsEditing,
    User,
    Form,
    Roles,
    RoleOptions,
    SelectedRoleIds,
    IsActive,
    IsLoading,
    IsSubmitting,
    ErrorMessage,
    TemporaryPassword,
    ValidationErrors,
    SetField,
    SetSelectedRoleIds,
    Submit,
    ToggleStatus,
    ResetPassword,
    Reload: Load,
  };
}
