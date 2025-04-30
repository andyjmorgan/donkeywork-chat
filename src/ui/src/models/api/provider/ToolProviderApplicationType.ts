export enum ToolProviderApplicationType {
  GoogleIdentity = 'GoogleIdentity',
  GoogleDrive = 'GoogleDrive',
  GoogleMail = 'GoogleMail',
  GoogleCalendar = 'GoogleCalendar',
  MicrosoftIdentity = 'MicrosoftIdentity',
  MicrosoftOutlook = 'MicrosoftOutlook',
  MicrosoftOneDrive = 'MicrosoftOneDrive',
  MicrosoftTodo = 'MicrosoftTodo',
  Discord = 'Discord',
  Swarmpit = 'Swarmpit',
  Serp = 'Serp',
  Delay = 'Delay',
  DateTime = 'DateTime'
}

export interface ToolProviderApplicationDefinition {
  provider: ToolProviderType;
  application: ToolProviderApplicationType;
  name: string;
  description: string;
  icon: string;
}