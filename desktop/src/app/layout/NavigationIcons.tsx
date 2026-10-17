import type { ReactNode } from 'react';
import type { NavigationIconName } from './NavigationModules';

interface MenuIconProps {
  Name: NavigationIconName;
}

// Iconos propios en SVG: la librería envuelve el nodo que se le pase.
function MenuIcon({ Name }: MenuIconProps) {
  if (Name === 'Users') {
    return (
      <svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" focusable="false">
        <path
          fill="currentColor"
          d="M16 11a4 4 0 1 0-4-4 4 4 0 0 0 4 4Zm-8 2a3 3 0 1 0-3-3 3 3 0 0 0 3 3Zm0 2c-2.7 0-5 1.3-5 3v2h6v-2c0-1 .4-1.9 1-2.7A9 9 0 0 0 8 15Zm8 0c-3.3 0-6 1.6-6 3.5V21h12v-2.5c0-1.9-2.7-3.5-6-3.5Z"
        />
      </svg>
    );
  }

  if (Name === 'Careers') {
    return (
      <svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" focusable="false">
        <path
          fill="currentColor"
          d="M9 4h6a2 2 0 0 1 2 2v1h3a2 2 0 0 1 2 2v3H2V9a2 2 0 0 1 2-2h3V6a2 2 0 0 1 2-2Zm1 3h4V6h-4ZM2 14h7v2h6v-2h7v5a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2Z"
        />
      </svg>
    );
  }

  if (Name === 'Subjects') {
    return (
      <svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" focusable="false">
        <path
          fill="currentColor"
          d="M5 3h12a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2Zm2 3v2h8V6H7Zm0 4v2h8v-2H7Zm0 4v2h5v-2H7Z"
        />
      </svg>
    );
  }

  if (Name === 'AcademicTerms') {
    return (
      <svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" focusable="false">
        <path
          fill="currentColor"
          d="M7 2h2v2h6V2h2v2h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2V2Zm12 8H5v10h14V10Zm-9 3h2v2H8v-2Zm4 0h2v2h-2v-2Zm-4 3h2v2H8v-2Zm4 0h2v2h-2v-2Z"
        />
      </svg>
    );
  }

  if (Name === 'Roles') {
    return (
      <svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" focusable="false">
        <path
          fill="currentColor"
          d="M12 2 4 5v6c0 5 3.4 9.7 8 11 4.6-1.3 8-6 8-11V5l-8-3Zm0 5a3 3 0 1 1 0 6 3 3 0 0 1 0-6Zm0 12c-1.7-.6-3.2-1.7-4.3-3.2.9-1.1 2.6-1.8 4.3-1.8s3.4.7 4.3 1.8A8.3 8.3 0 0 1 12 19Z"
        />
      </svg>
    );
  }

  if (Name === 'AuditLogs') {
    return (
      <svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" focusable="false">
        <path
          fill="currentColor"
          d="M6 2h9l5 5v13a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2Zm8 2v4h4l-4-4ZM8 12h8v2H8v-2Zm0 4h8v2H8v-2Z"
        />
      </svg>
    );
  }

  if (Name === 'Settings') {
    return (
      <svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" focusable="false">
        <path
          fill="currentColor"
          d="M12 8a4 4 0 1 0 4 4 4 4 0 0 0-4-4Zm8.9 4a7 7 0 0 0-.1-1.2l2-1.5-2-3.4-2.3 1a7.4 7.4 0 0 0-2-1.2L16.2 3h-4l-.3 2.7a7.4 7.4 0 0 0-2 1.2l-2.3-1-2 3.4 2 1.5a7 7 0 0 0 0 2.4l-2 1.5 2 3.4 2.3-1a7.4 7.4 0 0 0 2 1.2l.3 2.7h4l.3-2.7a7.4 7.4 0 0 0 2-1.2l2.3 1 2-3.4-2-1.5a7 7 0 0 0 .1-1.2Z"
        />
      </svg>
    );
  }

  return (
    <svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" focusable="false">
      <path fill="currentColor" d="M12 3 2 12h3v9h6v-6h2v6h6v-9h3L12 3Z" />
    </svg>
  );
}

// Traduce el nombre del catálogo de módulos al nodo del ícono.
export function GetNavigationIcon(Name: NavigationIconName): ReactNode {
  return <MenuIcon Name={Name} />;
}
