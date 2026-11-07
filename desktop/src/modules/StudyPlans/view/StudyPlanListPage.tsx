import { ITBadget, ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { AppButton, DataTable, FilterBar, SearchBox } from '../../../shared/components';
import type { DataTableColumn } from '../../../shared/components';
import type { StudyPlan } from '../model/StudyPlan';
import { useStudyPlanListViewModel } from '../viewmodel/useStudyPlanListViewModel';

// Listado de planes de estudio: vigencia, edición y baja.
export function StudyPlanListPage() {
  const ViewModel = useStudyPlanListViewModel();
  const Translation = useTranslation();
  const Navigator = useNavigate();

  const Columns: DataTableColumn<StudyPlan>[] = [
    { Field: 'Code', Header: Translation.t('StudyPlans.Columns.Code') },
    { Field: 'CareerName', Header: Translation.t('StudyPlans.Columns.Career') },
    { Field: 'Version', Header: Translation.t('StudyPlans.Columns.Version') },
    { Field: 'TotalCredits', Header: Translation.t('StudyPlans.Columns.TotalCredits'), Type: 'number' },
    { Field: 'AssignedCredits', Header: Translation.t('StudyPlans.Columns.AssignedCredits'), Type: 'number' },
    {
      Field: 'IsCurrent',
      Header: Translation.t('StudyPlans.Columns.IsCurrent'),
      Render: (Row) => (
        <ITBadget
          label={
            Row.IsCurrent
              ? Translation.t('StudyPlans.Current')
              : Translation.t('StudyPlans.NotCurrent')
          }
          color={Row.IsCurrent ? 'success' : 'gray'}
          size="sm"
        />
      ),
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
              OnClick={() => Navigator(`/study-plans/${Row.Id}`)}
            />
          )}
          {ViewModel.CanEdit && !Row.IsCurrent && (
            <AppButton
              Label={Translation.t('StudyPlans.Actions.SetCurrent')}
              Variant="text"
              Size="sm"
              IsLoading={ViewModel.BusyId === Row.Id}
              OnClick={() => {
                void ViewModel.SetCurrent(Row);
              }}
            />
          )}
          {ViewModel.CanDelete && !Row.IsCurrent && (
            <AppButton
              Label={Translation.t('Common.Delete')}
              Variant="text"
              Size="sm"
              Color="danger"
              IsLoading={ViewModel.BusyId === Row.Id}
              OnClick={() => {
                void ViewModel.Remove(Row);
              }}
            />
          )}
        </div>
      ),
    },
  ];

  return (
    <ITPage
      title={Translation.t('StudyPlans.Title')}
      description={Translation.t('StudyPlans.Subtitle')}
      maxWidth="7xl"
      loading={ViewModel.IsLoading && ViewModel.Items.length === 0}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Reload}
      actions={
        ViewModel.CanCreate ? (
          <AppButton
            Label={Translation.t('StudyPlans.New')}
            OnClick={() => Navigator('/study-plans/new')}
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
            Placeholder={Translation.t('StudyPlans.Search')}
            Name="PlanSearch"
          />
          <ITText as="span" muted className="USystemToolbarMeta">
            {Translation.t('Common.Total')}: {ViewModel.TotalCount}
          </ITText>
        </div>

        <FilterBar
          Fields={[
            { Field: 'CareerId', LabelKey: 'StudyPlans.Filters.Career', OptionsKey: 'Careers' },
            { Field: 'IsCurrent', LabelKey: 'StudyPlans.Filters.Current', OptionsKey: 'ActiveStatus' },
          ]}
          Values={ViewModel.Filters}
          OnChange={ViewModel.SetFilter}
          OnClear={ViewModel.ClearFilters}
        />

        <DataTable<StudyPlan>
          Columns={Columns}
          Rows={ViewModel.Items}
          TotalCount={ViewModel.TotalCount}
          Page={ViewModel.Page}
          PageSize={ViewModel.PageSize}
          OnPageChange={ViewModel.SetPage}
          IsLoading={ViewModel.IsLoading}
          EmptyMessage={Translation.t('StudyPlans.Empty')}
        />
      </ITStack>
    </ITPage>
  );
}


