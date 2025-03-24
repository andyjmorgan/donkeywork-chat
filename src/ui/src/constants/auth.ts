// Authentication constants
export const AUTH_CONFIG = {
  authUrl: 'https://identity.donkeywork.dev/realms/donkeywork',
  tokenUrl: 'https://identity.donkeywork.dev/realms/donkeywork/protocol/openid-connect/token',
  logoutUrl: 'https://identity.donkeywork.dev/realms/donkeywork/protocol/openid-connect/logout',
  clientId: 'donkeywork',
  clientSecret: import.meta.env.VITE_CLIENT_SECRET || '',
};

// Token storage keys
export const ACCESS_TOKEN_KEY = 'access_token';
export const REFRESH_TOKEN_KEY = 'refresh_token';
export const TOKEN_EXPIRY_KEY = 'token_expiry';