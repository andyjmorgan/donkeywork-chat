import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
// Import directly from component files instead of through barrel files
import AppLayout from './components/layout/AppLayout';
import ProtectedRoute from './components/auth/ProtectedRoute';
import Chat from './pages/chat/Chat';
import Conversations from './pages/chat/Conversations';
import Prompts from './pages/Prompts';
import Integrations from './pages/Integrations';
import AuthCallback from './pages/auth/AuthCallback';
import Logout from './pages/auth/Logout';
import Login from './pages/auth/Login';
import Profile from './pages/user/Profile';

// Import PrimeReact core styles (theme is loaded dynamically in AppLayout)
import 'primereact/resources/primereact.min.css';                  // core css
import 'primeicons/primeicons.css';                                // icons
import 'primeflex/primeflex.css';                                  // utility CSS classes
import './App.css';
import './styles/components/MenuScale.css';                      // Custom menu scaling
import './styles/components/Profile.css';                        // Profile page styling

const App: React.FC = () => {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* Auth routes */}
          <Route path="/callback" element={<AuthCallback />} />
          <Route path="/logout" element={<Logout />} />
          <Route path="/login" element={<Login />} />
          
          {/* Protected routes with layout */}
          <Route path="/home" element={<Navigate to="/" replace />} />
          
          <Route path="/" element={<Navigate to="/chat" replace />} />
          
          <Route path="/chat" element={
            <ProtectedRoute>
              <AppLayout>
                <Chat />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          {/* Keep the path parameter version for backward compatibility */}
          <Route path="/chat/:conversationId" element={
            <ProtectedRoute>
              <AppLayout>
                <Chat />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/conversations" element={
            <ProtectedRoute>
              <AppLayout>
                <Conversations />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/integrations" element={
            <ProtectedRoute>
              <AppLayout>
                <Integrations />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/prompts" element={
            <ProtectedRoute>
              <AppLayout>
                <Prompts />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/profile" element={
            <ProtectedRoute>
              <AppLayout>
                <Profile />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          {/* Redirect any unmatched route to home */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
};

export default App;
