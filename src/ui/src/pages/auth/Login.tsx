import React, { useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { useNavigate, useLocation } from 'react-router-dom';
import authService from '../../services/auth/authService';

// Ensure PrimeReact styles are loaded
import 'primereact/resources/primereact.min.css';
import 'primeicons/primeicons.css';
import 'primeflex/primeflex.css';

// Logo component for consistent branding
const Logo = ({ size = 140 }: { size?: number }) => (
  <img 
    src="/images/donkeywork.png" 
    alt="DonkeyWork Logo" 
    className="mb-3" 
    style={{ width: `${size}px`, height: 'auto' }} 
  />
);

const Login: React.FC = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  
  // Check for status in URL parameters
  const isLogoutSuccess = location.search.includes('logout=successful') || location.search.includes('logout=success');
  const isLogoutIncomplete = location.search.includes('logout=incomplete');
  const isLogoutError = location.search.includes('logout=error');
  const isSessionExpired = location.search.includes('session=expired');
  
  // Check authentication status and theme on component mount
  useEffect(() => {
    const savedTheme = localStorage.getItem('theme') || 'light';
    loadTheme(savedTheme === 'dark' ? 'lara-dark-purple' : 'lara-light-purple');
    
    // If user is already authenticated, redirect to home
    const checkAuthStatus = async () => {
      try {
        const { isAuthenticated } = await authService.checkAuth();
        if (isAuthenticated) {
          // Already authenticated, redirect to home
          navigate('/', { replace: true });
        }
      } catch (error) {
        console.error('Error checking auth status on login page:', error);
      }
    };
    
    checkAuthStatus();
  }, [navigate]);
  
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

  // Handle login button click
  const handleLogin = () => {
    const pathToStore = location.pathname === '/' ? '/' : location.pathname;
    sessionStorage.setItem('redirectAfterLogin', pathToStore);
    login();
  };

  // Handle clear cookies functionality
  const handleClearCookies = () => {
    // Attempt to clear all cookies directly
    const cookieNames = document.cookie.split(';').map(cookie => cookie.trim().split('=')[0]);
    cookieNames.forEach(name => {
      if (!name) return;
      document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/; secure;`;
      document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/;`;
    });
    
    // Clear storage but keep theme
    const theme = localStorage.getItem('theme');
    localStorage.clear();
    sessionStorage.clear();
    if (theme) localStorage.setItem('theme', theme);
    
    // Reload the page
    window.location.href = '/';
  };

  return (
    <div className="flex justify-content-center align-items-center h-screen surface-ground">
      <Card className="shadow-4 p-5 border-round-xl" style={{ maxWidth: '750px' }}>
        <div className="text-center mb-5">
          <Logo size={160} />
        </div>
        
        {isLogoutSuccess ? (
          <div className="text-center mb-4">
            <h2 className="text-2xl font-medium mb-2 text-primary">Logout Successful</h2>
            <p className="text-secondary mb-4">You have been successfully logged out.</p>
          </div>
        ) : isLogoutIncomplete ? (
          <div className="text-center mb-4">
            <h2 className="text-2xl font-medium mb-2 text-primary">Logout Incomplete</h2>
            <p className="text-secondary mb-4">Your logout may not have fully completed. For security, please clear your browser cookies.</p>
          </div>
        ) : isLogoutError ? (
          <div className="text-center mb-4">
            <h2 className="text-2xl font-medium mb-2 text-primary">Logout Error</h2>
            <p className="text-secondary mb-4">There was a problem logging you out. For security, please clear your browser cookies.</p>
          </div>
        ) : isSessionExpired ? (
          <div className="text-center mb-4">
            <h2 className="text-2xl font-medium mb-2 text-primary">Session Expired</h2>
            <p className="text-secondary mb-4">Your session has expired. Please log in again to continue.</p>
          </div>
        ) : (
          <div className="text-center mb-4">
            <h2 className="text-2xl font-medium mb-2 text-primary">Welcome to DonkeyWork Chat</h2>
            <p className="text-secondary mb-4">Please log in to continue.</p>
          </div>
        )}
        
        <div className="flex justify-content-center gap-3">
          {(isLogoutSuccess || isLogoutIncomplete || isLogoutError) && (
            <Button 
              label="Clear Cookies"
              className="p-button p-component p-button-warning"
              onClick={handleClearCookies}
            />
          )}
          
          <Button 
            label="Log In"
            className="p-button p-component p-button-primary"
            onClick={handleLogin}
          />
        </div>
      </Card>
    </div>
  );
};

export default Login;