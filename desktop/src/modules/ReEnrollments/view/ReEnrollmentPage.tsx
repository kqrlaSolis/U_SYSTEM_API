import { ITAlert, ITBadget, ITCard, ITInput, ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { AppButton, DataTable } from '../../../shared/components';
import type { DataTableColumn } from '../../../shared/components';
import type { EligibleSubject } from '../viewmodel/useReEnrollmentViewModel';
import { useReEnrollmentViewModel } from '../viewmodel/useReEnrollmentViewModel';

// Reinscripción: propuesta de materias elegibles y alta de la inscripción.
export function ReEnrollmentPage() {
  const ViewModel = useReEnrollmentViewModel();
  const Translation = useTranslation();

  const Columns: DataTableColumn<EligibleSubject>[] = [
    { Field: 'Code', Header: Translation.t('ReEnrollments.Columns.Code') },
    { Field: 'Name', Header: Translation.t('ReEnrollments.Columns.Name') },
    { Field: 'Credits', Header: Translation.t('ReEnrollments.Columns.Credits'), Type: 'number' },
    { Field: 'TermNumber', Header: Translation.t('ReEnrollments.Columns.Term'), Type: 'number' },
    {
      Field: 'IsEligible',
      Header: Translation.t('ReEnrollments.Columns.Eligible'),
      Render: (Row) => (
        <ITBadget
          label={
            Row.IsEligible
              ? Translation.t('ReEnrollments.Eligible')
              : Translation.t('ReEnrollments.Blocked')
          }
          color={Row.IsEligible ? 'success' : 'gray'}
          size="sm"
        />
      ),
    },
  ];

  return (
    <ITPage
      title={Translation.t('ReEnrollments.Title')}
      description={Translation.t('ReEnrollments.Subtitle')}
      maxWidth="7xl"
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
    >
      <ITStack spacing={4}>
        {ViewModel.SavedMessage !== null && (
          <ITAlert variant="success" title={Translation.t('Common.Saved')}>
            {Translation.t('ReEnrollments.Created')}
          </ITAlert>
        )}

        <div className="USystemToolbar">
          <ITInput
            name="ReEnrollmentStudentId"
            type="number"
            placeholder={Translation.t('ReEnrollments.StudentId')}
            value={ViewModel.StudentId}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetStudentId(Event.target.value);
            }}
          />
          <AppButton
            Label={Translation.t('ReEnrollments.Propose')}
            IsLoading={ViewModel.IsLoading}
            OnClick={() => {
              void ViewModel.Propose();
            }}
          />
        </div>

        {ViewModel.Proposal !== null && (
          <ITCard
            title={Translation.t('ReEnrollments.ProposalTitle', {
              Term: ViewModel.Proposal.NextTermNumber,
            })}
          >
            <ITStack spacing={3}>
              <ITText as="p" muted>
                {ViewModel.Proposal.StudentNumber} · {ViewModel.Proposal.FullName}
              </ITText>
              <DataTable<EligibleSubject>
                Columns={Columns}
                Rows={ViewModel.Proposal.Subjects}
                TotalCount={ViewModel.Proposal.Subjects.length}
                Page={1}
                PageSize={100}
                OnPageChange={() => undefined}
                IsLoading={ViewModel.IsLoading}
                EmptyMessage={Translation.t('ReEnrollments.Empty')}
              />
            </ITStack>
          </ITCard>
        )}

        <ITCard title={Translation.t('ReEnrollments.CreateTitle')}>
          <ITStack spacing={3}>
            <ITInput
              name="ReEnrollmentTermId"
              type="number"
              placeholder={Translation.t('ReEnrollments.TermId')}
              value={ViewModel.TermId}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetTermId(Event.target.value);
              }}
            />
            <ITInput
              name="ReEnrollmentGroupIds"
              placeholder={Translation.t('ReEnrollments.GroupIds')}
              value={ViewModel.GroupIds}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetGroupIds(Event.target.value);
              }}
            />
            <AppButton
              Label={Translation.t('ReEnrollments.Create')}
              IsLoading={ViewModel.IsLoading}
              OnClick={() => {
                void ViewModel.Create();
              }}
            />
          </ITStack>
        </ITCard>
      </ITStack>
    </ITPage>
  );
}
