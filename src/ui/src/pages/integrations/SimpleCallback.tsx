import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Card } from 'primereact/card';
import { ProgressBar } from 'primereact/progressbar';
import { Button } from 'primereact/button';
import { ToolProviderType } from '../../models/api/provider/ToolProviderType';
import { ProviderCallbackResponseModel } from '../../models/api/provider/ProviderCallbackResponseModel';
import { integrationsService } from '../../services/api';

// Logo component for consistent branding
const Logo = ({ size = 140 }: { size?: number }) => (
  <img 
    src="/images/donkeywork.png" 
    alt="DonkeyWork Logo" 
    className="mb-3" 
    style={{ width: `${size}px`, height: 'auto' }} 
  />
);

// Provider Logo component
const ProviderLogo = ({ providerType, size = 50 }: { providerType: string, size?: number }) => {
  let icon = '';
  let bgColor = '';
  let color = '';
  let alt = '';
  
  switch (providerType.toLowerCase()) {
    case 'microsoft':
      icon = 'pi pi-microsoft';
      bgColor = '#0078d4';
      color = '#ffffff';
      alt = 'Microsoft Logo';
      break;
    case 'google':
      icon = 'pi pi-google';
      bgColor = '#4285F4';
      color = '#ffffff';
      alt = 'Google Logo';
      break;
    case 'discord':
      icon = 'pi pi-discord';
      bgColor = '#5865F2';
      color = '#ffffff';
      alt = 'Discord Logo';
      break;
    default:
      icon = 'pi pi-link';
      bgColor = '#6366F1';
      color = '#ffffff';
      alt = 'Provider Logo';
  }
  
  return (
    <div className="flex justify-content-center mt-3 mb-2">
      <div 
        className="flex align-items-center justify-content-center border-circle"
        style={{ 
          width: `${size}px`, 
          height: `${size}px`, 
          backgroundColor: bgColor,
          color: color,
          fontSize: `${size/2}px`,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center'
        }} 
        aria-label={alt}
      >
        <i className={icon}></i>
      </div>
    </div>
  );
};

