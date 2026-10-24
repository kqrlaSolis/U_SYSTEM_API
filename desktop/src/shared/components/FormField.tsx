import { ITField } from '@axzydev/axzy_ui_system';
import type { ReactNode } from 'react';

export interface FormFieldProps {
  Label?: string;
  HtmlFor?: string;
  IsRequired?: boolean;
  ErrorMessage?: string | null;
  HelpText?: string;
  children: ReactNode;
}

// Envoltorio: traduce props PascalCase a la API real de ITField.
export function FormField({
  Label,
  HtmlFor,
  IsRequired = false,
  ErrorMessage,
  HelpText,
  children,
}: FormFieldProps) {
  return (
    <ITField
      label={Label}
      htmlFor={HtmlFor}
      required={IsRequired}
      error={ErrorMessage ?? undefined}
      helpText={HelpText}
    >
      {children}
    </ITField>
  );
}
