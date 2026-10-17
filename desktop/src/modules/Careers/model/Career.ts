// Contrato de carrera (GET /api/v1/careers)
export interface Career {
  Id: number;
  Code: string;
  Name: string;
  Level: string;
  TermCount: number;
  TotalCredits: number;
  Modality: string;
  IsActive: boolean;
}

// Alta y edición comparten forma; el alta ignora IsActive
export interface CareerRequest {
  Code: string;
  Name: string;
  Level: string;
  TermCount: number;
  TotalCredits: number;
  Modality: string;
  IsActive: boolean;
}
