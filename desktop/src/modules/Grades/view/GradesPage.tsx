import { ITAlert, ITCard, ITInput, ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { AppButton, DataTable } from '../../../shared/components';
import type { DataTableColumn } from '../../../shared/components';
import type { Grade } from '../model/Grade';
import { useGradesViewModel } from '../viewmodel/useGradesViewModel';

// Captura masiva de calificaciones por grupo.
export function GradesPage() {
  const ViewModel = useGradesViewModel();
  const Translation = useTranslation();

  const Columns: DataTableColumn<Grade>[] = [
    { Field: 'EnrollmentId', Header: Translation.t('Grades.Columns.Enrollment'), Type: 'number' },
    { Field: 'Partial1', Header: Translation.t('Grades.Columns.Partial1'), Type: 'number' },
    { Field: 'Partial2', Header: Translation.t('Grades.Columns.Partial2'), Type: 'number' },
    { Field: 'Partial3', Header: Translation.t('Grades.Columns.Partial3'), Type: 'number' },
    { Field: 'Ordinary', Header: Translation.t('Grades.Columns.Ordinary'), Type: 'number' },
    { Field: 'FinalScore', Header: Translation.t('Grades.Columns.Final'), Type: 'number' },
    { Field: 'Status', Header: Translation.t('Grades.Columns.Status') },
  ];

  return (
    <ITPage
      title={Translation.t('Grades.Title')}
      description={Translation.t('Grades.Subtitle')}
      maxWidth="7xl"
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
    >
      <ITStack spacing={4}>
        {ViewModel.SavedMessage !== null && (
          <ITAlert variant="success" title={Translation.t('Common.Saved')}>
            {Translation.t('Grades.Saved')}
          </ITAlert>
        )}

        <div className="USystemToolbar">
          <ITInput
            name="ClassGroupId"
            type="number"
            placeholder={Translation.t('Grades.ClassGroup')}
            value={ViewModel.ClassGroupId}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetClassGroupId(Event.target.value);
            }}
          />
          <AppButton
            Label={Translation.t('Grades.Load')}
            IsLoading={ViewModel.IsLoading}
            OnClick={() => {
              void ViewModel.Load();
            }}
          />
        </div>

        {ViewModel.Items.length > 0 && (
          <ITCard title={Translation.t('Grades.Current')}>
            <DataTable<Grade>
              Columns={Columns}
              Rows={ViewModel.Items}
              TotalCount={ViewModel.Items.length}
              Page={1}
              PageSize={100}
              OnPageChange={() => undefined}
              IsLoading={ViewModel.IsLoading}
              EmptyMessage={Translation.t('Grades.Empty')}
            />
          </ITCard>
        )}

        <ITCard title={Translation.t('Grades.Capture')}>
          <ITStack spacing={3}>
            <ITText as="p" muted>
              {Translation.t('Grades.CaptureHint')}
            </ITText>
            <ITInput
              name="GradesCsv"
              type="textarea"
              value={ViewModel.Csv}
              onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                ViewModel.SetCsv(Event.target.value);
              }}
            />
            <AppButton
              Label={Translation.t('Common.Save')}
              IsLoading={ViewModel.IsLoading}
              OnClick={() => {
                void ViewModel.Save();
              }}
            />
          </ITStack>
        </ITCard>
      </ITStack>
    </ITPage>
  );
}
