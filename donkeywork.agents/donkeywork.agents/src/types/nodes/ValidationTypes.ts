/**
 * Types for node validation results and errors
 */

export type ValidationError = {
  message: string;
  severity: 'error' | 'warning';
};

export type ValidationResult = {
  valid: boolean;
  errors: ValidationError[];
};