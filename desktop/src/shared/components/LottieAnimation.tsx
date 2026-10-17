import { Lottie } from 'lottie-react';

export interface LottieAnimationProps {
  // JSON de Lottie importado (resuelve el import de Vite)
  Data: object;
  Height?: number;
  Loop?: boolean;
  AriaLabel?: string;
}

// Envoltorio de lottie-react con props PascalCase: la animación es decorativa
// (aria-label opcional) y nunca bloquea el layout. lottie-web va en su propio chunk
// (vite.config.ts → manualChunks.Lottie) para no inflar el arranque.
export function LottieAnimation({ Data, Height = 220, Loop = true, AriaLabel }: LottieAnimationProps) {
  return (
    <div
      className="USystemLottie"
      style={{ height: Height }}
      role="img"
      aria-label={AriaLabel}
    >
      <Lottie src={Data} autoplay loop={Loop} style={{ height: '100%' }} />
    </div>
  );
}
