import { ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { AppButton, DataTable, SearchBox } from '../../../shared/components';
import type { DataTableColumn } from '../../../shared/components';
import type { User } from '../model/User';
import { useUserListViewModel } from '../viewmodel/useUserListViewModel';

const PageSizeOptions = [10, 20, 50];

export function UserListPage() {
  const ViewModel = useUserListViewModel();
  const Translation = useTranslation();
  const Navigator = useNavigate();

  const Columns: DataTableColumn<User>[] = [
    {
      Field: 'Username',
      Header: Translation.t('Users.Columns.Username'),
      Sortable: true,
    },
    {
      Field: 'FirstName',
      Header: Translation.t('Users.Columns.FullName'),
      Render: (CurrentUser) => `${CurrentUser.FirstName} ${CurrentUser.PaternalLastName}`,
    },
    {
      Field: 'Email',
      Header: Translation.t('Users.Columns.Email'),
    },
    {
      Field: 'IsActive',
      Header: Translation.t('Users.Columns.IsActive'),
      Type: 'boolean',
    },
  ];

  // La columna de acciones solo existe si el usuario puede editar
  if (ViewModel.CanEdit) {
    Columns.push({
      Field: 'Id',
      Header: Translation.t('Users.Columns.Actions'),
      Type: 'actions',
      Align: 'right',
      Render: (CurrentUser) => (
        <AppButton
          Label={Translation.t('Common.Edit')}
          Variant="text"
          Size="sm"
          AriaLabel={`${Translation.t('Common.Edit')} ${CurrentUser.Username}`}
          OnClick={() => Navigator(`/users/${CurrentUser.Id}`)}
        />
      ),
    });
  }

  return (
    <ITPage
      title={Translation.t('Users.List.Title')}
      description={Translation.t('Users.List.Subtitle')}
      maxWidth="7xl"
      actions={
        ViewModel.CanCreate ? (
          <AppButton
            Label={Translation.t('Users.List.New')}
            OnClick={() => Navigator('/users/new')}
          />
        ) : undefined
      }
      // ITPage resuelve los estados: esqueleto en la primera carga y error con reintento
      loading={ViewModel.IsLoading && ViewModel.Items.length === 0}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Reload}
    >
      <ITStack spacing={4}>
        <div className="USystemToolbar">
          <SearchBox
            Value={ViewModel.Search}
            OnChange={ViewModel.SetSearch}
            OnSearch={ViewModel.Reload}
            Placeholder={Translation.t('Users.List.SearchPlaceholder')}
            Name="UserSearch"
          />
          <ITText as="span" muted className="USystemToolbarMeta">
            {Translation.t('Users.List.Count', { Count: ViewModel.TotalCount })}
          </ITText>
        </div>

        <DataTable<User>
          Columns={Columns}
          Rows={ViewModel.Items}
          TotalCount={ViewModel.TotalCount}
          Page={ViewModel.Page}
          PageSize={ViewModel.PageSize}
          PageSizeOptions={PageSizeOptions}
          OnPageChange={ViewModel.SetPage}
          OnPageSizeChange={ViewModel.SetPageSize}
          IsLoading={ViewModel.IsLoading}
          EmptyMessage={Translation.t('Users.List.Empty')}
        />
      </ITStack>
    </ITPage>
  );
}
