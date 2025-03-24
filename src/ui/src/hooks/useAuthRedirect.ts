import { useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

// Hook for handling authentication-based redirects
function useAuthRedirect() {
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    if (!isAuthenticated) {
      // Store the current path for redirect after login
      sessionStorage.setItem('redirectAfterLogin', location.pathname);
      // Redirect to login
      navigate('/login');
    }
  }, [isAuthenticated, location.pathname, navigate]);

  return isAuthenticated;
}

export default useAuthRedirect;