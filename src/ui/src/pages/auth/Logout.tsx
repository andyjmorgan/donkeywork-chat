import React, { useEffect, useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { ProgressBar } from 'primereact/progressbar';
import authService from '../../services/auth/authService';
import { useNavigate } from 'react-router-dom';

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

const Logout: React.FC = () => {
  const [logoutStatus, setLogoutStatus] = useState<'processing' | 'success' | 'error'>('processing');
  const [message, setMessage] = useState<string>('Logging you out...');
  const { updateAuthState } = useAuth();
  const navigate = useNavigate();

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

  // Execute logout process on component mount
  useEffect(() => {
    const performLogout = async () => {
      try {
        // Clear all client-side state immediately
        updateAuthState(false);
        
        // Save theme preference before clearing storage
        const theme = localStorage.getItem('theme');
        
        // Clear local storage and session storage
        localStorage.clear();
        sessionStorage.clear();
        
        // Restore theme preference
        if (theme) {
          localStorage.setItem('theme', theme);
        }
        
        // Get the origin for the redirect URL
        const origin = window.location.origin;
        const redirectUrl = `${origin}/`;
        
        // First get the Keycloak logout URL from the backend
        const logoutUrlResponse = await fetch(
          `${authService.api.defaults.baseURL}/auth/logout-url?redirectUrl=${encodeURIComponent(redirectUrl)}`, 
          {
            method: 'GET',
            credentials: 'include',
            headers: {
              'Cache-Control': 'no-cache, no-store, must-revalidate',
              'Pragma': 'no-cache'
            }
          }
        );
        
        if (logoutUrlResponse.ok) {
          const { logoutUrl } = await logoutUrlResponse.json();
          
          // Change approach to avoid CORS issues:
          // 1. First perform a normal API logout to clear application session
          const response = await fetch(`${authService.api.defaults.baseURL}/auth/logout`, {
            method: 'POST',
            credentials: 'include',
            headers: {
              'Content-Type': 'application/json',
              'Cache-Control': 'no-cache, no-store, must-revalidate',
              'Pragma': 'no-cache'
            }
          });

          // 2. Then we'll use an iframe approach to clear the Keycloak session
          if (response.ok) {
            setLogoutStatus('success');
            setMessage('cya later, calculator.');
            
            // Create a hidden iframe to load the Keycloak logout URL
            const iframe = document.createElement('iframe');
            iframe.style.display = 'none';
            iframe.src = logoutUrl;
            
            // Add event listener to handle iframe load/error
            iframe.onload = () => {
              console.log('Keycloak logout iframe loaded');
              // Remove iframe after small delay
              setTimeout(() => {
                document.body.removeChild(iframe);
              }, 1000);
            };
            
            iframe.onerror = () => {
              console.error('Keycloak logout iframe failed to load');
              // Remove iframe on error
              document.body.removeChild(iframe);
            };
            
            // Add iframe to body to trigger the load
            document.body.appendChild(iframe);
          } else {
            setLogoutStatus('error');
            setMessage('There was an issue logging you out completely. For security, your browser session has been cleared.');
          }
        } else {
          console.error('Failed to get Keycloak logout URL');
          setLogoutStatus('error');
          setMessage('There was an issue logging you out completely. For security, your browser session has been cleared.');
        }
      } catch (error) {
        console.error('Logout process error:', error);
        setLogoutStatus('error');
        setMessage('An error occurred during logout. For security, your browser session has been cleared.');
      }
    };

    // Execute logout
    performLogout();
    
    // No automatic redirect
  }, [updateAuthState]);

  // Handle manual navigation
  const handleLoginRedirect = () => {
    navigate('/');
  };

  return (
    <div className="flex justify-content-center align-items-center h-screen surface-ground">
      <Card className="shadow-4 p-5 border-round-xl w-full md:w-8 lg:w-6" style={{ maxWidth: '750px' }}>
        <div className="text-center mb-5">
          <Logo size={160} />
        </div>
        
        {logoutStatus === 'processing' ? (
          <div className="text-center mb-4">
            <h2 className="text-2xl font-medium mb-2 text-primary">Logging Out</h2>
            <p className="text-secondary mb-4">Please wait while we complete your logout...</p>
            <div className="mt-3">
              <ProgressBar mode="indeterminate" style={{ height: '6px' }} />
            </div>
          </div>
        ) : logoutStatus === 'success' ? (
          <div className="text-center mb-4">
            <h2 className="text-2xl font-medium mb-2 text-primary">Logout Successful</h2>
            <p className="text-secondary mb-4">{message}</p>
          </div>
        ) : (
          <div className="text-center mb-4">
            <h2 className="text-2xl font-medium mb-2 text-primary">Logout Completed</h2>
            <p className="text-secondary mb-4">{message}</p>
          </div>
        )}
        
        <div className="flex justify-content-center gap-3 mt-4">
          <Button 
            label="Return to Login" 
            icon="pi pi-sign-in"
            className="p-button p-component p-button-primary"
            onClick={handleLoginRedirect}
          />
        </div>
      </Card>
    </div>
  );
};

export default Logout;