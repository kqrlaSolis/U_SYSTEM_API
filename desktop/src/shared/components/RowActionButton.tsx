import type { ReactNode } from 'react';
import { AppButton } from './AppButton';

export type RowActionIconName =
  | 'Edit'
  | 'Delete'
  | 'Open'
  | 'Close'
  | 'Cancel'
  | 'Issue'
  | 'SetCurrent'
  | 'Register'
  | 'Validate';

// Trazos en SVG propios (mismo criterio que NavigationIcons): viewBox 24x24, color heredado.
const IconPaths: Record<RowActionIconName, string> = {
  Edit: 'M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25ZM20.71 7.04a1 1 0 0 0 0-1.41l-2.34-2.34a1 1 0 0 0-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83Z',
  Delete: 'M6 19a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V7H6v12ZM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4Z',
  Open: 'M8 5v14l11-7L8 5Z',
  Close:
    'M18 8h-1V6a5 5 0 0 0-10 0v2H6a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V10a2 2 0 0 0-2-2Zm-6 9a2 2 0 1 1 2-2 2 2 0 0 1-2 2Zm3.1-9H8.9V6a3.1 3.1 0 0 1 6.2 0v2Z',
  Cancel:
    'M12 2a10 10 0 1 0 10 10A10 10 0 0 0 12 2Zm0 18a8 8 0 0 1-6.32-12.9l11.22 11.22A7.95 7.95 0 0 1 12 20Zm6.32-3.1L7.1 5.68A8 8 0 0 1 18.32 16.9Z',
  Issue:
    'M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8l-6-6Zm-3.06 16L7.4 14.46l1.41-1.41 2.12 2.12 4.24-4.24 1.41 1.41L10.94 18ZM13 9V3.5L18.5 9H13Z',
  SetCurrent:
    'M12 17.27 18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21 12 17.27Z',
  Register:
    'M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8l-6-6Zm2 14h-3v3h-2v-3H8v-2h3v-3h2v3h3v2Zm-3-7V3.5L18.5 9H13Z',
  Validate:
    'M12 2a10 10 0 1 0 10 10A10 10 0 0 0 12 2Zm-2 15-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9Z',
};

function RowActionIcon({ Name }: { Name: RowActionIconName }): ReactNode {
  return (
    <svg viewBox="0 0 24 24" width="18" height="18" aria-hidden="true" focusable="false">
      <path fill="currentColor" d={IconPaths[Name]} />
    </svg>
  );
}

export interface RowActionButtonProps {
  Icon: RowActionIconName;
  // Texto de la acción: es el nombre accesible y el tooltip del botón
  Label: string;
  // Qué fila afecta (usuario, código, folio…) para que el lector de pantalla no repita "Editar" sin contexto
  Subject?: string | number;
  OnClick: () => void;
  Color?: 'primary' | 'danger';
  IsLoading?: boolean;
}

// Botón de acción de fila: solo ícono a la vista, con nombre accesible y tooltip.
export function RowActionButton({
  Icon,
  Label,
  Subject,
  OnClick,
  Color = 'primary',
  IsLoading = false,
}: RowActionButtonProps) {
  return (
    <AppButton
      Variant="text"
      Size="sm"
      Color={Color}
      Icon={<RowActionIcon Name={Icon} />}
      AriaLabel={Subject === undefined ? Label : `${Label} ${Subject}`}
      IsLoading={IsLoading}
      OnClick={OnClick}
    />
  );
}
