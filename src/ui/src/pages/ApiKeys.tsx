import React from 'react';
import { ApiKeyList } from '../features/apikeys';

const ApiKeys: React.FC = () => {
  return (
    <div className="h-full">
      <ApiKeyList />
    </div>
  );
};

export default ApiKeys;