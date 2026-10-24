import { ITAlert, ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { AppButton, DataTable, SearchBox } from '../../../shared/components';
import type { DataTableColumn } from '../../../shared/components';
import type { AcademicTerm } from '../model/AcademicTerm';
import { useAcademicTermListViewModel } from '../viewmodel/useAcademicTermListViewModel';

// Calendario cuatrimestral: estado, apertura/cierre y periodo en curso.
export function AcademicTermListPage() {
  const ViewModel = useAcademicTermListViewModel();
  const Translation = useTranslation();
  const Navigator = useNavigate();

  const Columns: DataTableColumn<AcademicTerm>[] = [
    { Field: 'Code', Header: Translation.t('AcademicTerms.Columns.Code') },
    { Field: 'Name', Header: Translation.t('AcademicTerms.Columns.Name') },
    { Field: 'StartDate', Header: Translation.t('AcademicTerms.Columns.StartDate'), Type: 'date' },
    { Field: 'EndDate', Header: Translation.t('AcademicTerms.Columns.EndDate'), Type: 'date' },
    {
      Field: 'Status',
      Header: Translation.t('AcademicTerms.Columns.Status'),
      Render: (Row) => Translation.t(`AcademicTerms.Status.${Row.Status}`),
    },
    {
      Field: 'Id',
      Header: Translation.t('Common.Actions'),
      Type: 'actions',
      Align: 'right',
      Render: (Row) => (
        <div className="USystemRowActions">
          {ViewModel.CanEdit && (
            <AppButton
              Label={Translation.t('Common.Edit')}
              Variant="text"
              Size="sm"
              AriaLabel={`${Translation.t('Common.Edit')} ${Row.Code}`}
              OnClick={() => Navigator(`/academic-terms/${Row.Id}`)}
            />
          )}
          {ViewModel.CanOpen && Row.Status === 'Planned' && (
            <AppButton
              Label={Translation.t('AcademicTerms.Actions.Open')}
              Variant="text"
              Size="sm"
              IsLoading={ViewModel.BusyId === Row.Id}
              OnClick={() => {
                void ViewModel.Open(Row);
              }}
            />
          )}
          {ViewModel.CanClose && Row.Status !== 'Closed' && (
            <AppButton
              Label={Translation.t('AcademicTerms.Actions.Close')}
              Variant="text"
              Size="sm"
              Color="danger"
              IsLoading={ViewModel.BusyId === Row.Id}
              OnClick={() => {
                void ViewModel.Close(Row);
              }}
            />
          )}
        </div>
      ),
    },
  ];

  return (
    <ITPage
      title={Translation.t('AcademicTerms.Title')}
      description={Translation.t('AcademicTerms.Subtitle')}
      maxWidth="7xl"
      loading={ViewModel.IsLoading && ViewModel.Items.length === 0}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Reload}
      actions={
        ViewModel.CanCreate ? (
          <AppButton
            Label={Translation.t('AcademicTerms.New')}
            OnClick={() => Navigator('/academic-terms/new')}
          />
        ) : undefined
      }
    >
      <ITStack spacing={4}>
        <ITAlert variant={ViewModel.Current === null ? 'warning' : 'info'} title={Translation.t('AcademicTerms.Current')}>
          {ViewModel.Current === null
            ? Translation.t('AcademicTerms.NoCurrent')
            : `${ViewModel.Current.Code} · ${ViewModel.Current.Name}`}
        </ITAlert>

        <div className="USystemToolbar">
          <SearchBox
            Value={ViewModel.Search}
            OnChange={ViewModel.SetSearch}
            OnSearch={ViewModel.Reload}
            Placeholder={Translation.t('AcademicTerms.Search')}
            Name="TermSearch"
          />
          <ITText as="span" muted className="USystemToolbarMeta">
            {Translation.t('Common.Total')}: {ViewModel.TotalCount}
          </ITText>
        </div>

        <DataTable<AcademicTerm>
          Columns={Columns}
          Rows={ViewModel.Items}
          TotalCount={ViewModel.TotalCount}
          Page={ViewModel.Page}
          PageSize={ViewModel.PageSize}
          OnPageChange={ViewModel.SetPage}
          IsLoading={ViewModel.IsLoading}
          EmptyMessage={Translation.t('AcademicTerms.Empty')}
        />
      </ITStack>
    </ITPage>
  );
}
