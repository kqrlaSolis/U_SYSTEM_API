import { DefaultPageSize, usePagedQuery } from '../../../shared/hooks/usePagedQuery';
import { usePermissions } from '../../../shared/hooks/usePermissions';
import type { User } from '../model/User';
import { UsersService } from '../model/UsersService';

const CreatePermission = 'Users.Create';
const EditPermission = 'Users.Edit';

// ViewModel del listado: estado de la pantalla, paginación y permisos. Sin JSX.
export function useUserListViewModel() {
  const Permissions = usePermissions();
  const Paged = usePagedQuery<User>({
    FetchPage: UsersService.GetPaged,
    PageSize: DefaultPageSize,
  });

  return {
    Items: Paged.Items,
    TotalCount: Paged.TotalCount,
    Page: Paged.Page,
    PageSize: Paged.PageSize,
    Search: Paged.Search,
    IsLoading: Paged.IsLoading,
    ErrorMessage: Paged.ErrorMessage,
    CanCreate: Permissions.Can(CreatePermission),
    CanEdit: Permissions.Can(EditPermission),
    SetPage: Paged.SetPage,
    SetPageSize: Paged.SetPageSize,
    SetSearch: Paged.SetSearch,
    Reload: Paged.Reload,
  };
}
