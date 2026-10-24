import { ITBadget, ITCard, ITInput, ITPage, ITStack } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import { AppButton, FormField } from '../../../shared/components';
import { useAcademicTermFormViewModel } from '../viewmodel/useAcademicTermFormViewModel';

// Alta y edición del calendario de un cuatrimestre (fechas en formato YYYY-MM-DD).
export function AcademicTermFormPage() {
  const Params = useParams<{ Id?: string }>();
  const ParsedId = Params.Id === undefined ? Number.NaN : Number(Params.Id);
  const ViewModel = useAcademicTermFormViewModel(Number.isFinite(ParsedId) ? ParsedId : undefined);
  const Translation = useTranslation();
  const Navigator = useNavigate();

  const HandleSubmit = (Event: React.FormEvent<HTMLFormElement>) => {
    Event.preventDefault();
    void ViewModel.Submit();
  };

  const RenderDateField = (Field: string, LabelKey: string) => (
    <FormField
      key={Field}
      Label={Translation.t(LabelKey)}
      HtmlFor={Field}
      IsRequired
      ErrorMessage={ViewModel.Errors[Field]}
    >
      <ITInput
        name={Field}
        placeholder="YYYY-MM-DD"
        value={ViewModel.Values[Field] ?? ''}
        onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
          ViewModel.SetValue(Field, Event.target.value);
        }}
        disabled={ViewModel.IsSubmitting}
      />
    </FormField>
  );

  return (
    <ITPage
      title={Translation.t(ViewModel.IsEditing ? 'AcademicTerms.Edit' : 'AcademicTerms.New')}
      description={Translation.t('AcademicTerms.Subtitle')}
      maxWidth="5xl"
      loading={ViewModel.IsLoading}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Reload}
      actions={
        <AppButton
          Label={Translation.t('Common.Cancel')}
          Variant="text"
          OnClick={() => Navigator('/academic-terms')}
        />
      }
    >
      <form className="USystemStackForm" onSubmit={HandleSubmit} noValidate>
        {ViewModel.IsEditing && (
          <ITBadget
            label={Translation.t(`AcademicTerms.Status.${ViewModel.Status}`)}
            color="info"
            size="sm"
          />
        )}

        <ITCard title={Translation.t('AcademicTerms.Sections.Term')}>
          <ITStack spacing={4}>
            <FormField
              Label={Translation.t('AcademicTerms.Columns.Code')}
              HtmlFor="Code"
              IsRequired
              ErrorMessage={ViewModel.Errors.Code}
            >
              <ITInput
                name="Code"
                placeholder="YYYY-N"
                value={ViewModel.Values.Code}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetValue('Code', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField
              Label={Translation.t('AcademicTerms.Columns.Name')}
              HtmlFor="Name"
              IsRequired
              ErrorMessage={ViewModel.Errors.Name}
            >
              <ITInput
                name="Name"
                value={ViewModel.Values.Name}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetValue('Name', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField Label={Translation.t('AcademicTerms.Fields.AcademicYear')} HtmlFor="AcademicYear">
              <ITInput
                name="AcademicYear"
                type="number"
                value={ViewModel.Values.AcademicYear}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetValue('AcademicYear', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField Label={Translation.t('AcademicTerms.Fields.TermNumber')} HtmlFor="TermNumber">
              <ITInput
                name="TermNumber"
                type="number"
                value={ViewModel.Values.TermNumber}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetValue('TermNumber', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>
          </ITStack>
        </ITCard>

        <ITCard title={Translation.t('AcademicTerms.Sections.Calendar')}>
          <ITStack spacing={4}>
            {RenderDateField('StartDate', 'AcademicTerms.Fields.StartDate')}
            {RenderDateField('EndDate', 'AcademicTerms.Fields.EndDate')}
            {RenderDateField('EnrollmentStartDate', 'AcademicTerms.Fields.EnrollmentStartDate')}
            {RenderDateField('EnrollmentEndDate', 'AcademicTerms.Fields.EnrollmentEndDate')}
            {RenderDateField('ReEnrollmentStartDate', 'AcademicTerms.Fields.ReEnrollmentStartDate')}
            {RenderDateField('ReEnrollmentEndDate', 'AcademicTerms.Fields.ReEnrollmentEndDate')}
            {RenderDateField('ExamStartDate', 'AcademicTerms.Fields.ExamStartDate')}
            {RenderDateField('ExamEndDate', 'AcademicTerms.Fields.ExamEndDate')}
            {RenderDateField('GradeReportCloseDate', 'AcademicTerms.Fields.GradeReportCloseDate')}
          </ITStack>
        </ITCard>

        <AppButton
          Type="submit"
          Label={Translation.t('Common.Save')}
          IsLoading={ViewModel.IsSubmitting}
        />
      </form>
    </ITPage>
  );
}
