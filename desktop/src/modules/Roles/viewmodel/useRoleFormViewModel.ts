import { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import I18next from '../../../shared/i18n/Config';
import type { PermissionModule } from '../model/Permission';
import { RolesService } from '../model/RolesService';

// ViewModel del editor de rol: datos, permisos por módulo y guardado.
export function useRoleFormViewModel(RoleId?: number) {
  const Navigator = useNavigate();
  const IsEditing = RoleId !== undefined;

  const [Name, SetName] = useState('');
  const [Description, SetDescription] = useState('');
  const [IsActive, SetIsActive] = useState(true);
  const [SelectedCodes, SetSelectedCodes] = useState<string[]>([]);
  const [Catalog, SetCatalog] = useState<PermissionModule[]>([]);
  const [IsLoading, SetIsLoading] = useState(true);
  const [IsSubmitting, SetIsSubmitting] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [NameError, SetNameError] = useState<string | null>(null);
  const [IsSystem, SetIsSystem] = useState(false);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const Modules = await RolesService.GetPermissionCatalog();
      SetCatalog(Modules);

      if (IsEditing && RoleId !== undefined) {
        const Role = await RolesService.GetById(RoleId);
        SetName(Role.Name);
        SetDescription(Role.Description ?? '');
        SetIsActive(Role.IsActive);
        SetIsSystem(Role.IsSystem);
        SetSelectedCodes(Role.PermissionCodes);
      }
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [IsEditing, RoleId]);

  useEffect(() => {
    void Load();
  }, [Load]);

  const ToggleCode = useCallback((Code: string, Checked: boolean) => {
    SetSelectedCodes((Current) =>
      Checked ? [...Current.filter((Item) => Item !== Code), Code] : Current.filter((Item) => Item !== Code),
    );
  }, []);

  const ToggleModule = useCallback((Permissions: string[], Checked: boolean) => {
    SetSelectedCodes((Current) => {
      const Remaining = Current.filter((Code) => !Permissions.includes(Code));
      return Checked ? [...Remaining, ...Permissions] : Remaining;
    });
  }, []);

  const Submit = useCallback(async () => {
    if (Name.trim().length === 0) {
      SetNameError(I18next.t('Validation.Required'));
      return;
    }

    SetNameError(null);
    SetIsSubmitting(true);
    SetErrorMessage(null);

    const Request = {
      Name: Name.trim(),
      Description: Description.trim() === '' ? null : Description.trim(),
      IsActive,
      PermissionCodes: SelectedCodes,
    };

    try {
      if (!IsEditing || RoleId === undefined) {
        await RolesService.Create(Request);
      } else {
        await RolesService.Update(RoleId, Request);
      }

      Navigator('/roles', { replace: true });
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [Description, IsActive, IsEditing, Name, Navigator, RoleId, SelectedCodes]);

  const SelectedCount = useMemo(() => SelectedCodes.length, [SelectedCodes]);

  return {
    IsEditing,
    IsSystem,
    Name,
    Description,
    IsActive,
    SelectedCodes,
    Catalog,
    SelectedCount,
    IsLoading,
    IsSubmitting,
    ErrorMessage,
    NameError,
    SetName,
    SetDescription,
    SetIsActive,
    ToggleCode,
    ToggleModule,
    Submit,
    Reload: Load,
  };
}
