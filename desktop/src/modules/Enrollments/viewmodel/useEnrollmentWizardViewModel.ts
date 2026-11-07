import { useCallback, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import I18next from '../../../shared/i18n/Config';
import type { EnrollmentChecklist } from '../model/EnrollmentChecklist';
import { EnrollmentsService } from '../model/EnrollmentsService';

export type WizardStep = 1 | 2 | 3 | 4 | 5;

// ViewModel del asistente de inscripción: alumno, grupos, documentos, pago y confirmación.
export function useEnrollmentWizardViewModel() {
  const [Step, SetStep] = useState<WizardStep>(1);
  const [StudentId, SetStudentId] = useState('1');
  const [TermId, SetTermId] = useState('2');
  const [CareerId, SetCareerId] = useState('1');
  const [GroupIds, SetGroupIds] = useState('1');
  const [EnrollmentId, SetEnrollmentId] = useState('');
  const [FilePath, SetFilePath] = useState('');
  const [Checklist, SetChecklist] = useState<EnrollmentChecklist | null>(null);
  const [IsLoading, SetIsLoading] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [SuccessMessage, SetSuccessMessage] = useState<string | null>(null);

  const LoadChecklist = useCallback(async () => {
    const ParsedStudent = Number(StudentId);
    const ParsedTerm = Number(TermId);

    if (!Number.isFinite(ParsedStudent) || ParsedStudent <= 0 || !Number.isFinite(ParsedTerm) || ParsedTerm <= 0) {
      SetErrorMessage(I18next.t('Wizard.InvalidIds'));
      return;
    }

    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      SetChecklist(await EnrollmentsService.GetChecklist(ParsedStudent, ParsedTerm));
      SetStep(3);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [StudentId, TermId]);

  const RegisterDocument = useCallback(
    async (DocumentTypeId: number) => {
      const ParsedStudent = Number(StudentId);

      SetIsLoading(true);
      SetErrorMessage(null);
      SetSuccessMessage(null);

      try {
        await EnrollmentsService.RegisterDocument(ParsedStudent, {
          DocumentTypeId,
          FilePath: FilePath.trim() === '' ? null : FilePath.trim(),
          Notes: null,
        });
        SetSuccessMessage(I18next.t('Wizard.DocumentRegistered'));
        SetChecklist(await EnrollmentsService.GetChecklist(ParsedStudent, Number(TermId)));
      } catch (CaughtError) {
        SetErrorMessage(GetErrorMessage(CaughtError));
      } finally {
        SetIsLoading(false);
      }
    },
    [FilePath, StudentId, TermId],
  );

  const ValidateDocument = useCallback(
    async (DocumentId: number) => {
      const ParsedStudent = Number(StudentId);

      SetIsLoading(true);
      SetErrorMessage(null);
      SetSuccessMessage(null);

      try {
        await EnrollmentsService.ValidateDocument(DocumentId);
        SetSuccessMessage(I18next.t('Wizard.DocumentValidated'));
        SetChecklist(await EnrollmentsService.GetChecklist(ParsedStudent, Number(TermId)));
      } catch (CaughtError) {
        SetErrorMessage(GetErrorMessage(CaughtError));
      } finally {
        SetIsLoading(false);
      }
    },
    [StudentId, TermId],
  );

  const Confirm = useCallback(async () => {
    const ParsedStudent = Number(StudentId);
    const ParsedTerm = Number(TermId);
    const ParsedCareer = Number(CareerId);
    const ParsedEnrollment = Number(EnrollmentId);
    const Groups = GroupIds
      .split(',')
      .map((Item) => Number(Item.trim()))
      .filter((Item) => Number.isFinite(Item) && Item > 0);

    SetIsLoading(true);
    SetErrorMessage(null);
    SetSuccessMessage(null);

    try {
      let TargetEnrollment = ParsedEnrollment;

      if (!Number.isFinite(TargetEnrollment) || TargetEnrollment <= 0) {
        if (Groups.length === 0) {
          throw new Error(I18next.t('Wizard.InvalidIds'));
        }

        const Created = await EnrollmentsService.CreateReEnrollment(
          ParsedStudent,
          ParsedTerm,
          ParsedCareer,
          Groups,
        );
        TargetEnrollment = Created.Id;
      }

      await EnrollmentsService.Confirm(TargetEnrollment);
      SetSuccessMessage(I18next.t('Wizard.Confirmed'));
      SetStep(5);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [CareerId, EnrollmentId, GroupIds, StudentId, TermId]);

  return {
    Step,
    StudentId,
    TermId,
    CareerId,
    GroupIds,
    EnrollmentId,
    FilePath,
    Checklist,
    IsLoading,
    ErrorMessage,
    SuccessMessage,
    SetStudentId,
    SetTermId,
    SetCareerId,
    SetGroupIds,
    SetEnrollmentId,
    SetFilePath,
    SetStep,
    LoadChecklist,
    RegisterDocument,
    ValidateDocument,
    Confirm,
  };
}
