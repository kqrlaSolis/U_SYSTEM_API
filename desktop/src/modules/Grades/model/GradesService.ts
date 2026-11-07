import { HttpClient } from '../../../shared/http/HttpClient';
import type { Grade, GradeItemRequest, SaveGradesRequest } from './Grade';

const GradesPath = '/api/v1/class-groups';

// Servicio de calificaciones: captura masiva por grupo
export const GradesService = {
  async GetByClassGroup(ClassGroupId: number): Promise<Grade[]> {
    return HttpClient.Get<Grade[]>(`${GradesPath}/${ClassGroupId}/grades`);
  },

  async Save(ClassGroupId: number, Grades: GradeItemRequest[]): Promise<Grade[]> {
    const Request: SaveGradesRequest = { Grades };
    return HttpClient.Put<Grade[]>(`${GradesPath}/${ClassGroupId}/grades`, Request);
  },
};
