import { ITButton } from '@axzydev/axzy_ui_system';
import type { ReactNode } from 'react';

export type AppButtonVariant =
  | 'filled'
  | 'outlined'
  | 'raised'
  | 'rounded'
  | 'text'
  | 'raised-text'
  | 'icon-only'
  | 'link';

export type AppButtonColor =
  | 'primary'
  | 'secondary'
  | 'success'
  | 'danger'
  | 'warning'
  | 'info'
  | 'gray'
  | 'purple'
  | 'error';

export type AppButtonSize = 'sm' | 'md' | 'lg';

export interface AppButtonProps {
  Label?: string;
  OnClick?: () => void;
  Variant?: AppButtonVariant;
  Color?: AppButtonColor;
  Size?: AppButtonSize;
  Type?: 'button' | 'submit' | 'reset';
  Disabled?: boolean;
  IsLoading?: boolean;
  Icon?: ReactNode;
  AriaLabel?: string;
  children?: ReactNode;
}

// Envoltorio: traduce props PascalCase a la API real de ITButton.
export function AppButton({
  Label,
  OnClick,
  Variant = 'filled',
  Color = 'primary',
  Size = 'md',
  Type = 'button',
  Disabled = false,
  IsLoading = false,
  Icon,
  AriaLabel,
  children,
}: AppButtonProps) {
  return (
    <ITButton
      label={Label}
      onClick={OnClick}
      variant={Variant}
      color={Color}
      size={Size}
      type={Type}
      disabled={Disabled || IsLoading}
      icon={Icon}
      ariaLabel={AriaLabel}
      className={IsLoading ? 'USystemButtonLoading' : undefined}
    >
      {children}
    </ITButton>
  );
}
