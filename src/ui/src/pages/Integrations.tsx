import React from 'react';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { Badge } from 'primereact/badge';

const Integrations: React.FC = () => {
  // Sample integrations data
  const integrations = [
    // Connected integrations (shown first)
    { id: 1, name: 'Google Drive', description: 'Cloud storage', icon: 'pi pi-google', connected: true },
    { id: 2, name: 'Microsoft 365', description: 'Productivity suite including Office apps', icon: 'pi pi-microsoft', connected: true },
    { id: 3, name: 'Discord', description: 'Voice, video and text communication platform', icon: 'pi pi-discord', connected: true },
    
    // Available integrations
    { id: 4, name: 'Google Calendar', description: 'Calendar and scheduling', icon: 'pi pi-google', connected: false },
    { id: 5, name: 'Microsoft Teams', description: 'Team collaboration platform', icon: 'pi pi-microsoft', connected: false },
    { id: 6, name: 'Discord Channels', description: 'Manage selected Discord channels', icon: 'pi pi-discord', connected: false },
  ];

  const integrationTemplate = (integration: any) => {
    return (
      <div className="col-12 md:col-6 lg:col-4 xl:col-3 p-2">
        <Card 
          className="h-full integration-card"
          title={integration.name}
          subTitle={integration.description}
          pt={{ 
            root: { className: 'h-full flex flex-column' },
            content: { className: 'flex-1' },
            title: { className: 'text-primary' }
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
                  <Button label="Configure" icon="pi pi-cog" className="p-button-text p-button-secondary" />
                </>
              ) : (
                <Button label="Connect" icon="pi pi-link" className="p-button-primary ml-auto" />
              )}
            </div>
          }
        >
          <div className="flex flex-column h-full">
            <p className="m-0">
              {integration.connected 
                ? `This integration is active and configured.` 
                : `Connect to enable this integration.`}
            </p>
          </div>
        </Card>
      </div>
    );
  };

  return (
    <div className="grid">
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
        <div className="grid">
          {integrations.map(integration => 
            integrationTemplate(integration)
          )}
        </div>
      </div>
    </div>
  );
};

export default Integrations;