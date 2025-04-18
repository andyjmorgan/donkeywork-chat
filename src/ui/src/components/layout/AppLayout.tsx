import React, { useState, useRef, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Sidebar } from 'primereact/sidebar';
import { Button } from 'primereact/button';
import { Menu } from 'primereact/menu';
import { MenuItem } from 'primereact/menuitem';
import { Toolbar } from 'primereact/toolbar';
import { Avatar } from 'primereact/avatar';
import { InputSwitch } from 'primereact/inputswitch';
import { useAuth } from '../../context/AuthContext';
import { AppLayoutProps, ThemeState } from '../../models/ui/layout/AppLayoutTypes';

// PrimeReact styles are imported in App.tsx

const AppLayout: React.FC<AppLayoutProps> = ({ children }) => {
  const [sidebarVisible, setSidebarVisible] = useState<boolean>(true);
  const [mobileMenuVisible, setMobileMenuVisible] = useState<boolean>(false);
  const [darkMode, setDarkMode] = useState<boolean>(false);
  const menuRef = useRef<Menu>(null);
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  
  // Effect to handle theme switching
  useEffect(() => {
    // Load the theme preference from localStorage
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      setDarkMode(true);
      loadTheme('lara-dark-purple');
    } else {
      setDarkMode(false);
      loadTheme('lara-light-purple');
    }
  }, []);
  
  // Function to load a theme dynamically
  const loadTheme = (theme: string) => {
    const linkId = 'theme-css';
    const themeLink = document.getElementById(linkId) as HTMLLinkElement || document.createElement('link');
    
    if (!themeLink.id) {
      themeLink.id = linkId;
      themeLink.rel = 'stylesheet';
      document.head.appendChild(themeLink);
    }
    
    themeLink.href = `https://cdn.jsdelivr.net/npm/primereact@10.9.3/resources/themes/${theme}/theme.css`;
  };
  
  // Toggle dark mode
  const toggleDarkMode = (checked: boolean) => {
    setDarkMode(checked);
    const theme = checked ? 'lara-dark-purple' : 'lara-light-purple';
    localStorage.setItem('theme', checked ? 'dark' : 'light');
    loadTheme(theme);
  };
  
  // Simple menu items for sidebar
  const sidebarItems: MenuItem[] = [
    {
      label: 'Home',
      icon: 'pi pi-home',
      command: () => navigate('/'),
      template: (item, options) => {
        const active = (location.pathname === '/' || location.pathname === '/home') && 
                      !location.pathname.includes('/chat');
        return (
          <a 
            className={`${options.className} menu-item ${active ? 'active-route' : ''}`} 
            onClick={options.onClick}
          >
            <span className={`${options.iconClassName} ${active ? 'text-purple-500' : 'text-primary'}`}></span>
            <span className={options.labelClassName}>{item.label}</span>
          </a>
        );
      }
    },
    {
      label: 'Chat',
      icon: 'pi pi-comments',
      command: () => navigate('/chat'),
      template: (item, options) => {
        const active = location.pathname.includes('/chat');
        return (
          <a 
            className={`${options.className} menu-item ${active ? 'active-route' : ''}`} 
            onClick={options.onClick}
          >
            <span className={`${options.iconClassName} ${active ? 'text-purple-500' : 'text-primary'}`}></span>
            <span className={options.labelClassName}>{item.label}</span>
          </a>
        );
      }
    },
    {
      label: 'Conversations',
      icon: 'pi pi-history',
      command: () => navigate('/conversations'),
      template: (item, options) => {
        const active = location.pathname.includes('/conversations');
        return (
          <a 
            className={`${options.className} menu-item ${active ? 'active-route' : ''}`} 
            onClick={options.onClick}
          >
            <span className={`${options.iconClassName} ${active ? 'text-purple-500' : 'text-primary'}`}></span>
            <span className={options.labelClassName}>{item.label}</span>
          </a>
        );
      }
    },
    {
      label: 'Integrations',
      icon: 'pi pi-link',
      command: () => navigate('/integrations'),
      template: (item, options) => {
        const active = location.pathname.includes('/integrations');
        return (
          <a 
            className={`${options.className} menu-item ${active ? 'active-route' : ''}`} 
            onClick={options.onClick}
          >
            <span className={`${options.iconClassName} ${active ? 'text-purple-500' : 'text-primary'}`}></span>
            <span className={options.labelClassName}>{item.label}</span>
          </a>
        );
      }
    },
    {
      label: 'Prompts',
      icon: 'pi pi-book',
      command: () => navigate('/prompts'),
      template: (item, options) => {
        const active = location.pathname.includes('/prompts');
        return (
          <a 
            className={`${options.className} menu-item ${active ? 'active-route' : ''}`} 
            onClick={options.onClick}
          >
            <span className={`${options.iconClassName} ${active ? 'text-purple-500' : 'text-primary'}`}></span>
            <span className={options.labelClassName}>{item.label}</span>
          </a>
        );
      }
    },
    {
      label: 'API Keys',
      icon: 'pi pi-key',
      command: () => navigate('/apikeys'),
      template: (item, options) => {
        const active = location.pathname.includes('/apikeys');
        return (
          <a 
            className={`${options.className} menu-item ${active ? 'active-route' : ''}`} 
            onClick={options.onClick}
          >
            <span className={`${options.iconClassName} ${active ? 'text-purple-500' : 'text-primary'}`}></span>
            <span className={options.labelClassName}>{item.label}</span>
          </a>
        );
      }
    },
  ];

  // User menu items with logout
  const userMenuItems: MenuItem[] = [
    {
      template: (item, options) => {
        return (
          <div className="theme-toggle-container" onClick={(e) => e.stopPropagation()}>
            <button 
              className={`theme-toggle-btn ${!darkMode ? 'active' : ''}`} 
              onClick={() => toggleDarkMode(false)}
            >
              <i className="pi pi-sun"></i>
            </button>
            <button 
              className={`theme-toggle-btn ${darkMode ? 'active' : ''}`} 
              onClick={() => toggleDarkMode(true)}
            >
              <i className="pi pi-moon"></i>
            </button>
          </div>
        );
      }
    },
    { separator: true },
    {
      label: 'My Profile',
      icon: 'pi pi-user',
      command: () => navigate('/profile'),
      template: (item, options) => {
        const active = location.pathname === '/profile';
        return (
          <a className={`${options.className} menu-item ${active ? 'active-route' : ''}`} onClick={options.onClick}>
            <span className={`${options.iconClassName} ${active ? 'text-purple-500' : 'text-primary'}`}></span>
            <span className={options.labelClassName}>{item.label}</span>
          </a>
        );
      }
    },
    { separator: true },
    {
      label: 'Logout',
      icon: 'pi pi-sign-out',
      command: () => logout(),
      template: (item, options) => {
        return (
          <a className={`${options.className} menu-item`} onClick={options.onClick}>
            <span className={`${options.iconClassName} text-red-500`}></span>
            <span className={`${options.labelClassName} text-red-500`}>{item.label}</span>
          </a>
        );
      }
    }
  ];

  // Top navbar right content
  const topbarRight = (
    <div className="flex align-items-center gap-3">
      <div className="flex align-items-center cursor-pointer" onClick={(e) => menuRef.current && menuRef.current.toggle(e)}>
        <span className="font-bold hidden md:block mr-2">
          {user?.userName || (user?.emailAddress && user.emailAddress.split('@')[0]) || 'User'}
        </span>
        <Button
          icon="pi pi-chevron-down"
          className="p-button-text p-button-rounded"
          style={{ padding: '0.3rem' }}
        />
      </div>
      <Menu 
        model={userMenuItems} 
        popup 
        ref={menuRef} 
        pt={{
          root: { className: 'user-menu-root' },
          menu: { className: 'user-menu' },
          menuitem: { className: 'user-menu-item' },
          separator: { className: 'user-menu-separator' }
        }}
      />
    </div>
  );

  // Top navbar left content
  const topbarLeft = (
    <div className="flex align-items-center justify-content-between w-full md:w-auto">
      {/* Mobile hamburger menu button - shown only on mobile */}
      <Button 
        icon="pi pi-bars" 
        onClick={() => setMobileMenuVisible(true)} 
        className="p-button-text p-button-secondary md:hidden square-button"
        style={{ fontSize: '1.2rem' }}
      />
      
      {/* Logo and title - different versions for mobile and desktop */}
      {/* Desktop version - hidden on mobile */}
      <div 
        className="hidden md:flex align-items-center cursor-pointer" 
        onClick={() => navigate('/')}
        style={{ cursor: 'pointer' }}
      >
        <img src="https://chatbot.donkeywork.dev/src/assets/donkeywork-icon-40.png" alt="DonkeyWork Logo" className="mr-2" style={{ height: '28px', width: 'auto' }} />
        <h2 className="m-0 text-lg">DonkeyWork</h2>
      </div>
      
      {/* Mobile version - centered - hidden on desktop */}
      <div 
        className="md:hidden flex align-items-center cursor-pointer absolute left-50 transform-translate-x--50"
        onClick={() => navigate('/')}
        style={{ 
          cursor: 'pointer',
          left: '50%',
          transform: 'translateX(-50%)'
        }}
      >
        <img src="https://chatbot.donkeywork.dev/src/assets/donkeywork-icon-40.png" alt="DonkeyWork Logo" className="mr-2" style={{ height: '28px', width: 'auto' }} />
        <h2 className="m-0 text-lg">DonkeyWork</h2>
      </div>
      
      {/* Empty div to balance the hamburger menu on mobile */}
      <div className="md:hidden w-2rem"></div>
    </div>
  );

  // No need for useEffect as active state is handled in the template

  return (
    <div className="min-h-screen flex flex-column surface-ground" style={{ height: '100vh' }}>
      {/* Top navbar */}
      <Toolbar 
        left={topbarLeft} 
        right={topbarRight} 
        className="p-toolbar-separator-0 border-noround surface-card shadow-1 relative"
        style={{ flex: '0 0 auto' }}
        pt={{
          root: { className: 'mobile-toolbar' },
          start: { className: 'toolbar-start' },
          center: { className: 'toolbar-center' },
          end: { className: 'toolbar-end' }
        }}
      />

      <div className="flex flex-1 overflow-hidden h-full">
        {/* Desktop sidebar - fixed width that doesn't auto-resize */}
        <div 
          className={`sidebar-container hidden md:block transition-all transition-duration-300 surface-card ${
            sidebarVisible ? 'w-15rem' : 'w-4rem'
          }`}
          style={{ flexShrink: 0, height: '100%' }}
        >
          <div className="flex flex-column h-full">
            {/* Collapse/Expand menu item at top of sidebar */}
            <div className={`${sidebarVisible ? 'px-3' : ''} pt-3 ${sidebarVisible ? '' : 'text-center'}`}>
              <Menu model={[{
                label: sidebarVisible ? 'Collapse Menu' : '',
                icon: sidebarVisible ? 'pi pi-angle-left' : 'pi pi-angle-right',
                command: () => setSidebarVisible(!sidebarVisible),
                template: (item, options) => {
                  return (
                    <a 
                      className={`${options.className} menu-item toggle-menu-item square-menu-item`} 
                      onClick={options.onClick}
                      title={sidebarVisible ? 'Collapse Menu' : 'Expand Menu'}
                    >
                      <span className={`${options.iconClassName} text-primary`}></span>
                      {sidebarVisible && <span className={options.labelClassName}>{item.label}</span>}
                    </a>
                  );
                }
              }]} className="w-full border-none toggle-menu" />
            </div>
            
            {/* Add a PrimeReact separator after collapse button */}
            <div className={`separator-container top-separator ${sidebarVisible ? 'visible' : 'collapsed'}`}>
              <div className={`${sidebarVisible ? 'mx-3' : ''} separator-line`}></div>
            </div>
            
            {/* Menu container that fills all remaining space */}
            <div className={`${sidebarVisible ? 'px-3' : ''} pb-3 flex-grow-1 overflow-y-auto menu-container`}>
              <Menu model={sidebarItems} className="w-full border-none simple-sidebar" />
            </div>
            
          </div>
        </div>

        {/* Mobile sidebar */}
        <Sidebar 
          visible={mobileMenuVisible} 
          onHide={() => setMobileMenuVisible(false)}
          className="w-18rem mobile-sidebar"
          showCloseIcon={true}
          pt={{
            root: { style: { overflow: 'hidden' } },
            header: { className: 'mobile-sidebar-header' },
            closeButton: { className: 'mobile-sidebar-close' },
            closeIcon: { className: 'pi pi-times' },
            content: { className: 'surface-section p-0' }
          }}
          icons={
            <div 
              className="flex align-items-center cursor-pointer" 
              style={{ paddingRight: '3rem' }}
              onClick={() => {
                navigate('/');
                setMobileMenuVisible(false);
              }}
            >
              <img src="https://chatbot.donkeywork.dev/src/assets/donkeywork-icon-40.png" alt="DonkeyWork Logo" className="mr-2" style={{ height: '28px', width: 'auto' }} />
              <span className="text-lg font-medium">DonkeyWork</span>
            </div>
          }
        >
          <div className="p-3">
            <Menu model={sidebarItems} className="w-full border-none" />
          </div>
        </Sidebar>

        {/* Main content */}
        <div className="flex-grow-1 p-2 overflow-auto surface-section" style={{ height: '100%' }}>
          <div className="card p-2 h-full shadow-1">
            {children}
          </div>
        </div>
      </div>
    </div>
  );
};

export default AppLayout;