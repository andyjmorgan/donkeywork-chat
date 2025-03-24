import { ReactNode } from 'react';
import { MenuItem } from 'primereact/menuitem';

/**
 * Props for the AppLayout component
 */
export interface AppLayoutProps {
  children: ReactNode;
}

/**
 * Interface for the theme toggle functionality
 */
export interface ThemeState {
  darkMode: boolean;
  toggleDarkMode: (checked: boolean) => void;
  loadTheme: (theme: string) => void;
}

/**
 * Menu item structures used in the application layout
 */
export interface MenuConfig {
  sidebarItems: MenuItem[];
  userMenuItems: MenuItem[];
}