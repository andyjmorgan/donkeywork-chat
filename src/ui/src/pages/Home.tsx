import React from 'react';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { useAuth } from '../context/AuthContext';

const Home: React.FC = () => {
  const { user } = useAuth();

  return (
    <div className="grid">
      <div className="col-12">
        <Card className="mb-5">
          <div className="text-center mb-5">
            <h2 className="text-3xl font-bold mb-3 text-primary">Welcome to DonkeyWork</h2>
            <p className="text-lg">
              Hello, {user?.given_name || 'User'}! This is your dashboard.
            </p>
          </div>
        </Card>
      </div>

      <div className="col-12 md:col-6 lg:col-3">
        <Card
          title="Projects"
          subTitle="Your active projects"
          pt={{ 
            root: { className: 'h-full' },
            title: { className: 'text-primary' }
          }}
          footer={<Button label="View All" icon="pi pi-arrow-right" className="p-button-text" />}
        >
          <p className="m-0">
            You have 5 active projects.
          </p>
        </Card>
      </div>

      <div className="col-12 md:col-6 lg:col-3">
        <Card
          title="Tasks"
          subTitle="Your pending tasks"
          pt={{ 
            root: { className: 'h-full' },
            title: { className: 'text-purple-400' }
          }}
          footer={<Button label="View All" icon="pi pi-arrow-right" className="p-button-text p-button-secondary" />}
        >
          <p className="m-0">
            You have 12 pending tasks.
          </p>
        </Card>
      </div>

      <div className="col-12 md:col-6 lg:col-3">
        <Card
          title="Team"
          subTitle="Your team members"
          pt={{ 
            root: { className: 'h-full' },
            title: { className: 'text-indigo-400' }
          }}
          footer={<Button label="View All" icon="pi pi-arrow-right" className="p-button-text p-button-help" />}
        >
          <p className="m-0">
            8 team members in your groups.
          </p>
        </Card>
      </div>

      <div className="col-12 md:col-6 lg:col-3">
        <Card
          title="Reports"
          subTitle="Weekly reports"
          pt={{ 
            root: { className: 'h-full' },
            title: { className: 'text-teal-400' }
          }}
          footer={<Button label="View All" icon="pi pi-arrow-right" className="p-button-text p-button-info" />}
        >
          <p className="m-0">
            3 new reports available.
          </p>
        </Card>
      </div>
    </div>
  );
};

export default Home;