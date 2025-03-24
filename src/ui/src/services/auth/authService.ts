import axios from 'axios';
import { generateCodeVerifier, generateCodeChallenge } from './pkce';
import { userService } from '../api';
import type { AuthCheckResult } from '../../models/api/authTypes';
import ApiBase from '../api/apiBase';

// Using ApiBase for URL handling
class AuthServiceBase extends ApiBase {
  // We need to make getApiBaseUrl public to use it directly
  public getApiBaseUrl(): string {
    return super.getApiBaseUrl();
  }
}

const authServiceBase = new AuthServiceBase();
const API_URL = authServiceBase.getApiBaseUrl();

// Authentication configuration - FIXED to ensure client ID matches Keycloak exactly
const AUTH_CONFIG = {
  authUrl: 'https://identity.donkeywork.dev/realms/donkeywork',
  clientId: 'donkeywork', // Must match the client ID in Keycloak EXACTLY
  scope: 'openid profile email',
};

// PKCE state storage keys
const CODE_VERIFIER_KEY = 'code_verifier';
const AUTH_STATE_KEY = 'auth_state';

// API client with interceptors for handling auth failures
const api = axios.create({
  baseURL: API_URL,
  withCredentials: true, // Important for sending cookies with requests
});

// Generate and store PKCE code verifier
const setupPkce = async (): Promise<{ verifier: string; challenge: string }> => {
  const verifier = generateCodeVerifier();
  const challenge = await generateCodeChallenge(verifier);
  
  // Store the verifier in sessionStorage (not localStorage for better security)
  sessionStorage.setItem(CODE_VERIFIER_KEY, verifier);
  
  return { verifier, challenge };
};

// Redirect to Keycloak login with PKCE
const login = async (): Promise<void> => {
  try {
    // Generate PKCE code verifier and challenge
    const { challenge } = await setupPkce();
    
    // Generate a random state parameter
    const state = Math.random().toString(36).substring(2, 15);
    sessionStorage.setItem(AUTH_STATE_KEY, state);
    
    // Construct the redirect URL to Keycloak
    const redirectUri = `${window.location.origin}/callback`;
    const loginUrl = `${AUTH_CONFIG.authUrl}/protocol/openid-connect/auth?` +
      `client_id=${AUTH_CONFIG.clientId}` +
      `&redirect_uri=${encodeURIComponent(redirectUri)}` +
      `&response_type=code` +
      `&scope=${encodeURIComponent(AUTH_CONFIG.scope)}` +
      `&code_challenge=${challenge}` +
      `&code_challenge_method=S256` +
      `&state=${state}`;
    
    // Redirect to Keycloak login
    window.location.href = loginUrl;
  } catch (error) {
    console.error('Login setup failed:', error);
    throw new Error('Failed to setup authentication');
  }
};

// Hash to track processed codes
const processedCodes = new Set<string>();

// Handle the authentication callback from Keycloak
const handleAuthCallback = async (code: string, state: string): Promise<boolean> => {
  try {
    // Check if we've already processed this code
    if (processedCodes.has(code)) {
      return false;
    }
    
    // Mark this code as being processed
    processedCodes.add(code);
    
    // Verify state parameter
    const storedState = sessionStorage.getItem(AUTH_STATE_KEY);
    if (!storedState || storedState !== state) {
      console.error('State validation failed');
      return false;
    }
    
    // Get the code verifier
    const codeVerifier = sessionStorage.getItem(CODE_VERIFIER_KEY);
    if (!codeVerifier) {
      console.error('Code verifier not found');
      return false;
    }
    
    const redirectUri = `${window.location.origin}/callback`;
    
    // Use fetch directly instead of axios to avoid interceptor issues
    try {
      const response = await fetch(`${API_URL}/auth/exchange`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({
          code,
          codeVerifier,
          redirectUri,
        }),
      });
      
      if (!response.ok) {
        try {
          const errorText = await response.text();
          console.error('Token exchange failed:', response.status, errorText);
          // Try to parse as JSON if possible for more details
          try {
            const errorJson = JSON.parse(errorText);
            console.error('Detailed error:', errorJson);
          } catch (parseError) {
            // Just log as text if not JSON
            console.error('Error response (not JSON):', errorText);
          }
        } catch (readError) {
          console.error('Failed to read error response:', readError);
        }
        return false;
      }
      
      // Clean up PKCE storage
      sessionStorage.removeItem(CODE_VERIFIER_KEY);
      sessionStorage.removeItem(AUTH_STATE_KEY);
      
      return true;
    } catch (fetchError) {
      console.error('Fetch error during token exchange:', fetchError);
      return false;
    }
  } catch (error) {
    console.error('Auth callback handling failed:', error);
    return false;
  }
};

// Logout the user - simplified to use the dedicated logout page
const logout = async (): Promise<void> => {
  try {
    // Clear any local state immediately (for UI feedback)
    lastAuthCheck = 0;
    
    // Simply redirect to the dedicated logout page which will handle everything
    window.location.href = '/logout';
  } catch (error) {
    console.error('Logout redirect failed:', error);
    // Fall back to direct redirect in case of error
    window.location.href = '/logout';
  }
};

// Anti-loop protection variables
let lastAuthCheck = 0;
const MIN_AUTH_CHECK_INTERVAL = 5000; // 5 seconds

// Check if the user is authenticated and return user data in one call
const checkAuth = async (): Promise<AuthCheckResult> => {
  // Prevent rapid checks but don't auto-fail
  const now = Date.now();
  lastAuthCheck = now;
  
  try {
    const userData = await userService.getUserInfo();
    
    // If we got user data, we're authenticated
    if (userData) {
      return { 
        isAuthenticated: true, 
        userData 
      };
    }
    
    // No user data means we're not authenticated
    return { isAuthenticated: false, userData: null };
  } catch (error) {
    console.error('Authentication check error:', error);
    return { isAuthenticated: false, userData: null };
  }
};

// Legacy method for backward compatibility
const isAuthenticated = async (): Promise<boolean> => {
  const { isAuthenticated } = await checkAuth();
  return isAuthenticated;
};

// Super simple session expiration handler - just redirect on 401
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Only handle 401 errors (unauthorized)
    if (error.response?.status === 401) {
      window.location.href = '/login?session=expired';
    }
    
    // For other errors, reject normally
    return Promise.reject(error);
  }
);

// Authentication service class extending ApiBase
export class AuthService extends ApiBase {
  login = login;
  logout = logout;
  handleAuthCallback = handleAuthCallback;
  isAuthenticated = isAuthenticated;
  checkAuth = checkAuth;
  api = api;
}

// Create and export a singleton instance
const authService = new AuthService();
export default authService;