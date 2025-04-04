import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { Card } from 'primereact/card';
import { ProgressSpinner } from 'primereact/progressspinner';
import { UserProviderType } from '../../models/api/provider/UserProviderType';
import { toCamelCaseKeys } from '../../utils/caseConversion';

const IntegrationCallback: React.FC = () => {
  const { provider } = useParams<{ provider: string }>();
  const location = useLocation();
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);

  // Add a ref to track if the callback has been processed
  const callbackProcessed = React.useRef(false);

  useEffect(() => {
    // Prevent duplicate API calls
    if (callbackProcessed.current) {
      console.log('Callback already processed, skipping duplicate call');
      return;
    }
    
    console.log('IntegrationCallback - useEffect triggered');
    console.log('Provider param:', provider);
    console.log('Location:', location);
    
    const query = new URLSearchParams(location.search);
    console.log('Query params:', Object.fromEntries(query.entries()));
    
    const code = query.get('code');
    const errorFromQuery = query.get('error');
    
    console.log('Authorization code:', code);
    console.log('Error from query:', errorFromQuery);

    if (errorFromQuery) {
      console.log('Error detected in query params');
      setError(`Authorization failed: ${errorFromQuery}`);
      setTimeout(() => navigate('/integrations'), 3000);
      return;
    }

    if (!code) {
      console.log('No authorization code found');
      setError('No authorization code was provided');
      setTimeout(() => navigate('/integrations'), 3000);
      return;
    }

    // Determine the provider type
    let providerType: UserProviderType;
    switch (provider?.toLowerCase()) {
      case 'microsoft':
        providerType = UserProviderType.Microsoft;
        break;
      case 'google':
        providerType = UserProviderType.Google;
        break;
      case 'discord':
        providerType = UserProviderType.Discord;
        break;
      default:
        setError(`Unknown provider: ${provider}`);
        setTimeout(() => navigate('/integrations'), 3000);
        return;
    }

    // Create the full redirect URL that was originally used
    const redirectUrl = window.location.href.split('?')[0];
    console.log('Redirect URL:', redirectUrl);
    
    const callbackUrl = `/api/Provider/callback/${providerType}?code=${encodeURIComponent(code)}&redirectUrl=${encodeURIComponent(redirectUrl)}`;
    console.log('Callback URL:', callbackUrl);

    // Mark as processed to prevent duplicate calls
    callbackProcessed.current = true;
    
    // Make API call to handle the callback
    fetch(callbackUrl, {
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    })
      .then(response => {
        console.log('Callback response:', response);
        if (!response.ok) {
          console.error('Response not OK:', response.status, response.statusText);
          return response.json().then(data => {
            console.error('Error response data:', data);
            throw new Error(data.detail || 'Failed to connect to provider');
          });
        }
        return response.json();
      })
      .then(data => {
        console.log('Callback success data:', data);
        // Apply case conversion to match our interface expectations
        const camelCaseData = toCamelCaseKeys(data);
        console.log('Callback success data (camelCase):', camelCaseData);
        // Redirect back to the integrations page on success
        navigate('/integrations', { state: { success: true, provider: providerType } });
      })
      .catch(err => {
        console.error('Callback error:', err);
        setError(`Error: ${err.message}`);
        setTimeout(() => navigate('/integrations'), 3000);
      });
  // Only depend on navigate and provider, remove location.search to prevent rerunning
  }, [provider, navigate]);

  return (
    <div className="flex justify-content-center align-items-center" style={{ minHeight: '80vh' }}>
      <Card className="w-full md:w-6 lg:w-4">
        <div className="text-center">
          <h2 className="text-2xl font-bold mb-4">Processing {provider} Integration</h2>
          
          {error ? (
            <div className="p-error mb-3">{error}</div>
          ) : (
            <>
              <ProgressSpinner style={{ width: '50px', height: '50px' }} />
              <p className="mt-3">Finalizing your integration. Please wait...</p>
            </>
          )}
          
          {error && <p>Redirecting back to Integrations page...</p>}
        </div>
      </Card>
    </div>
  );
};

export default IntegrationCallback;