const SimpleCallback: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const [status, setStatus] = useState<'processing' | 'success' | 'error'>('processing');
  const [message, setMessage] = useState<string>('Processing your integration...');
  const [provider, setProvider] = useState<string>('');
  const [error, setError] = useState<string | null>(null);
  const callbackProcessed = React.useRef(false);

  useEffect(() => {
    const savedTheme = localStorage.getItem('theme') || 'light';
    const theme = savedTheme === 'dark' ? 'lara-dark-purple' : 'lara-light-purple';
    
    const linkId = 'theme-css';
    const themeLink = document.getElementById(linkId) as HTMLLinkElement || document.createElement('link');
    
    if (!themeLink.id) {
      themeLink.id = linkId;
      themeLink.rel = 'stylesheet';
      document.head.appendChild(themeLink);
    }
    
    themeLink.href = `https://cdn.jsdelivr.net/npm/primereact@10.9.3/resources/themes/${theme}/theme.css`;
  }, []);

  useEffect(() => {
    const processOAuthCallback = async () => {
      if (callbackProcessed.current) {
        return;
      }
      
      const pathParts = location.pathname.split('/');
      const providerFromPath = pathParts[pathParts.length - 1];
      setProvider(providerFromPath);
      
      const query = new URLSearchParams(location.search);
      const code = query.get('code');
      const errorFromQuery = query.get('error');
      const errorDesc = query.get('error_description');
      
      if (errorFromQuery) {
        setStatus('error');
        setMessage(`Authorization failed: ${errorDesc || errorFromQuery}`);
        setError(`${errorDesc || errorFromQuery}`);
        setTimeout(() => navigate('/integrations'), 3000);
        return;
      }
      
      if (!code) {
        setStatus('error');
        setMessage('No authorization code was provided');
        setError('No authorization code was provided');
        setTimeout(() => navigate('/integrations'), 3000);
        return;
      }
      
      let providerType: ToolProviderType;
      switch (providerFromPath.toLowerCase()) {
        case 'microsoft':
          providerType = ToolProviderType.Microsoft;
          break;
        case 'google':
          providerType = ToolProviderType.Google;
          break;
        case 'discord':
          providerType = ToolProviderType.Discord;
          break;
        case 'swarmpit':
          providerType = ToolProviderType.Swarmpit;
          break;
        case 'serp':
          providerType = ToolProviderType.Serp;
          break;
        default:
          setStatus('error');
          setMessage(`Unknown provider: ${providerFromPath}`);
          setError(`Unknown provider: ${providerFromPath}`);
          setTimeout(() => navigate('/integrations'), 3000);
          return;
      }
      
      // Create the full redirect URL that was originally used
      // Important: This needs to match what we sent in the original request 
      // when starting the OAuth flow in the Integrations.tsx component
      const redirectUrl = `${window.location.origin}/integrations/callback/${providerFromPath.toLowerCase()}`;
      const callbackUrl = `/api/Integrations/callback/${providerType}?code=${encodeURIComponent(code)}&redirectUrl=${encodeURIComponent(redirectUrl)}`;
      
      setMessage(`Connecting to ${providerFromPath}...`);
      callbackProcessed.current = true;
      
      try {
        // Use the new integrations service directly
        const fullUrl = new URL(callbackUrl, window.location.origin);
        const params = fullUrl.searchParams;
        const code = params.get('code') || '';
        const redirectUrlParam = params.get('redirectUrl') || '';
        
        // Make the API call through the service layer
        const responseData = await integrationsService.handleCallback(providerType, code, redirectUrlParam);
        setStatus('success');
        setMessage(`Successfully connected to ${providerFromPath}`);
        
        setTimeout(() => {
          navigate('/integrations', { state: { success: true, provider: providerType } });
        }, 1500);
      } catch (err) {
        setStatus('error');
        setMessage(`Error connecting to ${providerFromPath}`);
        setError(err instanceof Error ? err.message : String(err));
        setTimeout(() => navigate('/integrations'), 3000);
      }
    };
    
    processOAuthCallback();
  }, [navigate]);
  
  const goToIntegrations = () => {
    navigate('/integrations');
  };
  
  return (
    <div className="flex justify-content-center align-items-center h-screen surface-ground">
      <Card className="shadow-4 p-5 border-round-xl w-full md:w-8 lg:w-6" style={{ maxWidth: '750px' }}>
        <div className="text-center mb-3">
          <Logo size={140} />
        </div>
        
        <div className="text-center">
          {provider && <ProviderLogo providerType={provider} size={60} />}
        </div>
        
        {status === 'processing' ? (
          <div className="text-center mb-4">
            <h2 className="text-2xl font-medium mb-2 text-primary">Processing Integration</h2>
            <p className="text-secondary mb-4">{message}</p>
            <div className="mt-3">
              <ProgressBar mode="indeterminate" style={{ height: '6px' }} />
            </div>
          </div>
        ) : status === 'success' ? (
          <div className="text-center mb-4">
            <h2 className="text-2xl font-medium mb-2 text-primary">Connection Successful</h2>
            <p className="text-secondary mb-4">{message}</p>
            <div className="flex align-items-center justify-content-center mt-3">
              <i className="pi pi-check-circle text-green-500" style={{ fontSize: '2rem' }}></i>
            </div>
            <p className="text-sm text-secondary mt-3">Redirecting to integrations page...</p>
          </div>
        ) : (
          <div className="text-center mb-4">
            <h2 className="text-2xl font-medium mb-2 text-primary">Connection Error</h2>
            <p className="text-secondary mb-4">{message}</p>
            {error && (
              <div className="p-3 bg-red-50 border-round text-red-700 mb-3">
                <i className="pi pi-exclamation-triangle mr-2"></i>
                {error}
              </div>
            )}
            <p className="text-sm text-secondary mt-3">Redirecting to integrations page...</p>
          </div>
        )}
        
        <div className="flex justify-content-center gap-3 mt-4">
          <Button 
            label="Return to Integrations" 
            icon="pi pi-arrow-left"
            className="p-button p-component p-button-primary"
            onClick={goToIntegrations}
          />
        </div>
      </Card>
    </div>
  );
};

export default SimpleCallback;