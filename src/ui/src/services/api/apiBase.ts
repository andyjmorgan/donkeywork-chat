/**
 * Base API service that provides the API URL configuration
 * Uses the Vite proxy configuration to forward requests to the backend
 */
export class ApiBase {
  /**
   * Returns the base API URL
   * When using the Vite proxy, we use a relative URL which gets proxied
   */
  protected getApiBaseUrl(): string {
    return '/api';
  }
}

export default ApiBase;