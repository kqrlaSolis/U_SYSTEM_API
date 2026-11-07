import { ITCard, ITInput, ITPage, ITStack } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { AppButton, DataTable } from '../../../shared/components';
import type { DataTableColumn } from '../../../shared/components';
import type { Charge } from '../model/PaymentConcept';
import { useChargeListViewModel } from '../viewmodel/useChargeListViewModel';

// Cargos por alumno con alta y cancelación.
export function ChargeListPage() {
  const ViewModel = useChargeListViewModel();
  const Translation = useTranslation();

  const Columns: DataTableColumn<Charge>[] = [
    { Field: 'StudentId', Header: Translation.t('Payments.Columns.Student'), Type: 'number' },
    { Field: 'PaymentConceptId', Header: Translation.t('Payments.Columns.Concept'), Type: 'number' },
    { Field: 'Amount', Header: Translation.t('Payments.Columns.Amount'), Type: 'number' },
    { Field: 'Discount', Header: Translation.t('Payments.Columns.Discount'), Type: 'number' },
    { Field: 'Surcharge', Header: Translation.t('Payments.Columns.Surcharge'), Type: 'number' },
    { Field: 'Total', Header: Translation.t('Payments.Columns.Total'), Type: 'number' },
    { Field: 'DueDate', Header: Translation.t('Payments.Columns.DueDate'), Type: 'date' },
    { Field: 'Status', Header: Translation.t('Payments.Columns.Status') },
    {
      Field: 'Id',
      Header: Translation.t('Common.Actions'),
      Type: 'actions',
      Align: 'right',
      Render: (Row) =>
        Row.Status === 'Cancelled' || Row.Status === 'Paid' ? null : (
          <AppButton
            Label={Translation.t('Payments.CancelCharge')}
            Variant="text"
            Size="sm"
            Color="danger"
            IsLoading={ViewModel.IsLoading}
            OnClick={() => {
              void ViewModel.Cancel(Row.Id);
            }}
          />
        ),
    },
  ];

  return (
    <ITPage
      title={Translation.t('Payments.ChargesTitle')}
      description={Translation.t('Payments.ChargesSubtitle')}
      maxWidth="7xl"
      loading={ViewModel.IsLoading && ViewModel.Items.length === 0}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Load}
    >
      <ITStack spacing={4}>
        <ITCard title={Translation.t('Payments.NewCharge')}>
          <div className="USystemToolbar">
            <ITInput
              name="ChargeStudentId"
              type="number"
              placeholder={Translation.t('Payments.Columns.Student')}
              value={ViewModel.StudentId}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetStudentId(Event.target.value);
              }}
            />
            <ITInput
              name="ChargeTermId"
              type="number"
              placeholder={Translation.t('Payments.Columns.Term')}
              value={ViewModel.AcademicTermId}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetAcademicTermId(Event.target.value);
              }}
            />
            <ITInput
              name="ChargeConceptId"
              type="number"
              placeholder={Translation.t('Payments.Columns.Concept')}
              value={ViewModel.ConceptId}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetConceptId(Event.target.value);
              }}
            />
            <ITInput
              name="ChargeAmount"
              type="number"
              placeholder={Translation.t('Payments.Columns.Amount')}
              value={ViewModel.Amount}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetAmount(Event.target.value);
              }}
            />
            <ITInput
              name="ChargeDiscount"
              type="number"
              placeholder={Translation.t('Payments.Columns.Discount')}
              value={ViewModel.Discount}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetDiscount(Event.target.value);
              }}
            />
            <ITInput
              name="ChargeSurcharge"
              type="number"
              placeholder={Translation.t('Payments.Columns.Surcharge')}
              value={ViewModel.Surcharge}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetSurcharge(Event.target.value);
              }}
            />
            <ITInput
              name="ChargeDueDate"
              placeholder="YYYY-MM-DD"
              value={ViewModel.DueDate}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetDueDate(Event.target.value);
              }}
            />
            <AppButton
              Label={Translation.t('Payments.RegisterCharge')}
              IsLoading={ViewModel.IsLoading}
              OnClick={() => {
                void ViewModel.Create();
              }}
            />
          </div>
        </ITCard>

        <DataTable<Charge>
          Columns={Columns}
          Rows={ViewModel.Items}
          TotalCount={ViewModel.Items.length}
          Page={1}
          PageSize={100}
          OnPageChange={() => undefined}
          IsLoading={ViewModel.IsLoading}
          EmptyMessage={Translation.t('Payments.EmptyCharges')}
        />
      </ITStack>
    </ITPage>
  );
}
