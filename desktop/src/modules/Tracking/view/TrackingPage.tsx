import { ITAlert, ITCard, ITInput, ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { AppButton, DataTable } from '../../../shared/components';
import type { DataTableColumn } from '../../../shared/components';
import type { TranscriptEntry } from '../model/Tracking';
import { useTrackingViewModel } from '../viewmodel/useTrackingViewModel';

// Seguimiento académico: kardex y avance del alumno seleccionado.
export function TrackingPage() {
  const ViewModel = useTrackingViewModel();
  const Translation = useTranslation();

  const Columns: DataTableColumn<TranscriptEntry>[] = [
    { Field: 'AcademicTermId', Header: Translation.t('Tracking.Columns.Term'), Type: 'number' },
    { Field: 'ClassGroupCode', Header: Translation.t('Tracking.Columns.Group') },
    { Field: 'SubjectCode', Header: Translation.t('Tracking.Columns.Subject') },
    { Field: 'SubjectName', Header: Translation.t('Tracking.Columns.SubjectName') },
    { Field: 'FinalScore', Header: Translation.t('Tracking.Columns.FinalScore'), Type: 'number' },
    { Field: 'Status', Header: Translation.t('Tracking.Columns.Status') },
  ];

  return (
    <ITPage
      title={Translation.t('Tracking.Title')}
      description={Translation.t('Tracking.Subtitle')}
      maxWidth="7xl"
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
    >
      <ITStack spacing={4}>
        <div className="USystemToolbar">
          <ITInput
            name="TrackingStudentId"
            type="number"
            placeholder={Translation.t('Tracking.StudentId')}
            value={ViewModel.StudentId}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetStudentId(Event.target.value);
            }}
          />
          <AppButton
            Label={Translation.t('Tracking.Load')}
            IsLoading={ViewModel.IsLoading}
            OnClick={() => {
              void ViewModel.Load();
            }}
          />
        </div>

        {ViewModel.Progress !== null && (
          <ITCard title={Translation.t('Tracking.Progress')}>
            <dl className="USystemStatusList">
              <div>
                <dt>{Translation.t('Tracking.Student')}</dt>
                <dd>
                  {ViewModel.Progress.StudentNumber} · {ViewModel.Progress.FullName}
                </dd>
              </div>
              <div>
                <dt>{Translation.t('Tracking.Credits')}</dt>
                <dd>
                  {ViewModel.Progress.ApprovedCredits} / {ViewModel.Progress.PlanTotalCredits}
                </dd>
              </div>
              <div>
                <dt>{Translation.t('Tracking.Percent')}</dt>
                <dd>{ViewModel.Progress.ProgressPercent}%</dd>
              </div>
              <div>
                <dt>{Translation.t('Tracking.Passed')}</dt>
                <dd>
                  {ViewModel.Progress.PassedSubjects} · {Translation.t('Tracking.Failed')}:{' '}
                  {ViewModel.Progress.FailedSubjects}
                </dd>
              </div>
            </dl>
          </ITCard>
        )}

        {ViewModel.Transcript === null ? (
          <ITAlert variant="info" title={Translation.t('Tracking.Title')}>
            {Translation.t('Tracking.Hint')}
          </ITAlert>
        ) : (
          <ITCard title={Translation.t('Tracking.Transcript')}>
            <ITStack spacing={3}>
              <ITText as="p" muted>
                {ViewModel.Transcript.StudentNumber} · {ViewModel.Transcript.FullName}
              </ITText>
              <DataTable<TranscriptEntry>
                Columns={Columns}
                Rows={ViewModel.Transcript.Entries}
                TotalCount={ViewModel.Transcript.Entries.length}
                Page={1}
                PageSize={100}
                OnPageChange={() => undefined}
                IsLoading={ViewModel.IsLoading}
                EmptyMessage={Translation.t('Tracking.Empty')}
              />
            </ITStack>
          </ITCard>
        )}
      </ITStack>
    </ITPage>
  );
}
