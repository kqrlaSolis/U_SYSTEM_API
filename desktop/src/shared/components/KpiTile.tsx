import type { ReactNode } from 'react';

export interface KpiTileProps {
  Label: string;
  Value: string;
  Hint?: string | null;
  Icon?: ReactNode;
  // Con OnClick la tarjeta es un botón que lleva al módulo
  OnClick?: (() => void) | null;
}

// Indicador: etiqueta, cifra grande y una nota opcional. Lo usan el inicio y el dashboard.
export function KpiTile({ Label, Value, Hint = null, Icon, OnClick = null }: KpiTileProps) {
  const Content = (
    <>
      {Icon !== undefined && <span className="USystemKpiIcon">{Icon}</span>}
      <span className="USystemKpiLabel">{Label}</span>
      <span className="USystemKpiValue">{Value}</span>
      {Hint !== null && <span className="USystemKpiHint">{Hint}</span>}
    </>
  );

  return OnClick === null ? (
    <div className="USystemKpi">{Content}</div>
  ) : (
    <button type="button" className="USystemKpi USystemKpiLink" onClick={OnClick}>
      {Content}
    </button>
  );
}
