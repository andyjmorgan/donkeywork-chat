import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Button } from 'primereact/button';
import { ProgressBar } from 'primereact/progressbar';
import { useAuth } from '../../context/AuthContext';
import authService from '../../services/auth/authService';

const AuthCallback: React.FC = () => {
  const [error, setError] = useState<string | null>(null);
  const [isProcessing, setIsProcessing] = useState<boolean>(false);
  const [processedCode, setProcessedCode] = useState<string | null>(null);
  const { updateAuthState } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  
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

  // CRITICAL FIX: This prevents the code from being processed multiple times
  useEffect(() => {
    // Get the code from URL only once on component mount
    const searchParams = new URLSearchParams(location.search);
    const code = searchParams.get('code');
    const state = searchParams.get('state');

    if (!code || !state) {
      setError('Missing authentication data. Please try again.');
      return;
    }

    // CRITICAL: Only process if we haven't processed this code before
    if (code === processedCode) {
      return;
    }

    // Prevent processing in progress
    if (isProcessing) {
      return;
    }

    const processAuthentication = async () => {
      setIsProcessing(true);
      setProcessedCode(code); // Mark this code as being processed

      try {
        // Clear sessionStorage to prevent future attempts with the same data
        sessionStorage.removeItem('AUTH_PROCESSING');
        
        // Make sure we have the verifier
        const codeVerifier = sessionStorage.getItem('code_verifier');
        if (!codeVerifier) {
          console.error('AuthCallback: No code verifier found in session storage');
          setError('Missing authentication data (verifier). Please try again.');
          return;
        }
        
        try {
          const success = await authService.handleAuthCallback(code, state);
          
          if (success) {
            updateAuthState(true);
            
            // Redirect to the stored path or home page
            const redirectPath = sessionStorage.getItem('redirectAfterLogin') || '/';
            sessionStorage.removeItem('redirectAfterLogin');
            
            // Redirect with replace to prevent back navigation to callback page
            navigate(redirectPath, { replace: true });
          } else {
            console.error('AuthCallback: Token exchange failed');
            setError('Authentication failed. The server could not complete the login process.');
          }
        } catch (err) {
          console.error('AuthCallback: Error in token exchange:', err);
          setError('Error during authentication. Please try again.');
        }
      } finally {
        setIsProcessing(false);
      }
    };

    // Run the authentication process
    processAuthentication();
  }, [navigate, updateAuthState]); // Intentionally removed location and isProcessing dependencies

  if (error) {
    return (
      <div className="flex justify-content-center align-items-center h-screen surface-ground">
        <div className="card shadow-4 p-5 border-round-xl w-full md:w-8 lg:w-6">
          <div className="text-center mb-5">
            <img 
              src="/images/donkeywork.png" 
              alt="DonkeyWork Logo" 
              className="mb-4" 
              style={{ width: '160px', height: 'auto' }} 
            />
          </div>
          
          <div className="text-center mb-4">
            <i className="pi pi-exclamation-circle text-5xl text-red-500 mb-3"></i>
            <h2 className="text-2xl font-medium mb-2 text-primary">Authentication Error</h2>
            <p className="text-red-500 mb-4">{error}</p>
          </div>
          
          <div className="flex flex-column gap-3">
            <Button 
              label="Try Again"
              icon="pi pi-refresh"
              className="w-full"
              onClick={() => authService.login()}
            />
            
            <Button 
              label="Return to Home"
              icon="pi pi-home"
              severity="secondary"
              outlined
              className="w-full" 
              onClick={() => window.location.href = '/'}
            />
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="flex justify-content-center align-items-center h-screen surface-ground">
      <div className="card shadow-4 p-5 border-round-xl text-center">
        <div className="mb-4">
          <img 
            src="/images/donkeywork.png" 
            alt="DonkeyWork Logo" 
            className="mb-3" 
            style={{ width: '140px', height: 'auto' }} 
          />
        </div>
        <i className="pi pi-spin pi-spinner text-primary mb-3" style={{ fontSize: '2.5rem' }}></i>
        <h3 className="text-xl font-medium mb-2">Processing Authentication</h3>
        <p className="text-secondary mb-2">Please wait while we complete your login...</p>
        <div className="mt-3">
          <ProgressBar mode="indeterminate" style={{ height: '6px' }} />
        </div>
      </div>
    </div>
  );
};

export default AuthCallback;