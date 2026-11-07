import { ITConfirmDialog } from '@axzydev/axzy_ui_system';
import type { ReactNode } from 'react';
import type { AppButtonColor } from './AppButton';

export interface ConfirmDialogProps {
  IsOpen: boolean;
  Title?: string;
  Message: ReactNode;
  ConfirmLabel?: string;
  CancelLabel?: string;
  Variant?: AppButtonColor;
  IsLoading?: boolean;
  OnConfirm: () => void;
  OnClose: () => void;
}

// Envoltorio: traduce props PascalCase a la API real de ITConfirmDialog.
export function ConfirmDialog({
  IsOpen,
  Title,
  Message,
  ConfirmLabel,
  CancelLabel,
  Variant = 'danger',
  IsLoading = false,
  OnConfirm,
  OnClose,
}: ConfirmDialogProps) {
  return (
    <ITConfirmDialog
      isOpen={IsOpen}
      onClose={OnClose}
      onConfirm={OnConfirm}
      title={Title}
      message={Message}
      confirmLabel={ConfirmLabel}
      cancelLabel={CancelLabel}
      variant={Variant}
      loading={IsLoading}
    />
  );
}
