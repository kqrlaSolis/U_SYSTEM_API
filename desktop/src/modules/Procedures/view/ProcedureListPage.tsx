import { ITCard, ITInput, ITPage, ITStack } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { AppButton, DataTable } from '../../../shared/components';
import type { DataTableColumn } from '../../../shared/components';
import type { Procedure } from '../model/ProcedureType';
import { useProcedureListViewModel } from '../viewmodel/useProcedureListViewModel';

// Solicitudes de trámite con folio y emisión.
export function ProcedureListPage() {
  const ViewModel = useProcedureListViewModel();
  const Translation = useTranslation();

  const Columns: DataTableColumn<Procedure>[] = [
    { Field: 'FolioNumber', Header: Translation.t('Procedures.Folio') },
    { Field: 'ProcedureTypeCode', Header: Translation.t('Procedures.Type') },
    { Field: 'StudentId', Header: Translation.t('Procedures.Student'), Type: 'number' },
    { Field: 'Status', Header: Translation.t('Procedures.Status') },
    { Field: 'RequestedAt', Header: Translation.t('Procedures.RequestedAt'), Type: 'date' },
    {
      Field: 'Id',
      Header: Translation.t('Common.Actions'),
      Type: 'actions',
      Align: 'right',
      Render: (Row) =>
        Row.Status === 'Issued' ? null : (
          <AppButton
            Label={Translation.t('Procedures.Issue')}
            Variant="text"
            Size="sm"
            IsLoading={ViewModel.IsLoading}
            OnClick={() => {
              void ViewModel.Issue(Row.Id);
            }}
          />
        ),
    },
  ];

  return (
    <ITPage
      title={Translation.t('Procedures.Title')}
      description={Translation.t('Procedures.Subtitle')}
      maxWidth="7xl"
      loading={ViewModel.IsLoading && ViewModel.Items.length === 0}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Load}
    >
      <ITStack spacing={4}>
        <ITCard title={Translation.t('Procedures.RequestTitle')}>
          <div className="USystemToolbar">
            <ITInput
              name="ProcedureTypeId"
              type="number"
              placeholder={Translation.t('Procedures.TypeId')}
              value={ViewModel.ProcedureTypeId}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetProcedureTypeId(Event.target.value);
              }}
            />
            <ITInput
              name="ProcedureStudentId"
              type="number"
              placeholder={Translation.t('Procedures.StudentId')}
              value={ViewModel.StudentId}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetStudentId(Event.target.value);
              }}
            />
            <AppButton
              Label={Translation.t('Procedures.Request')}
              IsLoading={ViewModel.IsLoading}
              OnClick={() => {
                void ViewModel.Create();
              }}
            />
          </div>
        </ITCard>

        <DataTable<Procedure>
          Columns={Columns}
          Rows={ViewModel.Items}
          TotalCount={ViewModel.Items.length}
          Page={1}
          PageSize={100}
          OnPageChange={() => undefined}
          IsLoading={ViewModel.IsLoading}
          EmptyMessage={Translation.t('Procedures.Empty')}
        />
      </ITStack>
    </ITPage>
  );
}
