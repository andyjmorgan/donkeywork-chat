import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
// Import directly from component files instead of through barrel files
import AppLayout from './components/layout/AppLayout';
import ProtectedRoute from './components/auth/ProtectedRoute';
import Home from './pages/Home';
import Chat from './pages/chat/Chat';
import Conversations from './pages/chat/Conversations';
import Prompts from './pages/Prompts';
import ActionPrompts from './pages/ActionPrompts';
import Actions from './pages/Actions';
import ActionLogs from './pages/ActionLogs';
import Agents from './pages/Agents';
import AgentEditor from './pages/agents/AgentEditor';
import Integrations from './pages/Integrations';
import ApiKeys from './pages/ApiKeys';
import IntegrationCallback from './pages/integrations/Callback';
import SimpleCallback from './pages/integrations/SimpleCallback';
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
import './styles/components/Card.css';                           // Card component styling

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
          <Route path="/" element={
            <ProtectedRoute>
              <AppLayout>
                <Home />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/home" element={<Navigate to="/" replace />} />
          
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
          
          {/* Protected callback route (requires login) */}
          <Route path="/integrations/protected-callback/:provider" element={
            <ProtectedRoute>
              <AppLayout>
                <IntegrationCallback />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          {/* Unprotected callback route for direct OAuth returns */}
          <Route path="/integrations/callback/:provider" element={
            <SimpleCallback />
          } />
          
          <Route path="/prompts" element={
            <ProtectedRoute>
              <AppLayout>
                <Prompts />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/actionprompts" element={
            <ProtectedRoute>
              <AppLayout>
                <ActionPrompts />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/actions" element={
            <ProtectedRoute>
              <AppLayout>
                <Actions />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/actionlogs" element={
            <ProtectedRoute>
              <AppLayout>
                <ActionLogs />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/agents" element={
            <ProtectedRoute>
              <AppLayout>
                <Agents />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/agents/new" element={
            <ProtectedRoute>
              <AppLayout>
                <AgentEditor />
              </AppLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/agents/edit/:agentId" element={
            <ProtectedRoute>
              <AppLayout>
                <AgentEditor />
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
          
          <Route path="/apikeys" element={
            <ProtectedRoute>
              <AppLayout>
                <ApiKeys />
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
