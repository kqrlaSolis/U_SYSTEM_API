import {
  ITAlert,
  ITBadget,
  ITCard,
  ITInput,
  ITPage,
  ITSelect,
  ITStack,
  ITText,
} from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import { AppButton, FormField } from '../../../shared/components';
import { useStudyPlanFormViewModel } from '../viewmodel/useStudyPlanFormViewModel';

// Plan de estudios: datos, malla por cuatrimestre y seriación.
export function StudyPlanFormPage() {
  const Params = useParams<{ Id?: string }>();
  const ParsedId = Params.Id === undefined ? Number.NaN : Number(Params.Id);
  const ViewModel = useStudyPlanFormViewModel(Number.isFinite(ParsedId) ? ParsedId : undefined);
  const Translation = useTranslation();
  const Navigator = useNavigate();

  const HandleSubmit = (Event: React.FormEvent<HTMLFormElement>) => {
    Event.preventDefault();
    void ViewModel.SaveMetadata();
  };

  const CareerOptions = ViewModel.Careers.map((Career) => ({
    value: String(Career.Id),
    label: `${Career.Code} · ${Career.Name}`,
  }));

  const SubjectOptions = ViewModel.Subjects.map((Subject) => ({
    value: String(Subject.Id),
    label: `${Subject.Code} · ${Subject.Name}`,
  }));

  const TypeOptions = ViewModel.PlanTypes.map((Type) => ({
    value: Type,
    label: Translation.t(`StudyPlans.Types.${Type}`),
  }));

  return (
    <ITPage
      title={Translation.t(ViewModel.IsEditing ? 'StudyPlans.Edit' : 'StudyPlans.New')}
      description={Translation.t('StudyPlans.Subtitle')}
      maxWidth="7xl"
      loading={ViewModel.IsLoading}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Reload}
      actions={
        <div className="USystemRowActions">
          {ViewModel.IsEditing && ViewModel.Plan !== null && !ViewModel.Plan.IsCurrent && (
            <AppButton
              Label={Translation.t('StudyPlans.Actions.SetCurrent')}
              Variant="outlined"
              IsLoading={ViewModel.IsSubmitting}
              OnClick={() => {
                void ViewModel.SetCurrent();
              }}
            />
          )}
          <AppButton
            Label={Translation.t('Common.Cancel')}
            Variant="text"
            OnClick={() => Navigator('/study-plans')}
          />
        </div>
      }
    >
      <form className="USystemStackForm" onSubmit={HandleSubmit} noValidate>
        {ViewModel.SavedMessage !== null && (
          <ITAlert variant="success" title={Translation.t('Common.Saved')}>
            {ViewModel.SavedMessage}
          </ITAlert>
        )}

        <ITCard title={Translation.t('StudyPlans.Sections.Data')}>
          <ITStack spacing={4}>
            <ITSelect
              name="CareerId"
              label={Translation.t('StudyPlans.Columns.Career')}
              placeholder={Translation.t('StudyPlans.SelectCareer')}
              options={CareerOptions}
              value={ViewModel.Form.CareerId}
              onChange={(Event: React.ChangeEvent<HTMLSelectElement>) => {
                ViewModel.SetField('CareerId', Event.target.value);
              }}
              disabled={ViewModel.IsSubmitting}
            />

            <FormField
              Label={Translation.t('StudyPlans.Columns.Code')}
              HtmlFor="Code"
              IsRequired
              ErrorMessage={ViewModel.Errors.Code}
            >
              <ITInput
                name="Code"
                value={ViewModel.Form.Code}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetField('Code', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField
              Label={Translation.t('StudyPlans.Columns.Version')}
              HtmlFor="Version"
              IsRequired
              ErrorMessage={ViewModel.Errors.Version}
            >
              <ITInput
                name="Version"
                value={ViewModel.Form.Version}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetField('Version', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField
              Label={Translation.t('StudyPlans.Columns.StartDate')}
              HtmlFor="StartDate"
              IsRequired
              ErrorMessage={ViewModel.Errors.StartDate}
            >
              <ITInput
                name="StartDate"
                placeholder="YYYY-MM-DD"
                value={ViewModel.Form.StartDate}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetField('StartDate', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField Label={Translation.t('StudyPlans.Columns.EndDate')} HtmlFor="EndDate">
              <ITInput
                name="EndDate"
                placeholder="YYYY-MM-DD"
                value={ViewModel.Form.EndDate}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetField('EndDate', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField
              Label={Translation.t('StudyPlans.Columns.TotalCredits')}
              HtmlFor="TotalCredits"
            >
              <ITInput
                name="TotalCredits"
                type="number"
                value={ViewModel.Form.TotalCredits}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetField('TotalCredits', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <AppButton
              Type="submit"
              Label={Translation.t('StudyPlans.SaveData')}
              IsLoading={ViewModel.IsSubmitting}
            />
          </ITStack>
        </ITCard>

        {ViewModel.IsEditing && (
          <ITCard title={Translation.t('StudyPlans.Sections.Curriculum')}>
            <ITStack spacing={3}>
              <ITText as="p" muted>
                {Translation.t('StudyPlans.CreditsAssigned', { Count: ViewModel.AssignedCredits })}
              </ITText>

              {ViewModel.PlanSubjects.map((Item) => (
                <div key={Item.SubjectId} className="USystemSettingRow">
                  <div className="USystemSettingInfo">
                    <ITText as="span" className="USystemSettingKey">
                      <ITBadget
                        label={Translation.t('StudyPlans.Term', { Number: Item.TermNumber })}
                        color="info"
                        size="sm"
                      />{' '}
                      {ViewModel.GetSubjectLabel(Item.SubjectId)}
                    </ITText>
                    <ITText as="span" muted>
                      {Translation.t(`StudyPlans.Types.${Item.Type}`)}
                    </ITText>
                  </div>
                  <AppButton
                    Label={Translation.t('Common.Delete')}
                    Variant="text"
                    Size="sm"
                    Color="danger"
                    OnClick={() => ViewModel.RemoveSubject(Item.SubjectId)}
                  />
                </div>
              ))}

              <div className="USystemInlineActions">
                <ITSelect
                  name="NewSubjectId"
                  placeholder={Translation.t('StudyPlans.SelectSubject')}
                  options={SubjectOptions}
                  value={ViewModel.NewSubjectId}
                  onChange={(Event: React.ChangeEvent<HTMLSelectElement>) => {
                    ViewModel.SetNewSubjectId(Event.target.value);
                  }}
                />
                <ITInput
                  name="NewTermNumber"
                  type="number"
                  value={ViewModel.NewTermNumber}
                  onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                    ViewModel.SetNewTermNumber(Event.target.value);
                  }}
                />
                <ITSelect
                  name="NewType"
                  options={TypeOptions}
                  value={ViewModel.NewType}
                  onChange={(Event: React.ChangeEvent<HTMLSelectElement>) => {
                    ViewModel.SetNewType(Event.target.value);
                  }}
                />
                <AppButton
                  Label={Translation.t('StudyPlans.AddSubject')}
                  Variant="outlined"
                  Size="sm"
                  OnClick={ViewModel.AddSubject}
                />
              </div>

              <AppButton
                Label={Translation.t('StudyPlans.SaveCurriculum')}
                IsLoading={ViewModel.IsSubmitting}
                OnClick={() => {
                  void ViewModel.SaveSubjects();
                }}
              />
            </ITStack>
          </ITCard>
        )}

        {ViewModel.IsEditing && (
          <ITCard title={Translation.t('StudyPlans.Sections.Prerequisites')}>
            <ITStack spacing={3}>
              {ViewModel.Prerequisites.map((Item) => (
                <div
                  key={`${Item.SubjectId}-${Item.PrerequisiteSubjectId}`}
                  className="USystemSettingRow"
                >
                  <ITText as="span" className="USystemSettingKey">
                    {ViewModel.GetSubjectLabel(Item.SubjectId)} ←{' '}
                    {ViewModel.GetSubjectLabel(Item.PrerequisiteSubjectId)}
                  </ITText>
                  <AppButton
                    Label={Translation.t('Common.Delete')}
                    Variant="text"
                    Size="sm"
                    Color="danger"
                    OnClick={() =>
                      ViewModel.RemovePrerequisite(Item.SubjectId, Item.PrerequisiteSubjectId)
                    }
                  />
                </div>
              ))}

              <div className="USystemInlineActions">
                <ITSelect
                  name="NewFromId"
                  placeholder={Translation.t('StudyPlans.SelectSubject')}
                  options={SubjectOptions}
                  value={ViewModel.NewFromId}
                  onChange={(Event: React.ChangeEvent<HTMLSelectElement>) => {
                    ViewModel.SetNewFromId(Event.target.value);
                  }}
                />
                <ITText as="span" muted>
                  ←
                </ITText>
                <ITSelect
                  name="NewToId"
                  placeholder={Translation.t('StudyPlans.SelectPrerequisite')}
                  options={SubjectOptions}
                  value={ViewModel.NewToId}
                  onChange={(Event: React.ChangeEvent<HTMLSelectElement>) => {
                    ViewModel.SetNewToId(Event.target.value);
                  }}
                />
                <AppButton
                  Label={Translation.t('StudyPlans.AddPrerequisite')}
                  Variant="outlined"
                  Size="sm"
                  OnClick={ViewModel.AddPrerequisite}
                />
              </div>

              <AppButton
                Label={Translation.t('StudyPlans.SavePrerequisites')}
                IsLoading={ViewModel.IsSubmitting}
                OnClick={() => {
                  void ViewModel.SavePrerequisites();
                }}
              />
            </ITStack>
          </ITCard>
        )}
      </form>
    </ITPage>
  );
}
