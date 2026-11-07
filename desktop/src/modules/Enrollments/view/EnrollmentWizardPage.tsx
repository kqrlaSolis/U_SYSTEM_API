import { ITAlert, ITCard, ITInput, ITPage, ITSelect, ITStack, ITText } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { AppButton, DataTable } from '../../../shared/components';
import type { DataTableColumn } from '../../../shared/components';
import type { ChecklistDocument } from '../model/EnrollmentChecklist';
import { useFilterOptions } from '../../../shared/hooks/useFilterOptions';
import { useEnrollmentWizardViewModel } from '../viewmodel/useEnrollmentWizardViewModel';

// Asistente de inscripción: alumno, grupos, documentos, pago y confirmación.
export function EnrollmentWizardPage() {
  const ViewModel = useEnrollmentWizardViewModel();
  const Translation = useTranslation();
  const Options = useFilterOptions();

  const Columns: DataTableColumn<ChecklistDocument>[] = [
    { Field: 'Name', Header: Translation.t('Wizard.Document') },
    {
      Field: 'IsMandatory',
      Header: Translation.t('Wizard.Mandatory'),
      Type: 'boolean',
    },
    { Field: 'Status', Header: Translation.t('Wizard.Status') },
    {
      Field: 'DocumentTypeId',
      Header: Translation.t('Common.Actions'),
      Type: 'actions',
      Align: 'right',
      Render: (Row) => (
        <div className="USystemRowActions">
          <AppButton
            Label={Translation.t('Wizard.Register')}
            Variant="text"
            Size="sm"
            IsLoading={ViewModel.IsLoading}
            OnClick={() => {
              void ViewModel.RegisterDocument(Row.DocumentTypeId);
            }}
          />
          {Row.StudentDocumentId !== null && Row.Status !== 'Validated' && (
            <AppButton
              Label={Translation.t('Wizard.Validate')}
              Variant="text"
              Size="sm"
              IsLoading={ViewModel.IsLoading}
              OnClick={() => {
                void ViewModel.ValidateDocument(Row.StudentDocumentId as number);
              }}
            />
          )}
        </div>
      ),
    },
  ];

  return (
    <ITPage
      title={Translation.t('Wizard.Title')}
      description={Translation.t('Wizard.Subtitle')}
      maxWidth="7xl"
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
    >
      <ITStack spacing={4}>
        <ITText as="p" muted>
          {Translation.t('Wizard.Step', { Number: ViewModel.Step })}
        </ITText>

        {ViewModel.SuccessMessage !== null && (
          <ITAlert variant="success" title={Translation.t('Common.Saved')}>
            {ViewModel.SuccessMessage}
          </ITAlert>
        )}

        <ITCard title={Translation.t('Wizard.StepStudent')}>
          <div className="USystemToolbar">
            <ITSelect
              name="WizardStudentId"
              label={Translation.t('Wizard.StudentId')}
              options={Options.Students}
              value={ViewModel.StudentId}
              onChange={(Event: React.ChangeEvent<HTMLSelectElement>) => {
                ViewModel.SetStudentId(Event.target.value);
              }}
            />
            <AppButton
              Label={Translation.t('Wizard.Continue')}
              Variant="outlined"
              OnClick={() => ViewModel.SetStep(2)}
            />
          </div>
        </ITCard>

        <ITCard title={Translation.t('Wizard.StepGroups')}>
          <div className="USystemToolbar">
            <ITSelect
              name="WizardTermId"
              label={Translation.t('Wizard.TermId')}
              options={Options.Terms}
              value={ViewModel.TermId}
              onChange={(Event: React.ChangeEvent<HTMLSelectElement>) => {
                ViewModel.SetTermId(Event.target.value);
              }}
            />
            <ITSelect
              name="WizardCareerId"
              label={Translation.t('Wizard.CareerId')}
              options={Options.Careers}
              value={ViewModel.CareerId}
              onChange={(Event: React.ChangeEvent<HTMLSelectElement>) => {
                ViewModel.SetCareerId(Event.target.value);
              }}
            />
            <ITInput
              name="WizardGroupIds"
              placeholder={Translation.t('Wizard.GroupIds')}
              value={ViewModel.GroupIds}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetGroupIds(Event.target.value);
              }}
            />
            <ITText as="span" muted className="USystemToolbarMeta">
              {Translation.t('Wizard.AvailableGroups')}: {Options.Groups.map((Group) => Group.label).join(' · ')}
            </ITText>
            <AppButton
              Label={Translation.t('Wizard.LoadChecklist')}
              IsLoading={ViewModel.IsLoading}
              OnClick={() => {
                void ViewModel.LoadChecklist();
              }}
            />
          </div>
        </ITCard>

        {ViewModel.Checklist !== null && (
          <ITCard title={Translation.t('Wizard.StepDocuments')}>
            <ITStack spacing={3}>
              <div className="USystemToolbar">
                <ITInput
                  name="WizardFilePath"
                  placeholder={Translation.t('Wizard.FilePath')}
                  value={ViewModel.FilePath}
                  onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                    ViewModel.SetFilePath(Event.target.value);
                  }}
                />
              </div>
              <DataTable<ChecklistDocument>
                Columns={Columns}
                Rows={ViewModel.Checklist.Documents}
                TotalCount={ViewModel.Checklist.Documents.length}
                Page={1}
                PageSize={100}
                OnPageChange={() => undefined}
                IsLoading={ViewModel.IsLoading}
                EmptyMessage={Translation.t('Wizard.NoDocuments')}
              />
            </ITStack>
          </ITCard>
        )}

        {ViewModel.Checklist !== null && (
          <ITCard title={Translation.t('Wizard.StepPayment')}>
            <ITStack spacing={3}>
              <ITText as="p">
                {Translation.t('Wizard.PendingCharges', {
                  Count: ViewModel.Checklist.PendingCharges,
                  Amount: ViewModel.Checklist.PendingAmount,
                })}
              </ITText>
              <ITText as="p" muted>
                {ViewModel.Checklist.CanConfirm
                  ? Translation.t('Wizard.CanConfirm')
                  : Translation.t('Wizard.Blocked')}
              </ITText>
              {ViewModel.Checklist.Warnings.length > 0 && (
                <ITAlert variant="info" title={Translation.t('Wizard.StepDocuments')}>
                  {ViewModel.Checklist.Warnings
                    .map((Code) => Translation.t(Code))
                    .join(' · ')}
                </ITAlert>
              )}
              {ViewModel.Checklist.BlockingReasons.length > 0 && (
                <ITAlert variant="warning" title={Translation.t('Wizard.StepConfirm')}>
                  {ViewModel.Checklist.BlockingReasons
                    .map((Code) => Translation.t(Code))
                    .join(' · ')}
                </ITAlert>
              )}
            </ITStack>
          </ITCard>
        )}

        <ITCard title={Translation.t('Wizard.StepConfirm')}>
          <div className="USystemToolbar">
            <ITInput
              name="WizardEnrollmentId"
              type="number"
              placeholder={Translation.t('Wizard.EnrollmentId')}
              value={ViewModel.EnrollmentId}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetEnrollmentId(Event.target.value);
              }}
            />
            <AppButton
              Label={Translation.t('Wizard.Confirm')}
              IsLoading={ViewModel.IsLoading}
              Disabled={ViewModel.Checklist !== null && !ViewModel.Checklist.CanConfirm}
              OnClick={() => {
                void ViewModel.Confirm();
              }}
            />
          </div>
        </ITCard>
      </ITStack>
    </ITPage>
  );
}
