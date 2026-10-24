import { ITCard, ITInput, ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { AppButton, DataTable } from '../../../shared/components';
import type { DataTableColumn } from '../../../shared/components';
import type { AuditLog } from '../model/AuditLog';
import { useAuditLogListViewModel } from '../viewmodel/useAuditLogListViewModel';

// Consulta de la bitácora con filtros por módulo, acción y rango de fechas.
export function AuditLogListPage() {
  const ViewModel = useAuditLogListViewModel();
  const Translation = useTranslation();

  const Columns: DataTableColumn<AuditLog>[] = [
    { Field: 'CreatedAt', Header: Translation.t('AuditLogs.Columns.CreatedAt'), Type: 'date' },
    { Field: 'Username', Header: Translation.t('AuditLogs.Columns.Username') },
    { Field: 'Action', Header: Translation.t('AuditLogs.Columns.Action') },
    { Field: 'Module', Header: Translation.t('AuditLogs.Columns.Module') },
    { Field: 'Entity', Header: Translation.t('AuditLogs.Columns.Entity') },
    { Field: 'EntityId', Header: Translation.t('AuditLogs.Columns.EntityId') },
    { Field: 'IpAddress', Header: Translation.t('AuditLogs.Columns.IpAddress') },
  ];

  return (
    <ITPage
      title={Translation.t('AuditLogs.Title')}
      description={Translation.t('AuditLogs.Subtitle')}
      maxWidth="7xl"
      loading={ViewModel.IsLoading && ViewModel.Items.length === 0}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Reload}
    >
      <ITStack spacing={4}>
        <div className="USystemToolbar">
          <ITInput
            name="FilterModule"
            placeholder={Translation.t('AuditLogs.Filters.Module')}
            value={ViewModel.Filters.Module}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetFilter('Module', Event.target.value);
            }}
          />
          <ITInput
            name="FilterAction"
            placeholder={Translation.t('AuditLogs.Filters.Action')}
            value={ViewModel.Filters.Action}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetFilter('Action', Event.target.value);
            }}
          />
          <ITInput
            name="FilterFrom"
            placeholder={Translation.t('AuditLogs.Filters.From')}
            value={ViewModel.Filters.From}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetFilter('From', Event.target.value);
            }}
          />
          <ITInput
            name="FilterTo"
            placeholder={Translation.t('AuditLogs.Filters.To')}
            value={ViewModel.Filters.To}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetFilter('To', Event.target.value);
            }}
          />
          <AppButton Label={Translation.t('Common.Search')} OnClick={ViewModel.ApplyFilters} />
          <AppButton
            Label={Translation.t('Common.Clear')}
            Variant="text"
            OnClick={ViewModel.ClearFilters}
          />
          <ITText as="span" muted className="USystemToolbarMeta">
            {Translation.t('Common.Total')}: {ViewModel.TotalCount}
          </ITText>
        </div>

        <DataTable<AuditLog>
          Columns={Columns}
          Rows={ViewModel.Items}
          TotalCount={ViewModel.TotalCount}
          Page={ViewModel.Page}
          PageSize={ViewModel.PageSize}
          OnPageChange={ViewModel.SetPage}
          IsLoading={ViewModel.IsLoading}
          EmptyMessage={Translation.t('AuditLogs.Empty')}
          OnRowClick={(Row) => {
            void ViewModel.SelectRow(Row);
          }}
        />

        {ViewModel.Selected !== null && (
          <ITCard title={`${ViewModel.Selected.Action} · ${ViewModel.Selected.Module}`}>
            <ITStack spacing={3}>
              <ITText as="p" muted>
                {Translation.t('AuditLogs.Detail.Description')}
              </ITText>
              <ITText as="span" className="USystemSettingKey">
                {Translation.t('AuditLogs.Detail.OldValue')}
              </ITText>
              <pre className="USystemCodeBlock">{ViewModel.Selected.OldValue ?? '—'}</pre>
              <ITText as="span" className="USystemSettingKey">
                {Translation.t('AuditLogs.Detail.NewValue')}
              </ITText>
              <pre className="USystemCodeBlock">{ViewModel.Selected.NewValue ?? '—'}</pre>
              <AppButton
                Label={Translation.t('Common.Close')}
                Variant="text"
                OnClick={ViewModel.CloseDetail}
              />
            </ITStack>
          </ITCard>
        )}
      </ITStack>
    </ITPage>
  );
}
