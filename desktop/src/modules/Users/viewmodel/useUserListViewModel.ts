import { useCallback, useState } from 'react';
import { useAuth } from '../../../shared/auth/AuthContext';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import { DefaultPageSize, usePagedQuery } from '../../../shared/http/usePagedQuery';
import { usePermissions } from '../../../shared/auth/usePermissions';
import type { User } from '../model/User';
import { UsersService } from '../model/UsersService';

const CreatePermission = 'Users.Create';
const EditPermission = 'Users.Edit';
const DeletePermission = 'Users.Delete';

// ViewModel del listado: estado de la pantalla, paginación, permisos y baja lógica. Sin JSX.
export function useUserListViewModel() {
  const Permissions = usePermissions();
  const Auth = useAuth();
  const Paged = usePagedQuery<User>({
    FetchPage: UsersService.GetPaged,
    PageSize: DefaultPageSize,
  });

  // Usuario que espera confirmación en el diálogo de eliminar
  const [PendingDelete, SetPendingDelete] = useState<User | null>(null);
  const [IsDeleting, SetIsDeleting] = useState(false);
  const [ActionError, SetActionError] = useState<string | null>(null);

  const ReloadPage = Paged.Reload;
  const Reload = useCallback(() => {
    SetActionError(null);
    ReloadPage();
  }, [ReloadPage]);

  const RequestDelete = useCallback((Target: User) => {
    SetActionError(null);
    SetPendingDelete(Target);
  }, []);

  const CancelDelete = useCallback(() => {
    SetPendingDelete(null);
  }, []);

  const ConfirmDelete = useCallback(async () => {
    if (PendingDelete === null) {
      return;
    }

    SetIsDeleting(true);
    SetActionError(null);

    try {
      await UsersService.Delete(PendingDelete.Id);
      SetPendingDelete(null);
      ReloadPage();
    } catch (CaughtError) {
      // La API protege al último Administrator y al usuario actual: el motivo se muestra en la página
      SetPendingDelete(null);
      SetActionError(GetErrorMessage(CaughtError));
    } finally {
      SetIsDeleting(false);
    }
  }, [PendingDelete, ReloadPage]);

  // El usuario con sesión no ve el botón de eliminarse; la API también lo rechaza
  const CanDeleteUser = useCallback(
    (Target: User) => Permissions.Can(DeletePermission) && Target.Id !== Auth.CurrentUser?.Id,
    [Auth.CurrentUser?.Id, Permissions],
  );

  return {
    Items: Paged.Items,
    TotalCount: Paged.TotalCount,
    Page: Paged.Page,
    PageSize: Paged.PageSize,
    Search: Paged.Search,
    IsLoading: Paged.IsLoading,
    ErrorMessage: Paged.ErrorMessage,
    ActionError,
    PendingDelete,
    IsDeleting,
    CanCreate: Permissions.Can(CreatePermission),
    CanEdit: Permissions.Can(EditPermission),
    CanDelete: Permissions.Can(DeletePermission),
    CanDeleteUser,
    SetPage: Paged.SetPage,
    SetPageSize: Paged.SetPageSize,
    SetSearch: Paged.SetSearch,
    Reload,
    RequestDelete,
    CancelDelete,
    ConfirmDelete,
  };
}
