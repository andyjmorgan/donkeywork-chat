import React from 'react';
import { ConversationList } from '../../features/chat';

const Conversations: React.FC = () => {
  return (
    <div className="h-full">
      <ConversationList />
    </div>
  );
};

export default Conversations;