import { useCallback, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import { HttpClient } from '../../../shared/http/HttpClient';
import { TrackingService } from '../../Tracking/model/TrackingService';

export interface EligibleSubject {
  SubjectId: number;
  Code: string;
  Name: string;
  Credits: number;
  TermNumber: number;
  BlockedReason: string | null;
  IsEligible: boolean;
}

export interface ReEnrollmentProposal {
  StudentId: number;
  StudentNumber: string;
  FullName: string;
  StudyPlanId: number;
  NextTermNumber: number;
  Subjects: EligibleSubject[];
}

const ProposalPath = (StudentId: number) => `/api/v1/students/${StudentId}/re-enrollment-proposal`;

// ViewModel de reinscripción: propone materias elegibles y crea la reinscripción.
export function useReEnrollmentViewModel() {
  const [StudentId, SetStudentId] = useState('1');
  const [Proposal, SetProposal] = useState<ReEnrollmentProposal | null>(null);
  const [TermId, SetTermId] = useState('');
  const [GroupIds, SetGroupIds] = useState('');
  const [IsLoading, SetIsLoading] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [SavedMessage, SetSavedMessage] = useState<string | null>(null);

  const Propose = useCallback(async () => {
    const ParsedId = Number(StudentId);
    if (!Number.isFinite(ParsedId) || ParsedId <= 0) {
      return;
    }

    SetIsLoading(true);
    SetErrorMessage(null);
    SetSavedMessage(null);

    try {
      const Result = await HttpClient.Get<ReEnrollmentProposal>(ProposalPath(ParsedId));
      SetProposal(Result);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [StudentId]);

  const Create = useCallback(async () => {
    const ParsedId = Number(StudentId);
    const ParsedTerm = Number(TermId);
    const GroupList = GroupIds
      .split(',')
      .map((Item) => Number(Item.trim()))
      .filter((Item) => Number.isFinite(Item) && Item > 0);

    if (Proposal === null || !Number.isFinite(ParsedTerm) || ParsedTerm <= 0 || GroupList.length === 0) {
      SetErrorMessage('Validation.Required');
      return;
    }

    SetIsLoading(true);
    SetErrorMessage(null);
    SetSavedMessage(null);

    try {
      await HttpClient.Post<unknown>(ProposalPath(ParsedId), {
        StudentId: ParsedId,
        AcademicTermId: ParsedTerm,
        CareerId: 1,
        Notes: 'Reinscripción',
        ClassGroupIds: GroupList,
      });
      SetSavedMessage('ReEnrollments.Created');
      await TrackingService.GetProgress(ParsedId);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [GroupIds, Proposal, StudentId, TermId]);

  return {
    StudentId,
    Proposal,
    TermId,
    GroupIds,
    IsLoading,
    ErrorMessage,
    SavedMessage,
    SetStudentId,
    SetTermId,
    SetGroupIds,
    Propose,
    Create,
  };
}
