import { GetUserInformationResponse } from "./UserService/GetUserInformationResponse";

/**
 * Result of an authentication check
 */
export interface AuthCheckResult {
  /**
   * Whether the user is authenticated
   */
  isAuthenticated: boolean;
  
  /**
   * User data if authenticated, null otherwise
   */
  userData: GetUserInformationResponse | null;
}