import React, { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { Badge } from 'primereact/badge';
import { Toast } from 'primereact/toast';
import { ConfirmDialog, confirmDialog } from 'primereact/confirmdialog';
import { Accordion, AccordionTab } from 'primereact/accordion';
import { Chip } from 'primereact/chip';
import { Divider } from 'primereact/divider';
import { UserProviderType } from '../models/api/provider/UserProviderType';
import { UserProviderResponseModel } from '../models/api/provider/UserProviderResponseModel';
import { providerService } from '../services/api';
import { useRef } from 'react';
import '../styles/components/Integrations.css';

interface Integration {
  type: UserProviderType;
  name: string;
  description: string;
  icon: string;
  connected: boolean;
  scopes?: string[];
}

const Integrations: React.FC = () => {
  const [userProviders, setUserProviders] = useState<UserProviderResponseModel | null>(null);
  const [integrations, setIntegrations] = useState<Integration[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [highlightedProvider, setHighlightedProvider] = useState<UserProviderType | null>(null);
  const toast = useRef<Toast>(null);
  const location = useLocation();
  
  const providerDetails: Record<UserProviderType, Omit<Integration, 'connected' | 'scopes'>> = {
    [UserProviderType.Microsoft]: {
      type: UserProviderType.Microsoft,
      name: 'Microsoft 365',
      description: 'Access Microsoft services like Outlook, OneDrive, and more',
      icon: 'pi pi-microsoft'
    },
    [UserProviderType.Google]: {
      type: UserProviderType.Google,
      name: 'Google',
      description: 'Access Google services like Gmail, Drive, and more',
      icon: 'pi pi-google'
    },
    [UserProviderType.Discord]: {
      type: UserProviderType.Discord,
      name: 'Discord',
      description: 'Access Discord channels and messages',
      icon: 'pi pi-discord'
    }
  };

  useEffect(() => {
    loadUserProviders();
    
    if (location.state && location.state.success && location.state.provider) {
      setHighlightedProvider(location.state.provider);
      
      setTimeout(() => {
        setHighlightedProvider(null);
      }, 1000);
    }
  }, [location.state]);

  const loadUserProviders = async () => {
    try {
      setLoading(true);
      const data = await providerService.getUserProviders();
      setUserProviders(data);

      const availableProviders = Object.values(UserProviderType);
      
      const updatedIntegrations = availableProviders.map(providerType => {
        const details = providerDetails[providerType];
        const providerKey = providerType.toString();
        const providerConfig = data.ProviderConfiguration as Record<string, string[]>;
        const isConnected = !!providerConfig && !!providerConfig[providerKey];
        
        let scopes: string[] = [];
        if (providerConfig && providerConfig[providerKey]) {
          scopes = providerConfig[providerKey];
        }

        return {
          ...details,
          connected: isConnected,
          scopes: isConnected ? scopes : undefined
        };
      });

      setIntegrations(updatedIntegrations);
    } catch (error) {
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to load integrations' 
      });
    } finally {
      setLoading(false);
    }
  };

  const handleConnect = async (providerType: UserProviderType) => {
    try {
      const redirectUrl = `${window.location.origin}/integrations/simple-callback/${providerType.toLowerCase()}`;
      const response = await providerService.getProviderAuthUrl(providerType, redirectUrl);
      window.location.href = response.authorizationUrl;
    } catch (error) {
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Connection Error', 
        detail: `Failed to connect to ${providerType}` 
      });
    }
  };

  const handleDisconnect = (providerType: UserProviderType) => {
    confirmDialog({
      message: `Are you sure you want to disconnect ${providerDetails[providerType].name}?`,
      header: 'Confirm Disconnection',
      icon: 'pi pi-exclamation-triangle',
      acceptClassName: 'p-button-danger',
      accept: async () => {
        try {
          await providerService.deleteUserProvider(providerType);
          toast.current?.show({ 
            severity: 'success', 
            summary: 'Success', 
            detail: `Disconnected from ${providerDetails[providerType].name}` 
          });
          loadUserProviders();
        } catch (error) {
          toast.current?.show({ 
            severity: 'error', 
            summary: 'Error', 
            detail: `Failed to disconnect from ${providerDetails[providerType].name}` 
          });
        }
      }
    });
  };

  const integrationTemplate = (integration: Integration) => {
    return (
      <div className="col-12 md:col-6 lg:col-4 p-2" key={integration.type}>
        <Card 
          className={`h-full integration-card ${highlightedProvider === integration.type ? 'highlight' : ''}`}
          title={integration.name}
          subTitle={integration.description}
          pt={{ 
            root: { className: 'h-full flex flex-column' },
            content: { className: 'flex-1 p-3' },
            title: { className: 'text-primary' },
            footer: { className: 'pt-2 pb-3 px-3' },
            header: { className: 'pt-4 pb-2' }
          }}
          header={
            <div className="flex align-items-center justify-content-center p-4">
              <i className={`${integration.icon} text-4xl text-primary`}></i>
            </div>
          }
          footer={
            <div className="flex justify-content-between align-items-center mt-auto">
              {integration.connected ? (
                <>
                  <Badge value="Connected" severity="success" />
                  <Button 
                    label="Disconnect" 
                    icon="pi pi-unlink" 
                    className="p-button-warning"
                    style={{ border: '1px solid var(--red-500)' }}
                    onClick={() => handleDisconnect(integration.type)}
                  />
                </>
              ) : (
                <Button 
                  label="Connect" 
                  icon="pi pi-link" 
                  className="p-button-primary ml-auto"
                  onClick={() => handleConnect(integration.type)}
                  disabled={false}
                />
              )}
            </div>
          }
        >
          <div className="flex flex-column h-full">
            {integration.connected ? (
              <>
                <p className="m-0 mb-3 text-lg">
                  <i className="pi pi-check-circle text-green-500 mr-2"></i>
                  Connected and ready to use
                </p>
                <Divider className="my-3" />
                <Accordion className="integration-accordion" activeIndex={-1}>
                  <AccordionTab header={
                    <div className="flex align-items-center">
                      <i className="pi pi-key mr-2 text-primary"></i>
                      <span className="font-medium">Granted Permissions</span>
                      <span className="ml-2 text-sm text-500">({integration.scopes?.length || 0})</span>
                    </div>
                  }>
                    <div className="pt-2">
                      <div className="flex flex-wrap gap-2">
                        {integration.scopes?.map(scope => (
                          <Chip key={scope} label={scope} className="mb-1" />
                        ))}
                      </div>
                    </div>
                  </AccordionTab>
                </Accordion>
              </>
            ) : (
              <div className="h-full flex flex-column justify-content-between">
                <p className="m-0 mb-3 text-lg">
                  {integration.type === UserProviderType.Microsoft 
                    ? <><i className="pi pi-microsoft mr-2 text-primary-300"></i>Connect to use Microsoft services</>
                    : integration.type === UserProviderType.Google
                      ? <><i className="pi pi-google mr-2 text-primary-300"></i>Connect to use Google services</>
                      : <><i className="pi pi-discord mr-2 text-primary-300"></i>Connect to use Discord services</>
                  }
                </p>
                <p className="text-sm text-500 mt-4">
                  {integration.type === UserProviderType.Microsoft 
                    ? 'Access emails, calendars, files, and more'
                    : integration.type === UserProviderType.Google
                      ? 'Access Gmail, Drive, and other Google services'
                      : 'Access Discord channels, messages, and DMs'
                  }
                </p>
              </div>
            )}
          </div>
        </Card>
      </div>
    );
  };

  return (
    <div className="grid">
      <Toast ref={toast} />
      <ConfirmDialog />

      <div className="col-12">
        <Card className="mb-3">
          <div className="text-center mb-5">
            <h2 className="text-3xl font-bold mb-3 text-primary">Integrations</h2>
            <p className="text-lg">
              Connect with third-party services to extend functionality.
            </p>
          </div>
        </Card>
      </div>
      
      <div className="col-12">
        {loading ? (
          <div className="flex justify-content-center">
            <i className="pi pi-spin pi-spinner" style={{ fontSize: '3rem' }}></i>
          </div>
        ) : (
          <div className="grid">
            {integrations.map(integration => 
              integrationTemplate(integration)
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default Integrations;