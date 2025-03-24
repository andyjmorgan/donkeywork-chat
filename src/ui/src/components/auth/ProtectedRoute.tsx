import React, { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';
import { Button } from 'primereact/button';
import { useAuth } from '../../context/AuthContext';
import AuthService from '../../services/auth/authService';
import { ProtectedRouteProps, LogoProps, LoadingStateProps } from '../../models/ui/auth/ProtectedRouteTypes';

// Extracted UI components for cleaner code
const Logo: React.FC<LogoProps> = ({ size = 140 }) => (
  <img 
    src="/images/donkeywork.png" 
    alt="DonkeyWork Logo" 
    className="mb-3" 
    style={{ width: `${size}px`, height: 'auto' }} 
  />
);

const LoadingState: React.FC<LoadingStateProps> = ({ title, message }) => (
  <div className="flex justify-content-center align-items-center h-screen surface-ground">
    <div className="card shadow-4 p-5 border-round-xl text-center">
      <div className="mb-4">
        <Logo />
      </div>
      <i className="pi pi-spin pi-spinner text-primary mb-3" style={{ fontSize: '2.5rem' }}></i>
      <h3 className="text-xl font-medium mb-2">{title}</h3>
      <p className="text-secondary">{message}</p>
    </div>
  </div>
);

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated, login, loading } = useAuth();
  const location = useLocation();
  const [shouldAttemptLogin, setShouldAttemptLogin] = useState(false);
  const [checkedAuth, setCheckedAuth] = useState(false);
  
  // Load theme on component mount
  useEffect(() => {
    const savedTheme = localStorage.getItem('theme') || 'light';
    loadTheme(savedTheme === 'dark' ? 'lara-dark-purple' : 'lara-light-purple');
  }, []);
  
  // Function to load a theme dynamically
  const loadTheme = (theme: string) => {
    const linkId = 'theme-css';
    const themeLink = document.getElementById(linkId) as HTMLLinkElement || document.createElement('link');
    
    if (!themeLink.id) {
      themeLink.id = linkId;
      themeLink.rel = 'stylesheet';
      document.head.appendChild(themeLink);
    }
    
    themeLink.href = `https://cdn.jsdelivr.net/npm/primereact@10.9.3/resources/themes/${theme}/theme.css`;
  };
  
  // Handle authentication verification
  useEffect(() => {
    // Don't do anything on the callback route or if already loading/authenticated
    if (location.pathname === '/callback' || loading || isAuthenticated || checkedAuth) {
      return;
    }
    
    // Verify authentication status with the API
    const verifyAuth = async () => {
      try {
        // Use the AuthService.checkAuth method for consistency
        const { isAuthenticated: authStatus } = await AuthService.checkAuth();
        
        if (authStatus) {
          // User is authenticated according to API, reload to sync state
          window.location.reload();
          return;
        }
        
        // Not authenticated, show login
        setShouldAttemptLogin(true);
      } catch (error) {
        // Error checking auth, fall back to showing login
        setShouldAttemptLogin(true);
      } finally {
        setCheckedAuth(true);
      }
    };
    
    verifyAuth();
  }, [isAuthenticated, loading, location.pathname, checkedAuth]);
  
  // Set shouldAttemptLogin when verification is complete
  useEffect(() => {
    if (checkedAuth && !isAuthenticated && !loading && location.pathname !== '/callback') {
      setShouldAttemptLogin(true);
    }
  }, [checkedAuth, isAuthenticated, loading, location.pathname]);
  
  // Handle login transition
  const handleLogin = () => {
    const pathToStore = location.pathname === '/' ? '/' : location.pathname;
    sessionStorage.setItem('redirectAfterLogin', pathToStore);
    // Redirect to login page instead of directly calling login
    window.location.href = `/login${location.search}`;
  };
  
  // Render states
  
  // Loading state
  if (loading) {
    return <LoadingState title="Checking Authentication" message="Checking you are who you say you are 👀" />;
  }
  
  // Auth required state
  if (shouldAttemptLogin) {
    // Redirect to login page and preserve query parameters
    window.location.href = `/login${location.search}`;
    return <LoadingState title="Redirecting" message="Taking you to the login page..." />;
  }
  
  // Authenticated state
  if (isAuthenticated) {
    return <>{children}</>;
  }
  
  // Default loading state (while waiting for auth check or redirect)
  return <LoadingState title="Preparing Login" message="Redirecting you to the login page..." />;
};

export default ProtectedRoute;