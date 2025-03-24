import React, { createContext, useState, useEffect, ReactNode, useCallback } from 'react';
import AuthService from '../services/auth/authService';
import { userService } from '../services/api/userService';
import { GetUserInformationResponse } from '../models/api/UserService/GetUserInformationResponse';

interface AuthContextType {
  isAuthenticated: boolean;
  user: GetUserInformationResponse | null;
  login: () => Promise<void>;
  logout: () => Promise<void>;
  updateAuthState: (newState: boolean) => void;
  handleSessionExpired: () => void;
  loading: boolean;
}

const defaultAuthContext: AuthContextType = {
  isAuthenticated: false,
  user: null,
  login: async () => {},
  logout: async () => {},
  updateAuthState: () => {},
  handleSessionExpired: () => {},
  loading: true,
};

export const AuthContext = createContext<AuthContextType>(defaultAuthContext);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [user, setUser] = useState<GetUserInformationResponse | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  // Function to update auth state (used by AuthCallback)
  const updateAuthState = useCallback((newState: boolean) => {
    setIsAuthenticated(newState);
    // If logged out, clear user data
    if (!newState) {
      setUser(null);
    }
  }, []);

  // Define login and logout functions
  const login = async (): Promise<void> => {
    try {
      await AuthService.login();
    } catch (error) {
      console.error('Login failed:', error);
    }
  };
  
  // Simple session expiration handler
  const handleSessionExpired = useCallback(() => {
    // Clear authenticated state
    setIsAuthenticated(false);
    setUser(null);
    
    // Clear any cached data
    sessionStorage.removeItem('userPreferences');
    localStorage.removeItem('lastViewed');
    
    // Just redirect - the server will handle cookies with its own logout
    window.location.href = '/login?session=expired';
  }, []);

  const logout = async (): Promise<void> => {
    try {
      // Clear state immediately
      setIsAuthenticated(false);
      setUser(null);
      
      // Call backend to logout - redirect is handled within the service
      await AuthService.logout();
    } catch (error) {
      console.error('Logout failed in context:', error);
      // Fallback in case the service's error handling doesn't work
      window.location.href = '/login?logout=error';
    }
  };

  // Load user info when authenticated
  const loadUserInfo = useCallback(async () => {
    if (isAuthenticated) {
      try {
        const userData = await userService.getUserInfo();
        if (userData) {
          setUser(userData);
        }
      } catch (error) {
        console.error('Failed to refresh user info:', error);
      }
    }
  }, [isAuthenticated]);

  // Check authentication status on initial load
  useEffect(() => {
    let isMounted = true;
    
    const checkAuthStatus = async () => {
      try {
        if (!isMounted) return;
        setLoading(true);

        await new Promise(resolve => setTimeout(resolve, 500));

        const { isAuthenticated: authenticated, userData } = await AuthService.checkAuth();
        
        if (isMounted) {
          setIsAuthenticated(authenticated);
          if (authenticated && userData) {
            setUser(userData);
          } else {
            setUser(null);
          }
        }
      } catch (error) {
        console.error('Error checking auth status:', error);
        if (isMounted) {
          setIsAuthenticated(false);
          setUser(null);
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    checkAuthStatus();
    
    return () => {
      isMounted = false;
    };
  }, []);

  // Load user info when needed and refresh every 5 minutes
  useEffect(() => {
    // Initial load
    loadUserInfo();
    
    // Set up a refresh interval to periodically check for user info changes
    const refreshInterval = setInterval(() => {
      if (isAuthenticated) {
        loadUserInfo();
      }
    }, 5 * 60 * 1000); // Every 5 minutes
    
    return () => clearInterval(refreshInterval);
  }, [isAuthenticated, loadUserInfo]);

  const authContextValue: AuthContextType = {
    isAuthenticated,
    user,
    login,
    logout,
    updateAuthState,
    handleSessionExpired,
    loading,
  };

  return (
    <AuthContext.Provider value={authContextValue}>
      {children}
    </AuthContext.Provider>
  );
};

// Custom hook to use auth context
export const useAuth = () => {
  const context = React.useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};