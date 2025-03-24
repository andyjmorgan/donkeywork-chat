import {GetUserInformationResponse} from "../../models/api/UserService/GetUserInformationResponse";
import ApiBase from './apiBase';

export class UserService extends ApiBase {
  private static instance: UserService;

  private constructor() {
    super();
  }

  static getInstance(): UserService {
    if (!UserService.instance) {
      UserService.instance = new UserService();
    }
    return UserService.instance;
  }

  // Get current user information from the /me endpoint
  async getUserInfo(): Promise<GetUserInformationResponse | null> {
    try {
      // Use fetch with credentials to ensure cookies are sent
      const response = await fetch(`${this.getApiBaseUrl()}/user`, {
        credentials: 'include',
        headers: {
          'Cache-Control': 'no-cache'
        }
      });

      if (!response.ok) {
        throw new Error(`Failed to get user info: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Failed to get user info:', error);
      return null;
    }
  }
}

// Export a singleton instance as the default export
const userService = UserService.getInstance();
export { userService };
export default UserService.getInstance();