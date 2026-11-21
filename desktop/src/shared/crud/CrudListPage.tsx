import { ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { AppButton, DataTable, RowActionButton, SearchBox } from '../components';
import { FilterBar } from './FilterBar';
import type { DataTableColumn } from '../components';
import type { CrudConfig, CrudRow } from './CrudTypes';
import { useCrudListViewModel } from './useCrudListViewModel';

export interface CrudListPageProps<TRow extends CrudRow, TRequest> {
  Config: CrudConfig<TRow, TRequest>;
}

// Pantalla de listado genérica (ITPage + DataTable) para los catálogos con CRUD.
export function CrudListPage<TRow extends CrudRow, TRequest>({
  Config,
}: CrudListPageProps<TRow, TRequest>) {
  const ViewModel = useCrudListViewModel(Config);
  const Translation = useTranslation();
  const Navigator = useNavigate();

  const Columns: DataTableColumn<TRow>[] = Config.Columns.map((Column) => ({
    Field: Column.Field,
    Header: Translation.t(Column.HeaderKey),
    Type: Column.Type,
    Render: Column.Render,
  }));

  if (ViewModel.CanEdit || ViewModel.CanDelete || Config.RowActions !== undefined) {
    Columns.push({
      Field: 'Id',
      Header: Translation.t('Common.Actions'),
      Type: 'actions',
      Align: 'right',
      Render: (Row) => (
        <div className="USystemRowActions">
          {ViewModel.CanEdit && (
            <RowActionButton
              Icon="Edit"
              Label={Translation.t('Common.Edit')}
              Subject={Row.Id}
              OnClick={() => Navigator(`${Config.Route}/${Row.Id}`)}
            />
          )}
          {Config.RowActions?.filter((Action) => Action.IsVisible?.(Row) ?? true).map((Action) => (
            <RowActionButton
              key={Action.Name}
              Icon={Action.Icon}
              Label={Translation.t(Action.LabelKey)}
              Subject={Row.Id}
              Color={Action.Color ?? 'primary'}
              OnClick={() => {
                void Action.Run(Row).then(() => ViewModel.Reload());
              }}
            />
          ))}
          {ViewModel.CanDelete && (
            <RowActionButton
              Icon="Delete"
              Label={Translation.t('Common.Delete')}
              Subject={Row.Id}
              Color="danger"
              OnClick={() => {
                void ViewModel.Remove(Row);
              }}
            />
          )}
        </div>
      ),
    });
  }

  return (
    <ITPage
      title={Translation.t(Config.TitleKey)}
      description={Translation.t(Config.SubtitleKey)}
      maxWidth="7xl"
      loading={ViewModel.IsLoading && ViewModel.Items.length === 0}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Reload}
      actions={
        ViewModel.CanCreate ? (
          <AppButton
            Label={Translation.t(Config.NewKey)}
            OnClick={() => Navigator(`${Config.Route}/new`)}
          />
        ) : undefined
      }
    >
      <ITStack spacing={4}>
        <div className="USystemToolbar">
          <SearchBox
            Value={ViewModel.Search}
            OnChange={ViewModel.SetSearch}
            OnSearch={ViewModel.Reload}
            Placeholder={Translation.t(Config.SearchKey)}
            Name="CrudSearch"
          />
          <ITText as="span" muted className="USystemToolbarMeta">
            {Translation.t('Common.Total')}: {ViewModel.TotalCount}
          </ITText>
        </div>

        {Config.Filters !== undefined && Config.Filters.length > 0 && (
          <FilterBar
            Fields={Config.Filters}
            Values={ViewModel.Filters}
            OnChange={ViewModel.SetFilter}
            OnClear={ViewModel.ClearFilters}
          />
        )}

        <DataTable<TRow>
          Columns={Columns}
          Rows={ViewModel.Items}
          TotalCount={ViewModel.TotalCount}
          Page={ViewModel.Page}
          PageSize={ViewModel.PageSize}
          OnPageChange={ViewModel.SetPage}
          IsLoading={ViewModel.IsLoading}
          EmptyMessage={Translation.t(Config.EmptyKey)}
        />
      </ITStack>
    </ITPage>
  );
}
