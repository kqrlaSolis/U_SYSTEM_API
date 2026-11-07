import { HttpClient } from '../../../shared/http/HttpClient';
import type { AcademicProgress, Transcript } from './Tracking';

// Servicio de seguimiento: kardex y avance del alumno (solo lectura)
export const TrackingService = {
  async GetTranscript(StudentId: number): Promise<Transcript> {
    return HttpClient.Get<Transcript>(`/api/v1/students/${StudentId}/transcript`);
  },

  async GetProgress(StudentId: number): Promise<AcademicProgress> {
    return HttpClient.Get<AcademicProgress>(`/api/v1/students/${StudentId}/academic-progress`);
  },
};
