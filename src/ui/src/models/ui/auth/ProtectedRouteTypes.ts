import { ReactNode } from 'react';

/**
 * Props for the ProtectedRoute component
 */
export interface ProtectedRouteProps {
  children: ReactNode;
}

/**
 * Props for the Logo component
 */
export interface LogoProps {
  size?: number;
}

/**
 * Props for the LoadingState component
 */
export interface LoadingStateProps {
  title: string;
  message: string;
}