import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import I18next from '../../../shared/i18n/Config';
import type { AcademicTermRequest } from '../model/AcademicTerm';
import { AcademicTermsService } from '../model/AcademicTermsService';

export type AcademicTermFormValues = Record<string, string>;

const EmptyForm: AcademicTermFormValues = {
  Code: '',
  Name: '',
  AcademicYear: String(new Date().getFullYear()),
  TermNumber: '1',
  StartDate: '',
  EndDate: '',
  EnrollmentStartDate: '',
  EnrollmentEndDate: '',
  ReEnrollmentStartDate: '',
  ReEnrollmentEndDate: '',
  ExamStartDate: '',
  ExamEndDate: '',
  GradeReportCloseDate: '',
};

const DateFields = [
  'StartDate',
  'EndDate',
  'EnrollmentStartDate',
  'EnrollmentEndDate',
  'ReEnrollmentStartDate',
  'ReEnrollmentEndDate',
  'ExamStartDate',
  'ExamEndDate',
  'GradeReportCloseDate',
];

// ViewModel del alta/edición de un periodo: fechas, validación y guardado.
export function useAcademicTermFormViewModel(TermId?: number) {
  const Navigator = useNavigate();
  const IsEditing = TermId !== undefined;

  const [Values, SetValues] = useState<AcademicTermFormValues>(EmptyForm);
  const [Errors, SetErrors] = useState<Record<string, string>>({});
  const [Status, SetStatus] = useState<string>('Planned');
  const [IsLoading, SetIsLoading] = useState(IsEditing);
  const [IsSubmitting, SetIsSubmitting] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    if (!IsEditing || TermId === undefined) {
      SetIsLoading(false);
      return;
    }

    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const Term = await AcademicTermsService.GetById(TermId);
      SetStatus(Term.Status);
      SetValues({
        Code: Term.Code,
        Name: Term.Name,
        AcademicYear: String(Term.AcademicYear),
        TermNumber: String(Term.TermNumber),
        StartDate: Term.StartDate,
        EndDate: Term.EndDate,
        EnrollmentStartDate: Term.EnrollmentStartDate,
        EnrollmentEndDate: Term.EnrollmentEndDate,
        ReEnrollmentStartDate: Term.ReEnrollmentStartDate,
        ReEnrollmentEndDate: Term.ReEnrollmentEndDate,
        ExamStartDate: Term.ExamStartDate,
        ExamEndDate: Term.ExamEndDate,
        GradeReportCloseDate: Term.GradeReportCloseDate,
      });
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [IsEditing, TermId]);

  useEffect(() => {
    void Load();
  }, [Load]);

  const SetValue = useCallback((Name: string, Value: string) => {
    SetValues((Current) => ({ ...Current, [Name]: Value }));
  }, []);

  const Validate = useCallback((): Record<string, string> => {
    const ValidationErrors: Record<string, string> = {};

    if (Values.Code.trim().length === 0) {
      ValidationErrors.Code = I18next.t('Validation.Required');
    } else if (!/^\d{4}-[1-3]$/.test(Values.Code.trim())) {
      ValidationErrors.Code = I18next.t('AcademicTerms.Validation.CodeFormat');
    }

    if (Values.Name.trim().length === 0) {
      ValidationErrors.Name = I18next.t('Validation.Required');
    }

    for (const Field of DateFields) {
      if ((Values[Field] ?? '').trim().length === 0) {
        ValidationErrors[Field] = I18next.t('Validation.Required');
      }
    }

    if (Values.StartDate !== '' && Values.EndDate !== '' && Values.StartDate >= Values.EndDate) {
      ValidationErrors.EndDate = I18next.t('AcademicTerms.Validation.InvalidDates');
    }

    return ValidationErrors;
  }, [Values]);

  const Submit = useCallback(async () => {
    const ValidationErrors = Validate();
    SetErrors(ValidationErrors);

    if (Object.keys(ValidationErrors).length > 0) {
      return;
    }

    const Request: AcademicTermRequest = {
      Code: Values.Code.trim(),
      Name: Values.Name.trim(),
      AcademicYear: Number(Values.AcademicYear),
      TermNumber: Number(Values.TermNumber),
      StartDate: Values.StartDate,
      EndDate: Values.EndDate,
      EnrollmentStartDate: Values.EnrollmentStartDate,
      EnrollmentEndDate: Values.EnrollmentEndDate,
      ReEnrollmentStartDate: Values.ReEnrollmentStartDate,
      ReEnrollmentEndDate: Values.ReEnrollmentEndDate,
      ExamStartDate: Values.ExamStartDate,
      ExamEndDate: Values.ExamEndDate,
      GradeReportCloseDate: Values.GradeReportCloseDate,
    };

    SetIsSubmitting(true);
    SetErrorMessage(null);

    try {
      if (!IsEditing || TermId === undefined) {
        await AcademicTermsService.Create(Request);
      } else {
        await AcademicTermsService.Update(TermId, Request);
      }

      Navigator('/academic-terms', { replace: true });
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [IsEditing, Navigator, TermId, Validate, Values]);

  return {
    IsEditing,
    Status,
    Values,
    Errors,
    IsLoading,
    IsSubmitting,
    ErrorMessage,
    SetValue,
    Submit,
    Reload: Load,
    DateFields,
  };
}
