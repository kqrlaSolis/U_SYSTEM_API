import { ITBadget, ITCard, ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { ChangePasswordPath } from '../../../shared/auth/AuthPaths';
import { AppButton } from '../../../shared/components';
import { useProfileViewModel } from '../viewmodel/useProfileViewModel';

// Perfil del usuario autenticado: datos, roles y permisos vigentes.
export function ProfilePage() {
  const ViewModel = useProfileViewModel();
  const Translation = useTranslation();
  const Navigator = useNavigate();
  const User = ViewModel.User;

  return (
    <ITPage
      title={Translation.t('Profile.Title')}
      description={Translation.t('Profile.Subtitle')}
      maxWidth="5xl"
      loading={ViewModel.IsLoading}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      onRetry={ViewModel.Reload}
      actions={
        <AppButton
          Label={Translation.t('Profile.ChangePassword')}
          Variant="outlined"
          OnClick={() => Navigator(ChangePasswordPath)}
        />
      }
    >
      <ITStack spacing={4}>
        <ITCard title={Translation.t('Profile.Account')}>
          <dl className="USystemStatusList">
            <div>
              <dt>{Translation.t('Users.Columns.FullName')}</dt>
              <dd>{User?.FullName ?? '—'}</dd>
            </div>
            <div>
              <dt>{Translation.t('Users.Columns.Username')}</dt>
              <dd>{User?.Username ?? '—'}</dd>
            </div>
            <div>
              <dt>{Translation.t('Users.Columns.Email')}</dt>
              <dd>{User?.Email ?? '—'}</dd>
            </div>
            <div>
              <dt>{Translation.t('Profile.Roles')}</dt>
              <dd>
                {(User?.Roles ?? []).map((Role) => (
                  <ITBadget key={Role} label={Role} color="info" size="sm" />
                ))}
              </dd>
            </div>
            <div>
              <dt>{Translation.t('Profile.Permissions')}</dt>
              <dd>{User?.Permissions.length ?? 0}</dd>
            </div>
          </dl>
        </ITCard>

        <ITCard title={Translation.t('Profile.PermissionList')}>
          <ITText as="p" muted>
            {(User?.Permissions ?? []).join(' · ') || '—'}
          </ITText>
        </ITCard>
      </ITStack>
    </ITPage>
  );
}
