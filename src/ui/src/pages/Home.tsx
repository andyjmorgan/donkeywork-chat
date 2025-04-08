import React from 'react';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { Divider } from 'primereact/divider';
import { useNavigate } from 'react-router-dom';
import '../styles/components/Home.css';

const Home: React.FC = () => {
  const navigate = useNavigate();

  return (
    <div className="home-container">
            {/* Feature sections */}
      <div className="features-container">
        <Card className="about-card">
          <h2 className="section-title">About DonkeyWork.Chat</h2>
          <div className="tech-stack">
            <div className="tech-item">
              <i className="pi pi-server tech-icon backend-icon"></i>
              <span>C# Backend</span>
            </div>
            <div className="tech-item">
              <i className="pi pi-desktop tech-icon frontend-icon"></i>
              <span>React + Vite Frontend</span>
            </div>
            <div className="tech-item">
              <i className="pi pi-github tech-icon"></i>
              <span>Open Source</span>
            </div>
          </div>
          <p>
            DonkeyWork.Chat is an example AI-powered assistant to demonstrate how to leverage attributes and token
            scopes to deliver smart tooling to what the user has given us access to.
            Built with a modern tech stack including C# backend APIs and a React +
            Vite frontend, it combines powerful language models with direct access to your data and services.
          </p>
          <p>
            Created by <a href="https://github.com/andyjmorgan" target="_blank" rel="noopener noreferrer">Andrew
            Morgan</a>, vibe coding with Uncle Claude, this project is available on <a
              href="https://github.com/andyjmorgan/donkeywork-chat"
              target="_blank" rel="noopener noreferrer"
              className="github-link"><i className="pi pi-github"></i> GitHub</a>.
          </p>
        </Card>
        <div className="start-chatting-container">
          <Button
              label="Start Chatting"
              icon="pi pi-comments"
              className="start-button p-button-lg"
              onClick={() => navigate('/chat')}
          />
        </div>

        <h2 className="section-title integrations-title">Rich Third-Party Integrations</h2>

        <div className="integration-grid">
          <Card className="integration-card microsoft-card">
            <div className="integration-header">
              <i className="pi pi-microsoft integration-icon"></i>
              <h3>Microsoft 365</h3>
            </div>
            <Divider/>
            <ul className="integration-features">
              <li><i className="pi pi-envelope"></i> Access and send emails through Outlook</li>
              <li><i className="pi pi-file"></i> Search and manage OneDrive files</li>
              <li><i className="pi pi-check-square"></i> Create and update Microsoft To Do tasks</li>
              <li><i className="pi pi-calendar"></i> Manage your calendar events</li>
            </ul>
          </Card>

          <Card className="integration-card google-card">
            <div className="integration-header">
              <i className="pi pi-google integration-icon"></i>
              <h3>Google Workspace</h3>
            </div>
            <Divider/>
            <ul className="integration-features">
              <li><i className="pi pi-envelope"></i> Send and search Gmail messages</li>
              <li><i className="pi pi-file"></i> Access and manage Google Drive files</li>
              <li><i className="pi pi-calendar"></i> Interact with Google Calendar</li>
              <li><i className="pi pi-pencil"></i> Create and edit Google Docs</li>
            </ul>
          </Card>

          <Card className="integration-card discord-card">
            <div className="integration-header">
              <i className="pi pi-discord integration-icon"></i>
              <h3>Discord</h3>
            </div>
            <Divider/>
            <ul className="integration-features">
              <li><i className="pi pi-users"></i> List and access your Discord servers</li>
              <li><i className="pi pi-comment"></i> View channel messages and history</li>
              <li><i className="pi pi-bell"></i> Get notified of important messages</li>
              <li><i className="pi pi-send"></i> Interact with Discord communities</li>
            </ul>
          </Card>
        </div>

        <Card className="features-card">
          <h2 className="section-title">Advanced Features</h2>
          <div className="advanced-features-grid">
            <div className="feature-item">
              <div className="feature-icon-container">
                <i className="pi pi-database feature-icon"></i>
              </div>
              <h3>Conversation Storage</h3>
              <p>
                All your chat conversations are securely stored for future reference. Easily browse, search, and
                continue past conversations with our persistent storage system.
              </p>
            </div>

            <div className="feature-item">
              <div className="feature-icon-container">
                <i className="pi pi-wrench feature-icon"></i>
              </div>
              <h3>Tooling Inspection</h3>
              <p>
                Inspect and debug AI tool calls in real time. See exactly what tools are being used, view API requests,
                examine parameters, and analyze responses for complete transparency.
              </p>
            </div>

            <div className="feature-item">
              <div className="feature-icon-container">
                <i className="pi pi-book feature-icon"></i>
              </div>
              <h3>Custom Prompts Library</h3>
              <p>
                Create, store, and reuse custom prompts to guide AI interactions. Build a personalized library of
                effective
                prompts for specific tasks and workflows to enhance productivity.
              </p>
            </div>
          </div>
        </Card>

        <Card className="providers-card">
          <h2 className="section-title">AI Model Providers</h2>
          <div className="providers-content">
            <div className="providers-grid">
              <div className="provider-item">
                <img
                    src="/images/providers/openai.png"
                    alt="OpenAI Logo"
                    className="provider-logo"
                />
                <h3>OpenAI</h3>
                <p>
                  Access powerful language models like GPT-4o and GPT-3.5 Turbo. Take advantage of OpenAI's
                  state-of-the-art capabilities for text generation, reasoning, and problem-solving.
                </p>
              </div>

              <div className="provider-item">
                <img
                    src="/images/providers/anthropic.png"
                    alt="Anthropic Logo"
                    className="provider-logo"
                />
                <h3>Anthropic</h3>
                <p>
                  Leverage Anthropic's Claude models for helpful, harmless, and honest AI assistance.
                  Experience Claude's nuanced understanding, thoughtful responses, and extensive context windows.
                </p>
              </div>
            </div>

            <div className="chat-illustration">
              <div className="chat-bubble user-bubble">Find recent emails from Sarah with attachments</div>
              <div className="chat-bubble assistant-bubble">
                I found 3 recent emails from Sarah with attachments:
                <ul>
                  <li>Project Update (yesterday)</li>
                  <li>Q2 Report Draft (2 days ago)</li>
                  <li>Meeting Notes (last week)</li>
                </ul>
                Would you like me to retrieve any of these?
              </div>
            </div>
          </div>
        </Card>

        <div className="examples-container">
          <h2 className="section-title">Example Use Cases</h2>
          <div className="example-queries-grid">
            <div className="example-chip">"Find emails from John about the quarterly report"</div>
            <div className="example-chip">"List my Discord servers and channels"</div>
            <div className="example-chip">"Summarize the last 5 files I accessed in OneDrive"</div>
            <div className="example-chip">"Create a new task in my to-do list"</div>
            <div className="example-chip">"Search through my emails from last week"</div>
            <div className="example-chip">"Find documents containing budget information"</div>
            <div className="example-chip">"Show me message history from Discord"</div>
            <div className="example-chip">"Compare files from my Google Drive"</div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Home;
