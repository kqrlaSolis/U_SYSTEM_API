import { ITInput } from '@axzydev/axzy_ui_system';
import type React from 'react';

export interface SearchBoxProps {
  Value: string;
  OnChange: (Value: string) => void;
  OnSearch?: () => void;
  Placeholder?: string;
  Label?: string;
  Name?: string;
  Disabled?: boolean;
}

// Envoltorio: ITInput cubre el caso (no hay ITSearchBox en el catálogo).
export function SearchBox({
  Value,
  OnChange,
  OnSearch,
  Placeholder,
  Label,
  Name = 'Search',
  Disabled = false,
}: SearchBoxProps) {
  const HandleChange = (Event: React.ChangeEvent<HTMLInputElement>) => {
    OnChange(Event.target.value);
  };

  const HandleKeyDown = (Event: React.KeyboardEvent) => {
    if (Event.key === 'Enter' && OnSearch !== undefined) {
      OnSearch();
    }
  };

  return (
    <div className="USystemSearchBox">
      <ITInput
        name={Name}
        type="text"
        label={Label}
        placeholder={Placeholder}
        value={Value}
        onChange={HandleChange}
        onKeyDown={HandleKeyDown}
        disabled={Disabled}
        size="md"
      />
    </div>
  );
}
