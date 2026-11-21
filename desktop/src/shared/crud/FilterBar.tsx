import { ITSelect } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { AppButton } from '../components/AppButton';
import type { CrudOption } from './CrudTypes';
import { useCatalogOptions } from '../catalogs/useCatalogOptions';

// Origen de las opciones: catálogos de la API o listas fijas traducidas.
export type FilterOptionKey = 'Careers' | 'Terms' | 'Plans' | 'ActiveStatus' | 'StudentStatuses';

export interface FilterField {
  Field: string;
  LabelKey: string;
  OptionsKey: FilterOptionKey;
}

export interface FilterBarProps {
  Fields: FilterField[];
  Values: Record<string, string>;
  OnChange: (Field: string, Value: string) => void;
  OnClear: () => void;
}

// Barra de filtros compartida: selects con opciones de catálogo y botón de limpiar.
export function FilterBar({ Fields, Values, OnChange, OnClear }: FilterBarProps) {
  const Translation = useTranslation();
  const Options = useCatalogOptions();
  const HasValue = Fields.some((Field) => (Values[Field.Field] ?? '') !== '');

  const Resolve = (Key: FilterOptionKey): CrudOption[] => {
    if (Key === 'Careers') {
      return Options.Careers;
    }
    if (Key === 'Terms') {
      return Options.Terms;
    }
    if (Key === 'Plans') {
      return Options.Plans;
    }
    if (Key === 'ActiveStatus') {
      return [
        { value: 'true', label: Translation.t('Common.Yes') },
        { value: 'false', label: Translation.t('Common.No') },
      ];
    }
    return [
      { value: 'Active', label: Translation.t('Students.Status.Active') },
      { value: 'Inactive', label: Translation.t('Students.Status.Inactive') },
      { value: 'Graduated', label: Translation.t('Students.Status.Graduated') },
      { value: 'Dropout', label: Translation.t('Students.Status.Dropout') },
    ];
  };

  return (
    <div className="USystemFilterBar">
      {Fields.map((Field) => (
        <ITSelect
          key={Field.Field}
          name={`Filter-${Field.Field}`}
          label={Translation.t(Field.LabelKey)}
          options={Resolve(Field.OptionsKey)}
          value={Values[Field.Field] ?? ''}
          onChange={(Event: React.ChangeEvent<HTMLSelectElement>) => {
            OnChange(Field.Field, Event.target.value);
          }}
        />
      ))}
      {HasValue && (
        <AppButton
          Label={Translation.t('Common.Clear')}
          Variant="text"
          Size="sm"
          OnClick={OnClear}
        />
      )}
    </div>
  );
}
