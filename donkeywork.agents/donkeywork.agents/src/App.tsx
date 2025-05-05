import { useState, useCallback } from 'react';
import './App.css';
import { AgentBuilder } from './components/agent';

function App() {
  const [savedAgentJson, setSavedAgentJson] = useState<string | null>(null);

  const handleSaveAgent = useCallback((agentJson: string) => {
    setSavedAgentJson(agentJson);
    console.log('Agent saved:', agentJson);
    // Here you could send the agent to a server, store it in localStorage, etc.
  }, []);

  return (
    <div className="app-container">
      <AgentBuilder 
        onSave={handleSaveAgent}
      />

      {/* This is just to demonstrate the saved data, you can remove or adapt this */}
      {savedAgentJson && (
        <div className="saved-agent-info">
          <h3>Last Saved Agent:</h3>
          <pre>{savedAgentJson}</pre>
        </div>
      )}
    </div>
  );
}

export default App