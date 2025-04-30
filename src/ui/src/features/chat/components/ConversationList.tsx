import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { ConfirmDialog, confirmDialog } from 'primereact/confirmdialog';
import { Menu } from 'primereact/menu';
import { Tag } from 'primereact/tag';
import { Card } from 'primereact/card';
import { Divider } from 'primereact/divider';
import { Skeleton } from 'primereact/skeleton';
import { formatDistanceToNow, parseISO } from 'date-fns';
import { conversationService } from '../../../services/api/conversationService';
import { GetConversationsItemModel } from '../../../models/api/Conversation/GetConversationsItemModel';
import { ConversationItem } from '../../../models/ui/chat/ConversationListTypes';
import '../../../styles/components/ConversationList.css';

const ConversationList: React.FC = () => {
  const [conversations, setConversations] = useState<ConversationItem[]>([]);
  const [globalFilter, setGlobalFilter] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(true);
  const navigate = useNavigate();

  // Fetch conversations
  useEffect(() => {
    const fetchConversations = async () => {
      setLoading(true);
      try {
        const result = await conversationService.getConversations(0, 1000);
        
        // Convert API model to UI model with additional status field
        // For now, we'll consider all conversations active
        const conversationItems: ConversationItem[] = (result.conversations || result.items || []).map(conv => ({
          ...conv,
          status: 'active' as const,
          timestamp: parseISO(conv.updatedAt),
        }));
        
        setConversations(conversationItems);
      } catch (error) {
        console.error('Error fetching conversations:', error);
        // Show error message if needed
      } finally {
        setLoading(false);
      }
    };

    fetchConversations();
  }, []);

  // Handle conversation click - navigate to chat with conversation ID as query parameter
  const openConversation = (conversation: ConversationItem) => {
    navigate(`/chat?id=${conversation.id}`);
  };

  // Handle delete confirmation
  const confirmDelete = (conversation: ConversationItem) => {
    // Truncate title for display in confirmation dialog if it's too long
    const truncatedTitle = truncateText(conversation.title, 50);
    
    confirmDialog({
      message: `Are you sure you want to delete "${truncatedTitle}"?`,
      header: 'Delete Confirmation',
      icon: 'pi pi-exclamation-triangle',
      acceptClassName: 'p-button-danger',
      accept: () => deleteConversation(conversation)
    });
  };

  // Delete conversation
  const deleteConversation = async (conversation: ConversationItem) => {
    try {
      await conversationService.deleteConversation(conversation.id);
      // If successful, update the UI
      const updatedConversations = conversations.filter(c => c.id !== conversation.id);
      setConversations(updatedConversations);
    } catch (error) {
      console.error('Error deleting conversation:', error);
      // Show error message if needed
    }
  };


  // Archive/unarchive conversation
  const toggleArchive = (conversation: ConversationItem) => {
    // In a real implementation, you would call an API endpoint to update the status
    // For now, we'll just update the UI state
    const updatedConversations = conversations.map(c => {
      if (c.id === conversation.id) {
        return { ...c, status: c.status === 'active' ? 'archived' : 'active' };
      }
      return c;
    });
    setConversations(updatedConversations);
  };

  // Format timestamp to relative time
  const formatTimestamp = (timestamp: Date) => {
    return formatDistanceToNow(timestamp, { addSuffix: true });
  };

  // Function to truncate text with ellipsis
  const truncateText = (text: string, maxLength: number) => {
    if (!text) return '';
    return text.length > maxLength ? `${text.substring(0, maxLength)}...` : text;
  };

  // Render column templates
  const titleTemplate = (rowData: ConversationItem, column: any) => {
    // Limit title to 50 characters
    const truncatedTitle = truncateText(rowData.title, 50);
    
    return (
      <div className="flex align-items-center gap-2">
        <span className="font-semibold" title={rowData.title}>{truncatedTitle}</span>
        {rowData.status === 'archived' && (
          <Tag value="Archived" severity="secondary" />
        )}
      </div>
    );
  };

  const messagePreviewTemplate = (rowData: ConversationItem, column: any) => {
    // Limit message preview to 100 characters
    const truncatedMessage = truncateText(rowData.lastMessage, 100);
    
    return (
      <div className="text-sm text-color-secondary line-clamp-1" title={rowData.lastMessage}>
        {truncatedMessage || <em>No messages</em>}
      </div>
    );
  };

  const timestampTemplate = (rowData: ConversationItem, column: any) => {
    return <span className="text-sm">{formatTimestamp(rowData.timestamp)}</span>;
  };

  // Message count column template
  const messageCountTemplate = (rowData: ConversationItem, column: any) => {
    return <span>{rowData.messageCount}</span>;
  };

  const actionsTemplate = (rowData: ConversationItem, column: any) => {
    const menuRef = useRef<any>(null);
    
    const menuItems = [
      {
        label: 'View',
        icon: 'pi pi-eye',
        command: () => openConversation(rowData)
      },
      {
        separator: true
      },
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        className: 'p-error',
        command: () => confirmDelete(rowData)
      }
    ];
    
    return (
      <div className="flex justify-content-center">
        <Button
          icon="pi pi-ellipsis-v"
          className="p-button-rounded p-button-text p-button-plain action-menu-button"
          onClick={(e) => {
            e.stopPropagation();
            menuRef.current.toggle(e);
          }}
          aria-label="Options"
        />
        <Menu
          model={menuItems}
          popup
          ref={menuRef}
          popupAlignment="right"
        />
      </div>
    );
  };

  const header = (
    <div className="flex justify-content-between align-items-center">
      <h2 className="m-0 text-lg">Recent Conversations</h2>
      <div className="p-input-icon-left">
        <i className="pi pi-search" />
        <InputText 
          value={globalFilter} 
          onChange={(e) => setGlobalFilter(e.target.value)} 
          placeholder="Search" 
          className="p-inputtext-sm"
        />
      </div>
    </div>
  );

  // Create a new conversation
  const createNewConversation = () => {
    navigate('/chat');
  };

  // Generate skeleton rows for the table when loading
  const skeletonTemplate = () => {
    return (
      <div className="flex align-items-center gap-2">
        <Skeleton width="70%" height="1.2rem" />
      </div>
    );
  };
  
  // Generate array of empty objects for skeleton loading
  const skeletonArray = Array(10).fill({});

  return (
    <div className="conversation-list-container">
      <ConfirmDialog />
      
      {/* Summary Card */}
      <Card className="mb-3 conversation-summary-card">
        <div className="flex flex-column md:flex-row justify-content-between">
          <div className="mb-3 md:mb-0">
            <h2 className="m-0 mb-2 text-xl">Conversation History</h2>
            <p className="m-0 text-sm text-color-secondary">
              View, manage, and continue your recent conversations {!loading && conversations.length > 0 && `(${conversations.length} total)`}
            </p>
          </div>
          <div className="flex align-items-center">
            <Button 
              label="New Conversation" 
              icon="pi pi-plus" 
              className="p-button-primary" 
              style={{ height: "2.5rem", padding: "0.75rem 1.25rem" }}
              onClick={createNewConversation}
            />
          </div>
        </div>
        
        {/* Total count shown at the top right of the card header */}
      </Card>
      
      {/* Data Table */}
      {loading ? (
        <Card>
          <div className="mb-3">
            <Skeleton width="25%" height="1.5rem" className="mb-3" />
            <div className="flex justify-content-between mb-3">
              <Skeleton width="15%" height="1.2rem" />
              <Skeleton width="15%" height="1.2rem" />
            </div>
          </div>
          <DataTable 
            value={skeletonArray} 
            className="p-datatable-sm"
          >
            <Column body={skeletonTemplate} style={{ width: '30%' }} />
            <Column body={skeletonTemplate} style={{ width: '40%' }} />
            <Column body={() => <Skeleton width="2rem" height="1.2rem" />} style={{ width: '10%' }} />
            <Column body={() => <Skeleton width="4rem" height="1.2rem" />} style={{ width: '10%' }} />
            <Column body={() => (
              <div className="flex justify-content-center">
                <Skeleton shape="circle" size="2rem" />
              </div>
            )} style={{ width: '10%' }} />
          </DataTable>
        </Card>
      ) : (
        <DataTable 
          value={conversations} 
          paginator 
          rows={10} 
          rowsPerPageOptions={[5, 10, 25]} 
          globalFilter={globalFilter}
          header={header}
          emptyMessage="No conversations found"
          stripedRows
          className="p-datatable-sm p-datatable-responsive-stack"
          sortField="timestamp"
          sortOrder={-1}
          rowClassName={() => "cursor-pointer"}
          onRowDoubleClick={(e) => openConversation(e.data as ConversationItem)}
          responsiveLayout="stack"
          breakpoint="960px"
        >
          <Column field="title" header="Title" body={titleTemplate} sortable headerClassName="w-3 column-title" />
          <Column field="lastMessage" header="Last Message" body={messagePreviewTemplate} headerClassName="w-5 column-message" />
          <Column field="messageCount" header="Messages" body={messageCountTemplate} sortable headerClassName="w-1 column-count" />
          <Column field="timestamp" header="Time" body={timestampTemplate} sortable headerClassName="w-1 column-time" />
          <Column 
            header="Actions" 
            body={actionsTemplate} 
            headerClassName="w-2 column-actions text-center"
            bodyClassName="text-center"
            style={{ width: '10%' }}
            frozen
            alignFrozen="right"
          />
        </DataTable>
      )}
    </div>
  );
};

export default ConversationList;