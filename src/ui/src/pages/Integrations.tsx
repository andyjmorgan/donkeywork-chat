import React, { useEffect, useState, useRef } from 'react';
import { useLocation } from 'react-router-dom';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { Toast } from 'primereact/toast';
import { ConfirmDialog, confirmDialog } from 'primereact/confirmdialog';
import { Accordion, AccordionTab } from 'primereact/accordion';
import { Chip } from 'primereact/chip';
import { Divider } from 'primereact/divider';
import { Badge } from 'primereact/badge';
import { ToolProviderType } from '../models/api/provider/ToolProviderType';
import { ToolProviderAuthorizationType } from '../models/api/provider/ToolProviderAuthorizationType';
import { ToolProviderModel } from '../models/api/provider/ToolProviderModel';
import { ToolProviderApplicationType } from '../models/api/provider/ToolProviderApplicationType';
import { integrationsService } from '../services/api';
import '../styles/components/Integrations.css';
import GenericProviderDialog from '../components/GenericProviderDialog';

interface IntegrationHighlight {
  type: ToolProviderType;
  timestamp: number;
}

const Integrations: React.FC = () => {
  const [providers, setProviders] = useState<ToolProviderModel[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [highlightedProvider, setHighlightedProvider] = useState<IntegrationHighlight | null>(null);
  const [selectedGenericProvider, setSelectedGenericProvider] = useState<ToolProviderType | null>(null);
  const [selectedGenericProviderImage, setSelectedGenericProviderImage] = useState<string>('');
  const [selectedGenericProviderName, setSelectedGenericProviderName] = useState<string>('');
  const [configDialogVisible, setConfigDialogVisible] = useState(false);
  const toast = useRef<Toast>(null);
  const location = useLocation();
  
  useEffect(() => {
    loadData();
    
    if (location.state && location.state.success && location.state.provider) {
      setHighlightedProvider({
        type: location.state.provider,
        timestamp: Date.now()
      });
      
      setTimeout(() => {
        setHighlightedProvider(null);
      }, 1000);
    }
  }, [location.state]);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await integrationsService.getToolProviders();
      setProviders(data.toolProviders);
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

  const handleConnect = async (provider: ToolProviderModel) => {
    try {
      const redirectUrl = `${window.location.origin}/integrations/callback/${provider.providerType.toLowerCase()}`;
      const response = await integrationsService.getProviderAuthUrl(provider.providerType, redirectUrl);
      window.location.href = response.authorizationUrl;
    } catch (error) {
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Connection Error', 
        detail: `Failed to connect to ${provider.name}` 
      });
    }
  };

  const handleDisconnect = (provider: ToolProviderModel) => {
    confirmDialog({
      message: `Are you sure you want to disconnect ${provider.name}?`,
      header: 'Confirm Disconnection',
      icon: 'pi pi-exclamation-triangle',
      acceptClassName: 'p-button-danger',
      accept: async () => {
        try {
          await integrationsService.deleteProviderIntegration(provider.providerType);
          toast.current?.show({ 
            severity: 'success', 
            summary: 'Success', 
            detail: `Disconnected from ${provider.name}` 
          });
          loadData();
        } catch (error) {
          toast.current?.show({ 
            severity: 'error', 
            summary: 'Error', 
            detail: `Failed to disconnect from ${provider.name}` 
          });
        }
      }
    });
  };

  const handleConfigureGenericProvider = (provider: ToolProviderModel) => {
    setSelectedGenericProvider(provider.providerType);
    setSelectedGenericProviderImage(provider.icon || '/images/providers/generic.png');
    setSelectedGenericProviderName(provider.name);
    setConfigDialogVisible(true);
  };

  const handleSaveGenericProvider = () => {
    toast.current?.show({
      severity: 'success',
      summary: 'Success',
      detail: `Provider configuration saved successfully`
    });
    loadData();
  };

  const getProviderIcon = (provider: ToolProviderModel) => {
    // Check if provider has a built-in PrimeIcon
    let piIconClass = '';
    switch (provider.providerType) {
      case ToolProviderType.Microsoft: 
        piIconClass = 'pi pi-microsoft';
        break;
      case ToolProviderType.Google: 
        piIconClass = 'pi pi-google';
        break;
      case ToolProviderType.Discord: 
        piIconClass = 'pi pi-discord';
        break;
    }

    // If we have a built-in PrimeIcon, use it
    if (piIconClass) {
      return <i className={`${piIconClass} text-primary`} style={{ fontSize: '32px' }}></i>;
    }
    
    // If the provider icon starts with pi-, it's a PrimeIcon
    if (provider.icon && (provider.icon.startsWith('pi-') || provider.icon.startsWith('pi pi-'))) {
      const iconClass = provider.icon.startsWith('pi-') ? `pi ${provider.icon}` : provider.icon;
      return <i className={`${iconClass} text-primary`} style={{ fontSize: '32px' }}></i>;
    }
    
    // Otherwise, use the icon as an image URL or fallback to generic
    return (
      <img 
        src={provider.icon || '/images/providers/generic.png'} 
        alt={provider.name} 
        style={{ 
          width: '32px', 
          height: '32px', 
          objectFit: 'contain',
          borderRadius: '4px'
        }}
      />
    );
  };

  const renderApplicationIcon = (iconStr: string | undefined) => {
    if (!iconStr) return undefined;
    
    // Check if it's a PrimeIcons icon (starts with 'pi-')
    if (iconStr.startsWith('pi-')) {
      return `pi ${iconStr}`;
    } 
    // If it's already a full PI class
    else if (iconStr.startsWith('pi pi-')) {
      return iconStr;
    }
    // Otherwise it's an image URL
    else {
      // Return null to skip the icon property and use the template instead
      return null;
    }
  };

  const renderApplications = (provider: ToolProviderModel) => {
    if (!provider.applications || Object.keys(provider.applications).length === 0) {
      return null;
    }
    
    const appCount = Object.keys(provider.applications).length;
    
    return (
      <div className="application-section mt-2">
        <Accordion className="apps-accordion">
          <AccordionTab header={
            <div className="flex align-items-center">
              <i className="pi pi-cube mr-2 text-primary"></i>
              <span className="font-medium">Applications</span>
              <Badge value={appCount} className="ml-2" severity="info" />
            </div>
          }>
            <div className="application-chips pt-2">
              <div className="flex flex-wrap">
                {Object.values(provider.applications).map(app => {
                  const isPiIcon = app.icon && (app.icon.startsWith('pi-') || app.icon.startsWith('pi pi-'));
                  let iconElement;
                  
                  if (isPiIcon) {
                    const iconClass = app.icon.startsWith('pi-') ? `pi ${app.icon}` : app.icon;
                    iconElement = <i className={`app-icon ${iconClass}`}></i>;
                  } else if (app.icon) {
                    iconElement = <img src={app.icon} alt="" className="app-icon" />;
                  }
                  
                  return (
                    <div 
                      key={app.application} 
                      className="app-badge"
                      title={app.description}
                    >
                      {iconElement}
                      <span>{app.name}</span>
                    </div>
                  );
                })}
              </div>
            </div>
          </AccordionTab>
        </Accordion>
      </div>
    );
  };

  const integrationTemplate = (provider: ToolProviderModel) => {
    const isOAuthProvider = provider.authorizationType === ToolProviderAuthorizationType.OAuth;
    const isStaticProvider = provider.authorizationType === ToolProviderAuthorizationType.Static;
    const isHighlighted = highlightedProvider?.type === provider.providerType;

    return (
      <div className="col-12 sm:col-6 md:col-4 xl:col-3 p-2" key={provider.providerType}>
        <Card 
          className={`integration-card ${isHighlighted ? 'highlight' : ''}`}
          pt={{ 
            root: { className: 'flex flex-column' },
            content: { className: 'flex-1 p-2 pb-0' },
            footer: { className: 'p-2' },
            header: { className: 'p-0' }
          }}
          header={
            <div className="card-header-custom p-3">
              <div className="flex align-items-center">
                <div className="card-icon-container mr-2">
                  {getProviderIcon(provider)}
                </div>
                <div className="card-title-container">
                  <h3 className="card-title text-primary m-0">{provider.name}</h3>
                </div>
              </div>
            </div>
          }
          footer={
            <div className="flex justify-content-end align-items-center">
              {provider.isConnected ? (
                <div className="flex gap-2 flex-wrap">
                  {isStaticProvider && (
                    <Button 
                      label="Modify" 
                      className="p-button-primary p-button-sm"
                      onClick={() => handleConfigureGenericProvider(provider)}
                    />
                  )}
                  <Button 
                    label="Disconnect" 
                    icon="pi pi-unlink" 
                    className="p-button-danger p-button-outlined p-button-sm"
                    onClick={() => handleDisconnect(provider)}
                  />
                </div>
              ) : (
                <Button 
                  label={isStaticProvider ? "Configure" : "Connect"} 
                  className="p-button-primary p-button-sm"
                  onClick={() => isStaticProvider ? 
                    handleConfigureGenericProvider(provider) : 
                    handleConnect(provider)
                  }
                />
              )}
            </div>
          }
        >
          <div className="flex flex-column">
            <p className="card-description text-500 mb-2">{provider.description}</p>
            
            {provider.isConnected ? (
              renderApplications(provider)
            ) : (
              <div className="flex flex-column">
                <div className="authentication-type flex align-items-center mb-2">
                  {isOAuthProvider ? (
                    <><i className="pi pi-key mr-2 text-primary-300"></i><span>OAuth Authentication</span></>
                  ) : (
                    <><i className="pi pi-cog mr-2 text-primary-300"></i><span>Configuration Required</span></>
                  )}
                </div>
                {renderApplications(provider)}
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
          <div className="text-center mb-3">
            <h2 className="text-3xl font-bold mb-3 text-primary">Integrations</h2>
            <p className="text-lg">
              Connect with third-party services to extend functionality.
            </p>
          </div>
          
          {loading ? (
            <div className="flex justify-content-center p-5">
              <i className="pi pi-spin pi-spinner" style={{ fontSize: '2rem' }}></i>
            </div>
          ) : (
            <div className="grid">
              {providers.map(provider => 
                integrationTemplate(provider)
              )}
            </div>
          )}
        </Card>
      </div>

      <GenericProviderDialog
        visible={configDialogVisible}
        onHide={() => setConfigDialogVisible(false)}
        providerType={selectedGenericProvider as any}
        providerImage={selectedGenericProviderImage}
        providerName={selectedGenericProviderName}
        onSave={handleSaveGenericProvider}
      />
    </div>
  );
};

export default Integrations;