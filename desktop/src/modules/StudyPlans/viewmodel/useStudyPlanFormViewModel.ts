import { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import I18next from '../../../shared/i18n/Config';
import type { Career } from '../../Careers/model/Career';
import { CareersService } from '../../Careers/model/CareersService';
import type { Subject } from '../../Subjects/model/Subject';
import { SubjectsService } from '../../Subjects/model/SubjectsService';
import type { StudyPlan, StudyPlanPrerequisiteItem, StudyPlanSubjectItem } from '../model/StudyPlan';
import { StudyPlansService } from '../model/StudyPlansService';

export interface StudyPlanFormState {
  CareerId: string;
  Code: string;
  Version: string;
  StartDate: string;
  EndDate: string;
  TotalCredits: string;
}

const EmptyForm: StudyPlanFormState = {
  CareerId: '',
  Code: '',
  Version: '1',
  StartDate: '',
  EndDate: '',
  TotalCredits: '0',
};

const PlanTypes = ['Mandatory', 'Elective'];

// ViewModel del plan: datos, malla por cuatrimestre y seriación.
export function useStudyPlanFormViewModel(PlanId?: number) {
  const Navigator = useNavigate();
  const IsEditing = PlanId !== undefined;

  const [Form, SetForm] = useState<StudyPlanFormState>(EmptyForm);
  const [Plan, SetPlan] = useState<StudyPlan | null>(null);
  const [Careers, SetCareers] = useState<Career[]>([]);
  const [Subjects, SetSubjects] = useState<Subject[]>([]);
  const [PlanSubjects, SetPlanSubjects] = useState<StudyPlanSubjectItem[]>([]);
  const [Prerequisites, SetPrerequisites] = useState<StudyPlanPrerequisiteItem[]>([]);
  const [NewSubjectId, SetNewSubjectId] = useState('');
  const [NewTermNumber, SetNewTermNumber] = useState('1');
  const [NewType, SetNewType] = useState(PlanTypes[0]);
  const [NewFromId, SetNewFromId] = useState('');
  const [NewToId, SetNewToId] = useState('');
  const [IsLoading, SetIsLoading] = useState(true);
  const [IsSubmitting, SetIsSubmitting] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [Errors, SetErrors] = useState<Record<string, string>>({});
  const [SavedMessage, SetSavedMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const [CareerPage, SubjectPage] = await Promise.all([
        CareersService.GetPaged(1, 100, ''),
        SubjectsService.GetPaged(1, 200, ''),
      ]);
      SetCareers(CareerPage.Items);
      SetSubjects(SubjectPage.Items);

      if (IsEditing && PlanId !== undefined) {
        const Loaded = await StudyPlansService.GetById(PlanId);
        SetPlan(Loaded);
        SetForm({
          CareerId: String(Loaded.CareerId),
          Code: Loaded.Code,
          Version: Loaded.Version,
          StartDate: Loaded.StartDate,
          EndDate: Loaded.EndDate ?? '',
          TotalCredits: String(Loaded.TotalCredits),
        });
        SetPlanSubjects(
          Loaded.Subjects.map((Item) => ({
            SubjectId: Item.SubjectId,
            TermNumber: Item.TermNumber,
            Type: Item.Type,
          })),
        );
        SetPrerequisites(
          Loaded.Prerequisites.map((Item) => ({
            SubjectId: Item.SubjectId,
            PrerequisiteSubjectId: Item.PrerequisiteSubjectId,
          })),
        );
      }
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [IsEditing, PlanId]);

  useEffect(() => {
    void Load();
  }, [Load]);

  const SetField = useCallback((Field: keyof StudyPlanFormState, Value: string) => {
    SetForm((Current) => ({ ...Current, [Field]: Value }));
  }, []);

  const SubjectIndex = useMemo(() => {
    const Index = new Map<number, Subject>();
    for (const Subject of Subjects) {
      Index.set(Subject.Id, Subject);
    }
    return Index;
  }, [Subjects]);

  const AddSubject = useCallback(() => {
    const SubjectId = Number(NewSubjectId);
    const TermNumber = Number(NewTermNumber);

    if (!Number.isFinite(SubjectId) || SubjectId <= 0) {
      return;
    }

    if (PlanSubjects.some((Item) => Item.SubjectId === SubjectId)) {
      SetErrorMessage(I18next.t('StudyPlans.Errors.DuplicatedSubject'));
      return;
    }

    SetPlanSubjects((Current) => [
      ...Current,
      { SubjectId, TermNumber: TermNumber < 1 ? 1 : TermNumber, Type: NewType },
    ]);
    SetNewSubjectId('');
  }, [NewSubjectId, NewTermNumber, NewType, PlanSubjects]);

  const RemoveSubject = useCallback((SubjectId: number) => {
    SetPlanSubjects((Current) => Current.filter((Item) => Item.SubjectId !== SubjectId));
    SetPrerequisites((Current) =>
      Current.filter(
        (Item) => Item.SubjectId !== SubjectId && Item.PrerequisiteSubjectId !== SubjectId,
      ),
    );
  }, []);

  const AddPrerequisite = useCallback(() => {
    const SubjectId = Number(NewFromId);
    const PrerequisiteSubjectId = Number(NewToId);

    if (SubjectId === PrerequisiteSubjectId || SubjectId <= 0 || PrerequisiteSubjectId <= 0) {
      SetErrorMessage(I18next.t('StudyPlans.Errors.InvalidPrerequisite'));
      return;
    }

    if (
      Prerequisites.some(
        (Item) =>
          Item.SubjectId === SubjectId && Item.PrerequisiteSubjectId === PrerequisiteSubjectId,
      )
    ) {
      return;
    }

    SetPrerequisites((Current) => [...Current, { SubjectId, PrerequisiteSubjectId }]);
    SetNewFromId('');
    SetNewToId('');
  }, [NewFromId, NewToId, Prerequisites]);

  const RemovePrerequisite = useCallback((SubjectId: number, PrerequisiteSubjectId: number) => {
    SetPrerequisites((Current) =>
      Current.filter(
        (Item) =>
          !(Item.SubjectId === SubjectId && Item.PrerequisiteSubjectId === PrerequisiteSubjectId),
      ),
    );
  }, []);

  const SaveMetadata = useCallback(async () => {
    const ValidationErrors: Record<string, string> = {};

    if (Form.CareerId === '') {
      ValidationErrors.CareerId = I18next.t('Validation.Required');
    }
    if (Form.Code.trim() === '') {
      ValidationErrors.Code = I18next.t('Validation.Required');
    }
    if (Form.Version.trim() === '') {
      ValidationErrors.Version = I18next.t('Validation.Required');
    }
    if (Form.StartDate.trim() === '') {
      ValidationErrors.StartDate = I18next.t('Validation.Required');
    }

    SetErrors(ValidationErrors);
    if (Object.keys(ValidationErrors).length > 0) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);
    SetSavedMessage(null);

    const Request = {
      CareerId: Number(Form.CareerId),
      Code: Form.Code.trim(),
      Version: Form.Version.trim(),
      StartDate: Form.StartDate,
      EndDate: Form.EndDate.trim() === '' ? null : Form.EndDate,
      TotalCredits: Number(Form.TotalCredits === '' ? 0 : Form.TotalCredits),
    };

    try {
      if (!IsEditing || PlanId === undefined) {
        const Created = await StudyPlansService.Create(Request);
        Navigator(`/study-plans/${Created.Id}`, { replace: true });
      } else {
        const Updated = await StudyPlansService.Update(PlanId, Request);
        SetPlan(Updated);
        SetSavedMessage(I18next.t('StudyPlans.Saved'));
      }
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [Form, IsEditing, Navigator, PlanId]);

  const SaveSubjects = useCallback(async () => {
    if (PlanId === undefined) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);
    SetSavedMessage(null);

    try {
      const Updated = await StudyPlansService.ReplaceSubjects(PlanId, PlanSubjects);
      SetPlan(Updated);
      SetSavedMessage(I18next.t('StudyPlans.Saved'));
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [PlanId, PlanSubjects]);

  const SavePrerequisites = useCallback(async () => {
    if (PlanId === undefined) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);
    SetSavedMessage(null);

    try {
      const Updated = await StudyPlansService.ReplacePrerequisites(PlanId, Prerequisites);
      SetPlan(Updated);
      SetSavedMessage(I18next.t('StudyPlans.Saved'));
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [PlanId, Prerequisites]);

  const SetCurrent = useCallback(async () => {
    if (PlanId === undefined) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);

    try {
      const Updated = await StudyPlansService.SetCurrent(PlanId);
      SetPlan(Updated);
      SetSavedMessage(I18next.t('StudyPlans.Saved'));
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [PlanId]);

  const AssignedCredits = useMemo(
    () =>
      PlanSubjects.reduce(
        (Total, Item) => Total + (SubjectIndex.get(Item.SubjectId)?.Credits ?? 0),
        0,
      ),
    [PlanSubjects, SubjectIndex],
  );

  return {
    IsEditing,
    Plan,
    Form,
    Errors,
    Careers,
    Subjects,
    PlanSubjects,
    Prerequisites,
    NewSubjectId,
    NewTermNumber,
    NewType,
    NewFromId,
    NewToId,
    PlanTypes,
    AssignedCredits,
    IsLoading,
    IsSubmitting,
    ErrorMessage,
    SavedMessage,
    SetField,
    SetNewSubjectId,
    SetNewTermNumber,
    SetNewType,
    SetNewFromId,
    SetNewToId,
    AddSubject,
    RemoveSubject,
    AddPrerequisite,
    RemovePrerequisite,
    SaveMetadata,
    SaveSubjects,
    SavePrerequisites,
    SetCurrent,
    GetSubjectLabel: (SubjectId: number) => {
      const Subject = SubjectIndex.get(SubjectId);
      return Subject === undefined ? `#${SubjectId}` : `${Subject.Code} · ${Subject.Name}`;
    },
    Reload: Load,
  };
}


