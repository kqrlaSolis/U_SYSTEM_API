import { fireEvent, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { RenderWithTheme } from '../../test/RenderWithTheme';
import { RowActionButton } from './RowActionButton';

// El botón solo muestra el ícono: el nombre accesible y el tooltip salen de Label + Subject.
describe('RowActionButton', () => {
  it('expone nombre accesible y tooltip sin texto visible', () => {
    const OnClick = vi.fn();
    RenderWithTheme(
      <RowActionButton Icon="Edit" Label="Editar" Subject="martinamaro" OnClick={OnClick} />,
    );

    const Button = screen.getByRole('button', { name: 'Editar martinamaro' });
    expect(Button.getAttribute('title')).toBe('Editar martinamaro');
    expect(Button.textContent?.trim()).toBe('');

    fireEvent.click(Button);
    expect(OnClick).toHaveBeenCalledTimes(1);
  });

  it('usa solo la etiqueta cuando no hay sujeto', () => {
    RenderWithTheme(<RowActionButton Icon="Delete" Label="Eliminar" OnClick={() => {}} />);

    expect(screen.getByRole('button', { name: 'Eliminar' })).toBeTruthy();
  });
});
