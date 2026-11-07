import { useCallback, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import type { AcademicProgress, Transcript } from '../model/Tracking';
import { TrackingService } from '../model/TrackingService';

// ViewModel de seguimiento: carga kardex y avance de un alumno.
export function useTrackingViewModel() {
  const [StudentId, SetStudentId] = useState('1');
  const [Transcript, SetTranscript] = useState<Transcript | null>(null);
  const [Progress, SetProgress] = useState<AcademicProgress | null>(null);
  const [IsLoading, SetIsLoading] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    const ParsedId = Number(StudentId);

    if (!Number.isFinite(ParsedId) || ParsedId <= 0) {
      SetErrorMessage('Validation.Required');
      return;
    }

    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const [TranscriptResult, ProgressResult] = await Promise.all([
        TrackingService.GetTranscript(ParsedId),
        TrackingService.GetProgress(ParsedId),
      ]);
      SetTranscript(TranscriptResult);
      SetProgress(ProgressResult);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [StudentId]);

  return {
    StudentId,
    Transcript,
    Progress,
    IsLoading,
    ErrorMessage,
    SetStudentId,
    Load,
  };
}
